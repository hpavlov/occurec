namespace OccuRec
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
            this.ttsProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.tssCameraState = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssFrameNo = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssIntegrationRate = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssDroppedFrames = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssASCOMTelescope = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssASCOMFocuser = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssCameraControl = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssOcrErr = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssNTP = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssVTIOSD = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsbtnDisplayMode = new System.Windows.Forms.ToolStripSplitButton();
            this.tsmiInverted = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHueIntensity = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiHigh = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOff = new System.Windows.Forms.ToolStripMenuItem();
            this.tssRecordingFile = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssFreeDiskSpace = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlNewVersionAvailable = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssDisplayRate = new System.Windows.Forms.ToolStripStatusLabel();
            this.msMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.miASCOMConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.miDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.miOpenFile = new System.Windows.Forms.ToolStripMenuItem();
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
            this.pnlVideo = new System.Windows.Forms.Panel();
            this.picVideoFrame = new System.Windows.Forms.PictureBox();
            this.pnlTargets = new System.Windows.Forms.Panel();
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
            this.btnImageSettings = new System.Windows.Forms.Button();
            this.lblVideoFormat = new System.Windows.Forms.Label();
            this.btnStopRecording = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btnRecord = new System.Windows.Forms.Button();
            this.pnlAAV = new System.Windows.Forms.Panel();
            this.btnManualIntegration = new System.Windows.Forms.Button();
            this.pbarIntDetPercentDone = new System.Windows.Forms.ProgressBar();
            this.btnCalibrateIntegration = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnLockIntegration = new System.Windows.Forms.Button();
            this.pnlOneStacking = new System.Windows.Forms.Panel();
            this.lblOneStack = new System.Windows.Forms.Label();
            this.btnOneStack = new System.Windows.Forms.Button();
            this.pnlVtiOsd = new System.Windows.Forms.Panel();
            this.btnUpdateVtiOsd = new System.Windows.Forms.Button();
            this.btnConfirmUserVtiOsd = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.timerScheduler = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbConnectDisconnect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbCrosshair = new System.Windows.Forms.ToolStripButton();
            this.tsbAddGuidingStar = new System.Windows.Forms.ToolStripButton();
            this.tbsAddTarget = new System.Windows.Forms.ToolStripButton();
            this.tsbClearTargets = new System.Windows.Forms.ToolStripLabel();
            this.tsSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbTelControl = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbFocControl = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbCamControl = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tslTelFocStatus = new System.Windows.Forms.ToolStripLabel();
            this.pnlControlArea = new System.Windows.Forms.Panel();
            this.imageListToolbar = new System.Windows.Forms.ImageList(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.ucVideoControl = new OccuRec.Controls.ucCameraControl();
            this.statusStrip.SuspendLayout();
            this.msMain.SuspendLayout();
            this.pnlClient.SuspendLayout();
            this.pnlVideoFrames.SuspendLayout();
            this.pnlVideo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picVideoFrame)).BeginInit();
            this.pnlVideoControls.SuspendLayout();
            this.pnlOcrTesting.SuspendLayout();
            this.pnlCrossbar.SuspendLayout();
            this.gbxSchedules.SuspendLayout();
            this.pnlNextScheduledAction.SuspendLayout();
            this.pnlAAV.SuspendLayout();
            this.pnlOneStacking.SuspendLayout();
            this.pnlVtiOsd.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.pnlControlArea.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ttsProgressBar,
            this.tssCameraState,
            this.tssFrameNo,
            this.tssIntegrationRate,
            this.tssDroppedFrames,
            this.tssASCOMTelescope,
            this.tssASCOMFocuser,
            this.tssCameraControl,
            this.tssOcrErr,
            this.tssNTP,
            this.tssVTIOSD,
            this.tsbtnDisplayMode,
            this.tssRecordingFile,
            this.tssFreeDiskSpace,
            this.pnlNewVersionAvailable});
            this.statusStrip.Location = new System.Drawing.Point(0, 625);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(920, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // ttsProgressBar
            // 
            this.ttsProgressBar.Name = "ttsProgressBar";
            this.ttsProgressBar.Size = new System.Drawing.Size(100, 18);
            this.ttsProgressBar.Step = 5;
            this.ttsProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ttsProgressBar.Visible = false;
            // 
            // tssCameraState
            // 
            this.tssCameraState.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssCameraState.Name = "tssCameraState";
            this.tssCameraState.Size = new System.Drawing.Size(75, 17);
            this.tssCameraState.Text = "Disconnected";
            // 
            // tssFrameNo
            // 
            this.tssFrameNo.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssFrameNo.Name = "tssFrameNo";
            this.tssFrameNo.Size = new System.Drawing.Size(57, 19);
            this.tssFrameNo.Text = "Frame No";
            this.tssFrameNo.Visible = false;
            // 
            // tssIntegrationRate
            // 
            this.tssIntegrationRate.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssIntegrationRate.Name = "tssIntegrationRate";
            this.tssIntegrationRate.Size = new System.Drawing.Size(88, 19);
            this.tssIntegrationRate.Text = "IntegrationRate";
            this.tssIntegrationRate.Visible = false;
            // 
            // tssDroppedFrames
            // 
            this.tssDroppedFrames.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssDroppedFrames.ForeColor = System.Drawing.Color.OrangeRed;
            this.tssDroppedFrames.Name = "tssDroppedFrames";
            this.tssDroppedFrames.Size = new System.Drawing.Size(61, 19);
            this.tssDroppedFrames.Text = "0 Dropped";
            this.tssDroppedFrames.Visible = false;
            // 
            // tssASCOMTelescope
            // 
            this.tssASCOMTelescope.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssASCOMTelescope.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tssASCOMTelescope.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tssASCOMTelescope.ForeColor = System.Drawing.Color.Green;
            this.tssASCOMTelescope.Name = "tssASCOMTelescope";
            this.tssASCOMTelescope.Size = new System.Drawing.Size(30, 17);
            this.tssASCOMTelescope.Text = "TEL";
            // 
            // tssASCOMFocuser
            // 
            this.tssASCOMFocuser.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssASCOMFocuser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tssASCOMFocuser.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tssASCOMFocuser.ForeColor = System.Drawing.Color.Green;
            this.tssASCOMFocuser.Name = "tssASCOMFocuser";
            this.tssASCOMFocuser.Size = new System.Drawing.Size(32, 17);
            this.tssASCOMFocuser.Text = "FOC";
            // 
            // tssCameraControl
            // 
            this.tssCameraControl.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssCameraControl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tssCameraControl.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tssCameraControl.ForeColor = System.Drawing.Color.Green;
            this.tssCameraControl.Name = "tssCameraControl";
            this.tssCameraControl.Size = new System.Drawing.Size(31, 17);
            this.tssCameraControl.Text = "VID";
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
            // tssNTP
            // 
            this.tssNTP.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssNTP.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tssNTP.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.tssNTP.Name = "tssNTP";
            this.tssNTP.Size = new System.Drawing.Size(32, 19);
            this.tssNTP.Text = "NTP";
            this.tssNTP.Visible = false;
            // 
            // tssVTIOSD
            // 
            this.tssVTIOSD.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssVTIOSD.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tssVTIOSD.ForeColor = System.Drawing.Color.Red;
            this.tssVTIOSD.Name = "tssVTIOSD";
            this.tssVTIOSD.Size = new System.Drawing.Size(60, 19);
            this.tssVTIOSD.Text = "VTI-OSD";
            this.tssVTIOSD.Visible = false;
            // 
            // tsbtnDisplayMode
            // 
            this.tsbtnDisplayMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnDisplayMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiInverted,
            this.tsmiHueIntensity,
            this.toolStripSeparator7,
            this.tsmiHigh,
            this.tsmiLo,
            this.tsmiOff});
            this.tsbtnDisplayMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnDisplayMode.Name = "tsbtnDisplayMode";
            this.tsbtnDisplayMode.Size = new System.Drawing.Size(86, 20);
            this.tsbtnDisplayMode.Text = "Display Mode";
            this.tsbtnDisplayMode.Visible = false;
            // 
            // tsmiInverted
            // 
            this.tsmiInverted.CheckOnClick = true;
            this.tsmiInverted.Name = "tsmiInverted";
            this.tsmiInverted.Size = new System.Drawing.Size(139, 22);
            this.tsmiInverted.Text = "Inverted";
            this.tsmiInverted.Click += new System.EventHandler(this.tsmiInverted_Click);
            // 
            // tsmiHueIntensity
            // 
            this.tsmiHueIntensity.CheckOnClick = true;
            this.tsmiHueIntensity.Name = "tsmiHueIntensity";
            this.tsmiHueIntensity.Size = new System.Drawing.Size(139, 22);
            this.tsmiHueIntensity.Text = "Hue Intensity";
            this.tsmiHueIntensity.Click += new System.EventHandler(this.tsmiHueIntensity_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(136, 6);
            // 
            // tsmiHigh
            // 
            this.tsmiHigh.Name = "tsmiHigh";
            this.tsmiHigh.Size = new System.Drawing.Size(139, 22);
            this.tsmiHigh.Text = "Gamma: High";
            this.tsmiHigh.Click += new System.EventHandler(this.DisplayIntensifyModeClicked);
            // 
            // tsmiLo
            // 
            this.tsmiLo.Name = "tsmiLo";
            this.tsmiLo.Size = new System.Drawing.Size(139, 22);
            this.tsmiLo.Text = "Gamma: Lo";
            this.tsmiLo.Click += new System.EventHandler(this.DisplayIntensifyModeClicked);
            // 
            // tsmiOff
            // 
            this.tsmiOff.Checked = true;
            this.tsmiOff.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiOff.Name = "tsmiOff";
            this.tsmiOff.Size = new System.Drawing.Size(139, 22);
            this.tsmiOff.Text = "Gamma: Off";
            this.tsmiOff.Click += new System.EventHandler(this.DisplayIntensifyModeClicked);
            // 
            // tssRecordingFile
            // 
            this.tssRecordingFile.BackColor = System.Drawing.Color.MistyRose;
            this.tssRecordingFile.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssRecordingFile.Name = "tssRecordingFile";
            this.tssRecordingFile.Size = new System.Drawing.Size(73, 17);
            this.tssRecordingFile.Text = "File (xxx Mb)";
            this.tssRecordingFile.Visible = false;
            // 
            // tssFreeDiskSpace
            // 
            this.tssFreeDiskSpace.BackColor = System.Drawing.Color.Red;
            this.tssFreeDiskSpace.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssFreeDiskSpace.ForeColor = System.Drawing.Color.Yellow;
            this.tssFreeDiskSpace.Name = "tssFreeDiskSpace";
            this.tssFreeDiskSpace.Size = new System.Drawing.Size(69, 17);
            this.tssFreeDiskSpace.Text = "xxx Mb free";
            this.tssFreeDiskSpace.Visible = false;
            // 
            // pnlNewVersionAvailable
            // 
            this.pnlNewVersionAvailable.ActiveLinkColor = System.Drawing.Color.Red;
            this.pnlNewVersionAvailable.BackColor = System.Drawing.Color.Black;
            this.pnlNewVersionAvailable.IsLink = true;
            this.pnlNewVersionAvailable.LinkColor = System.Drawing.Color.Lime;
            this.pnlNewVersionAvailable.Name = "pnlNewVersionAvailable";
            this.pnlNewVersionAvailable.Size = new System.Drawing.Size(292, 17);
            this.pnlNewVersionAvailable.Text = "New version of OccuRec is available. Click here to upgrade.";
            this.pnlNewVersionAvailable.Visible = false;
            this.pnlNewVersionAvailable.Click += new System.EventHandler(this.pnlNewVersionAvailable_Click);
            // 
            // tssDisplayRate
            // 
            this.tssDisplayRate.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssDisplayRate.Name = "tssDisplayRate";
            this.tssDisplayRate.Size = new System.Drawing.Size(71, 17);
            this.tssDisplayRate.Text = "Display Rate";
            // 
            // msMain
            // 
            this.msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.miSettings,
            this.miHelp});
            this.msMain.Location = new System.Drawing.Point(0, 0);
            this.msMain.Name = "msMain";
            this.msMain.Size = new System.Drawing.Size(920, 24);
            this.msMain.TabIndex = 2;
            this.msMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miConnect,
            this.miASCOMConnect,
            this.miDisconnect,
            this.toolStripSeparator3,
            this.miOpenFile,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.fileToolStripMenuItem.Text = "&Video";
            this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // miConnect
            // 
            this.miConnect.Name = "miConnect";
            this.miConnect.Size = new System.Drawing.Size(153, 22);
            this.miConnect.Text = "&Connect AAV";
            this.miConnect.Click += new System.EventHandler(this.miConnect_Click);
            // 
            // miASCOMConnect
            // 
            this.miASCOMConnect.Enabled = false;
            this.miASCOMConnect.Name = "miASCOMConnect";
            this.miASCOMConnect.Size = new System.Drawing.Size(153, 22);
            this.miASCOMConnect.Text = "Connect &ASCOM";
            this.miASCOMConnect.Click += new System.EventHandler(this.miASCOMConnect_Click);
            // 
            // miDisconnect
            // 
            this.miDisconnect.Enabled = false;
            this.miDisconnect.Name = "miDisconnect";
            this.miDisconnect.Size = new System.Drawing.Size(153, 22);
            this.miDisconnect.Text = "&Disconnect";
            this.miDisconnect.Click += new System.EventHandler(this.miDisconnect_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(150, 6);
            // 
            // miOpenFile
            // 
            this.miOpenFile.Name = "miOpenFile";
            this.miOpenFile.Size = new System.Drawing.Size(153, 22);
            this.miOpenFile.Text = "&Open File";
            this.miOpenFile.Click += new System.EventHandler(this.miOpenFile_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(150, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // miSettings
            // 
            this.miSettings.Name = "miSettings";
            this.miSettings.Size = new System.Drawing.Size(58, 20);
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
            this.miHelp.Size = new System.Drawing.Size(40, 20);
            this.miHelp.Text = "&Help";
            // 
            // miHelpIndex
            // 
            this.miHelpIndex.Image = ((System.Drawing.Image)(resources.GetObject("miHelpIndex.Image")));
            this.miHelpIndex.Name = "miHelpIndex";
            this.miHelpIndex.Size = new System.Drawing.Size(181, 22);
            this.miHelpIndex.Text = "&Index";
            this.miHelpIndex.Click += new System.EventHandler(this.miHelpIndex_Click);
            // 
            // miYahooGroup
            // 
            this.miYahooGroup.Image = ((System.Drawing.Image)(resources.GetObject("miYahooGroup.Image")));
            this.miYahooGroup.Name = "miYahooGroup";
            this.miYahooGroup.Size = new System.Drawing.Size(181, 22);
            this.miYahooGroup.Text = "OccuRec Yahoo Group";
            this.miYahooGroup.Click += new System.EventHandler(this.miYahooGroup_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(178, 6);
            // 
            // miCheckForUpdates
            // 
            this.miCheckForUpdates.Name = "miCheckForUpdates";
            this.miCheckForUpdates.Size = new System.Drawing.Size(181, 22);
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
            this.pnlClient.Size = new System.Drawing.Size(920, 576);
            this.pnlClient.TabIndex = 4;
            // 
            // pnlVideoFrames
            // 
            this.pnlVideoFrames.Controls.Add(this.pnlVideo);
            this.pnlVideoFrames.Controls.Add(this.pnlTargets);
            this.pnlVideoFrames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlVideoFrames.Location = new System.Drawing.Point(0, 0);
            this.pnlVideoFrames.Name = "pnlVideoFrames";
            this.pnlVideoFrames.Size = new System.Drawing.Size(720, 576);
            this.pnlVideoFrames.TabIndex = 2;
            // 
            // pnlVideo
            // 
            this.pnlVideo.Controls.Add(this.picVideoFrame);
            this.pnlVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlVideo.Location = new System.Drawing.Point(0, 0);
            this.pnlVideo.Name = "pnlVideo";
            this.pnlVideo.Size = new System.Drawing.Size(720, 576);
            this.pnlVideo.TabIndex = 2;
            // 
            // picVideoFrame
            // 
            this.picVideoFrame.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picVideoFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picVideoFrame.Location = new System.Drawing.Point(0, 0);
            this.picVideoFrame.Name = "picVideoFrame";
            this.picVideoFrame.Size = new System.Drawing.Size(720, 576);
            this.picVideoFrame.TabIndex = 0;
            this.picVideoFrame.TabStop = false;
            this.picVideoFrame.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.picVideoFrame.MouseLeave += new System.EventHandler(this.pictureBox_MouseLeave);
            this.picVideoFrame.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            this.picVideoFrame.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            // 
            // pnlTargets
            // 
            this.pnlTargets.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTargets.Location = new System.Drawing.Point(0, 576);
            this.pnlTargets.Name = "pnlTargets";
            this.pnlTargets.Size = new System.Drawing.Size(720, 0);
            this.pnlTargets.TabIndex = 1;
            // 
            // pnlVideoControls
            // 
            this.pnlVideoControls.BackColor = System.Drawing.SystemColors.Control;
            this.pnlVideoControls.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlVideoControls.Controls.Add(this.ucVideoControl);
            this.pnlVideoControls.Controls.Add(this.pnlOcrTesting);
            this.pnlVideoControls.Controls.Add(this.pnlCrossbar);
            this.pnlVideoControls.Controls.Add(this.gbxSchedules);
            this.pnlVideoControls.Controls.Add(this.btnImageSettings);
            this.pnlVideoControls.Controls.Add(this.lblVideoFormat);
            this.pnlVideoControls.Controls.Add(this.btnStopRecording);
            this.pnlVideoControls.Controls.Add(this.label5);
            this.pnlVideoControls.Controls.Add(this.btnRecord);
            this.pnlVideoControls.Controls.Add(this.pnlAAV);
            this.pnlVideoControls.Controls.Add(this.pnlVtiOsd);
            this.pnlVideoControls.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlVideoControls.Enabled = false;
            this.pnlVideoControls.Location = new System.Drawing.Point(720, 0);
            this.pnlVideoControls.Name = "pnlVideoControls";
            this.pnlVideoControls.Size = new System.Drawing.Size(200, 576);
            this.pnlVideoControls.TabIndex = 1;
            // 
            // pnlOcrTesting
            // 
            this.pnlOcrTesting.Controls.Add(this.btnOcrTesting);
            this.pnlOcrTesting.Location = new System.Drawing.Point(4, 184);
            this.pnlOcrTesting.Name = "pnlOcrTesting";
            this.pnlOcrTesting.Size = new System.Drawing.Size(189, 40);
            this.pnlOcrTesting.TabIndex = 25;
            this.pnlOcrTesting.Visible = false;
            // 
            // btnOcrTesting
            // 
            this.btnOcrTesting.Location = new System.Drawing.Point(11, 8);
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
            this.gbxSchedules.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gbxSchedules.Controls.Add(this.pnlNextScheduledAction);
            this.gbxSchedules.Controls.Add(this.btnClearSchedule);
            this.gbxSchedules.Controls.Add(this.btnAddSchedule);
            this.gbxSchedules.Controls.Add(this.lbSchedule);
            this.gbxSchedules.Location = new System.Drawing.Point(8, 329);
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
            this.lblVideoFormat.Location = new System.Drawing.Point(106, 547);
            this.lblVideoFormat.Name = "lblVideoFormat";
            this.lblVideoFormat.Size = new System.Drawing.Size(77, 14);
            this.lblVideoFormat.TabIndex = 7;
            this.lblVideoFormat.Text = "N/A";
            // 
            // btnStopRecording
            // 
            this.btnStopRecording.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStopRecording.Enabled = false;
            this.btnStopRecording.Location = new System.Drawing.Point(101, 519);
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
            this.label5.Location = new System.Drawing.Point(12, 547);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Video File Format:";
            // 
            // btnRecord
            // 
            this.btnRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRecord.Enabled = false;
            this.btnRecord.Location = new System.Drawing.Point(12, 519);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(76, 23);
            this.btnRecord.TabIndex = 4;
            this.btnRecord.Text = "REC";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // pnlAAV
            // 
            this.pnlAAV.BackColor = System.Drawing.SystemColors.Info;
            this.pnlAAV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAAV.Controls.Add(this.btnManualIntegration);
            this.pnlAAV.Controls.Add(this.pbarIntDetPercentDone);
            this.pnlAAV.Controls.Add(this.btnCalibrateIntegration);
            this.pnlAAV.Controls.Add(this.label3);
            this.pnlAAV.Controls.Add(this.btnLockIntegration);
            this.pnlAAV.Controls.Add(this.pnlOneStacking);
            this.pnlAAV.Location = new System.Drawing.Point(4, 88);
            this.pnlAAV.Name = "pnlAAV";
            this.pnlAAV.Size = new System.Drawing.Size(189, 86);
            this.pnlAAV.TabIndex = 11;
            this.pnlAAV.Visible = false;
            // 
            // btnManualIntegration
            // 
            this.btnManualIntegration.Location = new System.Drawing.Point(96, 51);
            this.btnManualIntegration.Name = "btnManualIntegration";
            this.btnManualIntegration.Size = new System.Drawing.Size(80, 23);
            this.btnManualIntegration.TabIndex = 17;
            this.btnManualIntegration.Text = "Manual";
            this.btnManualIntegration.UseVisualStyleBackColor = true;
            this.btnManualIntegration.Visible = false;
            this.btnManualIntegration.Click += new System.EventHandler(this.btnManualIntegration_Click);
            // 
            // pbarIntDetPercentDone
            // 
            this.pbarIntDetPercentDone.Location = new System.Drawing.Point(127, 9);
            this.pbarIntDetPercentDone.Name = "pbarIntDetPercentDone";
            this.pbarIntDetPercentDone.Size = new System.Drawing.Size(46, 9);
            this.pbarIntDetPercentDone.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbarIntDetPercentDone.TabIndex = 16;
            this.pbarIntDetPercentDone.Visible = false;
            // 
            // btnCalibrateIntegration
            // 
            this.btnCalibrateIntegration.Location = new System.Drawing.Point(13, 51);
            this.btnCalibrateIntegration.Name = "btnCalibrateIntegration";
            this.btnCalibrateIntegration.Size = new System.Drawing.Size(79, 23);
            this.btnCalibrateIntegration.TabIndex = 15;
            this.btnCalibrateIntegration.Text = "Calibrate";
            this.btnCalibrateIntegration.UseVisualStyleBackColor = true;
            this.btnCalibrateIntegration.Visible = false;
            this.btnCalibrateIntegration.Click += new System.EventHandler(this.btnCalibrateIntegration_Click);
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
            this.btnLockIntegration.Text = "Lock at x4 video frames";
            this.btnLockIntegration.UseVisualStyleBackColor = true;
            this.btnLockIntegration.Click += new System.EventHandler(this.btnLockIntegration_Click);
            // 
            // pnlOneStacking
            // 
            this.pnlOneStacking.Controls.Add(this.lblOneStack);
            this.pnlOneStacking.Controls.Add(this.btnOneStack);
            this.pnlOneStacking.Location = new System.Drawing.Point(8, 45);
            this.pnlOneStacking.Name = "pnlOneStacking";
            this.pnlOneStacking.Size = new System.Drawing.Size(174, 31);
            this.pnlOneStacking.TabIndex = 18;
            this.pnlOneStacking.Visible = false;
            // 
            // lblOneStack
            // 
            this.lblOneStack.AutoSize = true;
            this.lblOneStack.Location = new System.Drawing.Point(8, 12);
            this.lblOneStack.Name = "lblOneStack";
            this.lblOneStack.Size = new System.Drawing.Size(66, 13);
            this.lblOneStack.TabIndex = 28;
            this.lblOneStack.Text = "No Stacking";
            // 
            // btnOneStack
            // 
            this.btnOneStack.Location = new System.Drawing.Point(88, 6);
            this.btnOneStack.Name = "btnOneStack";
            this.btnOneStack.Size = new System.Drawing.Size(80, 23);
            this.btnOneStack.TabIndex = 27;
            this.btnOneStack.Text = "x1 Stacking";
            this.btnOneStack.UseVisualStyleBackColor = true;
            this.btnOneStack.Click += new System.EventHandler(this.btnOneStack_Click);
            // 
            // pnlVtiOsd
            // 
            this.pnlVtiOsd.BackColor = System.Drawing.SystemColors.Info;
            this.pnlVtiOsd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlVtiOsd.Controls.Add(this.btnUpdateVtiOsd);
            this.pnlVtiOsd.Controls.Add(this.btnConfirmUserVtiOsd);
            this.pnlVtiOsd.Controls.Add(this.label4);
            this.pnlVtiOsd.Location = new System.Drawing.Point(4, 88);
            this.pnlVtiOsd.Name = "pnlVtiOsd";
            this.pnlVtiOsd.Size = new System.Drawing.Size(189, 59);
            this.pnlVtiOsd.TabIndex = 26;
            this.pnlVtiOsd.Visible = false;
            // 
            // btnUpdateVtiOsd
            // 
            this.btnUpdateVtiOsd.Location = new System.Drawing.Point(96, 26);
            this.btnUpdateVtiOsd.Name = "btnUpdateVtiOsd";
            this.btnUpdateVtiOsd.Size = new System.Drawing.Size(80, 23);
            this.btnUpdateVtiOsd.TabIndex = 19;
            this.btnUpdateVtiOsd.Text = "Configure";
            this.btnUpdateVtiOsd.UseVisualStyleBackColor = true;
            this.btnUpdateVtiOsd.Click += new System.EventHandler(this.btnUpdateVtiOsd_Click);
            // 
            // btnConfirmUserVtiOsd
            // 
            this.btnConfirmUserVtiOsd.Location = new System.Drawing.Point(13, 26);
            this.btnConfirmUserVtiOsd.Name = "btnConfirmUserVtiOsd";
            this.btnConfirmUserVtiOsd.Size = new System.Drawing.Size(79, 23);
            this.btnConfirmUserVtiOsd.TabIndex = 18;
            this.btnConfirmUserVtiOsd.Text = "Confirm";
            this.btnConfirmUserVtiOsd.UseVisualStyleBackColor = true;
            this.btnConfirmUserVtiOsd.Click += new System.EventHandler(this.btnConfirmUserVtiOsd_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "VTI-OSD Position";
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
            this.tsbAddGuidingStar,
            this.tbsAddTarget,
            this.tsbClearTargets,
            this.tsSeparator2,
            this.tsbTelControl,
            this.toolStripSeparator5,
            this.tsbFocControl,
            this.toolStripSeparator6,
            this.tsbCamControl,
            this.toolStripSeparator4,
            this.tslTelFocStatus});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(920, 25);
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
            // tsbAddGuidingStar
            // 
            this.tsbAddGuidingStar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddGuidingStar.Image = ((System.Drawing.Image)(resources.GetObject("tsbAddGuidingStar.Image")));
            this.tsbAddGuidingStar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddGuidingStar.Name = "tsbAddGuidingStar";
            this.tsbAddGuidingStar.Size = new System.Drawing.Size(23, 22);
            this.tsbAddGuidingStar.ToolTipText = "Select Guiding Star";
            this.tsbAddGuidingStar.Click += new System.EventHandler(this.tsbAddGuidingStar_Click);
            // 
            // tbsAddTarget
            // 
            this.tbsAddTarget.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbsAddTarget.Enabled = false;
            this.tbsAddTarget.Image = ((System.Drawing.Image)(resources.GetObject("tbsAddTarget.Image")));
            this.tbsAddTarget.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbsAddTarget.Name = "tbsAddTarget";
            this.tbsAddTarget.Size = new System.Drawing.Size(23, 22);
            this.tbsAddTarget.ToolTipText = "Select Target";
            this.tbsAddTarget.Click += new System.EventHandler(this.tbsAddTarget_Click);
            // 
            // tsbClearTargets
            // 
            this.tsbClearTargets.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClearTargets.Image = global::OccuRec.Properties.Resources.brush;
            this.tsbClearTargets.Name = "tsbClearTargets";
            this.tsbClearTargets.Size = new System.Drawing.Size(16, 22);
            this.tsbClearTargets.ToolTipText = "Remove Tracked Objects";
            this.tsbClearTargets.Click += new System.EventHandler(this.tsbClearTargets_Click);
            // 
            // tsSeparator2
            // 
            this.tsSeparator2.Name = "tsSeparator2";
            this.tsSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbTelControl
            // 
            this.tsbTelControl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbTelControl.Image = ((System.Drawing.Image)(resources.GetObject("tsbTelControl.Image")));
            this.tsbTelControl.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbTelControl.Name = "tsbTelControl";
            this.tsbTelControl.Size = new System.Drawing.Size(97, 22);
            this.tsbTelControl.Text = "Control Telescope";
            this.tsbTelControl.Click += new System.EventHandler(this.tsbTelControl_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbFocControl
            // 
            this.tsbFocControl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbFocControl.Image = ((System.Drawing.Image)(resources.GetObject("tsbFocControl.Image")));
            this.tsbFocControl.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFocControl.Name = "tsbFocControl";
            this.tsbFocControl.Size = new System.Drawing.Size(87, 22);
            this.tsbFocControl.Text = "Control Focuser";
            this.tsbFocControl.Click += new System.EventHandler(this.tsbFocControl_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbCamControl
            // 
            this.tsbCamControl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbCamControl.Image = ((System.Drawing.Image)(resources.GetObject("tsbCamControl.Image")));
            this.tsbCamControl.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCamControl.Name = "tsbCamControl";
            this.tsbCamControl.Size = new System.Drawing.Size(86, 22);
            this.tsbCamControl.Text = "Control Camera";
            this.tsbCamControl.Click += new System.EventHandler(this.tsbCamControl_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // tslTelFocStatus
            // 
            this.tslTelFocStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tslTelFocStatus.ForeColor = System.Drawing.Color.Green;
            this.tslTelFocStatus.Name = "tslTelFocStatus";
            this.tslTelFocStatus.Size = new System.Drawing.Size(0, 22);
            // 
            // pnlControlArea
            // 
            this.pnlControlArea.Controls.Add(this.pnlClient);
            this.pnlControlArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlControlArea.Location = new System.Drawing.Point(0, 49);
            this.pnlControlArea.Name = "pnlControlArea";
            this.pnlControlArea.Size = new System.Drawing.Size(920, 576);
            this.pnlControlArea.TabIndex = 6;
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
            // ucVideoControl
            // 
            this.ucVideoControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ucVideoControl.Location = new System.Drawing.Point(4, 230);
            this.ucVideoControl.Name = "ucVideoControl";
            this.ucVideoControl.Size = new System.Drawing.Size(190, 86);
            this.ucVideoControl.TabIndex = 27;
            this.ucVideoControl.Visible = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 647);
            this.Controls.Add(this.pnlControlArea);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.msMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.msMain;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "OccuRec ";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.Move += new System.EventHandler(this.frmMain_Move);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.msMain.ResumeLayout(false);
            this.msMain.PerformLayout();
            this.pnlClient.ResumeLayout(false);
            this.pnlVideoFrames.ResumeLayout(false);
            this.pnlVideo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picVideoFrame)).EndInit();
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
            this.pnlOneStacking.ResumeLayout(false);
            this.pnlOneStacking.PerformLayout();
            this.pnlVtiOsd.ResumeLayout(false);
            this.pnlVtiOsd.PerformLayout();
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
		private System.Windows.Forms.ToolStripMenuItem miSettings;
		private System.Windows.Forms.ToolStripMenuItem miConnect;
        private System.Windows.Forms.ToolStripMenuItem miDisconnect;
		private System.Windows.Forms.Button btnStopRecording;
        private System.Windows.Forms.Button btnRecord;
		private System.Windows.Forms.Label lblVideoFormat;
		private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.Panel pnlVideoFrames;
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
		private System.Windows.Forms.ToolStripProgressBar ttsProgressBar;
		private System.Windows.Forms.ToolStripStatusLabel tssCameraState;
		private System.Windows.Forms.ToolStripStatusLabel tssFrameNo;              
		private System.Windows.Forms.ToolStripStatusLabel tssDroppedFrames;
		private System.Windows.Forms.ToolStripStatusLabel tssOcrErr;
		private System.Windows.Forms.ToolStripStatusLabel tssRecordingFile;
        private System.Windows.Forms.ToolStripStatusLabel pnlNewVersionAvailable;
        private System.Windows.Forms.ToolStripMenuItem miHelp;
        private System.Windows.Forms.ToolStripMenuItem miHelpIndex;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem miCheckForUpdates;
        private System.Windows.Forms.ToolStripMenuItem miYahooGroup;
        private System.Windows.Forms.ToolStripButton tsbConnectDisconnect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ImageList imageListToolbar;
		private System.Windows.Forms.ToolStripMenuItem miOpenFile;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Button btnCalibrateIntegration;
        private System.Windows.Forms.ToolStripStatusLabel tssDisplayRate;
		private System.Windows.Forms.ProgressBar pbarIntDetPercentDone;
		private System.Windows.Forms.Button btnManualIntegration;
		private System.Windows.Forms.ToolStripStatusLabel tssFreeDiskSpace;
        private System.Windows.Forms.ToolStripStatusLabel tssASCOMTelescope;
        private System.Windows.Forms.ToolStripStatusLabel tssASCOMFocuser;
		private System.Windows.Forms.Panel pnlTargets;
		private System.Windows.Forms.Panel pnlVideo;
		private System.Windows.Forms.ToolStripSeparator tsSeparator2;
		private System.Windows.Forms.ToolStripButton tsbTelControl;
		private System.Windows.Forms.ToolStripButton tsbFocControl;
		private System.Windows.Forms.ToolStripButton tsbCamControl;
        protected internal System.Windows.Forms.PictureBox picVideoFrame;
        protected internal System.Windows.Forms.ToolStripButton tbsAddTarget;
        protected internal System.Windows.Forms.ToolStripButton tsbAddGuidingStar;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripLabel tslTelFocStatus;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripStatusLabel tssNTP;
		private System.Windows.Forms.ToolStripLabel tsbClearTargets;
		private System.Windows.Forms.ToolStripStatusLabel tssCameraControl;
		private System.Windows.Forms.ToolStripMenuItem miASCOMConnect;
		private System.Windows.Forms.ToolStripStatusLabel tssVTIOSD;
		private System.Windows.Forms.Panel pnlVtiOsd;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnUpdateVtiOsd;
		private System.Windows.Forms.Button btnConfirmUserVtiOsd;
		protected internal System.Windows.Forms.ToolStripSplitButton tsbtnDisplayMode;
		protected internal System.Windows.Forms.ToolStripMenuItem tsmiInverted;
		protected internal System.Windows.Forms.ToolStripMenuItem tsmiHueIntensity;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		protected internal System.Windows.Forms.ToolStripMenuItem tsmiHigh;
		protected internal System.Windows.Forms.ToolStripMenuItem tsmiLo;
		protected internal System.Windows.Forms.ToolStripMenuItem tsmiOff;
		private System.Windows.Forms.Label lblOneStack;
		private System.Windows.Forms.Button btnOneStack;
		private System.Windows.Forms.Panel pnlOneStacking;
		private Controls.ucCameraControl ucVideoControl;
	}
}

