using System.Collections.Generic;

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
        private int Value { get; }
    }
}