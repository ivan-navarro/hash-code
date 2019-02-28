﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace HashCode_Pizza
{
    class Program
    {
        const string OutputFolder = @"..\..\..\Output\";

        private const string HORIZONTAL = "H";
        private const string VERTICAL = "V";

        static void Main(string[] args)
        {
            var inputFile = args.Length > 0 ? args[0] : @"..\..\..\Input\a_example.txt";
            //var inputFile = args.Length > 0 ? args[0] : @"..\..\..\Input\b_lovely_landscapes.txt";
            //var inputFile = args.Length > 0 ? args[0] : @"..\..\..\Input\c_memorable_moments.txt";
            //var inputFile = args.Length > 0 ? args[0] : @"..\..\..\Input\d_pet_pictures.txt";
            //var inputFile = args.Length > 0 ? args[0] : @"..\..\..\Input\e_shiny_selfies.txt";

            var catalog = loadData(inputFile);

            foreach (var photo in catalog.Photos)
            {
                Console.WriteLine($"photo {photo}");
            }

            var slides = CreateSlides(catalog);

            var outputFile = Path.Combine(OutputFolder, Path.ChangeExtension(Path.GetFileName(inputFile), "out"));

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile))
            {
                sw.WriteLine(slides.Count);
                Console.WriteLine(slides.Count);
                foreach (var slide in slides)
                {
                    Console.WriteLine(string.Join(" ", slide.Photos.Select(p => p.Id)));
                    sw.WriteLine(string.Join(" ", slide.Photos.Select(p => p.Id)));
                }
            }

            Console.ReadLine();
        }

        private static List<Slide> CreateSlides(Catalog catalog)
        {
            var slides = new List<Slide>();
            Photo verticalPhoto = null;

            foreach (var photo in catalog.Photos)
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
                    }
                }
            }

            return slides; 
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

   
}
