namespace OccuRec.Config.Panels
{
	partial class ucAdvanced
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
            this.pnlPreserveOSDArea = new System.Windows.Forms.Panel();
            this.nudPreserveVTIBottomRow = new System.Windows.Forms.NumericUpDown();
            this.nudPreserveVTITopRow = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nudNTPDebugValue2 = new System.Windows.Forms.NumericUpDown();
            this.nudNTPDebugValue1 = new System.Windows.Forms.NumericUpDown();
            this.cbDebugIntegration = new System.Windows.Forms.CheckBox();
            this.cbxStatusSectionOnly = new System.Windows.Forms.CheckBox();
            this.cbxUserPreserveOSDLines = new System.Windows.Forms.CheckBox();
            this.pnlNTPDebug = new System.Windows.Forms.Panel();
            this.cbxGraphDebugMode = new System.Windows.Forms.CheckBox();
            this.cbxImageLayoutMode = new System.Windows.Forms.ComboBox();
            this.cbxCustomAdvImageLayout = new System.Windows.Forms.CheckBox();
            this.cbxSaveVtiOsdReport = new System.Windows.Forms.CheckBox();
            this.cbxCustomAdvCompression = new System.Windows.Forms.CheckBox();
            this.cbxAdvCompression = new System.Windows.Forms.ComboBox();
            this.cbxMustConfirmVTI = new System.Windows.Forms.CheckBox();
            this.pnlLocationCorss = new System.Windows.Forms.Panel();
            this.nudCorssTransparency = new System.Windows.Forms.NumericUpDown();
            this.nudCrossY = new System.Windows.Forms.NumericUpDown();
            this.nudCrossX = new System.Windows.Forms.NumericUpDown();
            this.cbxLocationCross = new System.Windows.Forms.CheckBox();
            this.cbxBeepOnStartStopRecording = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxPALStandard = new System.Windows.Forms.ComboBox();
            this.cbxNTSCStandard = new System.Windows.Forms.ComboBox();
            this.nudSaturationWarning = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.cbxOCR = new System.Windows.Forms.CheckBox();
            this.cbxOCRType = new System.Windows.Forms.ComboBox();
            this.nudOCRMinON = new System.Windows.Forms.NumericUpDown();
            this.nudOCRMaxOFF = new System.Windows.Forms.NumericUpDown();
            this.pnlOCR = new System.Windows.Forms.Panel();
            this.cbxZoneStats = new System.Windows.Forms.CheckBox();
            this.cbxNTPDebug = new System.Windows.Forms.CheckBox();
            this.pnlPreserveOSDArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPreserveVTIBottomRow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPreserveVTITopRow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNTPDebugValue2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNTPDebugValue1)).BeginInit();
            this.pnlNTPDebug.SuspendLayout();
            this.pnlLocationCorss.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCorssTransparency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCrossY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCrossX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaturationWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOCRMinON)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOCRMaxOFF)).BeginInit();
            this.pnlOCR.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlPreserveOSDArea
            // 
            this.pnlPreserveOSDArea.Controls.Add(this.nudPreserveVTIBottomRow);
            this.pnlPreserveOSDArea.Controls.Add(this.nudPreserveVTITopRow);
            this.pnlPreserveOSDArea.Controls.Add(this.label3);
            this.pnlPreserveOSDArea.Enabled = false;
            this.pnlPreserveOSDArea.Location = new System.Drawing.Point(162, 2);
            this.pnlPreserveOSDArea.Name = "pnlPreserveOSDArea";
            this.pnlPreserveOSDArea.Size = new System.Drawing.Size(141, 25);
            this.pnlPreserveOSDArea.TabIndex = 32;
            // 
            // nudPreserveVTIBottomRow
            // 
            this.nudPreserveVTIBottomRow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudPreserveVTIBottomRow.Location = new System.Drawing.Point(85, 3);
            this.nudPreserveVTIBottomRow.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.nudPreserveVTIBottomRow.Name = "nudPreserveVTIBottomRow";
            this.nudPreserveVTIBottomRow.Size = new System.Drawing.Size(49, 20);
            this.nudPreserveVTIBottomRow.TabIndex = 25;
            this.nudPreserveVTIBottomRow.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // nudPreserveVTITopRow
            // 
            this.nudPreserveVTITopRow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudPreserveVTITopRow.Location = new System.Drawing.Point(9, 3);
            this.nudPreserveVTITopRow.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.nudPreserveVTITopRow.Name = "nudPreserveVTITopRow";
            this.nudPreserveVTITopRow.Size = new System.Drawing.Size(50, 20);
            this.nudPreserveVTITopRow.TabIndex = 24;
            this.nudPreserveVTITopRow.Value = new decimal(new int[] {
            542,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(66, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "to";
            // 
            // nudNTPDebugValue2
            // 
            this.nudNTPDebugValue2.DecimalPlaces = 1;
            this.nudNTPDebugValue2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudNTPDebugValue2.Location = new System.Drawing.Point(170, 3);
            this.nudNTPDebugValue2.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudNTPDebugValue2.Name = "nudNTPDebugValue2";
            this.nudNTPDebugValue2.Size = new System.Drawing.Size(49, 20);
            this.nudNTPDebugValue2.TabIndex = 28;
            this.nudNTPDebugValue2.Value = new decimal(new int[] {
            50,
            0,
            0,
            65536});
            // 
            // nudNTPDebugValue1
            // 
            this.nudNTPDebugValue1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudNTPDebugValue1.Location = new System.Drawing.Point(121, 3);
            this.nudNTPDebugValue1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudNTPDebugValue1.Name = "nudNTPDebugValue1";
            this.nudNTPDebugValue1.Size = new System.Drawing.Size(43, 20);
            this.nudNTPDebugValue1.TabIndex = 27;
            this.nudNTPDebugValue1.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // cbDebugIntegration
            // 
            this.cbDebugIntegration.AutoSize = true;
            this.cbDebugIntegration.Location = new System.Drawing.Point(252, 67);
            this.cbDebugIntegration.Name = "cbDebugIntegration";
            this.cbDebugIntegration.Size = new System.Drawing.Size(161, 17);
            this.cbDebugIntegration.TabIndex = 24;
            this.cbDebugIntegration.Text = "Integration Detection Tuning";
            this.cbDebugIntegration.UseVisualStyleBackColor = true;
            // 
            // cbxStatusSectionOnly
            // 
            this.cbxStatusSectionOnly.AutoSize = true;
            this.cbxStatusSectionOnly.Location = new System.Drawing.Point(12, 196);
            this.cbxStatusSectionOnly.Name = "cbxStatusSectionOnly";
            this.cbxStatusSectionOnly.Size = new System.Drawing.Size(119, 17);
            this.cbxStatusSectionOnly.TabIndex = 26;
            this.cbxStatusSectionOnly.Text = "Status Section Only";
            this.cbxStatusSectionOnly.UseVisualStyleBackColor = true;
            this.cbxStatusSectionOnly.CheckedChanged += new System.EventHandler(this.cbxStatusSectionOnly_CheckedChanged);
            // 
            // cbxUserPreserveOSDLines
            // 
            this.cbxUserPreserveOSDLines.AutoSize = true;
            this.cbxUserPreserveOSDLines.Location = new System.Drawing.Point(12, 8);
            this.cbxUserPreserveOSDLines.Name = "cbxUserPreserveOSDLines";
            this.cbxUserPreserveOSDLines.Size = new System.Drawing.Size(147, 17);
            this.cbxUserPreserveOSDLines.TabIndex = 33;
            this.cbxUserPreserveOSDLines.Text = "VTI Preserve Lines (User)";
            this.cbxUserPreserveOSDLines.UseVisualStyleBackColor = true;
            this.cbxUserPreserveOSDLines.CheckedChanged += new System.EventHandler(this.cbxUserPreserveOSDLines_CheckedChanged);
            // 
            // pnlNTPDebug
            // 
            this.pnlNTPDebug.Controls.Add(this.cbxNTPDebug);
            this.pnlNTPDebug.Controls.Add(this.nudNTPDebugValue2);
            this.pnlNTPDebug.Controls.Add(this.nudNTPDebugValue1);
            this.pnlNTPDebug.Enabled = false;
            this.pnlNTPDebug.Location = new System.Drawing.Point(137, 192);
            this.pnlNTPDebug.Name = "pnlNTPDebug";
            this.pnlNTPDebug.Size = new System.Drawing.Size(220, 27);
            this.pnlNTPDebug.TabIndex = 34;
            // 
            // cbxGraphDebugMode
            // 
            this.cbxGraphDebugMode.AutoSize = true;
            this.cbxGraphDebugMode.Location = new System.Drawing.Point(12, 67);
            this.cbxGraphDebugMode.Name = "cbxGraphDebugMode";
            this.cbxGraphDebugMode.Size = new System.Drawing.Size(148, 17);
            this.cbxGraphDebugMode.TabIndex = 37;
            this.cbxGraphDebugMode.Text = "DirectShow Graph Debug";
            this.cbxGraphDebugMode.UseVisualStyleBackColor = true;
            // 
            // cbxImageLayoutMode
            // 
            this.cbxImageLayoutMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxImageLayoutMode.FormattingEnabled = true;
            this.cbxImageLayoutMode.Location = new System.Drawing.Point(164, 96);
            this.cbxImageLayoutMode.Name = "cbxImageLayoutMode";
            this.cbxImageLayoutMode.Size = new System.Drawing.Size(170, 21);
            this.cbxImageLayoutMode.TabIndex = 36;
            this.cbxImageLayoutMode.Visible = false;
            // 
            // cbxCustomAdvImageLayout
            // 
            this.cbxCustomAdvImageLayout.AutoSize = true;
            this.cbxCustomAdvImageLayout.Location = new System.Drawing.Point(12, 100);
            this.cbxCustomAdvImageLayout.Name = "cbxCustomAdvImageLayout";
            this.cbxCustomAdvImageLayout.Size = new System.Drawing.Size(146, 17);
            this.cbxCustomAdvImageLayout.TabIndex = 38;
            this.cbxCustomAdvImageLayout.Text = "ADV Image Layout (User)";
            this.cbxCustomAdvImageLayout.UseVisualStyleBackColor = true;
            this.cbxCustomAdvImageLayout.CheckedChanged += new System.EventHandler(this.cbxCustomAdvImageLayout_CheckedChanged);
            // 
            // cbxSaveVtiOsdReport
            // 
            this.cbxSaveVtiOsdReport.AutoSize = true;
            this.cbxSaveVtiOsdReport.Location = new System.Drawing.Point(12, 36);
            this.cbxSaveVtiOsdReport.Name = "cbxSaveVtiOsdReport";
            this.cbxSaveVtiOsdReport.Size = new System.Drawing.Size(277, 17);
            this.cbxSaveVtiOsdReport.TabIndex = 39;
            this.cbxSaveVtiOsdReport.Text = "Save Report on Unsuccessful VTI-OSD Identification";
            this.cbxSaveVtiOsdReport.UseVisualStyleBackColor = true;
            // 
            // cbxCustomAdvCompression
            // 
            this.cbxCustomAdvCompression.AutoSize = true;
            this.cbxCustomAdvCompression.Location = new System.Drawing.Point(12, 133);
            this.cbxCustomAdvCompression.Name = "cbxCustomAdvCompression";
            this.cbxCustomAdvCompression.Size = new System.Drawing.Size(142, 17);
            this.cbxCustomAdvCompression.TabIndex = 41;
            this.cbxCustomAdvCompression.Text = "ADV Compression (User)";
            this.cbxCustomAdvCompression.UseVisualStyleBackColor = true;
            this.cbxCustomAdvCompression.CheckedChanged += new System.EventHandler(this.cbxCustomAdvCompression_CheckedChanged);
            // 
            // cbxAdvCompression
            // 
            this.cbxAdvCompression.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxAdvCompression.FormattingEnabled = true;
            this.cbxAdvCompression.Items.AddRange(new object[] {
            "QuickLZ",
            "Lagarith16"});
            this.cbxAdvCompression.Location = new System.Drawing.Point(164, 129);
            this.cbxAdvCompression.Name = "cbxAdvCompression";
            this.cbxAdvCompression.Size = new System.Drawing.Size(170, 21);
            this.cbxAdvCompression.TabIndex = 40;
            this.cbxAdvCompression.Visible = false;
            // 
            // cbxMustConfirmVTI
            // 
            this.cbxMustConfirmVTI.AutoSize = true;
            this.cbxMustConfirmVTI.Location = new System.Drawing.Point(316, 8);
            this.cbxMustConfirmVTI.Name = "cbxMustConfirmVTI";
            this.cbxMustConfirmVTI.Size = new System.Drawing.Size(99, 17);
            this.cbxMustConfirmVTI.TabIndex = 42;
            this.cbxMustConfirmVTI.Text = "Manual Confirm";
            this.cbxMustConfirmVTI.UseVisualStyleBackColor = true;
            // 
            // pnlLocationCorss
            // 
            this.pnlLocationCorss.Controls.Add(this.nudCorssTransparency);
            this.pnlLocationCorss.Controls.Add(this.nudCrossY);
            this.pnlLocationCorss.Controls.Add(this.nudCrossX);
            this.pnlLocationCorss.Enabled = false;
            this.pnlLocationCorss.Location = new System.Drawing.Point(194, 228);
            this.pnlLocationCorss.Name = "pnlLocationCorss";
            this.pnlLocationCorss.Size = new System.Drawing.Size(232, 27);
            this.pnlLocationCorss.TabIndex = 44;
            // 
            // nudCorssTransparency
            // 
            this.nudCorssTransparency.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudCorssTransparency.Location = new System.Drawing.Point(141, 3);
            this.nudCorssTransparency.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCorssTransparency.Name = "nudCorssTransparency";
            this.nudCorssTransparency.Size = new System.Drawing.Size(49, 20);
            this.nudCorssTransparency.TabIndex = 29;
            this.nudCorssTransparency.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // nudCrossY
            // 
            this.nudCrossY.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudCrossY.Location = new System.Drawing.Point(58, 3);
            this.nudCrossY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudCrossY.Name = "nudCrossY";
            this.nudCrossY.Size = new System.Drawing.Size(49, 20);
            this.nudCrossY.TabIndex = 28;
            this.nudCrossY.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // nudCrossX
            // 
            this.nudCrossX.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudCrossX.Location = new System.Drawing.Point(3, 3);
            this.nudCrossX.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nudCrossX.Name = "nudCrossX";
            this.nudCrossX.Size = new System.Drawing.Size(49, 20);
            this.nudCrossX.TabIndex = 27;
            this.nudCrossX.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // cbxLocationCross
            // 
            this.cbxLocationCross.AutoSize = true;
            this.cbxLocationCross.Location = new System.Drawing.Point(12, 232);
            this.cbxLocationCross.Name = "cbxLocationCross";
            this.cbxLocationCross.Size = new System.Drawing.Size(133, 17);
            this.cbxLocationCross.TabIndex = 43;
            this.cbxLocationCross.Text = "Display Location Cross";
            this.cbxLocationCross.UseVisualStyleBackColor = true;
            this.cbxLocationCross.CheckedChanged += new System.EventHandler(this.cbxLocationCross_CheckedChanged);
            // 
            // cbxBeepOnStartStopRecording
            // 
            this.cbxBeepOnStartStopRecording.AutoSize = true;
            this.cbxBeepOnStartStopRecording.Location = new System.Drawing.Point(12, 265);
            this.cbxBeepOnStartStopRecording.Name = "cbxBeepOnStartStopRecording";
            this.cbxBeepOnStartStopRecording.Size = new System.Drawing.Size(176, 17);
            this.cbxBeepOnStartStopRecording.TabIndex = 45;
            this.cbxBeepOnStartStopRecording.Text = "\'Beep\' On Start/Stop Recording";
            this.cbxBeepOnStartStopRecording.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 326);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 13);
            this.label1.TabIndex = 46;
            this.label1.Text = "Preferred Video Standards";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(158, 326);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 47;
            this.label2.Text = "PAL:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(259, 326);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 48;
            this.label4.Text = "NTSC:";
            // 
            // cbxPALStandard
            // 
            this.cbxPALStandard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPALStandard.FormattingEnabled = true;
            this.cbxPALStandard.Items.AddRange(new object[] {
            "PAL_B",
            "PAL_D",
            "PAL_G",
            "PAL_H",
            "PAL_I",
            "PAL_M",
            "PAL_N",
            "PAL_60"});
            this.cbxPALStandard.Location = new System.Drawing.Point(192, 322);
            this.cbxPALStandard.Name = "cbxPALStandard";
            this.cbxPALStandard.Size = new System.Drawing.Size(61, 21);
            this.cbxPALStandard.TabIndex = 49;
            // 
            // cbxNTSCStandard
            // 
            this.cbxNTSCStandard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxNTSCStandard.FormattingEnabled = true;
            this.cbxNTSCStandard.Items.AddRange(new object[] {
            "NTSC_M",
            "NTSC_M_J",
            "NTSC_433"});
            this.cbxNTSCStandard.Location = new System.Drawing.Point(304, 322);
            this.cbxNTSCStandard.Name = "cbxNTSCStandard";
            this.cbxNTSCStandard.Size = new System.Drawing.Size(80, 21);
            this.cbxNTSCStandard.TabIndex = 50;
            // 
            // nudSaturationWarning
            // 
            this.nudSaturationWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudSaturationWarning.Location = new System.Drawing.Point(161, 295);
            this.nudSaturationWarning.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudSaturationWarning.Name = "nudSaturationWarning";
            this.nudSaturationWarning.Size = new System.Drawing.Size(50, 20);
            this.nudSaturationWarning.TabIndex = 24;
            this.nudSaturationWarning.Value = new decimal(new int[] {
            240,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 297);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 13);
            this.label5.TabIndex = 51;
            this.label5.Text = "Saturation Warning Level";
            // 
            // cbxOCR
            // 
            this.cbxOCR.AutoSize = true;
            this.cbxOCR.Location = new System.Drawing.Point(12, 164);
            this.cbxOCR.Name = "cbxOCR";
            this.cbxOCR.Size = new System.Drawing.Size(49, 17);
            this.cbxOCR.TabIndex = 52;
            this.cbxOCR.Text = "OCR";
            this.cbxOCR.UseVisualStyleBackColor = true;
            this.cbxOCR.CheckedChanged += new System.EventHandler(this.cbxOCR_CheckedChanged);
            // 
            // cbxOCRType
            // 
            this.cbxOCRType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxOCRType.FormattingEnabled = true;
            this.cbxOCRType.Items.AddRange(new object[] {
            "StarTech SVID2USB2",
            "EasyCAP"});
            this.cbxOCRType.Location = new System.Drawing.Point(7, 6);
            this.cbxOCRType.Name = "cbxOCRType";
            this.cbxOCRType.Size = new System.Drawing.Size(161, 21);
            this.cbxOCRType.TabIndex = 53;
            // 
            // nudOCRMinON
            // 
            this.nudOCRMinON.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudOCRMinON.Location = new System.Drawing.Point(228, 8);
            this.nudOCRMinON.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.nudOCRMinON.Name = "nudOCRMinON";
            this.nudOCRMinON.Size = new System.Drawing.Size(49, 20);
            this.nudOCRMinON.TabIndex = 54;
            this.nudOCRMinON.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // nudOCRMaxOFF
            // 
            this.nudOCRMaxOFF.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudOCRMaxOFF.Location = new System.Drawing.Point(173, 7);
            this.nudOCRMaxOFF.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.nudOCRMaxOFF.Name = "nudOCRMaxOFF";
            this.nudOCRMaxOFF.Size = new System.Drawing.Size(49, 20);
            this.nudOCRMaxOFF.TabIndex = 55;
            this.nudOCRMaxOFF.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // pnlOCR
            // 
            this.pnlOCR.Controls.Add(this.cbxZoneStats);
            this.pnlOCR.Controls.Add(this.cbxOCRType);
            this.pnlOCR.Controls.Add(this.nudOCRMinON);
            this.pnlOCR.Controls.Add(this.nudOCRMaxOFF);
            this.pnlOCR.Location = new System.Drawing.Point(80, 155);
            this.pnlOCR.Name = "pnlOCR";
            this.pnlOCR.Size = new System.Drawing.Size(376, 31);
            this.pnlOCR.TabIndex = 56;
            // 
            // cbxZoneStats
            // 
            this.cbxZoneStats.AutoSize = true;
            this.cbxZoneStats.Location = new System.Drawing.Point(283, 10);
            this.cbxZoneStats.Name = "cbxZoneStats";
            this.cbxZoneStats.Size = new System.Drawing.Size(78, 17);
            this.cbxZoneStats.TabIndex = 57;
            this.cbxZoneStats.Text = "Zone Stats";
            this.cbxZoneStats.UseVisualStyleBackColor = true;
            // 
            // cbxNTPDebug
            // 
            this.cbxNTPDebug.AutoSize = true;
            this.cbxNTPDebug.Location = new System.Drawing.Point(32, 6);
            this.cbxNTPDebug.Name = "cbxNTPDebug";
            this.cbxNTPDebug.Size = new System.Drawing.Size(83, 17);
            this.cbxNTPDebug.TabIndex = 58;
            this.cbxNTPDebug.Text = "NTP Debug";
            this.cbxNTPDebug.UseVisualStyleBackColor = true;
            // 
            // ucAdvanced
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlOCR);
            this.Controls.Add(this.cbxOCR);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nudSaturationWarning);
            this.Controls.Add(this.cbxNTSCStandard);
            this.Controls.Add(this.cbxPALStandard);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbxBeepOnStartStopRecording);
            this.Controls.Add(this.pnlLocationCorss);
            this.Controls.Add(this.cbxLocationCross);
            this.Controls.Add(this.cbxMustConfirmVTI);
            this.Controls.Add(this.cbxCustomAdvCompression);
            this.Controls.Add(this.cbxAdvCompression);
            this.Controls.Add(this.cbxSaveVtiOsdReport);
            this.Controls.Add(this.cbxCustomAdvImageLayout);
            this.Controls.Add(this.cbxGraphDebugMode);
            this.Controls.Add(this.cbxImageLayoutMode);
            this.Controls.Add(this.pnlNTPDebug);
            this.Controls.Add(this.cbxUserPreserveOSDLines);
            this.Controls.Add(this.pnlPreserveOSDArea);
            this.Controls.Add(this.cbDebugIntegration);
            this.Controls.Add(this.cbxStatusSectionOnly);
            this.Name = "ucAdvanced";
            this.Size = new System.Drawing.Size(925, 371);
            this.pnlPreserveOSDArea.ResumeLayout(false);
            this.pnlPreserveOSDArea.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPreserveVTIBottomRow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPreserveVTITopRow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNTPDebugValue2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNTPDebugValue1)).EndInit();
            this.pnlNTPDebug.ResumeLayout(false);
            this.pnlNTPDebug.PerformLayout();
            this.pnlLocationCorss.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudCorssTransparency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCrossY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCrossX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaturationWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOCRMinON)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOCRMaxOFF)).EndInit();
            this.pnlOCR.ResumeLayout(false);
            this.pnlOCR.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown nudNTPDebugValue2;
		private System.Windows.Forms.NumericUpDown nudNTPDebugValue1;
		private System.Windows.Forms.CheckBox cbDebugIntegration;
        private System.Windows.Forms.CheckBox cbxStatusSectionOnly;
		private System.Windows.Forms.Panel pnlPreserveOSDArea;
		private System.Windows.Forms.NumericUpDown nudPreserveVTIBottomRow;
		private System.Windows.Forms.NumericUpDown nudPreserveVTITopRow;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox cbxUserPreserveOSDLines;
		private System.Windows.Forms.Panel pnlNTPDebug;
		private System.Windows.Forms.CheckBox cbxGraphDebugMode;
		private System.Windows.Forms.ComboBox cbxImageLayoutMode;
		private System.Windows.Forms.CheckBox cbxCustomAdvImageLayout;
		private System.Windows.Forms.CheckBox cbxSaveVtiOsdReport;
		private System.Windows.Forms.CheckBox cbxCustomAdvCompression;
		private System.Windows.Forms.ComboBox cbxAdvCompression;
        private System.Windows.Forms.CheckBox cbxMustConfirmVTI;
        private System.Windows.Forms.Panel pnlLocationCorss;
        private System.Windows.Forms.NumericUpDown nudCrossY;
        private System.Windows.Forms.NumericUpDown nudCrossX;
        private System.Windows.Forms.CheckBox cbxLocationCross;
        private System.Windows.Forms.NumericUpDown nudCorssTransparency;
        private System.Windows.Forms.CheckBox cbxBeepOnStartStopRecording;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbxPALStandard;
        private System.Windows.Forms.ComboBox cbxNTSCStandard;
        private System.Windows.Forms.NumericUpDown nudSaturationWarning;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbxOCR;
        private System.Windows.Forms.ComboBox cbxOCRType;
        private System.Windows.Forms.NumericUpDown nudOCRMinON;
        private System.Windows.Forms.NumericUpDown nudOCRMaxOFF;
        private System.Windows.Forms.Panel pnlOCR;
        private System.Windows.Forms.CheckBox cbxZoneStats;
        private System.Windows.Forms.CheckBox cbxNTPDebug;
	}
}
