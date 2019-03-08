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
            try
            {
                var photos = loadData(inputFile);

                //DrawPhotos(photos);
                var bestScore = -1;
                SlideShow bestSlideShow = null;
                var slideShow2 = new SlideShow();
                slideShow2.ArrangeByInterest(photos);

                Parallel.For(0, Math.Min(photos.Count - 1, 5), i =>
                {
                    var slideShow = new SlideShow();
                    slideShow.ArrangeByTags(photos, i);
                    Console.WriteLine($"score for {inputFile} i={i} is {slideShow.Score}");

                    if (slideShow.Score > bestScore)
                    {
                        bestSlideShow = slideShow;
                        bestScore = bestSlideShow.Score;
                    }
                });

               
                Parallel.For(0, Math.Min(photos.Count - 1, 5), i =>
                {
                    var slideShow = new SlideShow();
                    slideShow.ArrangeByInterest(photos, i);

                    if (slideShow.Score > bestScore)
                    {
                        Console.WriteLine($"Improved solution {inputFile} i={i} {slideShow.Score} vs {bestScore}");
                        bestSlideShow = slideShow;
                        bestScore = bestSlideShow.Score;
                    }
                });

                // todo: reallocate 0 results?

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

                Console.WriteLine("score for " + inputFile + " is " + bestSlideShow.Score);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        private static SlideShow ImproveSolution(string inputFile, SlideShow bestSlideShow)
        {
            var maxValue = bestSlideShow.Score;

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

            return bestSlideShow;
        }

        private static void DrawPhotos(List<Photo> photos)
        {
            foreach (var photo in photos)
            {
                Console.WriteLine($"photo {photo}");
            }
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
