namespace AAVRec
{
	partial class frmMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tssCameraState = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssFrameNo = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssDisplayRate = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssRecordingFile = new System.Windows.Forms.ToolStripStatusLabel();
            this.msMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.miDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miActions = new System.Windows.Forms.ToolStripMenuItem();
            this.miSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlClient = new System.Windows.Forms.Panel();
            this.pnlVideoFrames = new System.Windows.Forms.Panel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.pnlVideoControls = new System.Windows.Forms.Panel();
            this.btnImageSettings = new System.Windows.Forms.Button();
            this.lblVideoFormat = new System.Windows.Forms.Label();
            this.btnStopRecording = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btnRecord = new System.Windows.Forms.Button();
            this.statusStrip.SuspendLayout();
            this.msMain.SuspendLayout();
            this.pnlClient.SuspendLayout();
            this.pnlVideoFrames.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.pnlVideoControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssCameraState,
            this.tssFrameNo,
            this.tssDisplayRate,
            this.tssRecordingFile});
            this.statusStrip.Location = new System.Drawing.Point(0, 502);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(840, 24);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // tssCameraState
            // 
            this.tssCameraState.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssCameraState.Name = "tssCameraState";
            this.tssCameraState.Size = new System.Drawing.Size(83, 19);
            this.tssCameraState.Text = "Disconnected";
            // 
            // tssFrameNo
            // 
            this.tssFrameNo.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssFrameNo.Name = "tssFrameNo";
            this.tssFrameNo.Size = new System.Drawing.Size(63, 19);
            this.tssFrameNo.Text = "Frame No";
            this.tssFrameNo.Visible = false;
            // 
            // tssDisplayRate
            // 
            this.tssDisplayRate.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssDisplayRate.Name = "tssDisplayRate";
            this.tssDisplayRate.Size = new System.Drawing.Size(75, 19);
            this.tssDisplayRate.Text = "Display Rate";
            this.tssDisplayRate.Visible = false;
            // 
            // tssRecordingFile
            // 
            this.tssRecordingFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.tssRecordingFile.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssRecordingFile.Name = "tssRecordingFile";
            this.tssRecordingFile.Size = new System.Drawing.Size(76, 19);
            this.tssRecordingFile.Text = "File (xxx Mb)";
            this.tssRecordingFile.Visible = false;
            // 
            // msMain
            // 
            this.msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.miActions,
            this.miSettings});
            this.msMain.Location = new System.Drawing.Point(0, 0);
            this.msMain.Name = "msMain";
            this.msMain.Size = new System.Drawing.Size(840, 24);
            this.msMain.TabIndex = 2;
            this.msMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miConnect,
            this.miDisconnect,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(93, 20);
            this.fileToolStripMenuItem.Text = "&Video Camera";
            // 
            // miConnect
            // 
            this.miConnect.Name = "miConnect";
            this.miConnect.Size = new System.Drawing.Size(133, 22);
            this.miConnect.Text = "&Connect";
            this.miConnect.Click += new System.EventHandler(this.miConnect_Click);
            // 
            // miDisconnect
            // 
            this.miDisconnect.Enabled = false;
            this.miDisconnect.Name = "miDisconnect";
            this.miDisconnect.Size = new System.Drawing.Size(133, 22);
            this.miDisconnect.Text = "&Disconnect";
            this.miDisconnect.Click += new System.EventHandler(this.miDisconnect_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(130, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // miActions
            // 
            this.miActions.Enabled = false;
            this.miActions.Name = "miActions";
            this.miActions.Size = new System.Drawing.Size(59, 20);
            this.miActions.Text = "&Actions";
            // 
            // miSettings
            // 
            this.miSettings.Name = "miSettings";
            this.miSettings.Size = new System.Drawing.Size(61, 20);
            this.miSettings.Text = "&Settings";
            this.miSettings.Click += new System.EventHandler(this.miConfigure_Click);
            // 
            // pnlClient
            // 
            this.pnlClient.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnlClient.Controls.Add(this.pnlVideoFrames);
            this.pnlClient.Controls.Add(this.pnlVideoControls);
            this.pnlClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlClient.Location = new System.Drawing.Point(0, 24);
            this.pnlClient.Name = "pnlClient";
            this.pnlClient.Size = new System.Drawing.Size(840, 478);
            this.pnlClient.TabIndex = 4;
            // 
            // pnlVideoFrames
            // 
            this.pnlVideoFrames.Controls.Add(this.pictureBox);
            this.pnlVideoFrames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlVideoFrames.Location = new System.Drawing.Point(0, 0);
            this.pnlVideoFrames.Name = "pnlVideoFrames";
            this.pnlVideoFrames.Size = new System.Drawing.Size(640, 478);
            this.pnlVideoFrames.TabIndex = 2;
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(640, 478);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // pnlVideoControls
            // 
            this.pnlVideoControls.BackColor = System.Drawing.SystemColors.Control;
            this.pnlVideoControls.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlVideoControls.Controls.Add(this.btnImageSettings);
            this.pnlVideoControls.Controls.Add(this.lblVideoFormat);
            this.pnlVideoControls.Controls.Add(this.btnStopRecording);
            this.pnlVideoControls.Controls.Add(this.label5);
            this.pnlVideoControls.Controls.Add(this.btnRecord);
            this.pnlVideoControls.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlVideoControls.Enabled = false;
            this.pnlVideoControls.Location = new System.Drawing.Point(640, 0);
            this.pnlVideoControls.Name = "pnlVideoControls";
            this.pnlVideoControls.Size = new System.Drawing.Size(200, 478);
            this.pnlVideoControls.TabIndex = 1;
            // 
            // btnImageSettings
            // 
            this.btnImageSettings.Location = new System.Drawing.Point(15, 6);
            this.btnImageSettings.Name = "btnImageSettings";
            this.btnImageSettings.Size = new System.Drawing.Size(165, 23);
            this.btnImageSettings.TabIndex = 10;
            this.btnImageSettings.Text = "Configure Image";
            this.btnImageSettings.UseVisualStyleBackColor = true;
            this.btnImageSettings.Click += new System.EventHandler(this.btnImageSettings_Click);
            // 
            // lblVideoFormat
            // 
            this.lblVideoFormat.ForeColor = System.Drawing.Color.Navy;
            this.lblVideoFormat.Location = new System.Drawing.Point(109, 457);
            this.lblVideoFormat.Name = "lblVideoFormat";
            this.lblVideoFormat.Size = new System.Drawing.Size(77, 14);
            this.lblVideoFormat.TabIndex = 7;
            this.lblVideoFormat.Text = "N/A";
            // 
            // btnStopRecording
            // 
            this.btnStopRecording.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStopRecording.Enabled = false;
            this.btnStopRecording.Location = new System.Drawing.Point(104, 427);
            this.btnStopRecording.Name = "btnStopRecording";
            this.btnStopRecording.Size = new System.Drawing.Size(76, 23);
            this.btnStopRecording.TabIndex = 5;
            this.btnStopRecording.Text = "STOP";
            this.btnStopRecording.UseVisualStyleBackColor = true;
            this.btnStopRecording.Click += new System.EventHandler(this.btnStopRecording_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 457);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Video File Format:";
            // 
            // btnRecord
            // 
            this.btnRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRecord.Enabled = false;
            this.btnRecord.Location = new System.Drawing.Point(15, 427);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(76, 23);
            this.btnRecord.TabIndex = 4;
            this.btnRecord.Text = "REC";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 526);
            this.Controls.Add(this.pnlClient);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.msMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.msMain;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.Text = "AAVRec v1.0";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.msMain.ResumeLayout(false);
            this.msMain.PerformLayout();
            this.pnlClient.ResumeLayout(false);
            this.pnlVideoFrames.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.pnlVideoControls.ResumeLayout(false);
            this.pnlVideoControls.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.MenuStrip msMain;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.Panel pnlClient;
		private System.Windows.Forms.Panel pnlVideoControls;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.ToolStripMenuItem miSettings;
		private System.Windows.Forms.ToolStripMenuItem miConnect;
        private System.Windows.Forms.ToolStripMenuItem miDisconnect;
		private System.Windows.Forms.ToolStripStatusLabel tssDisplayRate;
        private System.Windows.Forms.ToolStripStatusLabel tssFrameNo;
		private System.Windows.Forms.Button btnStopRecording;
		private System.Windows.Forms.Button btnRecord;
		private System.Windows.Forms.ToolStripMenuItem miActions;
        private System.Windows.Forms.ToolStripStatusLabel tssCameraState;
		private System.Windows.Forms.Label lblVideoFormat;
		private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.Panel pnlVideoFrames;
		private System.Windows.Forms.ToolStripStatusLabel tssRecordingFile;
        private System.Windows.Forms.Button btnImageSettings;

	}
}

