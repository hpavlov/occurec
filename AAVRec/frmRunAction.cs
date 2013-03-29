//tabs=4
// --------------------------------------------------------------------------------
//
// ASCOM Video Driver - Video Client
//
// Description:	Form to enter the action parameters to run an action
//
// Author:		(HDP) Hristo Pavlov <hristo_dpavlov@yahoo.com>
//
// --------------------------------------------------------------------------------
//


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AAVRec
{
	public partial class frmRunAction : Form
	{
		public frmRunAction()
		{
			InitializeComponent();
		}

		public void SetActionName(string actionName)
		{
			lblActionToRun.Text = actionName;
		}

		public string GetParameterValue()
		{
			return tbxParamValue.Text;
		}
	}
}
