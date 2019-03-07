using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
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
            var t1 = Task.Run(() => ProcessFile(@"..\..\..\Input\a_example.txt"));
            var t2 = Task.Run(() => ProcessFile(@"..\..\..\Input\b_lovely_landscapes.txt"));
            var t3 = Task.Run(() => ProcessFile(@"..\..\..\Input\c_memorable_moments.txt"));
            var t4 = Task.Run(() => ProcessFile(@"..\..\..\Input\d_pet_pictures.txt"));
            var t5 = Task.Run(() => ProcessFile(@"..\..\..\Input\e_shiny_selfies.txt"));

            //Task.WhenAll(t1, t2, t3, t4, t5).Wait();
            Console.ReadLine();
        }

        private static void ProcessFile(string inputFile)
        {
            try
            {

            
            var photos = loadData(inputFile);

            //foreach (var photo in catalog.Photos) //
            //{
            //    Console.WriteLine($"photo {photo}");
            //}

            var bestSlideShow = CreateSlideShow(photos);
            var maxValue = bestSlideShow.Value();


            for (int i = 0; i < 10; i++)
            {
                var slides = bestSlideShow.Slides.ToList();

                var worstInterest = int.MaxValue;
                // var latestValue = worstInterest;
                int worstPosition = 0;

                for (int j = 1; j < slides.Count - 2; j++)
                {
                    var currentInterest = InterestCalculator.SlideInterest(slides[j - 1], slides[j])
                                          + InterestCalculator.SlideInterest(slides[j], slides[j + 1]);

                    if (currentInterest < worstInterest)
                    {
                        Console.WriteLine($"{inputFile}: Worse interest found {currentInterest} < {worstInterest}");

                        worstInterest = currentInterest;
                        worstPosition = j;
                    }
                }

                var movedSlide = slides.ElementAt(worstPosition);

                var temp = bestSlideShow.Slides.ToList();
                temp.RemoveAt(worstPosition);

                for (int j = 0; j < slides.Count - 1; j++)
                {
                    var current = new SlideShow(temp);
                    current.Slides.Insert(j, movedSlide);

                    var value = current.Value();

                    if (value > maxValue)
                    {
                        Console.WriteLine($"{inputFile}: Better slide found {value} > {maxValue}");

                        bestSlideShow = current;
                        maxValue = value;
                    }
                }
            }

            var outputFile = Path.Combine(OutputFolder, Path.ChangeExtension(Path.GetFileName(inputFile), "out"));

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile))
            {
                var slides = bestSlideShow.Slides;
                sw.WriteLine(slides.Count);
                foreach (var slide in slides)
                {
                    sw.WriteLine(string.Join(" ", slide.Photos.Select(p => p.Id)));
                }
            }

            Console.WriteLine("file processed " + inputFile);
            Console.WriteLine("score for " + inputFile + " is " + bestSlideShow.Value());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        private static SlideShow CreateSlideShow(List<Photo> photos)
        {
            var slides = new List<Slide>();
            Photo verticalPhoto = null;

            foreach (var photo in photos)
            {
                if (photo.Orientation == HORIZONTAL)
                {
                    slides.Add(new HorizontalSlide(photo));
                }
                else
                {
                    if (verticalPhoto == null)
                    {
                        verticalPhoto = photo;
                    }
                    else
                    {
                        slides.Add(new VerticalSlide(photo, verticalPhoto));
                        verticalPhoto = null;
                    }
                }
            }

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
