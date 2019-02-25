using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode_Pizza
{
    class PizzaPlate
    {
        private readonly static Color[] SLICING_COLORS = new Color[]
        {
                Color.Violet,
                Color.Green,
                Color.Yellow,
                Color.Red,
                Color.Blue,
                Color.Cyan,
                Color.Pink,
                Color.PeachPuff,
                Color.Orange,
                Color.Olive
        };

        private const int SLICING_COLOR_BIT_SIZE = 5;

        private const int CHECK_SLICE_VALID = 0;
        private const int CHECK_SLICE_TOO_LOW = 1;
        private const int CHECK_SLICE_INVALID_SLICE = 2;
        private const int CHECK_SLICE_TOO_BIG = 3;

        private int mColumns;
        private int mRows;

        private int mMinIngPerSlice;
        private int mMaxSliceSize;

        private int[,] mPlate;

        public PizzaPlate(int rows, int columns, int[,] plate, int minIng, int maxSliceSize)
        {
            mRows = rows;
            mColumns = columns;
            mPlate = plate;
            mMinIngPerSlice = minIng;
            mMaxSliceSize = maxSliceSize;
        }

        public Bitmap generateSlicingBitmap(List<PizzaSlice> slices)
        {
            Bitmap bitmap = new Bitmap(mColumns * SLICING_COLOR_BIT_SIZE, mRows * SLICING_COLOR_BIT_SIZE);
            Graphics gfx = Graphics.FromImage(bitmap);
            SolidBrush brush = new SolidBrush(Color.White);

            // Clear bitmap
            //gfx.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);

            // Draw slicing
            foreach (PizzaSlice slice in slices)
            {
                brush.Color = SLICING_COLORS[Math.Abs(slice.ID) % SLICING_COLORS.Length];
                gfx.FillRectangle(brush, slice.ColumnMin * SLICING_COLOR_BIT_SIZE, slice.RowMin * SLICING_COLOR_BIT_SIZE, (slice.ColumnMax - slice.ColumnMin + 1) * SLICING_COLOR_BIT_SIZE, (slice.RowMax - slice.RowMin + 1) * SLICING_COLOR_BIT_SIZE);
            }

            // Ingredients
            brush.Color = Color.Black;
            for (int r = 0; r < mRows; r++)
                for (int c = 0; c < mColumns; c++)
                    if (this.mPlate[r, c] == 1)
                        bitmap.SetPixel(c * SLICING_COLOR_BIT_SIZE + SLICING_COLOR_BIT_SIZE/2, r * SLICING_COLOR_BIT_SIZE + SLICING_COLOR_BIT_SIZE/2, Color.Black);
                    else if (this.mPlate[r, c] == 2)
                        gfx.FillRectangle(brush, c * SLICING_COLOR_BIT_SIZE + 1, r * SLICING_COLOR_BIT_SIZE + SLICING_COLOR_BIT_SIZE/2, SLICING_COLOR_BIT_SIZE - 2, 1);

            brush.Dispose();
            gfx.Dispose();

            return bitmap;
        }

        public int GetSize() { return mColumns * mRows; }

        public List<PizzaSlice> PerformSlice()
        {
            int[,] plate = (int[,])mPlate.Clone();

            // Create greedy slicing. Iterating this phase did not yield better results
            List<PizzaSlice> slices = PerformSlice_PhaseTwo(plate);

            return slices;
        }

        private List<PizzaSlice> PerformSlice_PhaseTwo(int[,] plate)
        {
            int nextSliceId = -1;
            Dictionary<int, PizzaSlice> sliceHash = new Dictionary<int, PizzaSlice>();

            // Slice Pizza
            for (int r = 0; r < mRows; r++)
            {
                for (int c = 0; c < mColumns; c++)
                {
                    if (SlicePizzaAtPosition(plate, r, c, sliceHash, nextSliceId) == true)
                        nextSliceId--;
                }
            }
            
         

            return new List<PizzaSlice>(sliceHash.Values);
        }

        private bool SlicePizzaAtPosition(int[,] plate, int r, int c, Dictionary<int, PizzaSlice> sliceHash, int nextSliceId)
        {
            if (plate[r, c] < 0)
                return false;

            var bestSize = -1;
            PizzaSlice bestSlice = null;

            for (int w = r; w - r <= this.mMaxSliceSize && w < this.mRows; w++)
            {
                // (w - r) * (h - c) < this.mMaxSliceSize  && 
                for (int h = c; h < this.mColumns && (w - r) * (h - c) < this.mMaxSliceSize; h++)
                {
                    int isValidSlice = IsValidSlice(this.mPlate, r, w, c, h);
                    if (isValidSlice != CHECK_SLICE_VALID)
                        continue;

                    PizzaSlice newSlice = new PizzaSlice(nextSliceId, r, w, c, h);

                    var size = newSlice.GetSize();
                    if (size > bestSize)
                    {
                        bestSize = size;
                        bestSlice = newSlice;
                    }
                }
            }

            var found = bestSlice != null;
            if (found)
            {
                if (IsValidSlicing(sliceHash.Values.ToList()))
                {
                    bestSlice.RemoveSliceFromPlate(plate);
                    sliceHash.Add(bestSlice.ID, bestSlice);
                }
                else
                {
                }
            }

            return found;
        }

        public bool IsValidSlicing(List<PizzaSlice> slices)
        {
            int[,] plate = (int[,])mPlate.Clone();
            foreach (PizzaSlice slice in slices)
            {
                if (IsValidSlice(mPlate, slice.RowMin, slice.RowMax, slice.ColumnMin, slice.ColumnMax) != CHECK_SLICE_VALID)
                    return false;

                for (int r = slice.RowMin; r <= slice.RowMax; r++)
                    for (int c = slice.ColumnMin; c <= slice.ColumnMax; c++)
                    {
                        if (plate[r, c] < 0)
                            return false;
                        plate[r, c] = slice.ID;
                    }
            }

            return true;
        }

        private int IsValidSlice(int[,] plate, int minRow, int maxRow, int minCol, int maxCol)
        {
            int count1 = 0;
            int count2 = 0;

            if ((maxRow - minRow + 1)*(maxCol - minCol + 1) > mMaxSliceSize)
                return CHECK_SLICE_TOO_BIG;

            for (int r = minRow; r <= maxRow; r++)
            {
                for (int c = minCol; c <= maxCol; c++)
                {
                    int plateVal = plate[r, c];
                    if (plateVal <= 0)
                        return CHECK_SLICE_INVALID_SLICE;
                    else if (plateVal == 1)
                        count1++;
                    else if (plateVal == 2)
                        count2++;
                    else
                        throw new Exception("Valid plate value: " + plateVal);
                }
            }

            if ((count1 < this.mMinIngPerSlice) || (count2 < this.mMinIngPerSlice))
                return CHECK_SLICE_TOO_LOW;

            return CHECK_SLICE_VALID;
        }
    }
}
