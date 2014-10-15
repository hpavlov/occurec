/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OccuRec.Tracking
{
	public interface ITrackedObjectPsfFit
	{
		double XCenter { get; }
		double YCenter { get; }
		double FWHM { get; }
		double IMax { get; }
		double I0 { get; }
		double X0 { get; }
		double Y0 { get; }
		int MatrixSize { get; }
		bool IsSolved { get; }

		double GetPSFValueAt(double x, double y);
		double GetResidualAt(int x, int y);

		void DrawDataPixels(Graphics g, Rectangle rect, float aperture, Pen aperturePen, int bpp);
		void DrawGraph(Graphics g, Rectangle rect, int bpp);
	}

	public interface ITrackedObject
	{
		IImagePixel Center { get; }
		IImagePixel LastKnownGoodPosition { get; set; }
		bool IsLocated { get; }
		bool IsOffScreen { get; }
		ITrackedObjectConfig OriginalObject { get; }
		int TargetNo { get; }
		ITrackedObjectPsfFit PSFFit { get; }
		uint GetTrackingFlags();
	}

	public interface ITrackedObjectConfig
	{
		float ApertureInPixels { get; }
		bool IsWeakSignalObject { get; }
		int PsfFitMatrixSize { get; }
		float RefinedFWHM { get; set; }
		float ApertureStartingX { get; }
		float ApertureStartingY { get; }
		TrackingType TrackingType { get; }
		bool IsFixedAperture { get; }
		ImagePixel AsImagePixel { get; }
		float PositionTolerance { get; }
		bool IsCloseToOtherStars { get; }
	}

	public interface IMeasurableObject
	{
		bool IsOccultedStar { get; }
		bool MayHaveDisappeared { get; }
		int PsfFittingMatrixSize { get; }
	}
}
