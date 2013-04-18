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
            this.cbxTimeInUT = new System.Windows.Forms.CheckBox();
            this.btnBrowseOutputFolder = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tbxOutputLocation = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.cbxGraphDebugMode = new System.Windows.Forms.CheckBox();
            this.tabIntegration = new System.Windows.Forms.TabPage();
            this.nudMinSignDiff = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nudSignDiffFactor = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.tabIOTAVTI = new System.Windows.Forms.TabPage();
            this.gbxOcrTesting = new System.Windows.Forms.GroupBox();
            this.btnBrowseDebugFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxOcrDebugOutputLocation = new System.Windows.Forms.TextBox();
            this.cbxOcrTestMode = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbxSimlatorFilePath = new System.Windows.Forms.TextBox();
            this.btnBrowseSimulatorFile = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.openAavFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabIntegration.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinSignDiff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSignDiffFactor)).BeginInit();
            this.tabIOTAVTI.SuspendLayout();
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
            this.tabControl1.Controls.Add(this.tabIntegration);
            this.tabControl1.Controls.Add(this.tabIOTAVTI);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(399, 230);
            this.tabControl1.TabIndex = 7;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.cbxTimeInUT);
            this.tabGeneral.Controls.Add(this.btnBrowseOutputFolder);
            this.tabGeneral.Controls.Add(this.label6);
            this.tabGeneral.Controls.Add(this.tbxOutputLocation);
            this.tabGeneral.Controls.Add(this.label5);
            this.tabGeneral.Controls.Add(this.textBox1);
            this.tabGeneral.Controls.Add(this.cbxGraphDebugMode);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(391, 204);
            this.tabGeneral.TabIndex = 1;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // cbxTimeInUT
            // 
            this.cbxTimeInUT.AutoSize = true;
            this.cbxTimeInUT.Location = new System.Drawing.Point(17, 122);
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
            this.label6.Location = new System.Drawing.Point(17, 18);
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
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "File name format:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(17, 85);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(212, 20);
            this.textBox1.TabIndex = 1;
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
            // tabIntegration
            // 
            this.tabIntegration.Controls.Add(this.nudMinSignDiff);
            this.tabIntegration.Controls.Add(this.label4);
            this.tabIntegration.Controls.Add(this.nudSignDiffFactor);
            this.tabIntegration.Controls.Add(this.label3);
            this.tabIntegration.Location = new System.Drawing.Point(4, 22);
            this.tabIntegration.Name = "tabIntegration";
            this.tabIntegration.Size = new System.Drawing.Size(391, 204);
            this.tabIntegration.TabIndex = 2;
            this.tabIntegration.Text = "Integration Detection";
            this.tabIntegration.UseVisualStyleBackColor = true;
            // 
            // nudMinSignDiff
            // 
            this.nudMinSignDiff.DecimalPlaces = 2;
            this.nudMinSignDiff.Location = new System.Drawing.Point(195, 65);
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(15, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(174, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Minimal Signature Difference:";
            // 
            // nudSignDiffFactor
            // 
            this.nudSignDiffFactor.DecimalPlaces = 1;
            this.nudSignDiffFactor.Location = new System.Drawing.Point(195, 22);
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(21, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(168, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Signature Difference Factor:";
            // 
            // tabIOTAVTI
            // 
            this.tabIOTAVTI.Controls.Add(this.gbxOcrTesting);
            this.tabIOTAVTI.Location = new System.Drawing.Point(4, 22);
            this.tabIOTAVTI.Name = "tabIOTAVTI";
            this.tabIOTAVTI.Padding = new System.Windows.Forms.Padding(3);
            this.tabIOTAVTI.Size = new System.Drawing.Size(391, 204);
            this.tabIOTAVTI.TabIndex = 0;
            this.tabIOTAVTI.Text = "IOTA-VTI TimeStamp OCR Testing";
            this.tabIOTAVTI.UseVisualStyleBackColor = true;
            // 
            // gbxOcrTesting
            // 
            this.gbxOcrTesting.Controls.Add(this.btnBrowseDebugFolder);
            this.gbxOcrTesting.Controls.Add(this.label1);
            this.gbxOcrTesting.Controls.Add(this.tbxOcrDebugOutputLocation);
            this.gbxOcrTesting.Controls.Add(this.cbxOcrTestMode);
            this.gbxOcrTesting.Controls.Add(this.label7);
            this.gbxOcrTesting.Controls.Add(this.tbxSimlatorFilePath);
            this.gbxOcrTesting.Controls.Add(this.btnBrowseSimulatorFile);
            this.gbxOcrTesting.Location = new System.Drawing.Point(7, 7);
            this.gbxOcrTesting.Name = "gbxOcrTesting";
            this.gbxOcrTesting.Size = new System.Drawing.Size(378, 191);
            this.gbxOcrTesting.TabIndex = 19;
            this.gbxOcrTesting.TabStop = false;
            this.gbxOcrTesting.Text = "                                       ";
            // 
            // btnBrowseDebugFolder
            // 
            this.btnBrowseDebugFolder.Location = new System.Drawing.Point(338, 90);
            this.btnBrowseDebugFolder.Name = "btnBrowseDebugFolder";
            this.btnBrowseDebugFolder.Size = new System.Drawing.Size(30, 23);
            this.btnBrowseDebugFolder.TabIndex = 20;
            this.btnBrowseDebugFolder.Text = "...";
            this.btnBrowseDebugFolder.UseVisualStyleBackColor = true;
            this.btnBrowseDebugFolder.Click += new System.EventHandler(this.btnBrowseDebugFolder_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Output Location";
            // 
            // tbxOcrDebugOutputLocation
            // 
            this.tbxOcrDebugOutputLocation.Location = new System.Drawing.Point(10, 93);
            this.tbxOcrDebugOutputLocation.Name = "tbxOcrDebugOutputLocation";
            this.tbxOcrDebugOutputLocation.Size = new System.Drawing.Size(322, 20);
            this.tbxOcrDebugOutputLocation.TabIndex = 18;
            // 
            // cbxOcrTestMode
            // 
            this.cbxOcrTestMode.AutoSize = true;
            this.cbxOcrTestMode.Location = new System.Drawing.Point(14, -1);
            this.cbxOcrTestMode.Name = "cbxOcrTestMode";
            this.cbxOcrTestMode.Size = new System.Drawing.Size(113, 17);
            this.cbxOcrTestMode.TabIndex = 17;
            this.cbxOcrTestMode.Text = "Enable Test Mode";
            this.cbxOcrTestMode.UseVisualStyleBackColor = true;
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
            this.openAavFileDialog.FileName = "Open AAV File";
            this.openAavFileDialog.Filter = "AAV Files (*.aav)|*.aav";
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
            this.tabIntegration.ResumeLayout(false);
            this.tabIntegration.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinSignDiff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSignDiffFactor)).EndInit();
            this.tabIOTAVTI.ResumeLayout(false);
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
        private System.Windows.Forms.TabPage tabIntegration;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudSignDiffFactor;
        private System.Windows.Forms.CheckBox cbxGraphDebugMode;
        private System.Windows.Forms.NumericUpDown nudMinSignDiff;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox1;
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
        private System.Windows.Forms.CheckBox cbxOcrTestMode;
        private System.Windows.Forms.Button btnBrowseDebugFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxOcrDebugOutputLocation;
	}
}