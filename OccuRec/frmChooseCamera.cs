using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OccuRec.CameraDrivers;
using OccuRec.Drivers.DirectShowCapture.VideoCaptureImpl;
using OccuRec.Helpers;
using OccuRec.OCR;
using OccuRec.Properties;
using DirectShowLib;
using System.Diagnostics;

namespace OccuRec
{
    public partial class frmChooseCamera : Form
    {
		internal enum OcrConfigEntryType
		{
			VtiOsdNotAvailable,
			PreserveVtiOsd,
			OcrVtiOsd
		}

		internal class OcrConfigEntry
		{
			internal static OcrConfigEntry OcrVtiOsdEntry(string name, bool isCompatible)
			{
				return new OcrConfigEntry()
				{
					Name = name,
					EntryType = OcrConfigEntryType.OcrVtiOsd,
					IsComptible = isCompatible
				};
			}

			public OcrConfigEntryType EntryType;
			public string Name;
			public bool IsComptible;

			public override string ToString()
			{
				return Name;
			}
		}

        public frmChooseCamera()
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(Settings.Default.OutputLocation))
            {
                Settings.Default.OutputLocation = Path.GetFullPath(string.Format("{0}\\Videos", AppDomain.CurrentDomain.BaseDirectory));
                if (!Directory.Exists(Settings.Default.OutputLocation))
                    Directory.CreateDirectory(Settings.Default.OutputLocation);
            }
        }

		public IOccuRecCameraController CameraControlDriver { get; private set; }

	    private void frmChooseCamera_Load(object sender, EventArgs e)
        {
			CameraControlDriver = null;

			cbFileSIM.Enabled = Settings.Default.OcrSimulatorTestMode && File.Exists(Settings.Default.SimulatorFilePath);

#if !DEBUG
			cbFileSIM.Checked = false;
			cbFileSIM.Visible = false;
			cbFileSIM.Enabled = false;
#endif

            cbxCameraModel.Text = Settings.Default.CameraModel;

            cbxCaptureDevices.Items.Clear();
            foreach (DsDevice ds in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
            {
                cbxCaptureDevices.Items.Add(ds.Name);
            }

            if (cbxCaptureDevices.Items.Count > 0)
            {
                if (cbxCaptureDevices.Items.Contains(Settings.Default.PreferredCaptureDevice))
                    cbxCaptureDevices.SelectedIndex = cbxCaptureDevices.Items.IndexOf(Settings.Default.PreferredCaptureDevice);
                else
                    cbxCaptureDevices.SelectedIndex = 0;
            }

            RadioButton rbCodec;

            List<SystemCodecEntry> systemCodecs = VideoCodecs.GetSupportedVideoCodecs();
            foreach (SystemCodecEntry codec in systemCodecs)
            {
                rbCodec = gbxCodecs
                    .Controls
                    .Cast<Control>()
                    .SingleOrDefault(x => x is RadioButton && string.Equals(x.Text, codec.DeviceName.ToString())) as RadioButton;

                if (rbCodec != null)
                {
                    rbCodec.Enabled = codec.DeviceName != null && codec.IsInstalled;
                    rbCodec.Checked = codec.DeviceName == Settings.Default.PreferredCompressorDevice;
                    rbCodec.Tag = codec;
                }
            }

            rbCodec = gbxCodecs
                        .Controls
                        .Cast<Control>()
                        .SingleOrDefault(x => x is RadioButton && ((RadioButton)x).Checked) as RadioButton;

            if (rbCodec == null)
                rbCodecUncompressed.Checked = true;

            rbFileAAV.Checked = Settings.Default.FileFormat == "AAV";
            rbFileAVI.Checked = Settings.Default.FileFormat == "AVI";

            cbxIsIntegrating.Checked = Settings.Default.IsIntegrating;
            cbxFlipHorizontally.Checked = Settings.Default.HorizontalFlip;
            cbxFlipVertically.Checked = Settings.Default.VerticalFlip;

            SetSettingsVisibility();
        }

        private void LoadVTIConfig(int width, int height)
        {
			var prevSelectedEntry = cbxOCRConfigurations.SelectedItem as OcrConfigEntry;

            cbxOCRConfigurations.Items.Clear();

			OcrConfigEntry[] matchingOcrConfigEntries = OcrSettings.Instance.Configurations
				.Where(x => !x.Hidden)
				.Select(x => OcrConfigEntry.OcrVtiOsdEntry(x.Name, x.Alignment.Width == width && x.Alignment.Height == height))
				.ToArray();

			cbxOCRConfigurations.Items.Add(new OcrConfigEntry(){ Name = "VTI OSD not available", EntryType = OcrConfigEntryType.VtiOsdNotAvailable});

			cbxOCRConfigurations.Items.Add(new OcrConfigEntry() { Name = "Preserve VTI OSD", EntryType = OcrConfigEntryType.PreserveVtiOsd });

	        cbxOCRConfigurations.Items.AddRange(matchingOcrConfigEntries);

			if (prevSelectedEntry != null && prevSelectedEntry.EntryType != OcrConfigEntryType.PreserveVtiOsd)
			{
				if (prevSelectedEntry.EntryType == OcrConfigEntryType.OcrVtiOsd)
					cbxOCRConfigurations.SelectedIndex = GetIndexOfOcrConfigEntry(prevSelectedEntry.Name, prevSelectedEntry.EntryType);
				else
					cbxOCRConfigurations.SelectedIndex = GetIndexOfOcrConfigEntry(null, prevSelectedEntry.EntryType);
			}

			if (cbxOCRConfigurations.SelectedIndex == -1 && !string.IsNullOrEmpty(Settings.Default.SelectedOcrConfiguration))
			{
				cbxOCRConfigurations.SelectedIndex = GetIndexOfOcrConfigEntry(Settings.Default.SelectedOcrConfiguration, OcrConfigEntryType.OcrVtiOsd);
			}

			if (cbxOCRConfigurations.SelectedIndex == -1)
			{
				cbxOCRConfigurations.SelectedIndex = GetIndexOfOcrConfigEntry(null, OcrConfigEntryType.PreserveVtiOsd); // Select 'Preserve VTI OSD'
			}		
        }

		private int GetIndexOfOcrConfigEntry(string byName, OcrConfigEntryType byType)
		{
			int selectedIndex = -1;
			for (int i = 0; i < cbxOCRConfigurations.Items.Count; i++)
			{
				if (!string.IsNullOrEmpty(byName))
				{
					if (((OcrConfigEntry) cbxOCRConfigurations.Items[i]).IsComptible &&
					    cbxOCRConfigurations.Items[i].ToString() == byName)
					{
						selectedIndex = i;
						break;						
					}
				}
				else if (byType == ((OcrConfigEntry) cbxOCRConfigurations.Items[i]).EntryType)
				{
					selectedIndex = i;
					break;
				}
			}

			return selectedIndex;
		}

        private void btnOK_Click(object sender, EventArgs e)
        {            
            if (pnlCrossbar.Visible && cbxCrossbarInput.Enabled)
            {
                var selectedItem = (CrossbarHelper.CrossbarPinEntry)cbxCrossbarInput.SelectedItem;

                if (selectedItem == null)
                {
                    MessageBox.Show(
                        this,
                        "Please select a crossbar source.",
                        "OccuRec",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    cbxCameraModel.Focus();
                    return;
                }

                Settings.Default.CrossbarInputPin = selectedItem.PinIndex;
            }

            Settings.Default.CameraModel = cbxCameraModel.Text;
            Settings.Default.PreferredCaptureDevice = (string)cbxCaptureDevices.SelectedItem;

            if (rbFileAAV.Checked)
            {
                if (string.IsNullOrEmpty(cbxCameraModel.Text))
                {
                    MessageBox.Show(
                        this,
                        "Please specify a camera model.",
                        "OccuRec",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    cbxCameraModel.Focus();
                    return;
                }

				

				if (CameraControlDriver != null)
				{
					if (!CameraControlDriver.IsConfigured)
					{
						while (!CameraControlDriver.IsConfigured && ConfigureCurrentCameraDriver())
						{ }

						if (CameraControlDriver != null && !CameraControlDriver.IsConfigured)
						{
							MessageBox.Show(
								this,
								"Please complete the camera driver configuration or choose a different driver.",
								"OccuRec",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error);

							cbxCameraDriver.Focus();
							return;						
						}					
					}

					Settings.Default.CameraControlDriver = CameraControlDriver.DriverName;
				}

                Settings.Default.FileFormat = "AAV";

	            OcrConfigEntry selectedOcrItem = cbxOCRConfigurations.SelectedItem as OcrConfigEntry;

				if (selectedOcrItem.EntryType == OcrConfigEntryType.OcrVtiOsd)
                {
					if (!selectedOcrItem.IsComptible)
					{
						MessageBox.Show(
							string.Format("'{0}' is not compatible with the current video mode '{1}'.", selectedOcrItem.ToString(), cbxVideoFormats.SelectedItem.ToString()),
							"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						cbxOCRConfigurations.Focus();
						return;
					}

					Settings.Default.SelectedOcrConfiguration = selectedOcrItem.Name;
					Settings.Default.PreserveVTIEnabled = true;
                    Settings.Default.AavOcrEnabled = true;
                }
				else if (selectedOcrItem.EntryType == OcrConfigEntryType.PreserveVtiOsd)
                {
                    if (nudPreserveVTITopRow.Value == 0 && nudPreserveVTIBottomRow.Value == 0)
                    {
                        MessageBox.Show(
                            this, 
                            "Please specify VTI OSD rows to preserve or the timestamps will end up blurred. For more information see the online help from Help -> Index and then read the section 'How to use OccuRec'.",
                            "OccuRec",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        nudPreserveVTITopRow.Focus();
                        return;
                    }
					Settings.Default.PreserveVTIEnabled = true;
                    Settings.Default.AavOcrEnabled = false;
                }
				else if (selectedOcrItem.EntryType == OcrConfigEntryType.VtiOsdNotAvailable)
				{
					Settings.Default.PreserveVTIEnabled = false;
					Settings.Default.AavOcrEnabled = false;
				}

	            var selectedFormat = (VideoFormatHelper.SupportedVideoFormat)cbxVideoFormats.SelectedItem;
                if (selectedFormat != null)
                {
                    Settings.Default.PreserveVTIFirstRow = (int)nudPreserveVTITopRow.Value;
                    Settings.Default.PreserveVTILastRow = (int)nudPreserveVTIBottomRow.Value;
                    Settings.Default.PreserveVTIWidth = selectedFormat.Width;
                    Settings.Default.PreserveVTIHeight = selectedFormat.Height;
                    Settings.Default.Save();
                }
            }
            else if (rbFileAVI.Checked)
                Settings.Default.FileFormat = "AVI";

            Settings.Default.FileSimulation = cbFileSIM.Checked;

            RadioButton rbCodec = gbxCodecs
                                .Controls
                                .Cast<Control>()
                                .SingleOrDefault(x => x is RadioButton && ((RadioButton)x).Checked) as RadioButton;

            if (rbCodec != null && rbCodec.Tag is SystemCodecEntry)
                Settings.Default.PreferredCompressorDevice = ((SystemCodecEntry)rbCodec.Tag).DeviceName;
            else
                Settings.Default.PreferredCompressorDevice = VideoCodecs.UNCOMPRESSED_VIDEO;

            Settings.Default.IsIntegrating = cbxIsIntegrating.Checked;
            Settings.Default.HorizontalFlip = cbxFlipHorizontally.Checked;
            Settings.Default.VerticalFlip = cbxFlipVertically.Checked;

            Settings.Default.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void rbFileAAV_CheckedChanged(object sender, EventArgs e)
        {
            SetSettingsVisibility();
        }

        private void rbFileAVI_CheckedChanged(object sender, EventArgs e)
        {
            SetSettingsVisibility();
        }

        private void SetSettingsVisibility()
        {
            if (rbFileAVI.Checked)
            {
                gbxAAVSettings.Visible = false;
                gbxAAVSettings.SendToBack();
                gbxCodecs.Visible = true;
                gbxCodecs.BringToFront();
	            pnlFlipControls.Visible = false;
            }
            else
            {
                gbxCodecs.Visible = false;
                gbxCodecs.SendToBack();
                gbxAAVSettings.Visible = true;
                gbxAAVSettings.BringToFront();
				pnlFlipControls.Visible = true;
            }
        }

        void cbxCrossbarInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            string deviceName = (string)cbxCaptureDevices.SelectedItem;

            if (!string.IsNullOrEmpty(deviceName))
            {
                CrossbarHelper.CrossbarPinEntry selectedItem = (CrossbarHelper.CrossbarPinEntry) cbxCrossbarInput.SelectedItem;
                if (selectedItem != null)
                {
                    Cursor = Cursors.WaitCursor;
                    Update();
                    try
                    {
                        CrossbarHelper.ConnectToCrossbarSource(deviceName, selectedItem.PinIndex);

                        Settings.Default.SelectedCrossbarInputPin = selectedItem.PinName;
                        Settings.Default.Save();
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void cbxCaptureDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            string deviceName = (string)cbxCaptureDevices.SelectedItem;

            if (!string.IsNullOrEmpty(deviceName))
            {
                Cursor = Cursors.WaitCursor;
                Update();

                cbxCrossbarInput.Items.Clear();
                cbxCrossbarInput.SelectedIndexChanged -= new EventHandler(cbxCrossbarInput_SelectedIndexChanged);
                try
                {
                    CrossbarHelper.LoadCrossbarSources(deviceName, cbxCrossbarInput);
                    pnlCrossbar.Visible = cbxCrossbarInput.Items.Count > 0;
                }
                finally
                {
                    cbxCrossbarInput.SelectedIndexChanged += new EventHandler(cbxCrossbarInput_SelectedIndexChanged);
                    Cursor = Cursors.Default;
                }

                CrossbarHelper.CrossbarPinEntry selectedEntry = null;
                foreach (CrossbarHelper.CrossbarPinEntry entry in cbxCrossbarInput.Items)
                {
                    if (entry.PinName == Settings.Default.SelectedCrossbarInputPin)
                    {
                        selectedEntry = entry;
                        break;
                    }
                }
                if (selectedEntry != null)
                    cbxCrossbarInput.SelectedItem = selectedEntry;

                cbxVideoFormats.Items.Clear();
                cbxVideoFormats.SelectedIndexChanged -= new EventHandler(cbxVideoFormats_SelectedIndexChanged);
                try
                {
                    VideoFormatHelper.LoadSupportedVideoFormats(deviceName, cbxVideoFormats);
                }
                finally
                {
                    cbxVideoFormats.SelectedIndexChanged += new EventHandler(cbxVideoFormats_SelectedIndexChanged);
                    Cursor = Cursors.Default;
                }

                VideoFormatHelper.SupportedVideoFormat selectedVideoFormat = null;
                foreach (VideoFormatHelper.SupportedVideoFormat format in cbxVideoFormats.Items)
                {
                    if (Settings.Default.SelectedVideoFormat == format.AsSerialized())
                    {
                        selectedVideoFormat = format;
                        break;
                    }
                }

                if (selectedVideoFormat != null)
                    cbxVideoFormats.SelectedItem = selectedVideoFormat;
                else
                    cbxVideoFormats.SelectedIndex = 0;
            }
        }

	    private VideoFormatHelper.SupportedVideoFormat m_CurrenctlySelectedVideoFormat;

        private void cbxVideoFormats_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedFormat = (VideoFormatHelper.SupportedVideoFormat)cbxVideoFormats.SelectedItem;
            Settings.Default.SelectedVideoFormat = selectedFormat.AsSerialized();
            Settings.Default.Save();

            LoadVTIConfig(selectedFormat.Width, selectedFormat.Height);
        }

        private void cbxOCRConfigurations_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedFormat = (VideoFormatHelper.SupportedVideoFormat)cbxVideoFormats.SelectedItem;
			var selectedOcrItem = (OcrConfigEntry)cbxOCRConfigurations.SelectedItem;

			if (selectedOcrItem.EntryType == OcrConfigEntryType.PreserveVtiOsd && selectedFormat != null)
            {
				if (Settings.Default.PreserveVTIWidth == selectedFormat.Width && Settings.Default.PreserveVTIHeight == selectedFormat.Height)
                {
                    nudPreserveVTITopRow.Value = Settings.Default.PreserveVTIFirstRow;
                    nudPreserveVTIBottomRow.Value = Settings.Default.PreserveVTILastRow;
                }
                else
                {
                    nudPreserveVTITopRow.Value = 0;
                    nudPreserveVTIBottomRow.Value = 0;
                }

                pnlPreserveOSDArea.Enabled = true;
				pnlPreserveOSDArea.Visible = true;
            }
            else if (selectedOcrItem.EntryType == OcrConfigEntryType.OcrVtiOsd)
            {
                OcrConfiguration ocrConfig = OcrSettings.Instance.Configurations.SingleOrDefault(x => x.Name == cbxOCRConfigurations.Text);

                if (ocrConfig != null)
                {
                    nudPreserveVTITopRow.Value = ocrConfig.Alignment.FrameTopOdd - 1;
                    nudPreserveVTIBottomRow.Value = ocrConfig.Alignment.FrameTopOdd + (2 * ocrConfig.Alignment.CharHeight) + 2;                   
                }
				if (ocrConfig != null)
				{
					pnlPreserveOSDArea.Enabled = false;
					if (!selectedOcrItem.IsComptible)
					{
						MessageBox.Show(
							string.Format("'{0}' is not compatible with the current video mode '{1}'.", cbxOCRConfigurations.Text, cbxVideoFormats.SelectedItem.ToString()),
							"OccuRec", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}					
                else
                    pnlPreserveOSDArea.Enabled = true;

				pnlPreserveOSDArea.Visible = true;
            }
			else if (selectedOcrItem.EntryType == OcrConfigEntryType.VtiOsdNotAvailable)
			{
				pnlPreserveOSDArea.Visible = false;
			}
        }

		private void llOnlineHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.hristopavlov.net/OccuRec/IOTA-VTI/");
		}

		private void cbxCameraModel_SelectedIndexChanged(object sender, EventArgs e)
		{
			List<IOccuRecCameraController> availableDrivers = OccuRecVideoDrivers.GetAvailableDriversForCamera((string) cbxCameraModel.SelectedItem);
			cbxCameraDriver.Items.Clear();
			cbxCameraDriver.Items.AddRange(availableDrivers.ToArray());
			cbxCameraDriver.Enabled = availableDrivers.Count > 0;
			if (cbxCameraDriver.Items.Count == 0)
				cbxCameraDriver.Items.Add("No Video Drivers Available");

			// TODO: Once we have more than one driver for the same camera we will need to store the user's choice in Settings.Default.CameraControlDriver
			
			cbxCameraDriver.SelectedIndex = 0;
		}

		private void cbxCameraDriver_SelectedIndexChanged(object sender, EventArgs e)
		{
			var camController = cbxCameraDriver.SelectedItem as IOccuRecCameraController;
			btnConfigureCameraDriver.Visible = camController != null;
			btnConfigureCameraDriver.Enabled = camController != null && camController.RequiresConfiguration;

			CameraControlDriver = camController;
			if (CameraControlDriver != null)
				CameraControlDriver.Configuration = OccuRecVideoDrivers.GetDriverSettings(CameraControlDriver);
		}

		private void btnConfigureCameraDriver_Click(object sender, EventArgs e)
		{
			ConfigureCurrentCameraDriver();
		}

		private bool ConfigureCurrentCameraDriver()
		{
			bool configChanged = false;

			if (CameraControlDriver != null)
			{
				CameraControlDriver.Configuration = OccuRecVideoDrivers.GetDriverSettings(CameraControlDriver);
				configChanged = CameraControlDriver.ConfigureConnectionSettings(this);
				if (configChanged)
					OccuRecVideoDrivers.SetDriverSettings(CameraControlDriver);
			}

			return configChanged;
		}
    }
}
