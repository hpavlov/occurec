using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Helpers;

namespace OccuRec.ASCOM
{
	public partial class frmFocusControl : Form
	{
		internal TelescopeController TelescopeController;

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

				lblAbsRel.Text = state.Absolute ? "Absolte" : "Relative";
				if (!state.TempCompAvailable)
				{
					cbxTempComp.Enabled = false;
					cbxTempComp.Checked = false;
					lblTemp.Visible = false;
				}
				else
				{
					cbxTempComp.Enabled = true;
					cbxTempComp.Checked = state.TempComp;
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

		private void btnMoveIn_Click(object sender, EventArgs e)
		{
			DisableEnableControls(false);
			TelescopeController.FocuserMove(-10, UpdateFocuserStateOutOfThread);
		}

		private void btnMoveOut_Click(object sender, EventArgs e)
		{
			DisableEnableControls(false);
			TelescopeController.FocuserMove(10, UpdateFocuserStateOutOfThread);
		}

		private void DisableEnableControls(bool enabled)
		{
			btnMoveIn.Enabled = enabled;
			btnMoveOut.Enabled = enabled;
			cbxTempComp.Enabled = enabled;
		}
	}
}
