using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OccuRec.ASCOM
{
	public partial class frmCameraControl : Form
	{
		private IObservatoryController m_ObservatoryController;

		public IObservatoryController ObservatoryController
		{
			set { m_ObservatoryController = value; }
			private get { return m_ObservatoryController; }
		}

		public frmCameraControl()
		{
			InitializeComponent();
		}

	}
}
