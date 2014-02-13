namespace OccuRec
{
    partial class frmChooseCamera
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChooseCamera));
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.cbxCaptureDevices = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.cbxCameraModel = new System.Windows.Forms.ComboBox();
			this.gbxCodecs = new System.Windows.Forms.GroupBox();
			this.rbCodecLagarith = new System.Windows.Forms.RadioButton();
			this.rbCodecHuffyuv = new System.Windows.Forms.RadioButton();
			this.rbCodecXviD = new System.Windows.Forms.RadioButton();
			this.rbCodecUncompressed = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.cbFileSIM = new System.Windows.Forms.CheckBox();
			this.rbFileAVI = new System.Windows.Forms.RadioButton();
			this.rbFileAAV = new System.Windows.Forms.RadioButton();
			this.cbxIsIntegrating = new System.Windows.Forms.CheckBox();
			this.gbxAAVSettings = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.cbxCameraDriver = new System.Windows.Forms.ComboBox();
			this.llOnlineHelp = new System.Windows.Forms.LinkLabel();
			this.pnlPreserveOSDArea = new System.Windows.Forms.Panel();
			this.label11 = new System.Windows.Forms.Label();
			this.nudPreserveVTIBottomRow = new System.Windows.Forms.NumericUpDown();
			this.nudPreserveVTITopRow = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.cbxOCRConfigurations = new System.Windows.Forms.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this.pnlCrossbar = new System.Windows.Forms.Panel();
			this.cbxCrossbarInput = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.cbxVideoFormats = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.pnlFlipControls = new System.Windows.Forms.Panel();
			this.cbxFlipVertically = new System.Windows.Forms.CheckBox();
			this.cbxFlipHorizontally = new System.Windows.Forms.CheckBox();
			this.btnConfigureCameraDriver = new System.Windows.Forms.Button();
			this.gbxCodecs.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.gbxAAVSettings.SuspendLayout();
			this.pnlPreserveOSDArea.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudPreserveVTIBottomRow)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPreserveVTITopRow)).BeginInit();
			this.pnlCrossbar.SuspendLayout();
			this.pnlFlipControls.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(286, 323);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 7;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(367, 323);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(111, 13);
			this.label2.TabIndex = 12;
			this.label2.Text = "Video Capture Device";
			// 
			// cbxCaptureDevices
			// 
			this.cbxCaptureDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxCaptureDevices.FormattingEnabled = true;
			this.cbxCaptureDevices.Location = new System.Drawing.Point(12, 27);
			this.cbxCaptureDevices.Name = "cbxCaptureDevices";
			this.cbxCaptureDevices.Size = new System.Drawing.Size(210, 21);
			this.cbxCaptureDevices.TabIndex = 14;
			this.cbxCaptureDevices.SelectedIndexChanged += new System.EventHandler(this.cbxCaptureDevices_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(16, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(75, 13);
			this.label4.TabIndex = 19;
			this.label4.Text = "Camera Model";
			// 
			// cbxCameraModel
			// 
			this.cbxCameraModel.FormattingEnabled = true;
			this.cbxCameraModel.Items.AddRange(new object[] {
            "G-Star",
            "WAT-120N",
            "WAT-910HX",
            "WAT-910BD",
            "Mintron 12V1C-EX",
            "Samsung SCB-2000",
            "PC165-DNR"});
			this.cbxCameraModel.Location = new System.Drawing.Point(16, 34);
			this.cbxCameraModel.Name = "cbxCameraModel";
			this.cbxCameraModel.Size = new System.Drawing.Size(198, 21);
			this.cbxCameraModel.TabIndex = 20;
			this.cbxCameraModel.SelectedIndexChanged += new System.EventHandler(this.cbxCameraModel_SelectedIndexChanged);
			// 
			// gbxCodecs
			// 
			this.gbxCodecs.Controls.Add(this.rbCodecLagarith);
			this.gbxCodecs.Controls.Add(this.rbCodecHuffyuv);
			this.gbxCodecs.Controls.Add(this.rbCodecXviD);
			this.gbxCodecs.Controls.Add(this.rbCodecUncompressed);
			this.gbxCodecs.Location = new System.Drawing.Point(131, 135);
			this.gbxCodecs.Name = "gbxCodecs";
			this.gbxCodecs.Size = new System.Drawing.Size(262, 161);
			this.gbxCodecs.TabIndex = 21;
			this.gbxCodecs.TabStop = false;
			this.gbxCodecs.Text = "Video codec";
			// 
			// rbCodecLagarith
			// 
			this.rbCodecLagarith.AutoSize = true;
			this.rbCodecLagarith.Location = new System.Drawing.Point(23, 48);
			this.rbCodecLagarith.Name = "rbCodecLagarith";
			this.rbCodecLagarith.Size = new System.Drawing.Size(140, 17);
			this.rbCodecLagarith.TabIndex = 4;
			this.rbCodecLagarith.Text = "Lagarith Lossless Codec";
			this.rbCodecLagarith.UseVisualStyleBackColor = true;
			// 
			// rbCodecHuffyuv
			// 
			this.rbCodecHuffyuv.AutoSize = true;
			this.rbCodecHuffyuv.Checked = true;
			this.rbCodecHuffyuv.Location = new System.Drawing.Point(23, 19);
			this.rbCodecHuffyuv.Name = "rbCodecHuffyuv";
			this.rbCodecHuffyuv.Size = new System.Drawing.Size(95, 17);
			this.rbCodecHuffyuv.TabIndex = 3;
			this.rbCodecHuffyuv.TabStop = true;
			this.rbCodecHuffyuv.Text = "Huffyuv v2.1.1";
			this.rbCodecHuffyuv.UseVisualStyleBackColor = true;
			// 
			// rbCodecXviD
			// 
			this.rbCodecXviD.AutoSize = true;
			this.rbCodecXviD.Location = new System.Drawing.Point(23, 77);
			this.rbCodecXviD.Name = "rbCodecXviD";
			this.rbCodecXviD.Size = new System.Drawing.Size(123, 17);
			this.rbCodecXviD.TabIndex = 2;
			this.rbCodecXviD.Text = "Xvid MPEG-4 Codec";
			this.rbCodecXviD.UseVisualStyleBackColor = true;
			// 
			// rbCodecUncompressed
			// 
			this.rbCodecUncompressed.AutoSize = true;
			this.rbCodecUncompressed.Location = new System.Drawing.Point(23, 106);
			this.rbCodecUncompressed.Name = "rbCodecUncompressed";
			this.rbCodecUncompressed.Size = new System.Drawing.Size(96, 17);
			this.rbCodecUncompressed.TabIndex = 1;
			this.rbCodecUncompressed.Text = "Uncompressed";
			this.rbCodecUncompressed.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.cbFileSIM);
			this.groupBox2.Controls.Add(this.rbFileAVI);
			this.groupBox2.Controls.Add(this.rbFileAAV);
			this.groupBox2.Location = new System.Drawing.Point(15, 121);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(89, 196);
			this.groupBox2.TabIndex = 22;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Format";
			// 
			// cbFileSIM
			// 
			this.cbFileSIM.AutoSize = true;
			this.cbFileSIM.Location = new System.Drawing.Point(22, 85);
			this.cbFileSIM.Name = "cbFileSIM";
			this.cbFileSIM.Size = new System.Drawing.Size(45, 17);
			this.cbFileSIM.TabIndex = 27;
			this.cbFileSIM.Text = "SIM";
			this.cbFileSIM.UseVisualStyleBackColor = true;
			// 
			// rbFileAVI
			// 
			this.rbFileAVI.AutoSize = true;
			this.rbFileAVI.Location = new System.Drawing.Point(23, 58);
			this.rbFileAVI.Name = "rbFileAVI";
			this.rbFileAVI.Size = new System.Drawing.Size(42, 17);
			this.rbFileAVI.TabIndex = 1;
			this.rbFileAVI.Text = "AVI";
			this.rbFileAVI.UseVisualStyleBackColor = true;
			this.rbFileAVI.CheckedChanged += new System.EventHandler(this.rbFileAVI_CheckedChanged);
			// 
			// rbFileAAV
			// 
			this.rbFileAAV.AutoSize = true;
			this.rbFileAAV.Checked = true;
			this.rbFileAAV.Location = new System.Drawing.Point(23, 28);
			this.rbFileAAV.Name = "rbFileAAV";
			this.rbFileAAV.Size = new System.Drawing.Size(46, 17);
			this.rbFileAAV.TabIndex = 0;
			this.rbFileAAV.TabStop = true;
			this.rbFileAAV.Text = "AAV";
			this.rbFileAAV.UseVisualStyleBackColor = true;
			this.rbFileAAV.CheckedChanged += new System.EventHandler(this.rbFileAAV_CheckedChanged);
			// 
			// cbxIsIntegrating
			// 
			this.cbxIsIntegrating.AutoSize = true;
			this.cbxIsIntegrating.Checked = true;
			this.cbxIsIntegrating.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxIsIntegrating.Location = new System.Drawing.Point(227, 36);
			this.cbxIsIntegrating.Name = "cbxIsIntegrating";
			this.cbxIsIntegrating.Size = new System.Drawing.Size(76, 17);
			this.cbxIsIntegrating.TabIndex = 23;
			this.cbxIsIntegrating.Text = "Integrating";
			this.cbxIsIntegrating.UseVisualStyleBackColor = true;
			// 
			// gbxAAVSettings
			// 
			this.gbxAAVSettings.Controls.Add(this.btnConfigureCameraDriver);
			this.gbxAAVSettings.Controls.Add(this.label6);
			this.gbxAAVSettings.Controls.Add(this.cbxCameraDriver);
			this.gbxAAVSettings.Controls.Add(this.llOnlineHelp);
			this.gbxAAVSettings.Controls.Add(this.pnlPreserveOSDArea);
			this.gbxAAVSettings.Controls.Add(this.cbxOCRConfigurations);
			this.gbxAAVSettings.Controls.Add(this.label13);
			this.gbxAAVSettings.Controls.Add(this.label4);
			this.gbxAAVSettings.Controls.Add(this.cbxIsIntegrating);
			this.gbxAAVSettings.Controls.Add(this.cbxCameraModel);
			this.gbxAAVSettings.Location = new System.Drawing.Point(110, 121);
			this.gbxAAVSettings.Name = "gbxAAVSettings";
			this.gbxAAVSettings.Size = new System.Drawing.Size(332, 196);
			this.gbxAAVSettings.TabIndex = 22;
			this.gbxAAVSettings.TabStop = false;
			this.gbxAAVSettings.Text = "AAV settings";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(16, 60);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(74, 13);
			this.label6.TabIndex = 33;
			this.label6.Text = "Camera Driver";
			// 
			// cbxCameraDriver
			// 
			this.cbxCameraDriver.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxCameraDriver.Enabled = false;
			this.cbxCameraDriver.FormattingEnabled = true;
			this.cbxCameraDriver.Location = new System.Drawing.Point(16, 78);
			this.cbxCameraDriver.Name = "cbxCameraDriver";
			this.cbxCameraDriver.Size = new System.Drawing.Size(198, 21);
			this.cbxCameraDriver.TabIndex = 34;
			this.cbxCameraDriver.SelectedIndexChanged += new System.EventHandler(this.cbxCameraDriver_SelectedIndexChanged);
			// 
			// llOnlineHelp
			// 
			this.llOnlineHelp.AutoSize = true;
			this.llOnlineHelp.Location = new System.Drawing.Point(262, 112);
			this.llOnlineHelp.Name = "llOnlineHelp";
			this.llOnlineHelp.Size = new System.Drawing.Size(62, 13);
			this.llOnlineHelp.TabIndex = 32;
			this.llOnlineHelp.TabStop = true;
			this.llOnlineHelp.Text = "Online Help";
			this.llOnlineHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llOnlineHelp_LinkClicked);
			// 
			// pnlPreserveOSDArea
			// 
			this.pnlPreserveOSDArea.Controls.Add(this.label11);
			this.pnlPreserveOSDArea.Controls.Add(this.nudPreserveVTIBottomRow);
			this.pnlPreserveOSDArea.Controls.Add(this.nudPreserveVTITopRow);
			this.pnlPreserveOSDArea.Controls.Add(this.label3);
			this.pnlPreserveOSDArea.Location = new System.Drawing.Point(6, 163);
			this.pnlPreserveOSDArea.Name = "pnlPreserveOSDArea";
			this.pnlPreserveOSDArea.Size = new System.Drawing.Size(275, 25);
			this.pnlPreserveOSDArea.TabIndex = 31;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label11.Location = new System.Drawing.Point(7, 6);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(126, 13);
			this.label11.TabIndex = 26;
			this.label11.Text = "Preserve OSD rows from:";
			// 
			// nudPreserveVTIBottomRow
			// 
			this.nudPreserveVTIBottomRow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.nudPreserveVTIBottomRow.Location = new System.Drawing.Point(211, 3);
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
			this.nudPreserveVTITopRow.Location = new System.Drawing.Point(135, 3);
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
			this.label3.Location = new System.Drawing.Point(192, 6);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(16, 13);
			this.label3.TabIndex = 27;
			this.label3.Text = "to";
			// 
			// cbxOCRConfigurations
			// 
			this.cbxOCRConfigurations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxOCRConfigurations.FormattingEnabled = true;
			this.cbxOCRConfigurations.Location = new System.Drawing.Point(16, 128);
			this.cbxOCRConfigurations.Name = "cbxOCRConfigurations";
			this.cbxOCRConfigurations.Size = new System.Drawing.Size(308, 21);
			this.cbxOCRConfigurations.TabIndex = 28;
			this.cbxOCRConfigurations.SelectedIndexChanged += new System.EventHandler(this.cbxOCRConfigurations_SelectedIndexChanged);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(13, 112);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(118, 13);
			this.label13.TabIndex = 29;
			this.label13.Text = "VTI OSD Configuration:";
			// 
			// pnlCrossbar
			// 
			this.pnlCrossbar.Controls.Add(this.cbxCrossbarInput);
			this.pnlCrossbar.Controls.Add(this.label5);
			this.pnlCrossbar.Location = new System.Drawing.Point(228, 59);
			this.pnlCrossbar.Name = "pnlCrossbar";
			this.pnlCrossbar.Size = new System.Drawing.Size(214, 56);
			this.pnlCrossbar.TabIndex = 23;
			this.pnlCrossbar.Visible = false;
			// 
			// cbxCrossbarInput
			// 
			this.cbxCrossbarInput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxCrossbarInput.FormattingEnabled = true;
			this.cbxCrossbarInput.Location = new System.Drawing.Point(7, 25);
			this.cbxCrossbarInput.Name = "cbxCrossbarInput";
			this.cbxCrossbarInput.Size = new System.Drawing.Size(199, 21);
			this.cbxCrossbarInput.TabIndex = 18;
			this.cbxCrossbarInput.SelectedIndexChanged += new System.EventHandler(this.cbxCrossbarInput_SelectedIndexChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 7);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(75, 13);
			this.label5.TabIndex = 13;
			this.label5.Text = "Crossbar Input";
			// 
			// cbxVideoFormats
			// 
			this.cbxVideoFormats.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxVideoFormats.FormattingEnabled = true;
			this.cbxVideoFormats.Location = new System.Drawing.Point(12, 84);
			this.cbxVideoFormats.Name = "cbxVideoFormats";
			this.cbxVideoFormats.Size = new System.Drawing.Size(210, 21);
			this.cbxVideoFormats.TabIndex = 26;
			this.cbxVideoFormats.SelectedIndexChanged += new System.EventHandler(this.cbxVideoFormats_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 66);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(166, 13);
			this.label1.TabIndex = 27;
			this.label1.Text = "Video Resolution and Frame Rate";
			// 
			// pnlFlipControls
			// 
			this.pnlFlipControls.Controls.Add(this.cbxFlipVertically);
			this.pnlFlipControls.Controls.Add(this.cbxFlipHorizontally);
			this.pnlFlipControls.Location = new System.Drawing.Point(228, 16);
			this.pnlFlipControls.Name = "pnlFlipControls";
			this.pnlFlipControls.Size = new System.Drawing.Size(214, 37);
			this.pnlFlipControls.TabIndex = 28;
			// 
			// cbxFlipVertically
			// 
			this.cbxFlipVertically.AutoSize = true;
			this.cbxFlipVertically.Location = new System.Drawing.Point(109, 13);
			this.cbxFlipVertically.Name = "cbxFlipVertically";
			this.cbxFlipVertically.Size = new System.Drawing.Size(87, 17);
			this.cbxFlipVertically.TabIndex = 27;
			this.cbxFlipVertically.Text = "Flip Vertically";
			this.cbxFlipVertically.UseVisualStyleBackColor = true;
			// 
			// cbxFlipHorizontally
			// 
			this.cbxFlipHorizontally.AutoSize = true;
			this.cbxFlipHorizontally.Location = new System.Drawing.Point(4, 13);
			this.cbxFlipHorizontally.Name = "cbxFlipHorizontally";
			this.cbxFlipHorizontally.Size = new System.Drawing.Size(99, 17);
			this.cbxFlipHorizontally.TabIndex = 26;
			this.cbxFlipHorizontally.Text = "Flip Horizontally";
			this.cbxFlipHorizontally.UseVisualStyleBackColor = true;
			// 
			// btnConfigureCameraDriver
			// 
			this.btnConfigureCameraDriver.Location = new System.Drawing.Point(227, 76);
			this.btnConfigureCameraDriver.Name = "btnConfigureCameraDriver";
			this.btnConfigureCameraDriver.Size = new System.Drawing.Size(75, 23);
			this.btnConfigureCameraDriver.TabIndex = 35;
			this.btnConfigureCameraDriver.Text = "Configure";
			this.btnConfigureCameraDriver.UseVisualStyleBackColor = true;
			this.btnConfigureCameraDriver.Visible = false;
			this.btnConfigureCameraDriver.Click += new System.EventHandler(this.btnConfigureCameraDriver_Click);
			// 
			// frmChooseCamera
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(456, 358);
			this.Controls.Add(this.pnlFlipControls);
			this.Controls.Add(this.gbxAAVSettings);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cbxVideoFormats);
			this.Controls.Add(this.pnlCrossbar);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.cbxCaptureDevices);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.gbxCodecs);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmChooseCamera";
			this.Text = "Connect to camera";
			this.Load += new System.EventHandler(this.frmChooseCamera_Load);
			this.gbxCodecs.ResumeLayout(false);
			this.gbxCodecs.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.gbxAAVSettings.ResumeLayout(false);
			this.gbxAAVSettings.PerformLayout();
			this.pnlPreserveOSDArea.ResumeLayout(false);
			this.pnlPreserveOSDArea.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudPreserveVTIBottomRow)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPreserveVTITopRow)).EndInit();
			this.pnlCrossbar.ResumeLayout(false);
			this.pnlCrossbar.PerformLayout();
			this.pnlFlipControls.ResumeLayout(false);
			this.pnlFlipControls.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbxCaptureDevices;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbxCameraModel;
        private System.Windows.Forms.GroupBox gbxCodecs;
        private System.Windows.Forms.RadioButton rbCodecLagarith;
        private System.Windows.Forms.RadioButton rbCodecHuffyuv;
        private System.Windows.Forms.RadioButton rbCodecXviD;
        private System.Windows.Forms.RadioButton rbCodecUncompressed;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbFileAVI;
        private System.Windows.Forms.RadioButton rbFileAAV;
        private System.Windows.Forms.CheckBox cbxIsIntegrating;
        private System.Windows.Forms.GroupBox gbxAAVSettings;
        private System.Windows.Forms.Panel pnlCrossbar;
        private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cbxCrossbarInput;
        private System.Windows.Forms.CheckBox cbFileSIM;
        private System.Windows.Forms.ComboBox cbxVideoFormats;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudPreserveVTITopRow;
        private System.Windows.Forms.NumericUpDown nudPreserveVTIBottomRow;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cbxOCRConfigurations;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Panel pnlPreserveOSDArea;
		private System.Windows.Forms.Panel pnlFlipControls;
		private System.Windows.Forms.CheckBox cbxFlipVertically;
		private System.Windows.Forms.CheckBox cbxFlipHorizontally;
		private System.Windows.Forms.LinkLabel llOnlineHelp;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox cbxCameraDriver;
		private System.Windows.Forms.Button btnConfigureCameraDriver;
    }
}