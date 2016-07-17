/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Helpers;
using OccuRec.Properties;
using OccuRec.Utilities;

namespace OccuRec.Config.Panels
{
	public partial class ucAdvanced : SettingsPanel
	{
		public ucAdvanced()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			cbDebugIntegration.Checked = Settings.Default.IntegrationDetectionTuning;
			cbxStatusSectionOnly.Checked = Settings.Default.RecordStatusSectionOnly;
			nudNTPDebugValue1.SetNUDValue(Settings.Default.NTPDebugConfigValue1);
			nudNTPDebugValue2.SetNUDValue(Settings.Default.NTPDebugConfigValue2);
			cbxLiveOCR.Checked = Settings.Default.EasyCAPOCR;
			cbxUserPreserveOSDLines.Checked = Settings.Default.PreserveVTIUserSpecifiedValues;
			nudPreserveVTITopRow.SetNUDValue(Settings.Default.PreserveVTIFirstRow);
			nudPreserveVTIBottomRow.SetNUDValue(Settings.Default.PreserveVTILastRow);

			cbxGraphDebugMode.Checked = Settings.Default.VideoGraphDebugMode;
			cbxSaveVtiOsdReport.Checked = Settings.Default.VtiOsdSaveReport;

			cbxImageLayoutMode.Items.Clear();
			cbxImageLayoutMode.Items.Add(AavImageLayout.CompressedRaw);
			cbxImageLayoutMode.Items.Add(AavImageLayout.CompressedDiffCodeNoSigns);
			cbxImageLayoutMode.Items.Add(AavImageLayout.CompressedDiffCodeWithSigns);
			cbxImageLayoutMode.Items.Add(AavImageLayout.UncompressedRaw);

			cbxAdvCompression.Items.Clear();
			cbxAdvCompression.Items.Add(AavCompression.QuickLZ);
			cbxAdvCompression.Items.Add(AavCompression.Lagarith16);

			cbxImageLayoutMode.SelectedIndex = cbxImageLayoutMode.Items.IndexOf(Settings.Default.AavImageLayout);
			cbxAdvCompression.SelectedIndex = cbxAdvCompression.Items.IndexOf(Settings.Default.AavCompression);

			cbxCustomAdvImageLayout.Checked = Settings.Default.AavImageLayout != AavImageLayout.CompressedRaw;
			cbxImageLayoutMode.Visible = Settings.Default.AavImageLayout != AavImageLayout.CompressedRaw;

			cbxCustomAdvCompression.Checked = Settings.Default.AavCompression != AavCompression.QuickLZ;
			cbxAdvCompression.Visible = Settings.Default.AavCompression != AavCompression.QuickLZ;

            cbxMustConfirmVTI.Checked = Settings.Default.VTIMustConfirmManually;

            cbxLocationCross.Checked = Settings.Default.DisplayLocationCross;
		    nudCrossX.SetNUDValue(Settings.Default.LocationCrossX);
            nudCrossY.SetNUDValue(Settings.Default.LocationCrossY);
            nudCorssTransparency.SetNUDValue(Settings.Default.LocationCrossTransparency);

		    cbxBeepOnStartStopRecording.Checked = Settings.Default.BeepOnStartStopRecording;
		}

		public override void SaveSettings()
		{
			Settings.Default.IntegrationDetectionTuning = cbDebugIntegration.Checked;
			Settings.Default.RecordStatusSectionOnly = cbxStatusSectionOnly.Checked;
			Settings.Default.NTPDebugConfigValue1 = (int)nudNTPDebugValue1.Value;
			Settings.Default.NTPDebugConfigValue2 = (float)nudNTPDebugValue2.Value;
			Settings.Default.EasyCAPOCR = cbxLiveOCR.Checked;
			Settings.Default.PreserveVTIUserSpecifiedValues = cbxUserPreserveOSDLines.Checked;
			Settings.Default.PreserveVTIFirstRow = (int)nudPreserveVTITopRow.Value;
			Settings.Default.PreserveVTILastRow = (int)nudPreserveVTIBottomRow.Value;
			Settings.Default.VtiOsdSaveReport = cbxSaveVtiOsdReport.Checked;
		    Settings.Default.VTIMustConfirmManually = cbxMustConfirmVTI.Checked;

			Settings.Default.VideoGraphDebugMode = cbxGraphDebugMode.Checked;
			Settings.Default.AavImageLayout = (AavImageLayout)cbxImageLayoutMode.SelectedItem;
			Settings.Default.AavCompression = (AavCompression)cbxAdvCompression.SelectedItem;

            Settings.Default.DisplayLocationCross = cbxLocationCross.Checked;
            Settings.Default.LocationCrossX = (int)nudCrossX.Value;
            Settings.Default.LocationCrossY = (int)nudCrossY.Value;
            Settings.Default.LocationCrossTransparency = (int)nudCorssTransparency.Value;
            Settings.Default.BeepOnStartStopRecording = cbxBeepOnStartStopRecording.Checked;
		}

