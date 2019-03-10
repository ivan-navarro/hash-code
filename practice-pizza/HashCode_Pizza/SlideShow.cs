using System;
using System.Collections.Generic;
using System.Linq;

namespace HashCode_Pizza
{
    internal class SlideShow
    {
        private const string HORIZONTAL = "H";
        private const string VERTICAL = "V";

        public SlideShow(List<Slide> slides)
        {
            this.Slides = slides;
        }

        public SlideShow()
        {
        }

        public void ArrangeByTags(List<Photo> photos, int initialPhoto = 0)
        {
            var slides = OrganizeVerticalSlidesByTags(photos);

            var horizontalSlides = photos.Where(p => p.Orientation == HORIZONTAL).Select(hp => new HorizontalSlide(hp));
            slides.AddRange(horizontalSlides);

            var sortedSlides = slides.OrderByDescending(s => s.Tags.Count).ToList();

            this.ArrangeByInterest(sortedSlides, initialPhoto);

            this.CalculateValues();
        }

        //public void ArrangeCombiningVerticals(List<Photo> photos)
        //{
        //    this.ArrangeByInterest(photos);

        //    this.CalculateValues();
        //}

        private static List<Slide> OrganizeVerticalSlidesByTags(List<Photo> photos)
        {
            var slides = new List<Slide>();

            var verticalPhotos = photos.Where(p => p.Orientation == VERTICAL).OrderByDescending(p => p.Tags.Count)
                .ToList();

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

            return slides;
        }

        public void ArrangeByInterest(List<Photo> photos, int initialPhoto = 0)
        {
            //var remainingPhotos = new List<Photo>(photos);
            var sortedPhotos = photos.OrderByDescending(p => p.Tags.Count);

            var remainingVertical = sortedPhotos.Where(p => p.Orientation == VERTICAL).ToList();
            var remainingHorizontal = sortedPhotos.Where(p => p.Orientation == HORIZONTAL).ToList();
            var previousTags = new HashSet<string>();

            if (remainingHorizontal.Any())
            {
                var current = remainingHorizontal[0];
                remainingHorizontal.Remove(current);
                var firstSlide = new HorizontalSlide(current);
                this.Slides.Add(firstSlide);
                previousTags = firstSlide.Tags;
            }

            int remainingCount = remainingHorizontal.Count() + remainingVertical.Count() / 2;

            while (remainingCount-- > 0)
            {
                var maxInterest = -1;
                Slide bestSlide = null;
                int bestHorizontal = -1;
                int bestVertical1 = -1;
                int bestVertical2 = -1;

                for (int i = 0; i < Math.Min(100, remainingHorizontal.Count); i++)
                {
                    var interest =
                        InterestCalculator.CalculateFactorOfInterest(previousTags, remainingHorizontal[i].Tags);
                    if (interest > maxInterest)
                    {
                        maxInterest = interest;
                        bestHorizontal = i;

                        //Console.WriteLine($"better horizontal match for slide {remainingCount} is photo {i} with interest {interest}");
                    }
                }

                for (int i = 0; i < Math.Min(20, remainingVertical.Count - 1); i++)
                {
                    for (int j = Math.Min(40, remainingVertical.Count - 1); j > 0; j--)
                    {
                        if (i == j) continue;

                        var interest = InterestCalculator.CalculateFactorOfInterest(previousTags,
                            remainingVertical[i].Tags.Union(remainingVertical[j].Tags));
                        if (interest > maxInterest)
                        {
                            maxInterest = interest;
                            bestVertical1 = i;
                            bestVertical2 = j;

                            bestHorizontal = -1;

                            //Console.WriteLine($"better vertical match for slide {remainingCount} is photos {i}+{j} with interest {interest}");
                        }
                    }
                }


                if (maxInterest == 0)
                {
                    Console.WriteLine($"Slide without interest when {remainingCount}");
                }

                if (bestHorizontal >= 0)
                {
                    var photo = remainingHorizontal[bestHorizontal];

                    if (maxInterest > 0)
                    {
                        bestSlide = new HorizontalSlide(photo);
                        this.Slides.Add(bestSlide);
                        previousTags = bestSlide.Tags;
                    }

                    remainingHorizontal.Remove(photo);
                }
                else
                {
                   
                    var photo1 = remainingVertical[bestVertical1];
                    var photo2 = remainingVertical[bestVertical2];

                    if (maxInterest > 0)
                    {
                        bestSlide = new VerticalSlide(photo1, photo2);
                        this.Slides.Add(bestSlide);
                        previousTags = bestSlide.Tags;
                    }

                    remainingVertical.Remove(photo1);
                    remainingVertical.Remove(photo2);
                }
            }

            this.CalculateValues();
        }

        private void ArrangeByInterest(List<Slide> slides, int initialPhoto)
        {
            var current = slides[initialPhoto];
            AssignSlide(slides, current);
            int counter = 0;

            while (slides.Count > 0)
            {
                var maxInterest = InterestCalculator.CalculateFactorOfInterest(current.Tags, slides[0].Tags);
                var bestSlide = slides[0];

                for (int i = 1; i < Math.Min(200, slides.Count); i++)
                {
                    var interest = InterestCalculator.CalculateFactorOfInterest(current.Tags, slides[i].Tags);

                    if (interest > maxInterest)
                    {
                        maxInterest = interest;
                        bestSlide = slides[i];

                        //Console.WriteLine($"better match for slide {counter} is slide {i} with interest {interest}");
                    }
                }

                AssignSlide(slides, bestSlide);
                current = bestSlide;
                counter++;
            }
        }

        private void AssignSlide(List<Slide> slides, Slide slide)
        {
            this.Slides.Add(slide);
            slides.Remove(slide);
        }

        //private void AssignPhoto(List<Photo> photos, Slide slide)
        //{
        //    this.Slides.Add(slide);
        //    slides.Remove(slide);
        //}


        public List<Slide> Slides { get; } = new List<Slide>();

        public int Score { get; private set; }

        public int CalculateValues()
        {
            this.Score = 0;

            var previous = this.Slides[0];
            var previousInterest = 0;

            for (int i = 1; i < this.Slides.Count(); i++)
            {
                var current = this.Slides[i];
                var nextInterest = InterestCalculator.CalculateFactorOfInterest(current.Tags, previous.Tags);
                current.Value = previousInterest + nextInterest;

                this.Score += nextInterest;
                previousInterest = nextInterest;
                previous = current;
            }

            return this.Score;
        }

        // todo: calculate value from slides
        public int Value()
        {
            int i = 0;
            Slide Prev = null;

            foreach (Slide Current in Slides)
            {
                if (Prev != null)
                {
                    i += InterestCalculator.CalculateFactorOfInterest(Current.Tags, Prev.Tags);
                }

                Prev = Current;
            }

            return i;
        }
    }
}