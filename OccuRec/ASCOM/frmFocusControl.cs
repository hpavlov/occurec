using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccRec.ASCOMWrapper.Devices;
using OccRec.ASCOMWrapper.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Helpers;

namespace OccuRec.ASCOM
{
	public partial class frmFocusControl : Form
	{
		internal ObservatoryController TelescopeController;

		public frmFocusControl()
		{
			InitializeComponent();
		}

		private void frmFocusControl_Shown(object sender, EventArgs e)
		{
			TelescopeController.GetFocuserState(UpdateFocuserStateOutOfThread);
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
					
					lblTemp.Visible = false;
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
			btnOutSmall.Enabled = enabled;
            btnOutLarge.Enabled = enabled;
			cbxTempComp.Enabled = enabled;
            btnMove.Enabled = enabled;
            btnFocusTarget.Enabled = enabled;
		}

	    private bool m_SettingTempCompValue = false;

        private void cbxTempComp_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_SettingTempCompValue)
            {
                TelescopeController.FocuserSetTempComp(cbxTempComp.Checked, UpdateFocuserStateOutOfThread);
            }
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            TelescopeController.FocuserMove((int)nudMove.Value, UpdateFocuserStateOutOfThread);
        }

        private void btnInSmall_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            TelescopeController.FocuserMoveIn(FocuserStepSize.Smallest, UpdateFocuserStateOutOfThread);
        }

        private void btnOutSmall_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            TelescopeController.FocuserMoveOut(FocuserStepSize.Smallest, UpdateFocuserStateOutOfThread);
        }

        private void btnInLarge_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            TelescopeController.FocuserMoveIn(FocuserStepSize.Small, UpdateFocuserStateOutOfThread);
        }

        private void btnOutLarge_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            TelescopeController.FocuserMoveOut(FocuserStepSize.Small, UpdateFocuserStateOutOfThread);
        }

        private void btnInLargest_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            TelescopeController.FocuserMoveIn(FocuserStepSize.Large, UpdateFocuserStateOutOfThread);
        }

        private void btnOutLargest_Click(object sender, EventArgs e)
        {
            DisableEnableControls(false);
            TelescopeController.FocuserMoveOut(FocuserStepSize.Large, UpdateFocuserStateOutOfThread);
        }
	}
}
