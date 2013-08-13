namespace AAVRec
{
	partial class frmSettings
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings));
			this.btnCancel = new System.Windows.Forms.Button();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabGeneral = new System.Windows.Forms.TabPage();
			this.tbxNTPServer = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.cbxTimeInUT = new System.Windows.Forms.CheckBox();
			this.btnBrowseOutputFolder = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.tbxOutputLocation = new System.Windows.Forms.TextBox();
			this.tabAAVSettings = new System.Windows.Forms.TabPage();
			this.cbDebugIntegration = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label15 = new System.Windows.Forms.Label();
			this.nudCalibrIntegrRate = new System.Windows.Forms.NumericUpDown();
			this.label14 = new System.Windows.Forms.Label();
			this.nudGammaDiff = new System.Windows.Forms.NumericUpDown();
			this.label10 = new System.Windows.Forms.Label();
			this.nudPreserveTSTop = new System.Windows.Forms.NumericUpDown();
			this.label11 = new System.Windows.Forms.Label();
			this.nudPreserveTSHeight = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.nudSignDiffRatio = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.nudMinSignDiff = new System.Windows.Forms.NumericUpDown();
			this.cbxFrameProcessingMode = new System.Windows.Forms.ComboBox();
			this.label12 = new System.Windows.Forms.Label();
			this.tabOCR = new System.Windows.Forms.TabPage();
			this.pnlSelectOCRConfig = new System.Windows.Forms.Panel();
			this.cbxOCRConfigurations = new System.Windows.Forms.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this.cbxEnableAAVIOTAVTIOCR = new System.Windows.Forms.CheckBox();
			this.tabDebug = new System.Windows.Forms.TabPage();
			this.cbxGraphDebugMode = new System.Windows.Forms.CheckBox();
			this.cbxOcrCameraTestModeAav = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.cbxImageLayoutMode = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.nudMaxErrorsPerTestRun = new System.Windows.Forms.NumericUpDown();
			this.cbxOcrCameraTestModeAvi = new System.Windows.Forms.CheckBox();
			this.gbxOcrTesting = new System.Windows.Forms.GroupBox();
			this.label9 = new System.Windows.Forms.Label();
			this.rbNativeOCR = new System.Windows.Forms.RadioButton();
			this.rbManagedSim = new System.Windows.Forms.RadioButton();
			this.cbxSimulatorRunOCR = new System.Windows.Forms.CheckBox();
			this.cbxOcrSimlatorTestMode = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.tbxSimlatorFilePath = new System.Windows.Forms.TextBox();
			this.btnBrowseSimulatorFile = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.openAavFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.tabControl.SuspendLayout();
			this.tabGeneral.SuspendLayout();
			this.tabAAVSettings.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCalibrIntegrRate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudGammaDiff)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPreserveTSTop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPreserveTSHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSignDiffRatio)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinSignDiff)).BeginInit();
			this.tabOCR.SuspendLayout();
			this.pnlSelectOCRConfig.SuspendLayout();
			this.tabDebug.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxErrorsPerTestRun)).BeginInit();
			this.gbxOcrTesting.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(341, 272);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabGeneral);
			this.tabControl.Controls.Add(this.tabAAVSettings);
			this.tabControl.Controls.Add(this.tabOCR);
			this.tabControl.Controls.Add(this.tabDebug);
			this.tabControl.Location = new System.Drawing.Point(12, 12);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(404, 251);
			this.tabControl.TabIndex = 7;
			// 
			// tabGeneral
			// 
			this.tabGeneral.Controls.Add(this.tbxNTPServer);
			this.tabGeneral.Controls.Add(this.label2);
			this.tabGeneral.Controls.Add(this.cbxTimeInUT);
			this.tabGeneral.Controls.Add(this.btnBrowseOutputFolder);
			this.tabGeneral.Controls.Add(this.label6);
			this.tabGeneral.Controls.Add(this.tbxOutputLocation);
			this.tabGeneral.Location = new System.Drawing.Point(4, 22);
			this.tabGeneral.Name = "tabGeneral";
			this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
			this.tabGeneral.Size = new System.Drawing.Size(396, 225);
			this.tabGeneral.TabIndex = 1;
			this.tabGeneral.Text = "General";
			this.tabGeneral.UseVisualStyleBackColor = true;
			// 
			// tbxNTPServer
			// 
			this.tbxNTPServer.Location = new System.Drawing.Point(17, 102);
			this.tbxNTPServer.Name = "tbxNTPServer";
			this.tbxNTPServer.Size = new System.Drawing.Size(155, 20);
			this.tbxNTPServer.TabIndex = 17;
			this.tbxNTPServer.Text = "time.windows.com";
			this.toolTip1.SetToolTip(this.tbxNTPServer, "NTP Server is used to adjust the Windows clock used by the scheduler to start/sto" +
        "p recording.");
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 86);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 13);
			this.label2.TabIndex = 16;
			this.label2.Text = "NTP Server";
			// 
			// cbxTimeInUT
			// 
			this.cbxTimeInUT.AutoSize = true;
			this.cbxTimeInUT.Location = new System.Drawing.Point(17, 160);
			this.cbxTimeInUT.Name = "cbxTimeInUT";
			this.cbxTimeInUT.Size = new System.Drawing.Size(115, 17);
			this.cbxTimeInUT.TabIndex = 14;
			this.cbxTimeInUT.Text = "Display Time in UT";
			this.cbxTimeInUT.UseVisualStyleBackColor = true;
			// 
			// btnBrowseOutputFolder
			// 
			this.btnBrowseOutputFolder.Location = new System.Drawing.Point(345, 31);
			this.btnBrowseOutputFolder.Name = "btnBrowseOutputFolder";
			this.btnBrowseOutputFolder.Size = new System.Drawing.Size(30, 23);
			this.btnBrowseOutputFolder.TabIndex = 13;
			this.btnBrowseOutputFolder.Text = "...";
			this.btnBrowseOutputFolder.UseVisualStyleBackColor = true;
			this.btnBrowseOutputFolder.Click += new System.EventHandler(this.btnBrowseOutputFolder_Click);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(14, 18);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(113, 13);
			this.label6.TabIndex = 12;
			this.label6.Text = "Output Video Location";
			// 
			// tbxOutputLocation
			// 
			this.tbxOutputLocation.Location = new System.Drawing.Point(17, 34);
			this.tbxOutputLocation.Name = "tbxOutputLocation";
			this.tbxOutputLocation.Size = new System.Drawing.Size(322, 20);
			this.tbxOutputLocation.TabIndex = 11;
			// 
			// tabAAVSettings
			// 
			this.tabAAVSettings.Controls.Add(this.cbDebugIntegration);
			this.tabAAVSettings.Controls.Add(this.groupBox1);
			this.tabAAVSettings.Controls.Add(this.cbxFrameProcessingMode);
			this.tabAAVSettings.Controls.Add(this.label12);
			this.tabAAVSettings.Location = new System.Drawing.Point(4, 22);
			this.tabAAVSettings.Name = "tabAAVSettings";
			this.tabAAVSettings.Size = new System.Drawing.Size(396, 225);
			this.tabAAVSettings.TabIndex = 2;
			this.tabAAVSettings.Text = "AAV";
			this.tabAAVSettings.UseVisualStyleBackColor = true;
			// 
			// cbDebugIntegration
			// 
			this.cbDebugIntegration.AutoSize = true;
			this.cbDebugIntegration.Location = new System.Drawing.Point(216, 194);
			this.cbDebugIntegration.Name = "cbDebugIntegration";
			this.cbDebugIntegration.Size = new System.Drawing.Size(161, 17);
			this.cbDebugIntegration.TabIndex = 20;
			this.cbDebugIntegration.Text = "Integration Detection Tuning";
			this.cbDebugIntegration.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label15);
			this.groupBox1.Controls.Add(this.nudCalibrIntegrRate);
			this.groupBox1.Controls.Add(this.label14);
			this.groupBox1.Controls.Add(this.nudGammaDiff);
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Controls.Add(this.nudPreserveTSTop);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Controls.Add(this.nudPreserveTSHeight);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.nudSignDiffRatio);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.nudMinSignDiff);
			this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.groupBox1.Location = new System.Drawing.Point(17, 13);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(362, 155);
			this.groupBox1.TabIndex = 21;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Integration Detection";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label15.Location = new System.Drawing.Point(12, 94);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(138, 13);
			this.label15.TabIndex = 14;
			this.label15.Text = "Calibration Integration Rate:";
			// 
			// nudCalibrIntegrRate
			// 
			this.nudCalibrIntegrRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.nudCalibrIntegrRate.Location = new System.Drawing.Point(192, 91);
			this.nudCalibrIntegrRate.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this.nudCalibrIntegrRate.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
			this.nudCalibrIntegrRate.Name = "nudCalibrIntegrRate";
			this.nudCalibrIntegrRate.Size = new System.Drawing.Size(49, 20);
			this.nudCalibrIntegrRate.TabIndex = 15;
			this.nudCalibrIntegrRate.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label14.Location = new System.Drawing.Point(212, 32);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(79, 13);
			this.label14.TabIndex = 12;
			this.label14.Text = "Gamma Factor:";
			// 
			// nudGammaDiff
			// 
			this.nudGammaDiff.DecimalPlaces = 2;
			this.nudGammaDiff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.nudGammaDiff.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.nudGammaDiff.Location = new System.Drawing.Point(296, 28);
			this.nudGammaDiff.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.nudGammaDiff.Name = "nudGammaDiff";
			this.nudGammaDiff.Size = new System.Drawing.Size(49, 20);
			this.nudGammaDiff.TabIndex = 13;
			this.nudGammaDiff.Value = new decimal(new int[] {
            100,
            0,
            0,
            131072});
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(10, 126);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(173, 13);
			this.label10.TabIndex = 8;
			this.label10.Text = "Preserve Timestamp First Line Top:";
			// 
			// nudPreserveTSTop
			// 
			this.nudPreserveTSTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.nudPreserveTSTop.Location = new System.Drawing.Point(191, 124);
			this.nudPreserveTSTop.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
			this.nudPreserveTSTop.Name = "nudPreserveTSTop";
			this.nudPreserveTSTop.Size = new System.Drawing.Size(50, 20);
			this.nudPreserveTSTop.TabIndex = 9;
			this.nudPreserveTSTop.Value = new decimal(new int[] {
            542,
            0,
            0,
            0});
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label11.Location = new System.Drawing.Point(247, 126);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(41, 13);
			this.label11.TabIndex = 10;
			this.label11.Text = "Height:";
			// 
			// nudPreserveTSHeight
			// 
			this.nudPreserveTSHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.nudPreserveTSHeight.Location = new System.Drawing.Point(296, 124);
			this.nudPreserveTSHeight.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.nudPreserveTSHeight.Name = "nudPreserveTSHeight";
			this.nudPreserveTSHeight.Size = new System.Drawing.Size(49, 20);
			this.nudPreserveTSHeight.TabIndex = 11;
			this.nudPreserveTSHeight.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(12, 31);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(118, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Minimal Signature Ratio";
			// 
			// nudSignDiffRatio
			// 
			this.nudSignDiffRatio.DecimalPlaces = 1;
			this.nudSignDiffRatio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.nudSignDiffRatio.Location = new System.Drawing.Point(136, 28);
			this.nudSignDiffRatio.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
			this.nudSignDiffRatio.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudSignDiffRatio.Name = "nudSignDiffRatio";
			this.nudSignDiffRatio.Size = new System.Drawing.Size(49, 20);
			this.nudSignDiffRatio.TabIndex = 5;
			this.nudSignDiffRatio.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(11, 63);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(145, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Minimal Signature Difference:";
			// 
			// nudMinSignDiff
			// 
			this.nudMinSignDiff.DecimalPlaces = 2;
			this.nudMinSignDiff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.nudMinSignDiff.Location = new System.Drawing.Point(191, 60);
			this.nudMinSignDiff.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudMinSignDiff.Name = "nudMinSignDiff";
			this.nudMinSignDiff.Size = new System.Drawing.Size(49, 20);
			this.nudMinSignDiff.TabIndex = 7;
			this.nudMinSignDiff.Value = new decimal(new int[] {
            2,
            0,
            0,
            65536});
			// 
			// cbxFrameProcessingMode
			// 
			this.cbxFrameProcessingMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxFrameProcessingMode.FormattingEnabled = true;
			this.cbxFrameProcessingMode.Items.AddRange(new object[] {
            "Buffered",
            "Synchronous"});
			this.cbxFrameProcessingMode.Location = new System.Drawing.Point(17, 192);
			this.cbxFrameProcessingMode.Name = "cbxFrameProcessingMode";
			this.cbxFrameProcessingMode.Size = new System.Drawing.Size(144, 21);
			this.cbxFrameProcessingMode.TabIndex = 18;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(14, 176);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(91, 13);
			this.label12.TabIndex = 19;
			this.label12.Text = "Frame Processing";
			// 
			// tabOCR
			// 
			this.tabOCR.Controls.Add(this.pnlSelectOCRConfig);
			this.tabOCR.Controls.Add(this.cbxEnableAAVIOTAVTIOCR);
			this.tabOCR.Location = new System.Drawing.Point(4, 22);
			this.tabOCR.Name = "tabOCR";
			this.tabOCR.Padding = new System.Windows.Forms.Padding(3);
			this.tabOCR.Size = new System.Drawing.Size(396, 225);
			this.tabOCR.TabIndex = 3;
			this.tabOCR.Text = "OCR";
			this.tabOCR.UseVisualStyleBackColor = true;
			// 
			// pnlSelectOCRConfig
			// 
			this.pnlSelectOCRConfig.Controls.Add(this.cbxOCRConfigurations);
			this.pnlSelectOCRConfig.Controls.Add(this.label13);
			this.pnlSelectOCRConfig.Enabled = false;
			this.pnlSelectOCRConfig.Location = new System.Drawing.Point(36, 55);
			this.pnlSelectOCRConfig.Name = "pnlSelectOCRConfig";
			this.pnlSelectOCRConfig.Size = new System.Drawing.Size(263, 54);
			this.pnlSelectOCRConfig.TabIndex = 25;
			// 
			// cbxOCRConfigurations
			// 
			this.cbxOCRConfigurations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxOCRConfigurations.FormattingEnabled = true;
			this.cbxOCRConfigurations.Location = new System.Drawing.Point(88, 17);
			this.cbxOCRConfigurations.Name = "cbxOCRConfigurations";
			this.cbxOCRConfigurations.Size = new System.Drawing.Size(162, 21);
			this.cbxOCRConfigurations.TabIndex = 25;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(13, 21);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(72, 13);
			this.label13.TabIndex = 26;
			this.label13.Text = "Configuration:";
			// 
			// cbxEnableAAVIOTAVTIOCR
			// 
			this.cbxEnableAAVIOTAVTIOCR.AutoSize = true;
			this.cbxEnableAAVIOTAVTIOCR.Location = new System.Drawing.Point(17, 34);
			this.cbxEnableAAVIOTAVTIOCR.Name = "cbxEnableAAVIOTAVTIOCR";
			this.cbxEnableAAVIOTAVTIOCR.Size = new System.Drawing.Size(139, 17);
			this.cbxEnableAAVIOTAVTIOCR.TabIndex = 22;
			this.cbxEnableAAVIOTAVTIOCR.Text = "Enable Timestamp OCR";
			this.cbxEnableAAVIOTAVTIOCR.UseVisualStyleBackColor = true;
			this.cbxEnableAAVIOTAVTIOCR.CheckedChanged += new System.EventHandler(this.cbxEnableAAVIOTAVTIOCR_CheckedChanged);
			// 
			// tabDebug
			// 
			this.tabDebug.Controls.Add(this.cbxGraphDebugMode);
			this.tabDebug.Controls.Add(this.cbxOcrCameraTestModeAav);
			this.tabDebug.Controls.Add(this.label5);
			this.tabDebug.Controls.Add(this.cbxImageLayoutMode);
			this.tabDebug.Controls.Add(this.label8);
			this.tabDebug.Controls.Add(this.nudMaxErrorsPerTestRun);
			this.tabDebug.Controls.Add(this.cbxOcrCameraTestModeAvi);
			this.tabDebug.Controls.Add(this.gbxOcrTesting);
			this.tabDebug.Location = new System.Drawing.Point(4, 22);
			this.tabDebug.Name = "tabDebug";
			this.tabDebug.Padding = new System.Windows.Forms.Padding(3);
			this.tabDebug.Size = new System.Drawing.Size(396, 225);
			this.tabDebug.TabIndex = 0;
			this.tabDebug.Text = "Debug";
			this.tabDebug.UseVisualStyleBackColor = true;
			// 
			// cbxGraphDebugMode
			// 
			this.cbxGraphDebugMode.AutoSize = true;
			this.cbxGraphDebugMode.Location = new System.Drawing.Point(16, 161);
			this.cbxGraphDebugMode.Name = "cbxGraphDebugMode";
			this.cbxGraphDebugMode.Size = new System.Drawing.Size(150, 17);
			this.cbxGraphDebugMode.TabIndex = 28;
			this.cbxGraphDebugMode.Text = "Video Graph Debug Mode";
			this.cbxGraphDebugMode.UseVisualStyleBackColor = true;
			// 
			// cbxOcrCameraTestModeAav
			// 
			this.cbxOcrCameraTestModeAav.AutoSize = true;
			this.cbxOcrCameraTestModeAav.Location = new System.Drawing.Point(16, 129);
			this.cbxOcrCameraTestModeAav.Name = "cbxOcrCameraTestModeAav";
			this.cbxOcrCameraTestModeAav.Size = new System.Drawing.Size(182, 17);
			this.cbxOcrCameraTestModeAav.TabIndex = 27;
			this.cbxOcrCameraTestModeAav.Text = "Enable Camera Test Mode (AAV)";
			this.cbxOcrCameraTestModeAav.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(235, 120);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(92, 13);
			this.label5.TabIndex = 26;
			this.label5.Text = "Max errors per run";
			// 
			// cbxImageLayoutMode
			// 
			this.cbxImageLayoutMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxImageLayoutMode.FormattingEnabled = true;
			this.cbxImageLayoutMode.Location = new System.Drawing.Point(75, 191);
			this.cbxImageLayoutMode.Name = "cbxImageLayoutMode";
			this.cbxImageLayoutMode.Size = new System.Drawing.Size(152, 21);
			this.cbxImageLayoutMode.TabIndex = 20;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(15, 194);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(59, 13);
			this.label8.TabIndex = 19;
			this.label8.Text = "Img Layout";
			// 
			// nudMaxErrorsPerTestRun
			// 
			this.nudMaxErrorsPerTestRun.Location = new System.Drawing.Point(333, 116);
			this.nudMaxErrorsPerTestRun.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.nudMaxErrorsPerTestRun.Name = "nudMaxErrorsPerTestRun";
			this.nudMaxErrorsPerTestRun.Size = new System.Drawing.Size(52, 20);
			this.nudMaxErrorsPerTestRun.TabIndex = 25;
			this.nudMaxErrorsPerTestRun.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			// 
			// cbxOcrCameraTestModeAvi
			// 
			this.cbxOcrCameraTestModeAvi.AutoSize = true;
			this.cbxOcrCameraTestModeAvi.Location = new System.Drawing.Point(17, 111);
			this.cbxOcrCameraTestModeAvi.Name = "cbxOcrCameraTestModeAvi";
			this.cbxOcrCameraTestModeAvi.Size = new System.Drawing.Size(178, 17);
			this.cbxOcrCameraTestModeAvi.TabIndex = 24;
			this.cbxOcrCameraTestModeAvi.Text = "Enable Camera Test Mode (AVI)";
			this.cbxOcrCameraTestModeAvi.UseVisualStyleBackColor = true;
			// 
			// gbxOcrTesting
			// 
			this.gbxOcrTesting.Controls.Add(this.label9);
			this.gbxOcrTesting.Controls.Add(this.rbNativeOCR);
			this.gbxOcrTesting.Controls.Add(this.rbManagedSim);
			this.gbxOcrTesting.Controls.Add(this.cbxSimulatorRunOCR);
			this.gbxOcrTesting.Controls.Add(this.cbxOcrSimlatorTestMode);
			this.gbxOcrTesting.Controls.Add(this.label7);
			this.gbxOcrTesting.Controls.Add(this.tbxSimlatorFilePath);
			this.gbxOcrTesting.Controls.Add(this.btnBrowseSimulatorFile);
			this.gbxOcrTesting.Location = new System.Drawing.Point(7, 7);
			this.gbxOcrTesting.Name = "gbxOcrTesting";
			this.gbxOcrTesting.Size = new System.Drawing.Size(378, 89);
			this.gbxOcrTesting.TabIndex = 19;
			this.gbxOcrTesting.TabStop = false;
			this.gbxOcrTesting.Text = "                                                      ";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(215, 71);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(10, 13);
			this.label9.TabIndex = 28;
			this.label9.Text = "]";
			// 
			// rbNativeOCR
			// 
			this.rbNativeOCR.AutoSize = true;
			this.rbNativeOCR.Location = new System.Drawing.Point(91, 69);
			this.rbNativeOCR.Name = "rbNativeOCR";
			this.rbNativeOCR.Size = new System.Drawing.Size(56, 17);
			this.rbNativeOCR.TabIndex = 27;
			this.rbNativeOCR.Text = "Native";
			this.rbNativeOCR.UseVisualStyleBackColor = true;
			// 
			// rbManagedSim
			// 
			this.rbManagedSim.AutoSize = true;
			this.rbManagedSim.Checked = true;
			this.rbManagedSim.Location = new System.Drawing.Point(149, 69);
			this.rbManagedSim.Name = "rbManagedSim";
			this.rbManagedSim.Size = new System.Drawing.Size(70, 17);
			this.rbManagedSim.TabIndex = 26;
			this.rbManagedSim.TabStop = true;
			this.rbManagedSim.Text = "Managed";
			this.rbManagedSim.UseVisualStyleBackColor = true;
			// 
			// cbxSimulatorRunOCR
			// 
			this.cbxSimulatorRunOCR.AutoSize = true;
			this.cbxSimulatorRunOCR.Checked = true;
			this.cbxSimulatorRunOCR.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxSimulatorRunOCR.Location = new System.Drawing.Point(12, 68);
			this.cbxSimulatorRunOCR.Name = "cbxSimulatorRunOCR";
			this.cbxSimulatorRunOCR.Size = new System.Drawing.Size(81, 17);
			this.cbxSimulatorRunOCR.TabIndex = 25;
			this.cbxSimulatorRunOCR.Text = "Run OCR  [";
			this.cbxSimulatorRunOCR.UseVisualStyleBackColor = true;
			this.cbxSimulatorRunOCR.CheckedChanged += new System.EventHandler(this.cbxSimulatorRunOCR_CheckedChanged);
			// 
			// cbxOcrSimlatorTestMode
			// 
			this.cbxOcrSimlatorTestMode.AutoSize = true;
			this.cbxOcrSimlatorTestMode.Location = new System.Drawing.Point(14, -1);
			this.cbxOcrSimlatorTestMode.Name = "cbxOcrSimlatorTestMode";
			this.cbxOcrSimlatorTestMode.Size = new System.Drawing.Size(159, 17);
			this.cbxOcrSimlatorTestMode.TabIndex = 17;
			this.cbxOcrSimlatorTestMode.Text = "Enable Simulator Test Mode";
			this.cbxOcrSimlatorTestMode.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(12, 26);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(47, 13);
			this.label7.TabIndex = 15;
			this.label7.Text = "Test File";
			// 
			// tbxSimlatorFilePath
			// 
			this.tbxSimlatorFilePath.Location = new System.Drawing.Point(12, 42);
			this.tbxSimlatorFilePath.Name = "tbxSimlatorFilePath";
			this.tbxSimlatorFilePath.Size = new System.Drawing.Size(322, 20);
			this.tbxSimlatorFilePath.TabIndex = 14;
			// 
			// btnBrowseSimulatorFile
			// 
			this.btnBrowseSimulatorFile.Location = new System.Drawing.Point(340, 39);
			this.btnBrowseSimulatorFile.Name = "btnBrowseSimulatorFile";
			this.btnBrowseSimulatorFile.Size = new System.Drawing.Size(30, 23);
			this.btnBrowseSimulatorFile.TabIndex = 16;
			this.btnBrowseSimulatorFile.Text = "...";
			this.btnBrowseSimulatorFile.UseVisualStyleBackColor = true;
			this.btnBrowseSimulatorFile.Click += new System.EventHandler(this.btnBrowseSimulatorFile_Click);
			// 
			// btnSave
			// 
			this.btnSave.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnSave.Location = new System.Drawing.Point(260, 272);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 8;
			this.btnSave.Text = "Save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// openAavFileDialog
			// 
			this.openAavFileDialog.DefaultExt = "aav";
			this.openAavFileDialog.FileName = "Open AAV/AVI File";
			this.openAavFileDialog.Filter = "Supported Video Files (*.aav;*.avi)|*.aav;*.avi";
			// 
			// frmSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(428, 304);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "AAVRec Settings";
			this.Load += new System.EventHandler(this.frmSettings_Load);
			this.tabControl.ResumeLayout(false);
			this.tabGeneral.ResumeLayout(false);
			this.tabGeneral.PerformLayout();
			this.tabAAVSettings.ResumeLayout(false);
			this.tabAAVSettings.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCalibrIntegrRate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudGammaDiff)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPreserveTSTop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPreserveTSHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSignDiffRatio)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinSignDiff)).EndInit();
			this.tabOCR.ResumeLayout(false);
			this.tabOCR.PerformLayout();
			this.pnlSelectOCRConfig.ResumeLayout(false);
			this.pnlSelectOCRConfig.PerformLayout();
			this.tabDebug.ResumeLayout(false);
			this.tabDebug.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxErrorsPerTestRun)).EndInit();
			this.gbxOcrTesting.ResumeLayout(false);
			this.gbxOcrTesting.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabDebug;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TabPage tabAAVSettings;
        private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown nudSignDiffRatio;
        private System.Windows.Forms.NumericUpDown nudMinSignDiff;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnBrowseOutputFolder;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbxOutputLocation;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.CheckBox cbxTimeInUT;
        private System.Windows.Forms.Button btnBrowseSimulatorFile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbxSimlatorFilePath;
        private System.Windows.Forms.OpenFileDialog openAavFileDialog;
        private System.Windows.Forms.GroupBox gbxOcrTesting;
		private System.Windows.Forms.CheckBox cbxOcrSimlatorTestMode;
        private System.Windows.Forms.CheckBox cbxOcrCameraTestModeAvi;
        private System.Windows.Forms.CheckBox cbxSimulatorRunOCR;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudMaxErrorsPerTestRun;
        private System.Windows.Forms.CheckBox cbxOcrCameraTestModeAav;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbxImageLayoutMode;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbxNTPServer;
        private System.Windows.Forms.RadioButton rbNativeOCR;
        private System.Windows.Forms.RadioButton rbManagedSim;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox cbxEnableAAVIOTAVTIOCR;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.NumericUpDown nudPreserveTSTop;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.NumericUpDown nudPreserveTSHeight;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cbxFrameProcessingMode;
        private System.Windows.Forms.CheckBox cbDebugIntegration;
		private System.Windows.Forms.TabPage tabOCR;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.NumericUpDown nudGammaDiff;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.NumericUpDown nudCalibrIntegrRate;
		private System.Windows.Forms.CheckBox cbxGraphDebugMode;
		private System.Windows.Forms.Panel pnlSelectOCRConfig;
		private System.Windows.Forms.ComboBox cbxOCRConfigurations;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}