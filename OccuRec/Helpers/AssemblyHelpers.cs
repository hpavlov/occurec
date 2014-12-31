using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OccuRec.Helpers
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ReleaseDateAttribute : Attribute
    {
        public DateTime ReleaseDate;

        public ReleaseDateAttribute(string releaseDate)
        {
            ReleaseDate = DateTime.ParseExact(releaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class BetaReleaseAttribute : Attribute
    { }
}
