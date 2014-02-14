using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OccuRec.ASCOM
{
	public partial class frmCameraControl : Form
	{
		private IObservatoryController m_ObservatoryController;

		public IObservatoryController ObservatoryController
		{
			set
			{
				m_ObservatoryController = value;
				if (m_ObservatoryController != null)
				{
					m_ObservatoryController.VideoStateUpdated += m_ObservatoryController_VideoStateUpdated;
				}
			}
			private get { return m_ObservatoryController; }
		}

		void m_ObservatoryController_VideoStateUpdated(Interfaces.Devices.VideoState state)
		{
			lblExposure.Text = state.Exposure;
			lblGain.Text = state.Gain;
			lblGamma.Text = state.Gamma;

			btnExposureDown.Enabled = state.ExposureIndex > state.MinExposureIndex;
			btnExposureUp.Enabled = state.ExposureIndex < state.MaxExposureIndex;
			btnGainDown.Enabled = state.GainIndex > state.MinGainIndex;
			btnGainUp.Enabled = state.GainIndex < state.MaxGainIndex;
			btnGammaDown.Enabled = state.GammaIndex > state.MinGammaIndex;
			btnGammaUp.Enabled = state.GammaIndex < state.MaxGammaIndex;
		}

		public frmCameraControl()
		{
			InitializeComponent();

			if (!ObservatoryController.Supports5ButtonOSD)
				tcControls.TabPages.Remove(tabOSDControl);
		}

		private void btnOSDUp_Click(object sender, EventArgs e)
		{
			ObservatoryController.CameraOSDUp();
		}

		private void miDisconnect_Click(object sender, EventArgs e)
		{
			ObservatoryController.DisconnectTelescope();
			Close();
		}

		private void frmCameraControl_Load(object sender, EventArgs e)
		{
			btnExposureDown.Enabled = false;
			btnExposureUp.Enabled = false;
			btnGainDown.Enabled = false;
			btnGainUp.Enabled = false;
			btnGammaDown.Enabled = false;
			btnGammaUp.Enabled = false;

			ObservatoryController.GetFocuserState();
		}

		private void btnExposureDown_Click(object sender, EventArgs e)
		{
			ObservatoryController.CameraExposureDown();
		}

		private void btnExposureUp_Click(object sender, EventArgs e)
		{
			ObservatoryController.CameraExposureUp();
		}

		private void btnGainDown_Click(object sender, EventArgs e)
		{
			ObservatoryController.CameraGainDown();
		}

		private void btnGainUp_Click(object sender, EventArgs e)
		{
			ObservatoryController.CameraGainUp();
		}

		private void btnGammaDown_Click(object sender, EventArgs e)
		{
			ObservatoryController.CameraGammaDown();
		}

		private void btnGammaUp_Click(object sender, EventArgs e)
		{
			ObservatoryController.CameraGammaUp();
		}

	}
}
