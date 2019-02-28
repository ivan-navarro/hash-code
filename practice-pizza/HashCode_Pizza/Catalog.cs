using System.Collections.Generic;
using System.Reflection;

namespace HashCode_Pizza
{
    internal class SlideShow
    {
        public SlideShow(List<Slide> slides)
        {
            this.Slides = slides;
        }

        public List<Slide> Slides { get; }

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
                    Prev = Current;
                } 
                Prev = Current;
            }
            return i;

        }
    }
}