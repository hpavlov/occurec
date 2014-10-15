/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.Helpers;
using OccuRec.OCR;

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
			OcrConfiguration config = OcrSettings.Instance[Name];
			return 
				config != null &&
				config.Alignment.Width == videoFormat.Width &&
				config.Alignment.Height == videoFormat.Height;
		}

		public override string ToString()
		{
			return Name;
		}

		internal static OcrConfigEntry EasyCAP = OcrConfigEntry.OcrVtiOsdEntry("Compatible EasyCap + IOTA-VTI (NON TV-Safe, PAL)");
	}
}
