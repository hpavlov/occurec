namespace OccuRec
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
			this.cbxWarnForFileSystemIssues = new System.Windows.Forms.CheckBox();
			this.tbxNTPServer = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.cbxTimeInUT = new System.Windows.Forms.CheckBox();
			this.btnBrowseOutputFolder = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.tbxOutputLocation = new System.Windows.Forms.TextBox();
			this.tabAAVSettings = new System.Windows.Forms.TabPage();
			this.cbDebugIntegration = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cbForceIntegrationRateRestrictions = new System.Windows.Forms.CheckBox();
			this.label15 = new System.Windows.Forms.Label();
			this.nudCalibrIntegrRate = new System.Windows.Forms.NumericUpDown();
			this.label14 = new System.Windows.Forms.Label();
			this.nudGammaDiff = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.nudSignDiffRatio = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.nudMinSignDiff = new System.Windows.Forms.NumericUpDown();
			this.cbxFrameProcessingMode = new System.Windows.Forms.ComboBox();
			this.label12 = new System.Windows.Forms.Label();
			this.tabTelControl = new System.Windows.Forms.TabPage();
			this.cbxLiveTelescopeMode = new System.Windows.Forms.CheckBox();
			this.btnTestTelescopeConnection = new System.Windows.Forms.Button();
			this.btnTestFocuserConnection = new System.Windows.Forms.Button();
			this.lblConnectedTelescopeInfo = new System.Windows.Forms.Label();
			this.tbxTelescope = new System.Windows.Forms.TextBox();
			this.Telescope = new System.Windows.Forms.Label();
			this.btnSelectTelescope = new System.Windows.Forms.Button();
			this.lblConnectedFocuserInfo = new System.Windows.Forms.Label();
			this.tbxFocuser = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnSelectFocuser = new System.Windows.Forms.Button();
			this.tabDebug = new System.Windows.Forms.TabPage();
			this.pnlOCRTesting = new System.Windows.Forms.Panel();
			this.cbxOcrCameraTestModeAav = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.nudMaxErrorsPerTestRun = new System.Windows.Forms.NumericUpDown();
			this.cbxGraphDebugMode = new System.Windows.Forms.CheckBox();
			this.cbxImageLayoutMode = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
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
			this.btnClearFocuser = new System.Windows.Forms.Button();
			this.btnClearTelescope = new System.Windows.Forms.Button();
			this.tabControl.SuspendLayout();
			this.tabGeneral.SuspendLayout();
			this.tabAAVSettings.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCalibrIntegrRate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudGammaDiff)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSignDiffRatio)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinSignDiff)).BeginInit();
			this.tabTelControl.SuspendLayout();
			this.tabDebug.SuspendLayout();
			this.pnlOCRTesting.SuspendLayout();
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
			this.tabControl.Controls.Add(this.tabTelControl);
			this.tabControl.Controls.Add(this.tabDebug);
			this.tabControl.Location = new System.Drawing.Point(12, 12);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(404, 251);
			this.tabControl.TabIndex = 7;
			// 
			// tabGeneral
			// 
			this.tabGeneral.Controls.Add(this.cbxWarnForFileSystemIssues);
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
			// cbxWarnForFileSystemIssues
			// 
			this.cbxWarnForFileSystemIssues.AutoSize = true;
			this.cbxWarnForFileSystemIssues.Location = new System.Drawing.Point(17, 60);
			this.cbxWarnForFileSystemIssues.Name = "cbxWarnForFileSystemIssues";
			this.cbxWarnForFileSystemIssues.Size = new System.Drawing.Size(230, 17);
			this.cbxWarnForFileSystemIssues.TabIndex = 18;
			this.cbxWarnForFileSystemIssues.Text = "Enable disk space and file system warnings";
			this.cbxWarnForFileSystemIssues.UseVisualStyleBackColor = true;
			// 
			// tbxNTPServer
			// 
			this.tbxNTPServer.Location = new System.Drawing.Point(17, 129);
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
			this.label2.Location = new System.Drawing.Point(14, 113);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 13);
			this.label2.TabIndex = 16;
			this.label2.Text = "NTP Server";
			// 
			// cbxTimeInUT
			// 
			this.cbxTimeInUT.AutoSize = true;
			this.cbxTimeInUT.Location = new System.Drawing.Point(17, 187);
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
			this.cbDebugIntegration.Location = new System.Drawing.Point(216, 195);
			this.cbDebugIntegration.Name = "cbDebugIntegration";
			this.cbDebugIntegration.Size = new System.Drawing.Size(161, 17);
			this.cbDebugIntegration.TabIndex = 20;
			this.cbDebugIntegration.Text = "Integration Detection Tuning";
			this.cbDebugIntegration.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cbForceIntegrationRateRestrictions);
			this.groupBox1.Controls.Add(this.label15);
			this.groupBox1.Controls.Add(this.nudCalibrIntegrRate);
			this.groupBox1.Controls.Add(this.label14);
			this.groupBox1.Controls.Add(this.nudGammaDiff);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.nudSignDiffRatio);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.nudMinSignDiff);
			this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.groupBox1.Location = new System.Drawing.Point(17, 13);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(362, 152);
			this.groupBox1.TabIndex = 21;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Integration Detection";
			// 
			// cbForceIntegrationRateRestrictions
			// 
			this.cbForceIntegrationRateRestrictions.AutoSize = true;
			this.cbForceIntegrationRateRestrictions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.cbForceIntegrationRateRestrictions.Location = new System.Drawing.Point(15, 125);
			this.cbForceIntegrationRateRestrictions.Name = "cbForceIntegrationRateRestrictions";
			this.cbForceIntegrationRateRestrictions.Size = new System.Drawing.Size(190, 17);
			this.cbForceIntegrationRateRestrictions.TabIndex = 16;
			this.cbForceIntegrationRateRestrictions.Text = "Force Integration Rate Restrictions";
			this.cbForceIntegrationRateRestrictions.UseVisualStyleBackColor = true;
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
			this.cbxFrameProcessingMode.Location = new System.Drawing.Point(17, 193);
			this.cbxFrameProcessingMode.Name = "cbxFrameProcessingMode";
			this.cbxFrameProcessingMode.Size = new System.Drawing.Size(144, 21);
			this.cbxFrameProcessingMode.TabIndex = 18;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(14, 177);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(91, 13);
			this.label12.TabIndex = 19;
			this.label12.Text = "Frame Processing";
			// 
			// tabTelControl
			// 
			this.tabTelControl.Controls.Add(this.btnClearTelescope);
			this.tabTelControl.Controls.Add(this.btnClearFocuser);
			this.tabTelControl.Controls.Add(this.cbxLiveTelescopeMode);
			this.tabTelControl.Controls.Add(this.btnTestTelescopeConnection);
			this.tabTelControl.Controls.Add(this.btnTestFocuserConnection);
			this.tabTelControl.Controls.Add(this.lblConnectedTelescopeInfo);
			this.tabTelControl.Controls.Add(this.tbxTelescope);
			this.tabTelControl.Controls.Add(this.Telescope);
			this.tabTelControl.Controls.Add(this.btnSelectTelescope);
			this.tabTelControl.Controls.Add(this.lblConnectedFocuserInfo);
			this.tabTelControl.Controls.Add(this.tbxFocuser);
			this.tabTelControl.Controls.Add(this.label1);
			this.tabTelControl.Controls.Add(this.btnSelectFocuser);
			this.tabTelControl.Location = new System.Drawing.Point(4, 22);
			this.tabTelControl.Name = "tabTelControl";
			this.tabTelControl.Size = new System.Drawing.Size(396, 225);
			this.tabTelControl.TabIndex = 4;
			this.tabTelControl.Text = "Telescope Control";
			this.tabTelControl.UseVisualStyleBackColor = true;
			// 
			// cbxLiveTelescopeMode
			// 
			this.cbxLiveTelescopeMode.AutoSize = true;
			this.cbxLiveTelescopeMode.Location = new System.Drawing.Point(18, 195);
			this.cbxLiveTelescopeMode.Name = "cbxLiveTelescopeMode";
			this.cbxLiveTelescopeMode.Size = new System.Drawing.Size(254, 17);
			this.cbxLiveTelescopeMode.TabIndex = 10;
			this.cbxLiveTelescopeMode.Text = "Connect to Telescope when OccuRec is started";
			this.cbxLiveTelescopeMode.UseVisualStyleBackColor = true;
			this.cbxLiveTelescopeMode.CheckedChanged += new System.EventHandler(this.cbxLiveTelescopeMode_CheckedChanged);
			// 
			// btnTestTelescopeConnection
			// 
			this.btnTestTelescopeConnection.Location = new System.Drawing.Point(18, 153);
			this.btnTestTelescopeConnection.Name = "btnTestTelescopeConnection";
			this.btnTestTelescopeConnection.Size = new System.Drawing.Size(99, 23);
			this.btnTestTelescopeConnection.TabIndex = 9;
			this.btnTestTelescopeConnection.Text = "Test Connection";
			this.btnTestTelescopeConnection.UseVisualStyleBackColor = true;
			this.btnTestTelescopeConnection.Click += new System.EventHandler(this.btnTestTelescopeConnection_Click);
			// 
			// btnTestFocuserConnection
			// 
			this.btnTestFocuserConnection.Location = new System.Drawing.Point(18, 68);
			this.btnTestFocuserConnection.Name = "btnTestFocuserConnection";
			this.btnTestFocuserConnection.Size = new System.Drawing.Size(99, 23);
			this.btnTestFocuserConnection.TabIndex = 8;
			this.btnTestFocuserConnection.Text = "Test Connection";
			this.btnTestFocuserConnection.UseVisualStyleBackColor = true;
			this.btnTestFocuserConnection.Click += new System.EventHandler(this.btnTestFocuserConnection_Click);
			// 
			// lblConnectedTelescopeInfo
			// 
			this.lblConnectedTelescopeInfo.AutoSize = true;
			this.lblConnectedTelescopeInfo.ForeColor = System.Drawing.Color.Green;
			this.lblConnectedTelescopeInfo.Location = new System.Drawing.Point(123, 158);
			this.lblConnectedTelescopeInfo.Name = "lblConnectedTelescopeInfo";
			this.lblConnectedTelescopeInfo.Size = new System.Drawing.Size(57, 13);
			this.lblConnectedTelescopeInfo.TabIndex = 7;
			this.lblConnectedTelescopeInfo.Text = "Telescope";
			this.lblConnectedTelescopeInfo.Visible = false;
			// 
			// tbxTelescope
			// 
			this.tbxTelescope.Location = new System.Drawing.Point(18, 120);
			this.tbxTelescope.Name = "tbxTelescope";
			this.tbxTelescope.ReadOnly = true;
			this.tbxTelescope.Size = new System.Drawing.Size(259, 20);
			this.tbxTelescope.TabIndex = 6;
			this.tbxTelescope.TextChanged += new System.EventHandler(this.tbxTelescope_TextChanged);
			// 
			// Telescope
			// 
			this.Telescope.AutoSize = true;
			this.Telescope.Location = new System.Drawing.Point(15, 104);
			this.Telescope.Name = "Telescope";
			this.Telescope.Size = new System.Drawing.Size(57, 13);
			this.Telescope.TabIndex = 5;
			this.Telescope.Text = "Telescope";
			// 
			// btnSelectTelescope
			// 
			this.btnSelectTelescope.Location = new System.Drawing.Point(283, 117);
			this.btnSelectTelescope.Name = "btnSelectTelescope";
			this.btnSelectTelescope.Size = new System.Drawing.Size(99, 23);
			this.btnSelectTelescope.TabIndex = 4;
			this.btnSelectTelescope.Text = "Select Telescope";
			this.btnSelectTelescope.UseVisualStyleBackColor = true;
			this.btnSelectTelescope.Click += new System.EventHandler(this.btnSelectTelescope_Click);
			// 
			// lblConnectedFocuserInfo
			// 
			this.lblConnectedFocuserInfo.AutoSize = true;
			this.lblConnectedFocuserInfo.ForeColor = System.Drawing.Color.Green;
			this.lblConnectedFocuserInfo.Location = new System.Drawing.Point(123, 73);
			this.lblConnectedFocuserInfo.Name = "lblConnectedFocuserInfo";
			this.lblConnectedFocuserInfo.Size = new System.Drawing.Size(45, 13);
			this.lblConnectedFocuserInfo.TabIndex = 3;
			this.lblConnectedFocuserInfo.Text = "Focuser";
			this.lblConnectedFocuserInfo.Visible = false;
			// 
			// tbxFocuser
			// 
			this.tbxFocuser.Location = new System.Drawing.Point(18, 37);
			this.tbxFocuser.Name = "tbxFocuser";
			this.tbxFocuser.ReadOnly = true;
			this.tbxFocuser.Size = new System.Drawing.Size(259, 20);
			this.tbxFocuser.TabIndex = 2;
			this.tbxFocuser.TextChanged += new System.EventHandler(this.tbxFocuser_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(15, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(45, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Focuser";
			// 
			// btnSelectFocuser
			// 
			this.btnSelectFocuser.Location = new System.Drawing.Point(283, 34);
			this.btnSelectFocuser.Name = "btnSelectFocuser";
			this.btnSelectFocuser.Size = new System.Drawing.Size(99, 23);
			this.btnSelectFocuser.TabIndex = 0;
			this.btnSelectFocuser.Text = "Select Focuser";
			this.btnSelectFocuser.UseVisualStyleBackColor = true;
			this.btnSelectFocuser.Click += new System.EventHandler(this.button1_Click);
			// 
			// tabDebug
			// 
			this.tabDebug.Controls.Add(this.pnlOCRTesting);
			this.tabDebug.Controls.Add(this.cbxGraphDebugMode);
			this.tabDebug.Controls.Add(this.cbxImageLayoutMode);
			this.tabDebug.Controls.Add(this.label8);
			this.tabDebug.Controls.Add(this.gbxOcrTesting);
			this.tabDebug.Location = new System.Drawing.Point(4, 22);
			this.tabDebug.Name = "tabDebug";
			this.tabDebug.Padding = new System.Windows.Forms.Padding(3);
			this.tabDebug.Size = new System.Drawing.Size(396, 225);
			this.tabDebug.TabIndex = 0;
			this.tabDebug.Text = "Debug";
			this.tabDebug.UseVisualStyleBackColor = true;
			// 
			// pnlOCRTesting
			// 
			this.pnlOCRTesting.Controls.Add(this.cbxOcrCameraTestModeAav);
			this.pnlOCRTesting.Controls.Add(this.label5);
			this.pnlOCRTesting.Controls.Add(this.nudMaxErrorsPerTestRun);
			this.pnlOCRTesting.Location = new System.Drawing.Point(14, 101);
			this.pnlOCRTesting.Name = "pnlOCRTesting";
			this.pnlOCRTesting.Size = new System.Drawing.Size(363, 34);
			this.pnlOCRTesting.TabIndex = 29;
			// 
			// cbxOcrCameraTestModeAav
			// 
			this.cbxOcrCameraTestModeAav.AutoSize = true;
			this.cbxOcrCameraTestModeAav.Location = new System.Drawing.Point(3, 8);
			this.cbxOcrCameraTestModeAav.Name = "cbxOcrCameraTestModeAav";
			this.cbxOcrCameraTestModeAav.Size = new System.Drawing.Size(182, 17);
			this.cbxOcrCameraTestModeAav.TabIndex = 27;
			this.cbxOcrCameraTestModeAav.Text = "Enable Camera Test Mode (AAV)";
			this.cbxOcrCameraTestModeAav.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(210, 8);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(92, 13);
			this.label5.TabIndex = 26;
			this.label5.Text = "Max errors per run";
			// 
			// nudMaxErrorsPerTestRun
			// 
			this.nudMaxErrorsPerTestRun.Location = new System.Drawing.Point(308, 4);
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
			// btnClearFocuser
			// 
			this.btnClearFocuser.Location = new System.Drawing.Point(283, 63);
			this.btnClearFocuser.Name = "btnClearFocuser";
			this.btnClearFocuser.Size = new System.Drawing.Size(99, 23);
			this.btnClearFocuser.TabIndex = 11;
			this.btnClearFocuser.Text = "Clear Focuser";
			this.btnClearFocuser.UseVisualStyleBackColor = true;
			this.btnClearFocuser.Click += new System.EventHandler(this.btnClearFocuser_Click);
			// 
			// btnClearTelescope
			// 
			this.btnClearTelescope.Location = new System.Drawing.Point(283, 146);
			this.btnClearTelescope.Name = "btnClearTelescope";
			this.btnClearTelescope.Size = new System.Drawing.Size(99, 23);
			this.btnClearTelescope.TabIndex = 12;
			this.btnClearTelescope.Text = "Clear Telescope";
			this.btnClearTelescope.UseVisualStyleBackColor = true;
			this.btnClearTelescope.Click += new System.EventHandler(this.btnClearTelescope_Click);
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
			this.Text = "OccuRec Settings";
			this.Shown += new System.EventHandler(this.frmSettings_Shown);
			this.tabControl.ResumeLayout(false);
			this.tabGeneral.ResumeLayout(false);
			this.tabGeneral.PerformLayout();
			this.tabAAVSettings.ResumeLayout(false);
			this.tabAAVSettings.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCalibrIntegrRate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudGammaDiff)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSignDiffRatio)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinSignDiff)).EndInit();
			this.tabTelControl.ResumeLayout(false);
			this.tabTelControl.PerformLayout();
			this.tabDebug.ResumeLayout(false);
			this.tabDebug.PerformLayout();
			this.pnlOCRTesting.ResumeLayout(false);
			this.pnlOCRTesting.PerformLayout();
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
        private System.Windows.Forms.CheckBox cbxSimulatorRunOCR;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbxImageLayoutMode;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbxNTPServer;
        private System.Windows.Forms.RadioButton rbNativeOCR;
        private System.Windows.Forms.RadioButton rbManagedSim;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cbxFrameProcessingMode;
        private System.Windows.Forms.CheckBox cbDebugIntegration;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.NumericUpDown nudGammaDiff;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.NumericUpDown nudCalibrIntegrRate;
        private System.Windows.Forms.CheckBox cbxGraphDebugMode;
		private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabPage tabTelControl;
        private System.Windows.Forms.CheckBox cbxWarnForFileSystemIssues;
        private System.Windows.Forms.CheckBox cbForceIntegrationRateRestrictions;
        private System.Windows.Forms.Panel pnlOCRTesting;
        private System.Windows.Forms.CheckBox cbxOcrCameraTestModeAav;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudMaxErrorsPerTestRun;
		private System.Windows.Forms.Button btnSelectFocuser;
		private System.Windows.Forms.TextBox tbxFocuser;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblConnectedFocuserInfo;
		private System.Windows.Forms.Label lblConnectedTelescopeInfo;
		private System.Windows.Forms.TextBox tbxTelescope;
		private System.Windows.Forms.Label Telescope;
		private System.Windows.Forms.Button btnSelectTelescope;
		private System.Windows.Forms.Button btnTestFocuserConnection;
		private System.Windows.Forms.Button btnTestTelescopeConnection;
        private System.Windows.Forms.CheckBox cbxLiveTelescopeMode;
		private System.Windows.Forms.Button btnClearTelescope;
		private System.Windows.Forms.Button btnClearFocuser;
	}
}