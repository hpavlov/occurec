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

namespace OccuRec.ASCOM
{
    public partial class frmTelescopeControl : Form
    {
	    private IObservatoryController m_ObservatoryController = null;

	    internal IObservatoryController ObservatoryController
	    {
		    set
		    {
			    m_ObservatoryController = value;
				Text = string.Format("Telescope Control - {0}", m_ObservatoryController.ConnectedTelescopeDriverName());
		    }
			get { return m_ObservatoryController; }
	    }

        public frmTelescopeControl()
        {
            InitializeComponent();
        }

        private void OnPulseCompleted(ObservatoryControllerCallbackArgs args)
        {
            DisableEnableControls(true);
        }

        private void btnPulseNorth_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.TelescopePulseGuide(GuideDirections.guideNorth, PulseRate.Slowest, CallType.Async, OnPulseCompleted);
        }

        private void btnPulseNorth2_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.TelescopePulseGuide(GuideDirections.guideNorth, PulseRate.Slow, CallType.Async, OnPulseCompleted);
        }

        private void btnPulseNorth3_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.TelescopePulseGuide(GuideDirections.guideNorth, PulseRate.Fast, CallType.Async, OnPulseCompleted);
        }

        private void btnPulseSouth_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.TelescopePulseGuide(GuideDirections.guideSouth, PulseRate.Slowest, CallType.Async, OnPulseCompleted);
        }

        private void btnPulseSouth2_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.TelescopePulseGuide(GuideDirections.guideSouth, PulseRate.Slow, CallType.Async, OnPulseCompleted);
        }

        private void btnPulseSouth3_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.TelescopePulseGuide(GuideDirections.guideSouth, PulseRate.Fast, CallType.Async, OnPulseCompleted);
        }

        private void btnPulseWest_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.TelescopePulseGuide(GuideDirections.guideEast, PulseRate.Slowest, CallType.Async, OnPulseCompleted);
        }

        private void btnPulseWest2_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.TelescopePulseGuide(GuideDirections.guideEast, PulseRate.Slow, CallType.Async, OnPulseCompleted);
        }

        private void btnPulseWest3_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.TelescopePulseGuide(GuideDirections.guideEast, PulseRate.Fast, CallType.Async, OnPulseCompleted);
        }

        private void btnPulseEast_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.TelescopePulseGuide(GuideDirections.guideWest, PulseRate.Slowest, CallType.Async, OnPulseCompleted);
        }

        private void btnPulseEast2_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.TelescopePulseGuide(GuideDirections.guideWest, PulseRate.Slow, CallType.Async, OnPulseCompleted);
        }

        private void btnPulseEast3_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.TelescopePulseGuide(GuideDirections.guideWest, PulseRate.Fast, CallType.Async, OnPulseCompleted);
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
    }
}
