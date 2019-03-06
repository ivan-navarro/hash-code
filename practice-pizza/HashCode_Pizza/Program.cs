using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace HashCode_Pizza
{
     //let's code!
    class Program
    {
        const string OutputFolder = @"..\..\..\Output\";

        private const string HORIZONTAL = "H";
        private const string VERTICAL = "V";

        static void Main(string[] args)
        {
            Task.Run(() => ProcessFile(@"..\..\..\Input\a_example.txt"));
            Task.Run(() => ProcessFile(@"..\..\..\Input\b_lovely_landscapes.txt"));
            Task.Run(() => ProcessFile(@"..\..\..\Input\c_memorable_moments.txt"));
            Task.Run(() => ProcessFile(@"..\..\..\Input\d_pet_pictures.txt"));
            Task.Run(() => ProcessFile(@"..\..\..\Input\e_shiny_selfies.txt"));

            //ProcessFile(@"..\..\..\Input\d_pet_pictures.txt");

            Console.ReadLine();
        }

        private static void ProcessFile(string inputFile)
        {
            var photos = loadData(inputFile);

            //DrawPhotos(photos);

            var bestSlideShow = CreateSlideShow(photos);
            photos.Clear();

            var value = bestSlideShow.CalculateValues();

            //var maxValue = bestSlideShow.Value();

            //var newSlides = bestSlideShow.Slides.ToList();
            //for (int i = 0; i < 50; i++)
            //{
            //    Random r = new Random(i);
            //    var source = r.Next(newSlides.Count - 1);
            //    var dest = r.Next(newSlides.Count - 1);
            //    var temp = newSlides.ElementAt(source);
            //    newSlides.RemoveAt(source);
            //    newSlides.Insert(dest, temp);

            //    var current = new SlideShow(newSlides);
            //    var value = current.Value();

            //    if (value > maxValue)
            //    {
            //        Console.WriteLine($"Better slide found {value} > {maxValue}");

            //        bestSlideShow = current;
            //        maxValue = value;
            //    }
            //}

            var outputFile = Path.Combine(OutputFolder, Path.ChangeExtension(Path.GetFileName(inputFile), "out"));

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile))
            {
                var slides = bestSlideShow.Slides.ToList();
                sw.WriteLine(slides.Count);
                //Console.WriteLine(slides.Count);
                foreach (var slide in slides)
                {
                    sw.WriteLine(slide);
                    //Console.WriteLine(slide);
                }
            }

            Console.WriteLine("file processed " + inputFile);
            Console.WriteLine("score for "+inputFile+ " is "+ bestSlideShow.Value() + " vs " + value);
        }

        private static void DrawPhotos(List<Photo> photos)
        {
            foreach (var photo in photos)
            {
                Console.WriteLine($"photo {photo}");
            }
        }

        private static SlideShow CreateSlideShow(List<Photo> photos)
        {
            var slides = new List<Slide>();

            var verticalPhotos = photos.Where(p => p.Orientation == VERTICAL).OrderByDescending(p => p.Tags.Count).ToList();

            while (verticalPhotos.Count > 1)
            {
                Photo bestComplementary = null;
                var current = verticalPhotos[0];
                int minCommonTags = int.MaxValue;

                for (int i = 1; i < Math.Min(verticalPhotos.Count, 30); i++)
                {
                    var common = verticalPhotos[i].Tags.Intersect(current.Tags).Count();
                    if (common < minCommonTags)
                    {
                        minCommonTags = common;
                        bestComplementary = verticalPhotos[i];
                    }
                }

                slides.Add(new VerticalSlide(current, bestComplementary));

                verticalPhotos.Remove(current);
                verticalPhotos.Remove(bestComplementary);
            }

            var horizontalSlides = photos.Where(p => p.Orientation == HORIZONTAL).Select(hp => new HorizontalSlide(hp));
            slides.AddRange(horizontalSlides);

            return new SlideShow(slides); 
        }

        private static List<Photo> loadData(string fileName)
        { 
            int photoId = 0;
            var photos = new List<Photo>();

            // Load data
            using (System.IO.StreamReader sr = new System.IO.StreamReader(fileName))
            {
                int numImages = int.Parse(sr.ReadLine());
                for (int i = 0; i < numImages; i++)
                {
                   var line = sr.ReadLine();
                    string[] parts = line.Split(' ');

                    string orientation = parts[0];
                    int numTags = int.Parse(parts[1]);

                    var tags = new List<string>(numTags);
                    for (int t = 0; t < numTags; t++)
                    {
                        tags.Add(parts[2 + t]);
                    }

                    var photo = new Photo(photoId++, tags, orientation);
                    photos.Add(photo);
                }
            }

            return photos;
        }
    }

   
}
