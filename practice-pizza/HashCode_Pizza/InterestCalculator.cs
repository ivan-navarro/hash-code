using System;
using System.Collections.Generic;
using System.Linq;

namespace HashCode_Pizza
{
    internal static class InterestCalculator
    {
        public static int SlideInterest(Slide slide1, Slide slide2)
        {
            return CalculateFactorOfInterest(slide1.Tags, slide2.Tags);
        }

        public static int CalculateFactorOfInterest(IEnumerable<string> tags1, IEnumerable<string> tags2)
        {
            var commonTags = CalculateCommonTags(tags1, tags2);

            var tagsUniqueFrom1 = tags1.Count() - commonTags;

            var tagsUniqueFrom2 = tags2.Count() - commonTags;

            return Math.Min(Math.Min(commonTags, tagsUniqueFrom1), tagsUniqueFrom2);
        }

        public static int CalculateFactorOfInterest(List<string> tags1, List<string> tags2)
        {
            var commonTags = CalculateCommonTags(tags1, tags2);

            var tagsUniqueFrom1 = tags1.Count - commonTags;

            var tagsUniqueFrom2 = tags2.Count - commonTags;

            return Math.Min(Math.Min(commonTags, tagsUniqueFrom1), tagsUniqueFrom2);
        }

        public static int CalculateFactorOfInterest(HashSet<string> tags1, HashSet<string> tags2)
        {
            var commonTags = CalculateCommonTags(tags1, tags2);

            var tagsUniqueFrom1 = tags1.Count - commonTags;

            var tagsUniqueFrom2 = tags2.Count - commonTags;

            return Math.Min(Math.Min(commonTags, tagsUniqueFrom1), tagsUniqueFrom2);
        }

        private static int CalculateUniqueTags(IEnumerable<string> tags1, IEnumerable<string> tags2)
        {
            return tags1.Except(tags2).Count();
        }

        private static int CalculateCommonTags(IEnumerable<string> tags1, IEnumerable<string> tags2)
        {
            return tags1.Intersect(tags2).Count();
        }
    }
}
