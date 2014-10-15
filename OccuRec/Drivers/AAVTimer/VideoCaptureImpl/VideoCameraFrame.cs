/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OccuRec.Helpers;

namespace OccuRec.Drivers.AAVTimer.VideoCaptureImpl
{
	internal class VideoCameraFrame
	{
		public object Pixels;
	    public Bitmap PreviewBitmap;
		public long FrameNumber;

        public long UniqueFrameNumber;

		public VideoFrameLayout ImageLayout;

		public ImageStatus ImageStatus;
	}
}
