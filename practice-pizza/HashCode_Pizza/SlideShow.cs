using System;
using System.Collections.Generic;
using System.Linq;

namespace HashCode_Pizza
{
    internal class SlideShow
    {
        public SlideShow(List<Slide> slides)
        {
            var sortedSlides = slides.OrderByDescending(s => s.Tags.Count).ToList();
            this.Arrange(sortedSlides);
        }

        private void Arrange(List<Slide> slides)
        {
            var current = slides[0];
            MoveSlide(slides, current);
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

                MoveSlide(slides, bestSlide);
                current = bestSlide;
                counter++;
            }
        }

        private void MoveSlide(List<Slide> slides, Slide slide)
        {
            this.Slides.Add(slide);
            slides.Remove(slide);
        }

        public List<Slide> Slides { get; } = new List<Slide>();

        public int CalculateValues()
        {
            int totalValue = 0;

            var previous = this.Slides[0];
            var previousInterest = 0;

            for (int i = 1; i < this.Slides.Count(); i++)
            {
                var current = this.Slides[i];
                var nextInterest = InterestCalculator.CalculateFactorOfInterest(current.Tags, previous.Tags);
                current.Value = previousInterest + nextInterest;

                totalValue += nextInterest;
                previousInterest = nextInterest;
                previous = current;
            }

            return totalValue;
        }

        // todo: calculate value from slides
        public int Value()
        {
            int i = 0;
            Slide Prev= null;

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