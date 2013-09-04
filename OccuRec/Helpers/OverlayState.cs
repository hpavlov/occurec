using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Video.AstroDigitalVideo;

namespace OccuRec.Helpers
{
	public abstract class OverlayState
	{
		public abstract void Initialise();
		public abstract void Finalise();
		public abstract void ProcessFrame(Graphics g);

		public virtual void MouseMove(MouseEventArgs e)
		{ }

		public virtual void MouseLeave(EventArgs e)
		{ }

		public virtual void MouseDown(MouseEventArgs e)
		{ }

		public virtual void MouseUp(MouseEventArgs e)
		{ }
	}
}
