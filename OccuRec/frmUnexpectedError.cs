
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OccuRec
{
	public partial class frmUnexpectedError : Form
	{
		private static frmUnexpectedError frmInstance = new frmUnexpectedError();

		public frmUnexpectedError()
		{
			InitializeComponent();
		}

		internal void SetErrorMessage(string errorMessage)
		{
			lblError.Text = errorMessage;
		}

		internal void SetErrorMessage(Exception error)
		{
			lblError.Text = string.Format("{0}: {1}", error.GetType(), error.Message);
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		public static void ShowErrorMessage(string errorMessage)
		{
			if (!frmInstance.Visible)
			{
				frmInstance.SetErrorMessage(errorMessage);
				frmInstance.Show();
			}
		}

		public static void ShowErrorMessage(Exception error)
		{
			if (!frmInstance.Visible)
			{
				frmInstance.SetErrorMessage(error);
				frmInstance.Show();
			}
		}

		private void frmUnexpectedError_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason != CloseReason.ApplicationExitCall &&
			    e.CloseReason != CloseReason.TaskManagerClosing &&
			    e.CloseReason != CloseReason.WindowsShutDown)
			{
				Hide();

				e.Cancel = true;
			}
		}
	}
}
