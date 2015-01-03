/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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
using OccuRec.Utilities;

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
					    m_ObservatoryController.TelescopePositionChanged -= TelescopePositionChanged;
						m_ObservatoryController.TelescopeConnectionChanged -= FocuserConnectionChanged;
					}

					m_ObservatoryController = value;

					if (m_ObservatoryController != null)
					{
						m_ObservatoryController.TelescopeStateUpdated += TelescopeStateUpdated;
                        m_ObservatoryController.TelescopePositionChanged += TelescopePositionChanged;
						m_ObservatoryController.TelescopeConnectionChanged += FocuserConnectionChanged;						
					}
				}

				Text = m_ObservatoryController != null ? (m_ObservatoryController.ConnectedTelescopeDriverName() ?? "Telescope") : "Telescope";
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
		    TelescopePositionChanged(new TelescopeEquatorialPosition()
		        {
		            RightAscension = state.RightAscension,
		            Declination = state.Declination
		        });
		}

        void TelescopePositionChanged(TelescopeEquatorialPosition position)
        {
            tssRA.Text = AstroConvert.ToStringValue(position.RightAscension, "HHh MMm SSs");
            tssDE.Text = AstroConvert.ToStringValue(position.Declination, "+DD° MM' SS\"");
        }

        private void OnPulseCompleted(ObservatoryControllerCallbackArgs args)
        {
            DisableEnableControls(true);
        }

        private void btnPulseNorth_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);

            var rate = PulseRate.Slowest;
            if (rbSlow.Checked) 
                rate = PulseRate.Slow;
            else if (rbFast.Checked)
                rate = PulseRate.Fast;

            ObservatoryController.TelescopePulseGuide(GuideDirections.guideNorth, rate, CallType.Async, null, OnPulseCompleted);
        }

        private void btnPulseSouth_Click(object sender, EventArgs e)
        {
            var rate = PulseRate.Slowest;
            if (rbSlow.Checked)
                rate = PulseRate.Slow;
            else if (rbFast.Checked)
                rate = PulseRate.Fast;

            DisableEnableControls(false);
            ObservatoryController.TelescopePulseGuide(GuideDirections.guideSouth, rate, CallType.Async, null, OnPulseCompleted);
        }

        private void btnPulseWest_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);

            var rate = PulseRate.Slowest;
            if (rbSlow.Checked)
                rate = PulseRate.Slow;
            else if (rbFast.Checked)
                rate = PulseRate.Fast;

            ObservatoryController.TelescopePulseGuide(GuideDirections.guideEast, rate, CallType.Async, null, OnPulseCompleted);
        }

        private void btnPulseEast_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);

            var rate = PulseRate.Slowest;
            if (rbSlow.Checked)
                rate = PulseRate.Slow;
            else if (rbFast.Checked)
                rate = PulseRate.Fast;

            ObservatoryController.TelescopePulseGuide(GuideDirections.guideWest, rate, CallType.Async, null, OnPulseCompleted);
        }


        private void DisableEnableControls(bool enabled)
        {
            btnPulseEast.Enabled = enabled;
            btnPulseWest.Enabled = enabled;
            btnPulseNorth.Enabled = enabled;
            btnPulseSouth.Enabled = enabled;
        }

		private void miDisconnect_Click(object sender, EventArgs e)
		{
			ObservatoryController.DisconnectTelescope();
			Close();
		}

		private void miCalibratePulseGuiding_Click(object sender, EventArgs e)
		{
			if (!m_AnalisysManager.TriggerPulseGuidingCalibration())
				MessageBox.Show(this, "Pulse Guiding Calibration cannot be started right now. Plase try again later.", "OccuRec", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

        private void frmTelescopeControl_Shown(object sender, EventArgs e)
        {
            m_ObservatoryController.GetTelescopeState();
        }

        private void miSlew_Click(object sender, EventArgs e)
        {
            var frm = new frmEnterCoordinates();
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                m_ObservatoryController.TelescopeSlewTo(frm.RAHours, frm.DEDeg);
            }
        }
    }
}
