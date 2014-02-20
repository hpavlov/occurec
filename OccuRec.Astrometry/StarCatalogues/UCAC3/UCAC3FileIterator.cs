using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OccuRec.Astrometry.StarCatalogues.UCAC3
{
    public class UCAC3FileIterator
    {
        public static IEnumerator<string> UCAC3Files(string basePath)
        {
            for (int i = 1; i <= 360; i++)
            {
                yield return Path.GetFullPath(string.Format("{0}\\z{1}", basePath, i.ToString("000")));
            }
        }
    }
}
