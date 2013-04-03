//tabs=4
// --------------------------------------------------------------------------------
//
// ASCOM Video Driver - Video Client
//
// Description:	The settings form
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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AAVRec.OCR;
using AAVRec.Properties;

namespace AAVRec
{
	public partial class frmSettings : Form
	{
	    private OCRSettings m_OCRSettings;

		public frmSettings()
		{
			InitializeComponent();

		    m_OCRSettings = OCRSettings.Instance;

		    nudSignDiffFactor.Value = Math.Min(10, Math.Max(1, (decimal)Settings.Default.SignatureDiffFactorEx2));
		    cbxGraphDebugMode.Checked = Settings.Default.VideoGraphDebugMode;
		}

        private void frmSettings_Load(object sender, EventArgs e)
        {
            lblArea1Config.Text = string.Format("T:{0}; L:{1}; W:{2}; H:{3}", m_OCRSettings.TimeStampArea1.Top, m_OCRSettings.TimeStampArea1.Left, m_OCRSettings.TimeStampArea1.Width, m_OCRSettings.TimeStampArea1.Height);
            lblArea2Config.Text = string.Format("T:{0}; L:{1}; W:{2}; H:{3}", m_OCRSettings.TimeStampArea2.Top, m_OCRSettings.TimeStampArea2.Left, m_OCRSettings.TimeStampArea2.Width, m_OCRSettings.TimeStampArea2.Height);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Settings.Default.VideoGraphDebugMode = cbxGraphDebugMode.Checked;
            Settings.Default.SignatureDiffFactorEx2 = Math.Min(10, Math.Max(1, (double)nudSignDiffFactor.Value));
            Settings.Default.Save();

            DialogResult = DialogResult.OK;
            Close();
        }
	}
}
