using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.Helpers;

namespace OccuRec.Config
{
	internal enum OcrConfigEntryType
	{
		VtiOsdNotAvailable,
		PreserveVtiOsd,
		OcrVtiOsd
	}

	internal class OcrConfigEntry
	{
		internal static OcrConfigEntry OcrVtiOsdEntry(string name)
		{
			return new OcrConfigEntry()
			{
				Name = name,
				EntryType = OcrConfigEntryType.OcrVtiOsd
			};
		}

		public OcrConfigEntryType EntryType;
		public string Name;

		public bool IsCompatible(VideoFormatHelper.SupportedVideoFormat videoFormat)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return Name;
		}

		internal static OcrConfigEntry Default = new OcrConfigEntry();
	}
}
