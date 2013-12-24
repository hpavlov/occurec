namespace OccuRec.Config.Panels
{
	partial class ucDebug
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
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
			this.openAavFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.pnlOCRTesting.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxErrorsPerTestRun)).BeginInit();
			this.gbxOcrTesting.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlOCRTesting
			// 
			this.pnlOCRTesting.Controls.Add(this.cbxOcrCameraTestModeAav);
			this.pnlOCRTesting.Controls.Add(this.label5);
			this.pnlOCRTesting.Controls.Add(this.nudMaxErrorsPerTestRun);
			this.pnlOCRTesting.Location = new System.Drawing.Point(10, 97);
			this.pnlOCRTesting.Name = "pnlOCRTesting";
			this.pnlOCRTesting.Size = new System.Drawing.Size(363, 34);
			this.pnlOCRTesting.TabIndex = 34;
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
			this.cbxGraphDebugMode.Location = new System.Drawing.Point(12, 157);
			this.cbxGraphDebugMode.Name = "cbxGraphDebugMode";
			this.cbxGraphDebugMode.Size = new System.Drawing.Size(150, 17);
			this.cbxGraphDebugMode.TabIndex = 33;
			this.cbxGraphDebugMode.Text = "Video Graph Debug Mode";
			this.cbxGraphDebugMode.UseVisualStyleBackColor = true;
			// 
			// cbxImageLayoutMode
			// 
			this.cbxImageLayoutMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxImageLayoutMode.FormattingEnabled = true;
			this.cbxImageLayoutMode.Location = new System.Drawing.Point(71, 187);
			this.cbxImageLayoutMode.Name = "cbxImageLayoutMode";
			this.cbxImageLayoutMode.Size = new System.Drawing.Size(152, 21);
			this.cbxImageLayoutMode.TabIndex = 32;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(11, 190);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(59, 13);
			this.label8.TabIndex = 30;
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
			this.gbxOcrTesting.Location = new System.Drawing.Point(3, 3);
			this.gbxOcrTesting.Name = "gbxOcrTesting";
			this.gbxOcrTesting.Size = new System.Drawing.Size(378, 89);
			this.gbxOcrTesting.TabIndex = 31;
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
			this.cbxSimulatorRunOCR.ClientSizeChanged += new System.EventHandler(this.cbxSimulatorRunOCR_CheckedChanged);
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
			// openAavFileDialog
			// 
			this.openAavFileDialog.DefaultExt = "aav";
			this.openAavFileDialog.FileName = "Open AAV/AVI File";
			this.openAavFileDialog.Filter = "Supported Video Files (*.aav;*.avi)|*.aav;*.avi";
			// 
			// ucDebug
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlOCRTesting);
			this.Controls.Add(this.cbxGraphDebugMode);
			this.Controls.Add(this.cbxImageLayoutMode);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.gbxOcrTesting);
			this.Name = "ucDebug";
			this.Size = new System.Drawing.Size(428, 274);
			this.pnlOCRTesting.ResumeLayout(false);
			this.pnlOCRTesting.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxErrorsPerTestRun)).EndInit();
			this.gbxOcrTesting.ResumeLayout(false);
			this.gbxOcrTesting.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel pnlOCRTesting;
		private System.Windows.Forms.CheckBox cbxOcrCameraTestModeAav;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown nudMaxErrorsPerTestRun;
		private System.Windows.Forms.CheckBox cbxGraphDebugMode;
		private System.Windows.Forms.ComboBox cbxImageLayoutMode;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox gbxOcrTesting;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.RadioButton rbNativeOCR;
		private System.Windows.Forms.RadioButton rbManagedSim;
		private System.Windows.Forms.CheckBox cbxSimulatorRunOCR;
		private System.Windows.Forms.CheckBox cbxOcrSimlatorTestMode;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox tbxSimlatorFilePath;
		private System.Windows.Forms.Button btnBrowseSimulatorFile;
		private System.Windows.Forms.OpenFileDialog openAavFileDialog;
	}
}
