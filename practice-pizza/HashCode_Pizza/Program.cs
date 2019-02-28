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
            var inputFile = args.Length > 0 ? args[0] : @"..\..\..\Input\a_example.txt";
            //var inputFile = args.Length > 0 ? args[0] : @"..\..\..\Input\b_small - Copy.in";

            var catalog = loadData(inputFile);

            foreach (var photo in catalog.Photos)
            {
                Console.WriteLine($"photo {photo}");
            }

            var slide = CreateSlides(catalog);

            var outputFile = Path.Combine(OutputFolder, Path.ChangeExtension(Path.GetFileName(inputFile), "out"));
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile))
            {
                foreach (var photo in catalog.Photos)
                {
                    sw.WriteLine($"photo {photo}");
                    Console.WriteLine($"photo {photo}");
                }
            }

            Console.ReadLine();
        }

        private static Slide CreateSlides(Catalog catalog)
        {
            return new Slide(); 
        }

        private static Catalog loadData(string fileName)
        { 
            var catalog = new Catalog();
            int photoId = 1;

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
                    catalog.Photos.Add(photo);
                }
            }

            return catalog;
        }
    }

    internal class Slide
    {
        public List<Photo> Photos { get; set; }

        public HashSet<string> Tags { get; set; }

        public void LoadPhoto(Photo photo)
        {
            this.Photos.Add(photo);
            this.Tags.UnionWith(photo.Tags);
        }
    }

    internal class HorizontalSlide : Slide
    {
        public HorizontalSlide(Photo photo)
        {
            this.LoadPhoto(photo);
        }
    }

    internal class VerticalSlide : Slide
    {
        public VerticalSlide(Photo photo1, Photo photo2)
        {
            this.LoadPhoto(photo1);
            this.LoadPhoto(photo2);
        }
    }
}
