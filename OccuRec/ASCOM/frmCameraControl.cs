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

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);

			ObservatoryController = null;
		}

		private IObservatoryController m_ObservatoryController;

		public IObservatoryController ObservatoryController
		{
			set
			{
				if (m_ObservatoryController != null && value == null)
				{
					m_ObservatoryController.VideoStateUpdated -= m_ObservatoryController_VideoStateUpdated;
					m_ObservatoryController.VideoError -= m_ObservatoryController_VideoError;
				}
				m_ObservatoryController = value;
				if (m_ObservatoryController != null)
				{
					m_ObservatoryController.VideoStateUpdated += m_ObservatoryController_VideoStateUpdated;
                    m_ObservatoryController.VideoError += m_ObservatoryController_VideoError;
				}

				Text = m_ObservatoryController != null ? string.Format("Camera Control - {0}", m_ObservatoryController.ConnectedVideoCameraDriverName()) : "Camera Control";
			}
			private get { return m_ObservatoryController; }
		}

        void m_ObservatoryController_VideoError(string error)
        {
            SetSize(true);
            tbxErrors.AppendText(error + "\r\n");
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

			if (!btnOSDSet.Enabled)
			{
				btnOSDUp.Enabled = true;
				btnOSDDown.Enabled = true;
				btnOSDLeft.Enabled = true;
				btnOSDRight.Enabled = true;
				btnOSDSet.Enabled = true;				
			}
		}

		public frmCameraControl()
		{
			InitializeComponent();

            SetSize(false);
		}

		private void frmCameraControl_Shown(object sender, EventArgs e)
		{
            m_ObservatoryController.GetCameraState();
		}

		private void miDisconnect_Click(object sender, EventArgs e)
		{
			SetDisabledStateControls();
			ObservatoryController.DisconnectVideoCamera();
			Close();
		}

		private void SetDisabledStateControls()
		{
			btnExposureDown.Enabled = false;
			btnExposureUp.Enabled = false;
			btnGainDown.Enabled = false;
			btnGainUp.Enabled = false;
			btnGammaDown.Enabled = false;
			btnGammaUp.Enabled = false;
			btnOSDUp.Enabled = false;
			btnOSDDown.Enabled = false;
			btnOSDLeft.Enabled = false;
			btnOSDRight.Enabled = false;
			btnOSDSet.Enabled = false;

			tbxErrors.Clear();
			SetSize(false);
		}

		private void frmCameraControl_Load(object sender, EventArgs e)
		{
			if (!ObservatoryController.Supports5ButtonOSD)
				tcControls.TabPages.Remove(tabOSDControl);

			SetDisabledStateControls();
			ObservatoryController.GetCameraState();
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

        private void SetSize(bool errorMode)
        {
            if (errorMode)
            {
				Height = 268;
                tbxErrors.Visible = true;
            }
            else
            {
                Height = 205;
                tbxErrors.Visible = false;
            }
        }

        private void btnOSDUp_Click(object sender, EventArgs e)
        {
            ObservatoryController.CameraOSDUp();
        }

        private void btnOSDDown_Click(object sender, EventArgs e)
        {
            ObservatoryController.CameraOSDDown();
        }

        private void btnOSDRight_Click(object sender, EventArgs e)
        {
            ObservatoryController.CameraOSDRight();
        }

        private void btnOSDLeft_Click(object sender, EventArgs e)
        {
            ObservatoryController.CameraOSDLeft();
        }

        private void btnOSDSet_Click(object sender, EventArgs e)
        {
            ObservatoryController.CameraOSDSet();
        }

		private void miReset_Click(object sender, EventArgs e)
		{
			SetDisabledStateControls();
			ObservatoryController.DisconnectVideoCamera(CallType.Async, null, (arg) => ObservatoryController.TryConnectVideoCamera());
		}

	}
}
