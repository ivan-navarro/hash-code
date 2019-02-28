using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode_Pizza
{
    public class FactorOfInterestCalculator
    {
        public int CalculateFactorOfInterest(IList<string> tags1, IList<string> tags2)
        {
            var commonTags = CalculateCommonTags(tags1, tags2);

            var tagsUniqueFrom1 = CalculateUniqueTags(tags1, tags2);

            var tagsUniqueFrom2 = CalculateCommonTags(tags2, tags1);

            return Math.Min(Math.Min(commonTags, tagsUniqueFrom1), tagsUniqueFrom2);
        }

        private int CalculateUniqueTags(ICollection<string> tags1, ICollection<string> tags2)
        {
            return tags1.Except(tags2).Count();
        }

        private int CalculateCommonTags(ICollection<string> tags1, ICollection<string> tags2)
        {
            return tags1.Intersect(tags2).Count();
        }
    }
}
