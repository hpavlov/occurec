using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OccuRec.FieldIdentification.UCAC4
{
	public class UCAC4FileIterator
	{
		public static IEnumerator<string> UCAC4Files(string basePath)
		{
			for (int i = 1; i <= 900; i++)
			{
				yield return Path.GetFullPath(string.Format("{0}\\z{1}", basePath, i.ToString("000")));
			}
		}

	}
}
