/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.OCR
{
    public class OcredChar
    {
        private int charId;
        private int left;
        private int charWidth;
        private int charHeight;

        public OcredChar(int charId, int left, int charWidth, int charHeight)
        {
            this.charId = charId;
            this.left = left;
            this.charWidth = charWidth;
            this.charHeight = charHeight;
	        this.FailedToRecognizeCorrectly = false;
        }

        internal void PopulateZones(List<OcrZone> zones)
        {
            foreach(OcrZone zone in zones)
            {
                Zones.Add(new int[zone.Pixels.Count]);
            }
        }

        public List<int[]> Zones = new List<int[]>();

        public int CharId { get { return charId; }}

        public int LeftFrom { get { return left; } }

        public int LeftTo { get { return left + charWidth - 1; } }

        public char RecognizedChar { get; set; }

		public bool FailedToRecognizeCorrectly { get; set; }

        public bool IsInsideChar(int left, int charTop)
        {
            return this.left >= left && this.left + charWidth < left && charTop <= charHeight;
        }

        internal double[] ComputeZones()
        {
            var rv = new double[Zones.Count];

            for (int i = 0; i < Zones.Count; i++)
            {
                rv[i] = Zones[i].Average();
            }

            return rv;
        }

		internal void ComputeSplitZones(out double[] topZones, out double[] bottomZones)
		{
			topZones = new double[Zones.Count];
			bottomZones = new double[Zones.Count];

			for (int i = 0; i < Zones.Count; i++)
			{
				int bottomStartPos = Zones[i].Length / 2;
				double topSum = 0;
				double bottomSum = 0;

				for (int j = 0; j < Zones[i].Length; j++)
				{
					if (j < bottomStartPos)
						topSum += Zones[i][j];
					else
						bottomSum += Zones[i][j];
				}

				topZones[i] = topSum / bottomStartPos;
				bottomZones[i] = bottomSum / bottomStartPos;
			}
		}
    }
}
