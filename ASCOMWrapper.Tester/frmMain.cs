using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OccuRec.ASCOM.Wrapper;
using OccuRec.ASCOM.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;

namespace ASCOMWrapper.Tester
{
	public partial class frmMain : Form
	{
		private ASCOMClient m_Client;

		public frmMain()
		{
			InitializeComponent();			
		}

		private void button1_Click(object sender, EventArgs e)
		{
			tbxFocuserProgId.Text = m_Client.ChooseFocuser();
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			m_Client = new ASCOMClient();
			m_Client.Initialise(false);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			IASCOMFocuser focuser = m_Client.CreateFocuser(tbxFocuserProgId.Text);
			focuser.Connected = true;
			MessageBox.Show(focuser.Description);
		}
	}
}
