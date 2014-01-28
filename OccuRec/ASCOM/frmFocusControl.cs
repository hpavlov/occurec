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
		internal ObservatoryController ObservatoryController;

		public frmFocusControl()
		{
			InitializeComponent();
		}

		private void frmFocusControl_Shown(object sender, EventArgs e)
		{
			ObservatoryController.GetFocuserState(UpdateFocuserStateOutOfThread);
		}

		private void UpdateFocuserStateOutOfThread(FocuserState state)
		{
			Invoke(new Action<FocuserState>(UpdateFocuserState), state);
		}

		private void UpdateFocuserState(FocuserState state)
		{
			if (state != null)
			{
				Trace.WriteLine(state.AsXmlString());

				DisableEnableControls(true);

                Text = state.Absolute ? "Focus Control - Absolte" : "Focus Control - Relative";
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

                if (double.IsNaN(state.Temperature))
                {
                    lblTemp.Visible = false;
                }
                else
                {
                    lblTemp.Text = state.Temperature.ToString("0.0") + "°";
                    lblTemp.Visible = true;
                }

				lblPosition.Text = state.Position.ToString();

				pnlFocuserControls.Enabled = true;
			}
			else
			{
				pnlFocuserControls.Enabled = false;
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
                ObservatoryController.FocuserSetTempComp(cbxTempComp.Checked, UpdateFocuserStateOutOfThread);
            }
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.FocuserMove((int)nudMove.Value, UpdateFocuserStateOutOfThread);
        }

        private void btnInSmall_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.FocuserMoveIn(FocuserStepSize.Smallest, UpdateFocuserStateOutOfThread);
        }

        private void btnOutSmall_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.FocuserMoveOut(FocuserStepSize.Smallest, UpdateFocuserStateOutOfThread);
        }

        private void btnInLarge_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.FocuserMoveIn(FocuserStepSize.Small, UpdateFocuserStateOutOfThread);
        }

        private void btnOutLarge_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.FocuserMoveOut(FocuserStepSize.Small, UpdateFocuserStateOutOfThread);
        }

        private void btnInLargest_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.FocuserMoveIn(FocuserStepSize.Large, UpdateFocuserStateOutOfThread);
        }

        private void btnOutLargest_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            ObservatoryController.FocuserMoveOut(FocuserStepSize.Large, UpdateFocuserStateOutOfThread);
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            ObservatoryController.DisconnectFocuser();
            Close();
        }
	}
}