		private void cbxUserPreserveOSDLines_CheckedChanged(object sender, EventArgs e)
		{
			pnlPreserveOSDArea.Enabled = cbxUserPreserveOSDLines.Checked;
		}

		private void cbxStatusSectionOnly_CheckedChanged(object sender, EventArgs e)
		{
			pnlNTPDebug.Enabled = cbxStatusSectionOnly.Checked;
		}

		private void cbxCustomAdvImageLayout_CheckedChanged(object sender, EventArgs e)
		{
			cbxImageLayoutMode.Visible = cbxCustomAdvImageLayout.Checked;
		}

		private void cbxCustomAdvCompression_CheckedChanged(object sender, EventArgs e)
		{
			cbxAdvCompression.Visible = cbxCustomAdvCompression.Checked;
		}

        private void cbxLocationCross_CheckedChanged(object sender, EventArgs e)
        {
            pnlLocationCorss.Enabled = true;
        }

		//private void LoadVTIConfig(int width, int height)
		//{
		//	var prevSelectedEntry = cbxOCRConfigurations.SelectedItem as OcrConfigEntry;

		//	cbxOCRConfigurations.Items.Clear();

		//	OcrConfigEntry[] matchingOcrConfigEntries = OcrSettings.Instance.Configurations
		//		.Where(x => !x.Hidden)
		//		.Select(x => OcrConfigEntry.OcrVtiOsdEntry(x.Name, x.Alignment.Width == width && x.Alignment.Height == height))
		//		.ToArray();

		//	cbxOCRConfigurations.Items.Add(new OcrConfigEntry() { Name = "VTI OSD not available", EntryType = OcrConfigEntryType.VtiOsdNotAvailable });

		//	cbxOCRConfigurations.Items.Add(new OcrConfigEntry() { Name = "Preserve VTI OSD", EntryType = OcrConfigEntryType.PreserveVtiOsd });

		//	cbxOCRConfigurations.Items.AddRange(matchingOcrConfigEntries);

		//	if (prevSelectedEntry != null && prevSelectedEntry.EntryType != OcrConfigEntryType.PreserveVtiOsd)
		//	{
		//		if (prevSelectedEntry.EntryType == OcrConfigEntryType.OcrVtiOsd)
		//			cbxOCRConfigurations.SelectedIndex = GetIndexOfOcrConfigEntry(prevSelectedEntry.Name, prevSelectedEntry.EntryType);
		//		else
		//			cbxOCRConfigurations.SelectedIndex = GetIndexOfOcrConfigEntry(null, prevSelectedEntry.EntryType);
		//	}

		//	if (cbxOCRConfigurations.SelectedIndex == -1 && !string.IsNullOrEmpty(Settings.Default.SelectedOcrConfiguration))
		//	{
		//		cbxOCRConfigurations.SelectedIndex = GetIndexOfOcrConfigEntry(Settings.Default.SelectedOcrConfiguration, OcrConfigEntryType.OcrVtiOsd);
		//	}

		//	if (cbxOCRConfigurations.SelectedIndex == -1)
		//	{
		//		cbxOCRConfigurations.SelectedIndex = GetIndexOfOcrConfigEntry(null, OcrConfigEntryType.PreserveVtiOsd); // Select 'Preserve VTI OSD'
		//	}
		//}

