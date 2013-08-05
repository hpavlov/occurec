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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tssCameraState = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssFrameNo = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssOcrErr = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssDroppedFrames = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssDisplayRate = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssIntegrationRate = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssRecordingFile = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlNewVersionAvailable = new System.Windows.Forms.ToolStripStatusLabel();
            this.msMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.miDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelpIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.miYahooGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.miCheckForUpdates = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlClient = new System.Windows.Forms.Panel();
            this.pnlVideoFrames = new System.Windows.Forms.Panel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.pnlVideoControls = new System.Windows.Forms.Panel();
            this.pnlOcrTesting = new System.Windows.Forms.Panel();
            this.btnOcrTesting = new System.Windows.Forms.Button();
            this.pnlCrossbar = new System.Windows.Forms.Panel();
            this.cbxCrossbarInput = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gbxSchedules = new System.Windows.Forms.GroupBox();
            this.pnlNextScheduledAction = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSecheduleWhatsNext = new System.Windows.Forms.Label();
            this.btnClearSchedule = new System.Windows.Forms.Button();
            this.btnAddSchedule = new System.Windows.Forms.Button();
            this.lbSchedule = new System.Windows.Forms.ListBox();
            this.pnlAAV = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.btnLockIntegration = new System.Windows.Forms.Button();
            this.btnImageSettings = new System.Windows.Forms.Button();
            this.lblVideoFormat = new System.Windows.Forms.Label();
            this.btnStopRecording = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btnRecord = new System.Windows.Forms.Button();
            this.timerScheduler = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbConnectDisconnect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbCrosshair = new System.Windows.Forms.ToolStripButton();
            this.tbsAddTarget = new System.Windows.Forms.ToolStripButton();
            this.tsbAddTrackingStar = new System.Windows.Forms.ToolStripButton();
            this.pnlControlArea = new System.Windows.Forms.Panel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.imageListToolbar = new System.Windows.Forms.ImageList(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip.SuspendLayout();
            this.msMain.SuspendLayout();
            this.pnlClient.SuspendLayout();
            this.pnlVideoFrames.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.pnlVideoControls.SuspendLayout();
            this.pnlOcrTesting.SuspendLayout();
            this.pnlCrossbar.SuspendLayout();
            this.gbxSchedules.SuspendLayout();
            this.pnlNextScheduledAction.SuspendLayout();
            this.pnlAAV.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.pnlControlArea.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssCameraState,
            this.tssFrameNo,
            this.tssOcrErr,
            this.tssDroppedFrames,
            this.tssDisplayRate,
            this.tssIntegrationRate,
            this.tssRecordingFile,
            this.pnlNewVersionAvailable});
            this.statusStrip.Location = new System.Drawing.Point(0, 541);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(903, 24);
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
            // tssOcrErr
            // 
            this.tssOcrErr.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssOcrErr.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tssOcrErr.ForeColor = System.Drawing.Color.Red;
            this.tssOcrErr.Name = "tssOcrErr";
            this.tssOcrErr.Size = new System.Drawing.Size(76, 19);
            this.tssOcrErr.Text = "OCR ERR 23";
            this.tssOcrErr.Visible = false;
            // 
            // tssDroppedFrames
            // 
            this.tssDroppedFrames.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssDroppedFrames.ForeColor = System.Drawing.Color.OrangeRed;
            this.tssDroppedFrames.Name = "tssDroppedFrames";
            this.tssDroppedFrames.Size = new System.Drawing.Size(66, 19);
            this.tssDroppedFrames.Text = "0 Dropped";
            this.tssDroppedFrames.Visible = false;
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
            // tssIntegrationRate
            // 
            this.tssIntegrationRate.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssIntegrationRate.Name = "tssIntegrationRate";
            this.tssIntegrationRate.Size = new System.Drawing.Size(92, 19);
            this.tssIntegrationRate.Text = "IntegrationRate";
            this.tssIntegrationRate.Visible = false;
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
            // pnlNewVersionAvailable
            // 
            this.pnlNewVersionAvailable.ActiveLinkColor = System.Drawing.Color.Red;
            this.pnlNewVersionAvailable.BackColor = System.Drawing.Color.Black;
            this.pnlNewVersionAvailable.IsLink = true;
            this.pnlNewVersionAvailable.LinkColor = System.Drawing.Color.Lime;
            this.pnlNewVersionAvailable.Name = "pnlNewVersionAvailable";
            this.pnlNewVersionAvailable.Size = new System.Drawing.Size(313, 19);
            this.pnlNewVersionAvailable.Text = "New version of AAVRec is available. Click here to upgrade.";
            this.pnlNewVersionAvailable.Visible = false;
            this.pnlNewVersionAvailable.Click += new System.EventHandler(this.pnlNewVersionAvailable_Click);
            // 
            // msMain
            // 
            this.msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.miSettings,
            this.miHelp});
            this.msMain.Location = new System.Drawing.Point(0, 0);
            this.msMain.Name = "msMain";
            this.msMain.Size = new System.Drawing.Size(903, 24);
            this.msMain.TabIndex = 2;
            this.msMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miConnect,
            this.miDisconnect,
            this.toolStripSeparator3,
            this.miOpenFile,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.fileToolStripMenuItem.Text = "&Video";
            // 
            // miOpenFile
            // 
            this.miOpenFile.Name = "miOpenFile";
            this.miOpenFile.Size = new System.Drawing.Size(152, 22);
            this.miOpenFile.Text = "&Open File";
            this.miOpenFile.Click += new System.EventHandler(this.miOpenFile_Click);
            // 
            // miConnect
            // 
            this.miConnect.Name = "miConnect";
            this.miConnect.Size = new System.Drawing.Size(152, 22);
            this.miConnect.Text = "&Connect";
            this.miConnect.Click += new System.EventHandler(this.miConnect_Click);
            // 
            // miDisconnect
            // 
            this.miDisconnect.Enabled = false;
            this.miDisconnect.Name = "miDisconnect";
            this.miDisconnect.Size = new System.Drawing.Size(152, 22);
            this.miDisconnect.Text = "&Disconnect";
            this.miDisconnect.Click += new System.EventHandler(this.miDisconnect_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(149, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // miSettings
            // 
            this.miSettings.Name = "miSettings";
            this.miSettings.Size = new System.Drawing.Size(61, 20);
            this.miSettings.Text = "&Settings";
            this.miSettings.Click += new System.EventHandler(this.miConfigure_Click);
            // 
            // miHelp
            // 
            this.miHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miHelpIndex,
            this.miYahooGroup,
            this.toolStripMenuItem1,
            this.miCheckForUpdates});
            this.miHelp.Name = "miHelp";
            this.miHelp.Size = new System.Drawing.Size(44, 20);
            this.miHelp.Text = "&Help";
            // 
            // miHelpIndex
            // 
            this.miHelpIndex.Image = ((System.Drawing.Image)(resources.GetObject("miHelpIndex.Image")));
            this.miHelpIndex.Name = "miHelpIndex";
            this.miHelpIndex.Size = new System.Drawing.Size(189, 22);
            this.miHelpIndex.Text = "&Index";
            this.miHelpIndex.Click += new System.EventHandler(this.miHelpIndex_Click);
            // 
            // miYahooGroup
            // 
            this.miYahooGroup.Image = ((System.Drawing.Image)(resources.GetObject("miYahooGroup.Image")));
            this.miYahooGroup.Name = "miYahooGroup";
            this.miYahooGroup.Size = new System.Drawing.Size(189, 22);
            this.miYahooGroup.Text = "AAVRec Yahoo Group";
            this.miYahooGroup.Click += new System.EventHandler(this.miYahooGroup_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(186, 6);
            // 
            // miCheckForUpdates
            // 
            this.miCheckForUpdates.Name = "miCheckForUpdates";
            this.miCheckForUpdates.Size = new System.Drawing.Size(189, 22);
            this.miCheckForUpdates.Text = "&Check for Updates";
            this.miCheckForUpdates.Click += new System.EventHandler(this.miCheckForUpdates_Click);
            // 
            // pnlClient
            // 
            this.pnlClient.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnlClient.Controls.Add(this.pnlVideoFrames);
            this.pnlClient.Controls.Add(this.pnlVideoControls);
            this.pnlClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlClient.Location = new System.Drawing.Point(0, 0);
            this.pnlClient.Name = "pnlClient";
            this.pnlClient.Size = new System.Drawing.Size(903, 492);
            this.pnlClient.TabIndex = 4;
            // 
            // pnlVideoFrames
            // 
            this.pnlVideoFrames.Controls.Add(this.pictureBox);
            this.pnlVideoFrames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlVideoFrames.Location = new System.Drawing.Point(0, 0);
            this.pnlVideoFrames.Name = "pnlVideoFrames";
            this.pnlVideoFrames.Size = new System.Drawing.Size(703, 492);
            this.pnlVideoFrames.TabIndex = 2;
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(703, 492);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // pnlVideoControls
            // 
            this.pnlVideoControls.BackColor = System.Drawing.SystemColors.Control;
            this.pnlVideoControls.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlVideoControls.Controls.Add(this.pnlOcrTesting);
            this.pnlVideoControls.Controls.Add(this.pnlCrossbar);
            this.pnlVideoControls.Controls.Add(this.gbxSchedules);
            this.pnlVideoControls.Controls.Add(this.pnlAAV);
            this.pnlVideoControls.Controls.Add(this.btnImageSettings);
            this.pnlVideoControls.Controls.Add(this.lblVideoFormat);
            this.pnlVideoControls.Controls.Add(this.btnStopRecording);
            this.pnlVideoControls.Controls.Add(this.label5);
            this.pnlVideoControls.Controls.Add(this.btnRecord);
            this.pnlVideoControls.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlVideoControls.Enabled = false;
            this.pnlVideoControls.Location = new System.Drawing.Point(703, 0);
            this.pnlVideoControls.Name = "pnlVideoControls";
            this.pnlVideoControls.Size = new System.Drawing.Size(200, 492);
            this.pnlVideoControls.TabIndex = 1;
            // 
            // pnlOcrTesting
            // 
            this.pnlOcrTesting.Controls.Add(this.btnOcrTesting);
            this.pnlOcrTesting.Location = new System.Drawing.Point(4, 146);
            this.pnlOcrTesting.Name = "pnlOcrTesting";
            this.pnlOcrTesting.Size = new System.Drawing.Size(189, 85);
            this.pnlOcrTesting.TabIndex = 25;
            this.pnlOcrTesting.Visible = false;
            // 
            // btnOcrTesting
            // 
            this.btnOcrTesting.Location = new System.Drawing.Point(11, 15);
            this.btnOcrTesting.Name = "btnOcrTesting";
            this.btnOcrTesting.Size = new System.Drawing.Size(162, 23);
            this.btnOcrTesting.TabIndex = 1;
            this.btnOcrTesting.Text = "Run OCR Testing";
            this.btnOcrTesting.UseVisualStyleBackColor = true;
            this.btnOcrTesting.Click += new System.EventHandler(this.btnOcrTesting_Click);
            // 
            // pnlCrossbar
            // 
            this.pnlCrossbar.Controls.Add(this.cbxCrossbarInput);
            this.pnlCrossbar.Controls.Add(this.label2);
            this.pnlCrossbar.Location = new System.Drawing.Point(8, 31);
            this.pnlCrossbar.Name = "pnlCrossbar";
            this.pnlCrossbar.Size = new System.Drawing.Size(178, 51);
            this.pnlCrossbar.TabIndex = 24;
            this.pnlCrossbar.Visible = false;
            // 
            // cbxCrossbarInput
            // 
            this.cbxCrossbarInput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCrossbarInput.FormattingEnabled = true;
            this.cbxCrossbarInput.Location = new System.Drawing.Point(7, 21);
            this.cbxCrossbarInput.Name = "cbxCrossbarInput";
            this.cbxCrossbarInput.Size = new System.Drawing.Size(165, 21);
            this.cbxCrossbarInput.TabIndex = 18;
            this.cbxCrossbarInput.SelectedIndexChanged += new System.EventHandler(this.cbxCrossbarInput_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Crossbar Input";
            // 
            // gbxSchedules
            // 
            this.gbxSchedules.Controls.Add(this.pnlNextScheduledAction);
            this.gbxSchedules.Controls.Add(this.btnClearSchedule);
            this.gbxSchedules.Controls.Add(this.btnAddSchedule);
            this.gbxSchedules.Controls.Add(this.lbSchedule);
            this.gbxSchedules.Location = new System.Drawing.Point(6, 237);
            this.gbxSchedules.Name = "gbxSchedules";
            this.gbxSchedules.Size = new System.Drawing.Size(187, 184);
            this.gbxSchedules.TabIndex = 12;
            this.gbxSchedules.TabStop = false;
            this.gbxSchedules.Text = "Schedule";
            // 
            // pnlNextScheduledAction
            // 
            this.pnlNextScheduledAction.Controls.Add(this.label1);
            this.pnlNextScheduledAction.Controls.Add(this.lblSecheduleWhatsNext);
            this.pnlNextScheduledAction.Location = new System.Drawing.Point(2, 150);
            this.pnlNextScheduledAction.Name = "pnlNextScheduledAction";
            this.pnlNextScheduledAction.Size = new System.Drawing.Size(176, 29);
            this.pnlNextScheduledAction.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "NEXT:";
            // 
            // lblSecheduleWhatsNext
            // 
            this.lblSecheduleWhatsNext.AutoSize = true;
            this.lblSecheduleWhatsNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSecheduleWhatsNext.ForeColor = System.Drawing.Color.Red;
            this.lblSecheduleWhatsNext.Location = new System.Drawing.Point(44, 8);
            this.lblSecheduleWhatsNext.Name = "lblSecheduleWhatsNext";
            this.lblSecheduleWhatsNext.Size = new System.Drawing.Size(98, 13);
            this.lblSecheduleWhatsNext.TabIndex = 8;
            this.lblSecheduleWhatsNext.Text = "Rec in 00:02:34";
            // 
            // btnClearSchedule
            // 
            this.btnClearSchedule.Location = new System.Drawing.Point(76, 19);
            this.btnClearSchedule.Name = "btnClearSchedule";
            this.btnClearSchedule.Size = new System.Drawing.Size(63, 23);
            this.btnClearSchedule.TabIndex = 6;
            this.btnClearSchedule.Text = "Clear All";
            this.btnClearSchedule.UseVisualStyleBackColor = true;
            this.btnClearSchedule.Click += new System.EventHandler(this.btnClearSchedule_Click);
            // 
            // btnAddSchedule
            // 
            this.btnAddSchedule.Location = new System.Drawing.Point(7, 19);
            this.btnAddSchedule.Name = "btnAddSchedule";
            this.btnAddSchedule.Size = new System.Drawing.Size(63, 23);
            this.btnAddSchedule.TabIndex = 5;
            this.btnAddSchedule.Text = "Add";
            this.btnAddSchedule.UseVisualStyleBackColor = true;
            this.btnAddSchedule.Click += new System.EventHandler(this.btnAddSchedule_Click);
            // 
            // lbSchedule
            // 
            this.lbSchedule.FormattingEnabled = true;
            this.lbSchedule.Location = new System.Drawing.Point(7, 48);
            this.lbSchedule.Name = "lbSchedule";
            this.lbSchedule.ScrollAlwaysVisible = true;
            this.lbSchedule.Size = new System.Drawing.Size(167, 95);
            this.lbSchedule.TabIndex = 0;
            // 
            // pnlAAV
            // 
            this.pnlAAV.Controls.Add(this.label3);
            this.pnlAAV.Controls.Add(this.btnLockIntegration);
            this.pnlAAV.Location = new System.Drawing.Point(4, 88);
            this.pnlAAV.Name = "pnlAAV";
            this.pnlAAV.Size = new System.Drawing.Size(189, 51);
            this.pnlAAV.TabIndex = 11;
            this.pnlAAV.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Integration Processing";
            // 
            // btnLockIntegration
            // 
            this.btnLockIntegration.Enabled = false;
            this.btnLockIntegration.Location = new System.Drawing.Point(12, 22);
            this.btnLockIntegration.Name = "btnLockIntegration";
            this.btnLockIntegration.Size = new System.Drawing.Size(162, 23);
            this.btnLockIntegration.TabIndex = 0;
            this.btnLockIntegration.Text = "Lock at x4";
            this.btnLockIntegration.UseVisualStyleBackColor = true;
            this.btnLockIntegration.Click += new System.EventHandler(this.btnLockIntegration_Click);
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
            this.lblVideoFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblVideoFormat.ForeColor = System.Drawing.Color.Navy;
            this.lblVideoFormat.Location = new System.Drawing.Point(106, 463);
            this.lblVideoFormat.Name = "lblVideoFormat";
            this.lblVideoFormat.Size = new System.Drawing.Size(77, 14);
            this.lblVideoFormat.TabIndex = 7;
            this.lblVideoFormat.Text = "N/A";
            // 
            // btnStopRecording
            // 
            this.btnStopRecording.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStopRecording.Enabled = false;
            this.btnStopRecording.Location = new System.Drawing.Point(101, 435);
            this.btnStopRecording.Name = "btnStopRecording";
            this.btnStopRecording.Size = new System.Drawing.Size(76, 23);
            this.btnStopRecording.TabIndex = 5;
            this.btnStopRecording.Text = "STOP";
            this.btnStopRecording.UseVisualStyleBackColor = true;
            this.btnStopRecording.Click += new System.EventHandler(this.btnStopRecording_Click);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 463);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Video File Format:";
            // 
            // btnRecord
            // 
            this.btnRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRecord.Enabled = false;
            this.btnRecord.Location = new System.Drawing.Point(12, 435);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(76, 23);
            this.btnRecord.TabIndex = 4;
            this.btnRecord.Text = "REC";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // timerScheduler
            // 
            this.timerScheduler.Enabled = true;
            this.timerScheduler.Interval = 1000;
            this.timerScheduler.Tick += new System.EventHandler(this.timerScheduler_Tick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbConnectDisconnect,
            this.toolStripSeparator1,
            this.tsbCrosshair,
            this.tbsAddTarget,
            this.tsbAddTrackingStar});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(903, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbConnectDisconnect
            // 
            this.tsbConnectDisconnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConnectDisconnect.Image = ((System.Drawing.Image)(resources.GetObject("tsbConnectDisconnect.Image")));
            this.tsbConnectDisconnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConnectDisconnect.Name = "tsbConnectDisconnect";
            this.tsbConnectDisconnect.Size = new System.Drawing.Size(23, 22);
            this.tsbConnectDisconnect.Text = "toolStripButton1";
            this.tsbConnectDisconnect.Click += new System.EventHandler(this.tsbConnectDisconnect_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbCrosshair
            // 
            this.tsbCrosshair.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCrosshair.Image = ((System.Drawing.Image)(resources.GetObject("tsbCrosshair.Image")));
            this.tsbCrosshair.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCrosshair.Name = "tsbCrosshair";
            this.tsbCrosshair.Size = new System.Drawing.Size(23, 22);
            this.tsbCrosshair.ToolTipText = "Crosshair";
            this.tsbCrosshair.Visible = false;
            // 
            // tbsAddTarget
            // 
            this.tbsAddTarget.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbsAddTarget.Image = ((System.Drawing.Image)(resources.GetObject("tbsAddTarget.Image")));
            this.tbsAddTarget.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbsAddTarget.Name = "tbsAddTarget";
            this.tbsAddTarget.Size = new System.Drawing.Size(23, 22);
            this.tbsAddTarget.ToolTipText = "Select Target";
            // 
            // tsbAddTrackingStar
            // 
            this.tsbAddTrackingStar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddTrackingStar.Image = ((System.Drawing.Image)(resources.GetObject("tsbAddTrackingStar.Image")));
            this.tsbAddTrackingStar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddTrackingStar.Name = "tsbAddTrackingStar";
            this.tsbAddTrackingStar.Size = new System.Drawing.Size(23, 22);
            this.tsbAddTrackingStar.ToolTipText = "Select Tracking Star";
            // 
            // pnlControlArea
            // 
            this.pnlControlArea.Controls.Add(this.pnlClient);
            this.pnlControlArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlControlArea.Location = new System.Drawing.Point(0, 49);
            this.pnlControlArea.Name = "pnlControlArea";
            this.pnlControlArea.Size = new System.Drawing.Size(903, 492);
            this.pnlControlArea.TabIndex = 6;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(109, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // imageListToolbar
            // 
            this.imageListToolbar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListToolbar.ImageStream")));
            this.imageListToolbar.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListToolbar.Images.SetKeyName(0, "Camera-Transparent-32x32.png");
            this.imageListToolbar.Images.SetKeyName(1, "Camera-Transparent-32x32-Disc.png");
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "avi";
            this.openFileDialog.Filter = "Support Video Files (*.avi)|*.avi";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(903, 565);
            this.Controls.Add(this.pnlControlArea);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.msMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.msMain;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.Text = "AAVRec ";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.msMain.ResumeLayout(false);
            this.msMain.PerformLayout();
            this.pnlClient.ResumeLayout(false);
            this.pnlVideoFrames.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.pnlVideoControls.ResumeLayout(false);
            this.pnlVideoControls.PerformLayout();
            this.pnlOcrTesting.ResumeLayout(false);
            this.pnlCrossbar.ResumeLayout(false);
            this.pnlCrossbar.PerformLayout();
            this.gbxSchedules.ResumeLayout(false);
            this.pnlNextScheduledAction.ResumeLayout(false);
            this.pnlNextScheduledAction.PerformLayout();
            this.pnlAAV.ResumeLayout(false);
            this.pnlAAV.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnlControlArea.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripStatusLabel tssCameraState;
		private System.Windows.Forms.Label lblVideoFormat;
		private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.Panel pnlVideoFrames;
		private System.Windows.Forms.ToolStripStatusLabel tssRecordingFile;
        private System.Windows.Forms.Button btnImageSettings;
        private System.Windows.Forms.ToolStripStatusLabel tssIntegrationRate;
        private System.Windows.Forms.Panel pnlAAV;
        private System.Windows.Forms.Button btnLockIntegration;
        private System.Windows.Forms.GroupBox gbxSchedules;
        private System.Windows.Forms.Button btnClearSchedule;
        private System.Windows.Forms.Button btnAddSchedule;
        private System.Windows.Forms.ListBox lbSchedule;
        private System.Windows.Forms.Label lblSecheduleWhatsNext;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlNextScheduledAction;
        private System.Windows.Forms.Timer timerScheduler;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlCrossbar;
        private System.Windows.Forms.ComboBox cbxCrossbarInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlOcrTesting;
        private System.Windows.Forms.Button btnOcrTesting;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel pnlControlArea;
        private System.Windows.Forms.ToolStripButton tsbCrosshair;
        private System.Windows.Forms.ToolStripStatusLabel tssOcrErr;
        private System.Windows.Forms.ToolStripMenuItem miHelp;
        private System.Windows.Forms.ToolStripMenuItem miHelpIndex;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem miCheckForUpdates;
        private System.Windows.Forms.ToolStripMenuItem miYahooGroup;
        private System.Windows.Forms.ToolStripButton tsbConnectDisconnect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripStatusLabel pnlNewVersionAvailable;
        private System.Windows.Forms.ImageList imageListToolbar;
        private System.Windows.Forms.ToolStripStatusLabel tssDroppedFrames;
		private System.Windows.Forms.ToolStripButton tbsAddTarget;
		private System.Windows.Forms.ToolStripButton tsbAddTrackingStar;
		private System.Windows.Forms.ToolStripMenuItem miOpenFile;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.OpenFileDialog openFileDialog;

	}
}

