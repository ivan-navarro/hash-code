using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using System.IO;

namespace HashCode_Pizza
{
    class Program
    {
        const string OutputFolder = @"..\..\..\Output\";


        static void Main(string[] args)
        {
            //var inputFile = args.Length > 0 ? args[0] : @"..\..\..\Input\a_example.in";
            //var inputFile = args.Length > 0 ? args[0] : @"..\..\..\Input\b_small.in";
            var inputFile = args.Length > 0 ? args[0] : @"..\..\..\Input\c_medium.in";

            PizzaPlate pizzaPlate = loadData(inputFile);

            List<PizzaSlice> slices = pizzaPlate.PerformSlice();

            if (pizzaPlate.IsValidSlicing(slices) == false)
                Console.WriteLine("ERROR: Invalid slicing");

            Console.WriteLine("Max theoretical score: {0}", pizzaPlate.GetSize());
            int solutionScore = slices.Sum(item => item.GetSize());
            Console.WriteLine("Solution score: {0}", solutionScore);

            Bitmap bitmap = pizzaPlate.generateSlicingBitmap(slices);

            var outputFile = Path.Combine(OutputFolder, Path.ChangeExtension(Path.GetFileName(inputFile), "out"));
            var outputImage = outputFile + ".png";
            bitmap.Save(outputImage, System.Drawing.Imaging.ImageFormat.Png);

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile))
            {
                sw.WriteLine(slices.Count);
                foreach (PizzaSlice slice in slices)
                {
                    sw.Write(slice.RowMin);
                    sw.Write(' ');
                    sw.Write(slice.ColumnMin);
                    sw.Write(' ');
                    sw.Write(slice.RowMax);
                    sw.Write(' ');
                    sw.Write(slice.ColumnMax);

                    sw.WriteLine();
                }
            }

            Process.Start(outputImage);

            Console.ReadLine();
        }

        private static PizzaPlate loadData(string fileName)
        { 
            // Load data
            using (System.IO.StreamReader sr = new System.IO.StreamReader(fileName))
            {
                string line = sr.ReadLine();
                string[] parts = line.Split(' ');
                int rows = int.Parse(parts[0]);
                int columns = int.Parse(parts[1]);
                int minIng = int.Parse(parts[2]);
                int maxIng = int.Parse(parts[3]);
                int[,] plate = new int[rows, columns];
                for (int r = 0; r < rows; r++)
                {
                    line = sr.ReadLine();
                    for (int c = 0; c < columns; c++)
                    {
                        if (line[c] == 'T')
                            plate[r, c] = 1;
                        else if (line[c] == 'M')
                            plate[r, c] = 2;
                        else throw new Exception("Invalid data in row: " + line);
                    }
                }

                return new PizzaPlate(rows, columns, plate, minIng, maxIng);
            }
        }
    }
}
