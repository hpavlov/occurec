using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Properties;
using OccuRec.Utilities;

namespace OccuRec
{
	public partial class frmConfigureVtiOsdLines : Form
	{
		private int m_Height = 0;
		private int m_OldFirstLine = -1;
		private int m_OldLastLine = -1;

		public frmConfigureVtiOsdLines()
		{
			InitializeComponent();
		}

		internal void SetFrameHeight(int height)
		{
			m_Height = height;
		}

		private void frmConfigureVtiOsdLines_Load(object sender, EventArgs e)
		{
			if (m_Height <= 0)
			{
				nudPreserveVTIBottomRow.Enabled = false;
				nudPreserveVTITopRow.Enabled = false;
			}
			else
			{
				nudPreserveVTITopRow.Maximum = m_Height;
				nudPreserveVTIBottomRow.Maximum = m_Height;

				if (Settings.Default.PreserveVTIUserSpecifiedValues)
				{
					nudPreserveVTITopRow.SetNUDValue(Settings.Default.PreserveVTIFirstRow);
					nudPreserveVTIBottomRow.SetNUDValue(Settings.Default.PreserveVTILastRow);
				}
				else
				{
					nudPreserveVTITopRow.SetNUDValue(m_Height - 28);
					nudPreserveVTIBottomRow.SetNUDValue(m_Height);
				}
			}

			m_OldFirstLine = Settings.Default.PreserveVTIFirstRow;
			m_OldLastLine = Settings.Default.PreserveVTILastRow;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (nudPreserveVTITopRow.Value > nudPreserveVTIBottomRow.Value)
			{
				MessageBox.Show("The FROM row number must be smaller than the TO row number.", "OccuRec", MessageBoxButtons.OK, MessageBoxIcon.Error);
				nudPreserveVTITopRow.Focus();
				return;
			}

			Settings.Default.PreserveVTIUserSpecifiedValues = true;
			Settings.Default.PreserveVTIFirstRow = (int)nudPreserveVTITopRow.Value;
			Settings.Default.PreserveVTILastRow = (int)nudPreserveVTIBottomRow.Value;
			Settings.Default.Save();

			DialogResult = DialogResult.OK;
			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Settings.Default.PreserveVTIFirstRow = m_OldFirstLine;
			Settings.Default.PreserveVTILastRow = m_OldLastLine;
		}

		private void nudPreserveVTITopRow_ValueChanged(object sender, EventArgs e)
		{
			Settings.Default.PreserveVTIFirstRow = (int)nudPreserveVTITopRow.Value;			
		}

		private void nudPreserveVTIBottomRow_ValueChanged(object sender, EventArgs e)
		{
			Settings.Default.PreserveVTILastRow = (int)nudPreserveVTIBottomRow.Value;
		}
	}
}