		//private int GetIndexOfOcrConfigEntry(string byName, OcrConfigEntryType byType)
		//{
		//	int selectedIndex = -1;
		//	for (int i = 0; i < cbxOCRConfigurations.Items.Count; i++)
		//	{
		//		if (!string.IsNullOrEmpty(byName))
		//		{
		//			if (((OcrConfigEntry)cbxOCRConfigurations.Items[i]).IsComptible &&
		//				cbxOCRConfigurations.Items[i].ToString() == byName)
		//			{
		//				selectedIndex = i;
		//				break;
		//			}
		//		}
		//		else if (byType == ((OcrConfigEntry)cbxOCRConfigurations.Items[i]).EntryType)
		//		{
		//			selectedIndex = i;
		//			break;
		//		}
		//	}

		//	return selectedIndex;
		//}



		//private VideoFormatHelper.SupportedVideoFormat m_CurrenctlySelectedVideoFormat;

		//private void cbxVideoFormats_SelectedIndexChanged(object sender, EventArgs e)
		//{
		//	var selectedFormat = (VideoFormatHelper.SupportedVideoFormat)cbxVideoFormats.SelectedItem;
		//	Settings.Default.SelectedVideoFormat = selectedFormat.AsSerialized();
		//	Settings.Default.Save();

		//	LoadVTIConfig(selectedFormat.Width, selectedFormat.Height);
		//}

		//private void cbxOCRConfigurations_SelectedIndexChanged(object sender, EventArgs e)
		//{
		//	var selectedFormat = (VideoFormatHelper.SupportedVideoFormat)cbxVideoFormats.SelectedItem;
		//	var selectedOcrItem = (OcrConfigEntry)cbxOCRConfigurations.SelectedItem;

		//	if (selectedOcrItem.EntryType == OcrConfigEntryType.PreserveVtiOsd && selectedFormat != null)
		//	{
		//		if (Settings.Default.PreserveVTIWidth == selectedFormat.Width && Settings.Default.PreserveVTIHeight == selectedFormat.Height)
		//		{
		//			nudPreserveVTITopRow.Value = Settings.Default.PreserveVTIFirstRow;
		//			nudPreserveVTIBottomRow.Value = Settings.Default.PreserveVTILastRow;
		//		}
		//		else
		//		{
		//			nudPreserveVTITopRow.Value = 0;
		//			nudPreserveVTIBottomRow.Value = 0;
		//		}

		//		pnlPreserveOSDArea.Enabled = true;
		//		pnlPreserveOSDArea.Visible = true;
		//	}
		//	else if (selectedOcrItem.EntryType == OcrConfigEntryType.OcrVtiOsd)
		//	{
		//		OcrConfiguration ocrConfig = OcrSettings.Instance.Configurations.SingleOrDefault(x => x.Name == cbxOCRConfigurations.Text);

		//		if (ocrConfig != null)
		//		{
		//			nudPreserveVTITopRow.Value = ocrConfig.Alignment.FrameTopOdd - 1;
		//			nudPreserveVTIBottomRow.Value = ocrConfig.Alignment.FrameTopOdd + (2 * ocrConfig.Alignment.CharHeight) + 2;
		//		}
		//		if (ocrConfig != null)
		//		{
		//			pnlPreserveOSDArea.Enabled = false;
		//			if (!selectedOcrItem.IsComptible)
		//			{
		//				MessageBox.Show(
		//					string.Format("'{0}' is not compatible with the current video mode '{1}'.", cbxOCRConfigurations.Text, cbxVideoFormats.SelectedItem.ToString()),
		//					"OccuRec", MessageBoxButtons.OK, MessageBoxIcon.Error);
		//			}
		//		}
		//		else
		//			pnlPreserveOSDArea.Enabled = true;

		//		pnlPreserveOSDArea.Visible = true;
		//	}
		//	else if (selectedOcrItem.EntryType == OcrConfigEntryType.VtiOsdNotAvailable)
		//	{
		//		pnlPreserveOSDArea.Visible = false;
		//	}
		//}

	}
}
