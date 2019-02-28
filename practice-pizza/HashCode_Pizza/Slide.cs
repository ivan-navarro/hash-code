using System.Collections.Generic;
using System.Linq;

namespace HashCode_Pizza
{
    internal class Slide
    {
        public List<Photo> Photos { get; set; } = new List<Photo>();

        public HashSet<string> Tags { get; set; } = new HashSet<string>();


        public void LoadPhoto(Photo photo)
        {
            this.Photos.Add(photo);
            this.Tags.UnionWith(photo.Tags);
        }

        public override string ToString()
        {
            return string.Join(" ", this.Photos.Select(p => p.Id));
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