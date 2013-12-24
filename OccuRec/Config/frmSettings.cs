using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OccRec.ASCOMWrapper;
using OccuRec.ASCOM;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Config.Panels;
using OccuRec.Helpers;
using OccuRec.OCR;
using OccuRec.Properties;

namespace OccuRec.Config
{
	public partial class frmSettings : Form
	{
		private int m_CurrentPropertyPageId = -1;
		private Dictionary<int, SettingsPanel> m_PropertyPages = new Dictionary<int, SettingsPanel>();

		private SettingsPanel m_CurrentPanel = null;

		public frmSettings()
		{
			InitializeComponent();

			InitAllPropertyPages();

			foreach (SettingsPanel panel in m_PropertyPages.Values)
				panel.LoadSettings();
		}

		private void InitAllPropertyPages()
		{
			m_PropertyPages.Add(0, new ucGeneral());

			m_PropertyPages.Add(1, new ucAAV());
			m_PropertyPages.Add(2, new ucNTPTime());
			m_PropertyPages.Add(3, new ucTelescopeControl());
			m_PropertyPages.Add(4, new ucTraking());

#if !DEBUG
		    tabControl.TabPages.Remove(tabDebug);
#else
			m_PropertyPages.Add(5, new ucDebug());
#endif
		}

		private void SetFormTitle(TreeNode currentNode)
		{
			if (currentNode != null)
			{
				string newTitle = "OccuRec Settings";

				if (currentNode.Parent == null)
				{
					// Select the first sibling
					if (currentNode.Nodes.Count > 0)
						newTitle = string.Format("OccuRec Settings - {0} - {1}", currentNode.Text, currentNode.Nodes[0].Text);
					else
						newTitle = string.Format("OccuRec Settings - {0}", currentNode.Text);
				}
				else
					newTitle = string.Format("OccuRec Settings - {0} - {1}", currentNode.Parent.Text, currentNode.Text);

				this.Text = newTitle;
			}
		}

		void tvSettings_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			e.Cancel = m_CurrentPanel != null && !m_CurrentPanel.ValidateSettings();
		}

		private void tvSettings_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node != null)
			{
				int propPageId = int.Parse((string)e.Node.Tag);

				if (m_CurrentPropertyPageId != propPageId)
				{
					SettingsPanel propPage = null;
					if (m_PropertyPages.TryGetValue(propPageId, out propPage))
					{
						LoadPropertyPage(propPage);
						m_CurrentPanel = propPage;
						m_CurrentPropertyPageId = propPageId;
						SetFormTitle(e.Node);
					}
				}

				if (e.Node.Nodes.Count > 0)
					e.Node.Expand();
			}
		}

		private void LoadPropertyPage(Control propPage)
		{
			if (pnlPropertyPage.Controls.Count == 1)
				pnlPropertyPage.Controls.Remove(pnlPropertyPage.Controls[0]);

			if (propPage != null)
			{
				pnlPropertyPage.Controls.Add(propPage);
				propPage.Dock = DockStyle.Fill;
			}
		}


        private void btnOK_Click(object sender, EventArgs e)
        {
			if (m_CurrentPanel != null)
			{
				if (m_CurrentPanel.ValidateSettings())
				{
					foreach (SettingsPanel panel in m_PropertyPages.Values)
					{
						if (panel.ValidateSettings())
							panel.SaveSettings();
						else
						{
							m_CurrentPanel = panel;
							LoadPropertyPage(m_CurrentPanel);
							return;
						}
					}
				}
				else
					return;
			}

			Settings.Default.Save();

			foreach (SettingsPanel panel in m_PropertyPages.Values)
			{
				panel.OnPostSaveSettings();
			}

			DialogResult = DialogResult.OK;
			Close();
        }

		private void frmSettings_Load(object sender, EventArgs e)
		{
			tvSettings.SelectedNode = tvSettings.Nodes["ndGeneral"];
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btnResetDefaults_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(
				this,
				"You are about to reset all OccuRec settings to their default values. All customisations will be lost.\r\n\r\nDo you want to continue?",
				"OccuRec",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning,
				MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			{
				Settings.Default.Reset();

				foreach (SettingsPanel panel in m_PropertyPages.Values)
				{
					panel.LoadSettings();
					panel.Reset();
				}

				MessageBox.Show(
					this,
					"OccuRec settings have been reset.",
					"OccuRec",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
			}
		}
	}
}
