using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Drivers;

namespace OccuRec
{
	public partial class frmOneStacking : Form
	{
		internal VideoCameraFrameRate? FrameRate;
		internal int StackRate { get; private set; }

		private double m_SingleFrameDurationSeconds = 0;
		private int m_CurrentStackingRate = 0;

		public frmOneStacking()
		{
			InitializeComponent();

			StackRate = 0;
		}

		internal void SetCurrentStackingRate(int currentStackingRate)
		{
			m_CurrentStackingRate = currentStackingRate;
		}

		private void cbxStackRate_SelectedIndexChanged(object sender, EventArgs e)
		{
			StackRate = Convert.ToInt32(cbxStackRate.SelectedItem);

			double stackAccuracy = 0.5 * StackRate * m_SingleFrameDurationSeconds;
			if (stackAccuracy == 0)
				lblEffectiveTimeResolution.Text = "N/A";
			else
				lblEffectiveTimeResolution.Text = stackAccuracy.ToString("0.00 sec");
		}

		private void frmOneStacking_Load(object sender, EventArgs e)
		{
			lblEffectiveTimeResolution.Text = "N/A";

			if (FrameRate.HasValue && FrameRate.Value == VideoCameraFrameRate.PAL)
			{
				m_SingleFrameDurationSeconds = 0.04;
			}
			else if (FrameRate.HasValue && FrameRate.Value == VideoCameraFrameRate.NTSC)
			{
				m_SingleFrameDurationSeconds = 0.0333667;
			}
			else
			{				
				m_SingleFrameDurationSeconds = 0;
			}

			if (m_CurrentStackingRate > 0)
			{
				int currIdx = cbxStackRate.Items.IndexOf(m_CurrentStackingRate.ToString());
				if (currIdx > -1)
					cbxStackRate.SelectedIndex = currIdx;

				btnRemoveStacking.Visible = true;
			}
			else
			{
				cbxStackRate.SelectedIndex = cbxStackRate.Items.Count - 1;
				btnRemoveStacking.Visible = false;
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			Close();
		}
	}
}
