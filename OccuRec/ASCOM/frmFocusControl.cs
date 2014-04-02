using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.ASCOM.Wrapper.Devices;
using OccuRec.ASCOM.Wrapper.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Helpers;
using OccuRec.Utilities;

namespace OccuRec.ASCOM
{
	public partial class frmFocusControl : Form
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

	    internal IObservatoryController ObservatoryController
	    {
	        set
	        {
		        if (m_ObservatoryController != value)
		        {
			        if (m_ObservatoryController != null)
			        {
				        m_ObservatoryController.FocuserStateUpdated -= UpdateFocuserState;
				        m_ObservatoryController.FocuserConnectionChanged -= FocuserConnectionChanged;
			        }

			        m_ObservatoryController = value;

			        if (m_ObservatoryController != null)
			        {
				        m_ObservatoryController.FocuserStateUpdated += UpdateFocuserState;
				        m_ObservatoryController.FocuserConnectionChanged += FocuserConnectionChanged;
			        }
		        }

				Text = m_ObservatoryController != null 
					? string.Format("Focuser Control - {0}", m_ObservatoryController.ConnectedFocuserDriverName()) 
					: "Focuser Control";
	        }
	    }

	    public frmFocusControl()
		{
			InitializeComponent();
		}

		private void frmFocusControl_Shown(object sender, EventArgs e)
		{
			UpdateFocuserState(null);
            m_ObservatoryController.GetFocuserState();
			if (m_ObservatoryController.IsConnectedToFocuser())
				Text = string.Format("");
		}

		private void UpdateFocuserState(FocuserState state)
		{
			if (state != null)
			{
				Trace.WriteLine(state.AsXmlString());

				DisableEnableControls(true);

                Text = string.Format("Focus Control - {0} ({1})", m_ObservatoryController.ConnectedFocuserDriverName(), state.Absolute ? "Absolte" : "Relative");
				if (!state.TempCompAvailable)
				{
                    cbxTempComp.Visible = false;
                    m_SettingTempCompValue = true;
                    try
                    {
                        cbxTempComp.Checked = false;
                    }
                    finally
                    {
                        m_SettingTempCompValue = false;
                    }
				}
				else
				{
                    cbxTempComp.Visible = true;
                    m_SettingTempCompValue = true;
                    try
                    {
                        cbxTempComp.Checked = state.TempComp;
                    }
                    finally
                    {
                        m_SettingTempCompValue = false;
                    }
				}

				lblPosition.Text = state.Position.ToString();

				pnlFocuserControls.Enabled = true;
				gbxTargetControl.Enabled = true;
			}
			else
			{
				pnlFocuserControls.Enabled = false;
				gbxTargetControl.Enabled = false;
			}
		}

		void FocuserConnectionChanged(ASCOMConnectionState state)
		{
			if (state == ASCOMConnectionState.Connected || state == ASCOMConnectionState.Ready)
			{
				DisableEnableControls(true);
			}
			else if (state == ASCOMConnectionState.Disconnected || state == ASCOMConnectionState.Engaged)
			{
				DisableEnableControls(false);
			}
		}

		private void DisableEnableControls(bool enabled)
		{
			btnInSmall.Enabled = enabled;
            btnInLarge.Enabled = enabled;
            btnInLargest.Enabled = enabled;
			btnOutSmall.Enabled = enabled;
            btnOutLarge.Enabled = enabled;
            btnOutLargest.Enabled = enabled;
			cbxTempComp.Enabled = enabled;
            btnMove.Enabled = enabled;
            btnFocusTarget.Enabled = enabled;
		}

	    private bool m_SettingTempCompValue = false;

        private void cbxTempComp_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_SettingTempCompValue)
            {
                m_ObservatoryController.FocuserSetTempComp(cbxTempComp.Checked);
            }
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            m_ObservatoryController.FocuserMove((int)nudMove.Value);
        }

        private void btnInSmall_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            m_ObservatoryController.FocuserMoveIn(FocuserStepSize.Smallest);
        }

        private void btnOutSmall_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            m_ObservatoryController.FocuserMoveOut(FocuserStepSize.Smallest);
        }

        private void btnInLarge_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            m_ObservatoryController.FocuserMoveIn(FocuserStepSize.Small);
        }

        private void btnOutLarge_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            m_ObservatoryController.FocuserMoveOut(FocuserStepSize.Small);
        }

        private void btnInLargest_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            m_ObservatoryController.FocuserMoveIn(FocuserStepSize.Large);
        }

        private void btnOutLargest_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            m_ObservatoryController.FocuserMoveOut(FocuserStepSize.Large);
        }

		private void miDisconnect_Click(object sender, EventArgs e)
		{
			m_ObservatoryController.DisconnectFocuser();
			Close();
		}
	}
}
