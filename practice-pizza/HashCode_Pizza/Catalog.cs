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
        private int Value()
        {
            int i = 0;
            Slide Current;
            Slide Prev;

            foreach (Slide Current in Slides)
            {
                if (Prev != null)
                {
                    i+= AssemblyAlgorithmIdAttribute.
                }
                else
                {
                    //first item! 
                    Prev = Current;
                }
            }

            return i;

        }
    }
}