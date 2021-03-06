﻿/* This Source Code Form is subject to the terms of the Mozilla Public
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
using DirectShowLib;
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
            cbxNTPDebug.Checked = Settings.Default.NTPDebugEnabled;
			nudNTPDebugValue1.SetNUDValue(Settings.Default.NTPDebugConfigValue1);
			nudNTPDebugValue2.SetNUDValue(Settings.Default.NTPDebugConfigValue2);
            nudRestartRecordingMin.SetNUDValue(Settings.Default.StatusSectionOnlyRestartRecordingIntervalMins);
            cbxOCR.Checked = Settings.Default.EasyCAPOCR || Settings.Default.StarTechSVID2OCR;
            if (Settings.Default.EasyCAPOCR) cbxOCRType.SelectedIndex = 1;
            else if (Settings.Default.StarTechSVID2OCR) cbxOCRType.SelectedIndex = 0;
            else cbxOCRType.SelectedIndex = -1;
            nudOCRMinON.SetNUDValue(Settings.Default.OCRMinONLevel);
            nudOCRMaxOFF.SetNUDValue(Settings.Default.OCRMaxOFFLevel);
		    int flag = 0;
            flag += Settings.Default.CollectZoneStats ? 0x01 : 0x00;
            flag += Settings.Default.DisableFieldNoCheck ? 0x02 : 0x00;
            cbxOcrFlags.SelectedIndex = flag;

			cbxUserPreserveOSDLines.Checked = Settings.Default.PreserveVTIUserSpecifiedValues;
			nudPreserveVTITopRow.SetNUDValue(Settings.Default.PreserveVTIFirstRow);
			nudPreserveVTIBottomRow.SetNUDValue(Settings.Default.PreserveVTILastRow);
            nudSaturationWarning.SetNUDValue(Settings.Default.SaturationWarning);

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

            SetSelectedPALStandard(Settings.Default.PALStandard);
            SetSelectedNTSCStandard(Settings.Default.NTSCStandard);

            if (cbxPALStandard.SelectedIndex == -1) cbxPALStandard.SelectedIndex = 0;
		    if (cbxNTSCStandard.SelectedIndex == -1) cbxNTSCStandard.SelectedIndex = 0;
		}

		public override void SaveSettings()
		{
			Settings.Default.IntegrationDetectionTuning = cbDebugIntegration.Checked;
			Settings.Default.RecordStatusSectionOnly = cbxStatusSectionOnly.Checked;
		    Settings.Default.NTPDebugEnabled = cbxNTPDebug.Checked;
			Settings.Default.NTPDebugConfigValue1 = (int)nudNTPDebugValue1.Value;
			Settings.Default.NTPDebugConfigValue2 = (float)nudNTPDebugValue2.Value;
            Settings.Default.StatusSectionOnlyRestartRecordingIntervalMins = (int)nudRestartRecordingMin.Value;
            Settings.Default.SaturationWarning = (int)nudSaturationWarning.Value;
			Settings.Default.EasyCAPOCR = cbxOCR.Checked && cbxOCRType.SelectedIndex == 1;
            Settings.Default.StarTechSVID2OCR = cbxOCR.Checked && cbxOCRType.SelectedIndex == 0;
            Settings.Default.OCRMinONLevel = (int)nudOCRMinON.Value;
            Settings.Default.OCRMaxOFFLevel = (int)nudOCRMaxOFF.Value;
		    int flag = cbxOcrFlags.SelectedIndex;
            Settings.Default.CollectZoneStats = (flag & 0x01) == 0x01;
            Settings.Default.DisableFieldNoCheck = (flag & 0x02) == 0x02;

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

            Settings.Default.PALStandard = GetSelectedPALStandard();
            Settings.Default.NTSCStandard = GetSelectedNTSCStandard();
		}

	    private AnalogVideoStandard GetSelectedPALStandard()
	    {
	        if (cbxPALStandard.Text == "PAL_B") return AnalogVideoStandard.PAL_B;
            if (cbxPALStandard.Text == "PAL_D") return AnalogVideoStandard.PAL_D;
            if (cbxPALStandard.Text == "PAL_G") return AnalogVideoStandard.PAL_G;
            if (cbxPALStandard.Text == "PAL_H") return AnalogVideoStandard.PAL_H;
            if (cbxPALStandard.Text == "PAL_I") return AnalogVideoStandard.PAL_I;
            if (cbxPALStandard.Text == "PAL_M") return AnalogVideoStandard.PAL_M;
            if (cbxPALStandard.Text == "PAL_N") return AnalogVideoStandard.PAL_N;
            if (cbxPALStandard.Text == "PAL_60") return AnalogVideoStandard.PAL_60;

            return AnalogVideoStandard.PAL_B;
	    }

	    private void SetSelectedPALStandard(AnalogVideoStandard standard)
	    {
	        switch (standard)
	        {
                case AnalogVideoStandard.PAL_B:
	                cbxPALStandard.SelectedIndex = cbxPALStandard.Items.IndexOf("PAL_B");
                    return;

                case AnalogVideoStandard.PAL_D:
                    cbxPALStandard.SelectedIndex = cbxPALStandard.Items.IndexOf("PAL_D");
                    return;

                case AnalogVideoStandard.PAL_G:
                    cbxPALStandard.SelectedIndex = cbxPALStandard.Items.IndexOf("PAL_G");
                    return;

                case AnalogVideoStandard.PAL_H:
                    cbxPALStandard.SelectedIndex = cbxPALStandard.Items.IndexOf("PAL_H");
                    return;

                case AnalogVideoStandard.PAL_I:
                    cbxPALStandard.SelectedIndex = cbxPALStandard.Items.IndexOf("PAL_I");
                    return;

                case AnalogVideoStandard.PAL_M:
                    cbxPALStandard.SelectedIndex = cbxPALStandard.Items.IndexOf("PAL_M");
                    return;

                case AnalogVideoStandard.PAL_N:
                    cbxPALStandard.SelectedIndex = cbxPALStandard.Items.IndexOf("PAL_N");
                    return;

                case AnalogVideoStandard.PAL_60:
                    cbxPALStandard.SelectedIndex = cbxPALStandard.Items.IndexOf("PAL_60");
                    return;
	        }
	    }

        private AnalogVideoStandard GetSelectedNTSCStandard()
        {
            if (cbxNTSCStandard.Text == "NTSC_M") return AnalogVideoStandard.NTSC_M;
            if (cbxNTSCStandard.Text == "NTSC_M_J") return AnalogVideoStandard.NTSC_M_J;
            if (cbxNTSCStandard.Text == "NTSC_433") return AnalogVideoStandard.NTSC_433;

            return AnalogVideoStandard.NTSC_M;
        }

        private void SetSelectedNTSCStandard(AnalogVideoStandard standard)
        {
            switch (standard)
            {
                case AnalogVideoStandard.NTSC_M:
                    cbxNTSCStandard.SelectedIndex = cbxNTSCStandard.Items.IndexOf("NTSC_M");
                    return;

                case AnalogVideoStandard.NTSC_M_J:
                    cbxNTSCStandard.SelectedIndex = cbxNTSCStandard.Items.IndexOf("NTSC_M_J");
                    return;

                case AnalogVideoStandard.NTSC_433:
                    cbxNTSCStandard.SelectedIndex = cbxNTSCStandard.Items.IndexOf("NTSC_433");
                    return;
            }
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

        private void cbxOCR_CheckedChanged(object sender, EventArgs e)
        {
            pnlOCR.Visible = cbxOCR.Checked;
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
