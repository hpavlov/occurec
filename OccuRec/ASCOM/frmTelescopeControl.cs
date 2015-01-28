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

		private void ReadPulseRateOrSlewRate(out PulseRate? rate, out double? distanceArcSec)
		{
			distanceArcSec = null;
			rate = null;

			if (rbSlow.Checked || rbSlowest.Checked || rbFast.Checked)
			{
				rate = PulseRate.Slowest;
				if (rbSlow.Checked)
					rate = PulseRate.Slow;
				else if (rbFast.Checked)
					rate = PulseRate.Fast;
				return;
			}
			else
			{
				if (rb05MinPerSec.Checked)
					distanceArcSec = 0.5;
				else if (rb2MinPerSec.Checked)
					distanceArcSec = 2;
				else if (rb5MinPerSec.Checked)
					distanceArcSec = 5;
				else if (rb10MinPerSec.Checked)
					distanceArcSec = 10;
				else if (rb30minPerSec.Checked)
					distanceArcSec = 30;
			}
		}

		private void MoveToDirection(GuideDirections direction)
		{
			PulseRate? rate;
			double? degreesPerMinute;
			ReadPulseRateOrSlewRate(out rate, out degreesPerMinute);

			if (rate.HasValue)
			{
				DisableEnableControls(false);
				ObservatoryController.TelescopePulseGuide(direction, rate.Value, CallType.Async, null, OnPulseCompleted);
			}
			else if (degreesPerMinute.HasValue)
			{
				DisableEnableControls(false);
                if (degreesPerMinute == 0.5)
                    ObservatoryController.TelescopeSetSlewRate(double.NaN, callback: (arg) => ObservatoryController.TelescopeStartSlewing(direction, CallType.Async, null, OnPulseCompleted));
                else if (degreesPerMinute == 2)
                    ObservatoryController.TelescopeSetSlewRate(0, callback: (arg) => ObservatoryController.TelescopeStartSlewing(direction, CallType.Async, null, OnPulseCompleted)); 
				else
                    ObservatoryController.TelescopeSetSlewRate(degreesPerMinute.Value / 60.0, callback: (arg) => ObservatoryController.TelescopeStartSlewing(direction, CallType.Async, null, OnPulseCompleted)); 
			}
		}

        private void btnPulseNorth_Click(object sender, EventArgs e)
        {
	        MoveToDirection(GuideDirections.guideNorth);
        }

        private void btnPulseSouth_Click(object sender, EventArgs e)
        {
			MoveToDirection(GuideDirections.guideSouth);
        }

        private void btnPulseWest_Click(object sender, EventArgs e)
        {
            MoveToDirection(GuideDirections.guideEast);
        }

        private void btnPulseEast_Click(object sender, EventArgs e)
        {
            MoveToDirection(GuideDirections.guideWest);
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
	        frm.IsSyncMode = false;
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                m_ObservatoryController.TelescopeSlewTo(frm.RAHours, frm.DEDeg);
            }
        }

		private void rbFast_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void miSyncPosition_Click(object sender, EventArgs e)
		{
			var frm = new frmEnterCoordinates();
			frm.IsSyncMode = false;
			if (frm.ShowDialog(this) == DialogResult.OK)
			{
				m_ObservatoryController.TelescopeSyncToCoordinates(frm.RAHours, frm.DEDeg);
			}
		}

		private void btnGetPosition_Click(object sender, EventArgs e)
		{
			ObservatoryController.GetTelescopeState();
		}

        private void btnStopSlew_Click(object sender, EventArgs e)
        {
            m_ObservatoryController.TelescopeStopSlewing();
        }
    }
}
