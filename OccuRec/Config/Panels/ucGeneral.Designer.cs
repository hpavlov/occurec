namespace OccuRec.Config.Panels
{
	partial class ucGeneral
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
            this.cbxWarnForFileSystemIssues = new System.Windows.Forms.CheckBox();
            this.cbxTimeInUT = new System.Windows.Forms.CheckBox();
            this.btnBrowseOutputFolder = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tbxOutputLocation = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.gbxFSWarnings = new System.Windows.Forms.GroupBox();
            this.cbxWarnFAT16 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxWarnFreeSpace = new System.Windows.Forms.CheckBox();
            this.nudWarnGBFreeLeft = new System.Windows.Forms.NumericUpDown();
            this.cbxRunningOnBatteryCheck = new System.Windows.Forms.CheckBox();
            this.cbxUpdateSystemTimeFromNTP = new System.Windows.Forms.CheckBox();
            this.gbxFSWarnings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWarnGBFreeLeft)).BeginInit();
            this.SuspendLayout();
            // 
            // cbxWarnForFileSystemIssues
            // 
            this.cbxWarnForFileSystemIssues.AutoSize = true;
            this.cbxWarnForFileSystemIssues.CheckAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.cbxWarnForFileSystemIssues.Location = new System.Drawing.Point(12, 46);
            this.cbxWarnForFileSystemIssues.Name = "cbxWarnForFileSystemIssues";
            this.cbxWarnForFileSystemIssues.Size = new System.Drawing.Size(230, 17);
            this.cbxWarnForFileSystemIssues.TabIndex = 25;
            this.cbxWarnForFileSystemIssues.Text = "Enable disk space and file system warnings";
            this.cbxWarnForFileSystemIssues.UseVisualStyleBackColor = true;
            this.cbxWarnForFileSystemIssues.CheckedChanged += new System.EventHandler(this.cbxWarnForFileSystemIssues_CheckedChanged);
            // 
            // cbxTimeInUT
            // 
            this.cbxTimeInUT.AutoSize = true;
            this.cbxTimeInUT.Location = new System.Drawing.Point(3, 178);
            this.cbxTimeInUT.Name = "cbxTimeInUT";
            this.cbxTimeInUT.Size = new System.Drawing.Size(115, 17);
            this.cbxTimeInUT.TabIndex = 22;
            this.cbxTimeInUT.Text = "Display Time in UT";
            this.cbxTimeInUT.UseVisualStyleBackColor = true;
            // 
            // btnBrowseOutputFolder
            // 
            this.btnBrowseOutputFolder.Location = new System.Drawing.Point(334, 13);
            this.btnBrowseOutputFolder.Name = "btnBrowseOutputFolder";
            this.btnBrowseOutputFolder.Size = new System.Drawing.Size(30, 23);
            this.btnBrowseOutputFolder.TabIndex = 21;
            this.btnBrowseOutputFolder.Text = "...";
            this.btnBrowseOutputFolder.UseVisualStyleBackColor = true;
            this.btnBrowseOutputFolder.Click += new System.EventHandler(this.btnBrowseOutputFolder_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(113, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Output Video Location";
            // 
            // tbxOutputLocation
            // 
            this.tbxOutputLocation.Location = new System.Drawing.Point(6, 16);
            this.tbxOutputLocation.Name = "tbxOutputLocation";
            this.tbxOutputLocation.Size = new System.Drawing.Size(322, 20);
            this.tbxOutputLocation.TabIndex = 19;
            // 
            // gbxFSWarnings
            // 
            this.gbxFSWarnings.Controls.Add(this.cbxWarnFAT16);
            this.gbxFSWarnings.Controls.Add(this.label1);
            this.gbxFSWarnings.Controls.Add(this.cbxWarnFreeSpace);
            this.gbxFSWarnings.Controls.Add(this.nudWarnGBFreeLeft);
            this.gbxFSWarnings.Location = new System.Drawing.Point(6, 48);
            this.gbxFSWarnings.Name = "gbxFSWarnings";
            this.gbxFSWarnings.Size = new System.Drawing.Size(358, 95);
            this.gbxFSWarnings.TabIndex = 27;
            this.gbxFSWarnings.TabStop = false;
            // 
            // cbxWarnFAT16
            // 
            this.cbxWarnFAT16.AutoSize = true;
            this.cbxWarnFAT16.Location = new System.Drawing.Point(16, 59);
            this.cbxWarnFAT16.Name = "cbxWarnFAT16";
            this.cbxWarnFAT16.Size = new System.Drawing.Size(272, 17);
            this.cbxWarnFAT16.TabIndex = 29;
            this.cbxWarnFAT16.Text = "Warn if FAT-16 file system is used as output location";
            this.cbxWarnFAT16.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(185, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Gb free disk space remaining";
            // 
            // cbxWarnFreeSpace
            // 
            this.cbxWarnFreeSpace.AutoSize = true;
            this.cbxWarnFreeSpace.Location = new System.Drawing.Point(16, 29);
            this.cbxWarnFreeSpace.Name = "cbxWarnFreeSpace";
            this.cbxWarnFreeSpace.Size = new System.Drawing.Size(115, 17);
            this.cbxWarnFreeSpace.TabIndex = 27;
            this.cbxWarnFreeSpace.Text = "Warn on less than ";
            this.cbxWarnFreeSpace.UseVisualStyleBackColor = true;
            // 
            // nudWarnGBFreeLeft
            // 
            this.nudWarnGBFreeLeft.DecimalPlaces = 1;
            this.nudWarnGBFreeLeft.Location = new System.Drawing.Point(133, 28);
            this.nudWarnGBFreeLeft.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudWarnGBFreeLeft.Name = "nudWarnGBFreeLeft";
            this.nudWarnGBFreeLeft.Size = new System.Drawing.Size(49, 20);
            this.nudWarnGBFreeLeft.TabIndex = 26;
            this.nudWarnGBFreeLeft.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // cbxRunningOnBatteryCheck
            // 
            this.cbxRunningOnBatteryCheck.AutoSize = true;
            this.cbxRunningOnBatteryCheck.CheckAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.cbxRunningOnBatteryCheck.Location = new System.Drawing.Point(3, 152);
            this.cbxRunningOnBatteryCheck.Name = "cbxRunningOnBatteryCheck";
            this.cbxRunningOnBatteryCheck.Size = new System.Drawing.Size(189, 17);
            this.cbxRunningOnBatteryCheck.TabIndex = 28;
            this.cbxRunningOnBatteryCheck.Text = "Show warning if running on battery";
            this.cbxRunningOnBatteryCheck.UseVisualStyleBackColor = true;
            // 
            // cbxUpdateSystemTimeFromNTP
            // 
            this.cbxUpdateSystemTimeFromNTP.AutoSize = true;
            this.cbxUpdateSystemTimeFromNTP.Location = new System.Drawing.Point(3, 201);
            this.cbxUpdateSystemTimeFromNTP.Name = "cbxUpdateSystemTimeFromNTP";
            this.cbxUpdateSystemTimeFromNTP.Size = new System.Drawing.Size(166, 17);
            this.cbxUpdateSystemTimeFromNTP.TabIndex = 29;
            this.cbxUpdateSystemTimeFromNTP.Text = "Update system time from NTP";
            this.cbxUpdateSystemTimeFromNTP.UseVisualStyleBackColor = true;
            // 
            // ucGeneral
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbxUpdateSystemTimeFromNTP);
            this.Controls.Add(this.cbxRunningOnBatteryCheck);
            this.Controls.Add(this.cbxWarnForFileSystemIssues);
            this.Controls.Add(this.gbxFSWarnings);
            this.Controls.Add(this.cbxTimeInUT);
            this.Controls.Add(this.btnBrowseOutputFolder);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbxOutputLocation);
            this.Name = "ucGeneral";
            this.Size = new System.Drawing.Size(424, 298);
            this.gbxFSWarnings.ResumeLayout(false);
            this.gbxFSWarnings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWarnGBFreeLeft)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox cbxWarnForFileSystemIssues;
		private System.Windows.Forms.CheckBox cbxTimeInUT;
		private System.Windows.Forms.Button btnBrowseOutputFolder;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbxOutputLocation;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.GroupBox gbxFSWarnings;
		private System.Windows.Forms.CheckBox cbxWarnFAT16;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox cbxWarnFreeSpace;
		private System.Windows.Forms.NumericUpDown nudWarnGBFreeLeft;
        private System.Windows.Forms.CheckBox cbxRunningOnBatteryCheck;
        private System.Windows.Forms.CheckBox cbxUpdateSystemTimeFromNTP;
	}
}
