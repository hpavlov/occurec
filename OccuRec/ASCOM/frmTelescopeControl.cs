using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.ASCOM.Wrapper.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.FrameAnalysis;

namespace OccuRec.ASCOM
{
    public partial class frmTelescopeControl : Form
    {

		private FrameAnalysisManager m_AnalisysManager;

		public frmTelescopeControl()
		{
			InitializeComponent();
		}

		public frmTelescopeControl(FrameAnalysisManager analisysManager)
		{
			InitializeComponent();

			m_AnalisysManager = analisysManager;
		}

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

	    private IObservatoryController m_ObservatoryController = null;

	    internal IObservatoryController ObservatoryController
	    {
		    set
		    {
				if (m_ObservatoryController != value)
				{
					if (m_ObservatoryController != null)
					{
						m_ObservatoryController.TelescopeStateUpdated -= TelescopeStateUpdated;
						m_ObservatoryController.TelescopeConnectionChanged -= FocuserConnectionChanged;
					}

					m_ObservatoryController = value;
					m_ObservatoryController.TelescopeStateUpdated += TelescopeStateUpdated;
					m_ObservatoryController.TelescopeConnectionChanged += FocuserConnectionChanged;
				}

				Text = m_ObservatoryController != null ? string.Format("Telescope Control - {0}", m_ObservatoryController.ConnectedTelescopeDriverName()) : "Telescope Control";
		    }
			get { return m_ObservatoryController; }
	    }

		private void FocuserConnectionChanged(ASCOMConnectionState state)
	    {
		    if (state == ASCOMConnectionState.Connected || state == ASCOMConnectionState.Ready)
				DisableEnableControls(true);
			else if (state == ASCOMConnectionState.Disconnected || state == ASCOMConnectionState.Engaged)
				DisableEnableControls(false);
	    }

		void TelescopeStateUpdated(TelescopeState state)
		{
			
		}

        private void OnPulseCompleted(ObservatoryControllerCallbackArgs args)
        {
            DisableEnableControls(true);
        }

        private void btnPulseNorth_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
			ObservatoryController.TelescopePulseGuide(GuideDirections.guideNorth, PulseRate.Slowest, CallType.Async, null, OnPulseCompleted);
        }

        private void btnPulseNorth2_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
			ObservatoryController.TelescopePulseGuide(GuideDirections.guideNorth, PulseRate.Slow, CallType.Async, null, OnPulseCompleted);
        }

        private void btnPulseNorth3_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
			ObservatoryController.TelescopePulseGuide(GuideDirections.guideNorth, PulseRate.Fast, CallType.Async, null, OnPulseCompleted);
        }

        private void btnPulseSouth_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
			ObservatoryController.TelescopePulseGuide(GuideDirections.guideSouth, PulseRate.Slowest, CallType.Async, null, OnPulseCompleted);
        }

        private void btnPulseSouth2_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
			ObservatoryController.TelescopePulseGuide(GuideDirections.guideSouth, PulseRate.Slow, CallType.Async, null, OnPulseCompleted);
        }

        private void btnPulseSouth3_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
			ObservatoryController.TelescopePulseGuide(GuideDirections.guideSouth, PulseRate.Fast, CallType.Async, null, OnPulseCompleted);
        }

        private void btnPulseWest_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
			ObservatoryController.TelescopePulseGuide(GuideDirections.guideEast, PulseRate.Slowest, CallType.Async, null, OnPulseCompleted);
        }

        private void btnPulseWest2_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
			ObservatoryController.TelescopePulseGuide(GuideDirections.guideEast, PulseRate.Slow, CallType.Async, null, OnPulseCompleted);
        }

        private void btnPulseWest3_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
			ObservatoryController.TelescopePulseGuide(GuideDirections.guideEast, PulseRate.Fast, CallType.Async, null, OnPulseCompleted);
        }

        private void btnPulseEast_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
			ObservatoryController.TelescopePulseGuide(GuideDirections.guideWest, PulseRate.Slowest, CallType.Async, null, OnPulseCompleted);
        }

        private void btnPulseEast2_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
			ObservatoryController.TelescopePulseGuide(GuideDirections.guideWest, PulseRate.Slow, CallType.Async, null, OnPulseCompleted);
        }

        private void btnPulseEast3_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
			ObservatoryController.TelescopePulseGuide(GuideDirections.guideWest, PulseRate.Fast, CallType.Async, null, OnPulseCompleted);
        }

        private void DisableEnableControls(bool enabled)
        {
            btnPulseEast.Enabled = enabled;
            btnPulseEast2.Enabled = enabled;
            btnPulseEast3.Enabled = enabled;
            btnPulseWest.Enabled = enabled;
            btnPulseWest2.Enabled = enabled;
            btnPulseWest3.Enabled = enabled;
            btnPulseNorth.Enabled = enabled;
            btnPulseNorth2.Enabled = enabled;
            btnPulseNorth3.Enabled = enabled;
            btnPulseSouth.Enabled = enabled;
            btnPulseSouth2.Enabled = enabled;
            btnPulseSouth3.Enabled = enabled;
        }

		private void miDisconnect_Click(object sender, EventArgs e)
		{
			ObservatoryController.DisconnectTelescope();
			Close();
		}

		private void miCalibratePulseGuiding_Click(object sender, EventArgs e)
		{
			m_AnalisysManager.TriggerPulseGuidingCalibration();
		}
    }
}
