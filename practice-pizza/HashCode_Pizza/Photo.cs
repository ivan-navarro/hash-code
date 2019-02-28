using System.Collections.Generic;

namespace HashCode_Pizza
{
    internal class Photo
    {
        public int Id { get; }

        public Photo(int id, List<string> tags, string orientation)
        {
            this.Tags = tags;
            this.Orientation = orientation;
            this.Id = id;
        }

        public List<string> Tags { get; }

        public string Orientation { get; }

        public override string ToString()
        {
            return $"Id:{this.Id} Orientation:{this.Orientation} Tags: {string.Join(" ", this.Tags)}";
        }
    }
}