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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings));
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.tbxNTPServer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxTimeInUT = new System.Windows.Forms.CheckBox();
            this.btnBrowseOutputFolder = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tbxOutputLocation = new System.Windows.Forms.TextBox();
            this.cbxGraphDebugMode = new System.Windows.Forms.CheckBox();
            this.tabAAVSettings = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.nudSignDiffFactor = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nudMinSignDiff = new System.Windows.Forms.NumericUpDown();
            this.cbxImageLayoutMode = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tabIOTAVTI = new System.Windows.Forms.TabPage();
            this.cbxOcrCameraTestModeAav = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.nudMaxErrorsPerTestRun = new System.Windows.Forms.NumericUpDown();
            this.cbxOcrCameraTestModeAvi = new System.Windows.Forms.CheckBox();
            this.btnBrowseDebugFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxOcrDebugOutputLocation = new System.Windows.Forms.TextBox();
            this.gbxOcrTesting = new System.Windows.Forms.GroupBox();
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
            this.label9 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabAAVSettings.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSignDiffFactor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinSignDiff)).BeginInit();
            this.tabIOTAVTI.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxErrorsPerTestRun)).BeginInit();
            this.gbxOcrTesting.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(332, 248);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabGeneral);
            this.tabControl1.Controls.Add(this.tabAAVSettings);
            this.tabControl1.Controls.Add(this.tabIOTAVTI);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(399, 230);
            this.tabControl1.TabIndex = 7;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.tbxNTPServer);
            this.tabGeneral.Controls.Add(this.label2);
            this.tabGeneral.Controls.Add(this.cbxTimeInUT);
            this.tabGeneral.Controls.Add(this.btnBrowseOutputFolder);
            this.tabGeneral.Controls.Add(this.label6);
            this.tabGeneral.Controls.Add(this.tbxOutputLocation);
            this.tabGeneral.Controls.Add(this.cbxGraphDebugMode);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(391, 204);
            this.tabGeneral.TabIndex = 1;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // tbxNTPServer
            // 
            this.tbxNTPServer.Location = new System.Drawing.Point(17, 82);
            this.tbxNTPServer.Name = "tbxNTPServer";
            this.tbxNTPServer.Size = new System.Drawing.Size(170, 20);
            this.tbxNTPServer.TabIndex = 17;
            this.tbxNTPServer.Text = "time.windows.com";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "NTP Server";
            // 
            // cbxTimeInUT
            // 
            this.cbxTimeInUT.AutoSize = true;
            this.cbxTimeInUT.Location = new System.Drawing.Point(224, 85);
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
            // cbxGraphDebugMode
            // 
            this.cbxGraphDebugMode.AutoSize = true;
            this.cbxGraphDebugMode.Location = new System.Drawing.Point(17, 170);
            this.cbxGraphDebugMode.Name = "cbxGraphDebugMode";
            this.cbxGraphDebugMode.Size = new System.Drawing.Size(150, 17);
            this.cbxGraphDebugMode.TabIndex = 0;
            this.cbxGraphDebugMode.Text = "Video Graph Debug Mode";
            this.cbxGraphDebugMode.UseVisualStyleBackColor = true;
            // 
            // tabAAVSettings
            // 
            this.tabAAVSettings.Controls.Add(this.groupBox1);
            this.tabAAVSettings.Controls.Add(this.cbxImageLayoutMode);
            this.tabAAVSettings.Controls.Add(this.label8);
            this.tabAAVSettings.Location = new System.Drawing.Point(4, 22);
            this.tabAAVSettings.Name = "tabAAVSettings";
            this.tabAAVSettings.Size = new System.Drawing.Size(391, 204);
            this.tabAAVSettings.TabIndex = 2;
            this.tabAAVSettings.Text = "AAV";
            this.tabAAVSettings.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.nudSignDiffFactor);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.nudMinSignDiff);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(15, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(325, 99);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Integration Detection";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(25, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Signature Difference Factor:";
            // 
            // nudSignDiffFactor
            // 
            this.nudSignDiffFactor.DecimalPlaces = 1;
            this.nudSignDiffFactor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudSignDiffFactor.Location = new System.Drawing.Point(199, 29);
            this.nudSignDiffFactor.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudSignDiffFactor.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSignDiffFactor.Name = "nudSignDiffFactor";
            this.nudSignDiffFactor.Size = new System.Drawing.Size(47, 20);
            this.nudSignDiffFactor.TabIndex = 5;
            this.nudSignDiffFactor.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(19, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(145, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Minimal Signature Difference:";
            // 
            // nudMinSignDiff
            // 
            this.nudMinSignDiff.DecimalPlaces = 2;
            this.nudMinSignDiff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudMinSignDiff.Location = new System.Drawing.Point(199, 61);
            this.nudMinSignDiff.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudMinSignDiff.Name = "nudMinSignDiff";
            this.nudMinSignDiff.Size = new System.Drawing.Size(47, 20);
            this.nudMinSignDiff.TabIndex = 7;
            this.nudMinSignDiff.Value = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            // 
            // cbxImageLayoutMode
            // 
            this.cbxImageLayoutMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxImageLayoutMode.FormattingEnabled = true;
            this.cbxImageLayoutMode.Location = new System.Drawing.Point(24, 161);
            this.cbxImageLayoutMode.Name = "cbxImageLayoutMode";
            this.cbxImageLayoutMode.Size = new System.Drawing.Size(152, 21);
            this.cbxImageLayoutMode.TabIndex = 20;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 142);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(93, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Aav Image Layout";
            // 
            // tabIOTAVTI
            // 
            this.tabIOTAVTI.Controls.Add(this.cbxOcrCameraTestModeAav);
            this.tabIOTAVTI.Controls.Add(this.label5);
            this.tabIOTAVTI.Controls.Add(this.nudMaxErrorsPerTestRun);
            this.tabIOTAVTI.Controls.Add(this.cbxOcrCameraTestModeAvi);
            this.tabIOTAVTI.Controls.Add(this.btnBrowseDebugFolder);
            this.tabIOTAVTI.Controls.Add(this.label1);
            this.tabIOTAVTI.Controls.Add(this.tbxOcrDebugOutputLocation);
            this.tabIOTAVTI.Controls.Add(this.gbxOcrTesting);
            this.tabIOTAVTI.Location = new System.Drawing.Point(4, 22);
            this.tabIOTAVTI.Name = "tabIOTAVTI";
            this.tabIOTAVTI.Padding = new System.Windows.Forms.Padding(3);
            this.tabIOTAVTI.Size = new System.Drawing.Size(391, 204);
            this.tabIOTAVTI.TabIndex = 0;
            this.tabIOTAVTI.Text = "OCR Testing";
            this.tabIOTAVTI.UseVisualStyleBackColor = true;
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
            // btnBrowseDebugFolder
            // 
            this.btnBrowseDebugFolder.Location = new System.Drawing.Point(344, 164);
            this.btnBrowseDebugFolder.Name = "btnBrowseDebugFolder";
            this.btnBrowseDebugFolder.Size = new System.Drawing.Size(30, 23);
            this.btnBrowseDebugFolder.TabIndex = 23;
            this.btnBrowseDebugFolder.Text = "...";
            this.btnBrowseDebugFolder.UseVisualStyleBackColor = true;
            this.btnBrowseDebugFolder.Click += new System.EventHandler(this.btnBrowseDebugFolder_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 151);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Output Location";
            // 
            // tbxOcrDebugOutputLocation
            // 
            this.tbxOcrDebugOutputLocation.Location = new System.Drawing.Point(16, 167);
            this.tbxOcrDebugOutputLocation.Name = "tbxOcrDebugOutputLocation";
            this.tbxOcrDebugOutputLocation.Size = new System.Drawing.Size(322, 20);
            this.tbxOcrDebugOutputLocation.TabIndex = 21;
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
            this.btnSave.Location = new System.Drawing.Point(251, 248);
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
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(215, 71);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(10, 13);
            this.label9.TabIndex = 28;
            this.label9.Text = "]";
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 283);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AAVRec Settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.tabAAVSettings.ResumeLayout(false);
            this.tabAAVSettings.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSignDiffFactor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinSignDiff)).EndInit();
            this.tabIOTAVTI.ResumeLayout(false);
            this.tabIOTAVTI.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxErrorsPerTestRun)).EndInit();
            this.gbxOcrTesting.ResumeLayout(false);
            this.gbxOcrTesting.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabIOTAVTI;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TabPage tabAAVSettings;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudSignDiffFactor;
        private System.Windows.Forms.CheckBox cbxGraphDebugMode;
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
        private System.Windows.Forms.Button btnBrowseDebugFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxOcrDebugOutputLocation;
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
	}
}