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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChooseCamera));
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.cbxCaptureDevices = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.cbxCameraModel = new System.Windows.Forms.ComboBox();
			this.cbFileSIM = new System.Windows.Forms.CheckBox();
			this.btnConfigureCameraDriver = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.cbxCameraDriver = new System.Windows.Forms.ComboBox();
			this.pnlCrossbar = new System.Windows.Forms.Panel();
			this.cbxCrossbarInput = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.cbxVideoFormats = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.pnlFlipControls = new System.Windows.Forms.Panel();
			this.cbxFlipVertically = new System.Windows.Forms.CheckBox();
			this.cbxFlipHorizontally = new System.Windows.Forms.CheckBox();
			this.pnlSimpleFrameRate = new System.Windows.Forms.Panel();
			this.rbOtherMode = new System.Windows.Forms.RadioButton();
			this.rbNTSC = new System.Windows.Forms.RadioButton();
			this.rbPAL = new System.Windows.Forms.RadioButton();
			this.cbxIsIntegrating = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.toolTipControl = new System.Windows.Forms.ToolTip(this.components);
			this.pnlCrossbar.SuspendLayout();
			this.pnlFlipControls.SuspendLayout();
			this.pnlSimpleFrameRate.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(214, 233);
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
			this.btnCancel.Location = new System.Drawing.Point(295, 233);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(111, 13);
			this.label2.TabIndex = 12;
			this.label2.Text = "Video Capture Device";
			// 
			// cbxCaptureDevices
			// 
			this.cbxCaptureDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxCaptureDevices.FormattingEnabled = true;
			this.cbxCaptureDevices.Location = new System.Drawing.Point(6, 34);
			this.cbxCaptureDevices.Name = "cbxCaptureDevices";
			this.cbxCaptureDevices.Size = new System.Drawing.Size(210, 21);
			this.cbxCaptureDevices.TabIndex = 14;
			this.cbxCaptureDevices.SelectedIndexChanged += new System.EventHandler(this.cbxCaptureDevices_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 120);
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
			this.cbxCameraModel.Location = new System.Drawing.Point(6, 138);
			this.cbxCameraModel.Name = "cbxCameraModel";
			this.cbxCameraModel.Size = new System.Drawing.Size(210, 21);
			this.cbxCameraModel.TabIndex = 20;
			this.cbxCameraModel.SelectedIndexChanged += new System.EventHandler(this.cbxCameraModel_SelectedIndexChanged);
			// 
			// cbFileSIM
			// 
			this.cbFileSIM.AutoSize = true;
			this.cbFileSIM.Location = new System.Drawing.Point(301, 140);
			this.cbFileSIM.Name = "cbFileSIM";
			this.cbFileSIM.Size = new System.Drawing.Size(45, 17);
			this.cbFileSIM.TabIndex = 27;
			this.cbFileSIM.Text = "SIM";
			this.cbFileSIM.UseVisualStyleBackColor = true;
			// 
			// btnConfigureCameraDriver
			// 
			this.btnConfigureCameraDriver.Location = new System.Drawing.Point(229, 180);
			this.btnConfigureCameraDriver.Name = "btnConfigureCameraDriver";
			this.btnConfigureCameraDriver.Size = new System.Drawing.Size(75, 23);
			this.btnConfigureCameraDriver.TabIndex = 35;
			this.btnConfigureCameraDriver.Text = "Configure";
			this.btnConfigureCameraDriver.UseVisualStyleBackColor = true;
			this.btnConfigureCameraDriver.Visible = false;
			this.btnConfigureCameraDriver.Click += new System.EventHandler(this.btnConfigureCameraDriver_Click);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 164);
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
			this.cbxCameraDriver.Location = new System.Drawing.Point(6, 182);
			this.cbxCameraDriver.Name = "cbxCameraDriver";
			this.cbxCameraDriver.Size = new System.Drawing.Size(210, 21);
			this.cbxCameraDriver.TabIndex = 34;
			this.cbxCameraDriver.SelectedIndexChanged += new System.EventHandler(this.cbxCameraDriver_SelectedIndexChanged);
			// 
			// pnlCrossbar
			// 
			this.pnlCrossbar.Controls.Add(this.cbxCrossbarInput);
			this.pnlCrossbar.Controls.Add(this.label5);
			this.pnlCrossbar.Location = new System.Drawing.Point(222, 9);
			this.pnlCrossbar.Name = "pnlCrossbar";
			this.pnlCrossbar.Size = new System.Drawing.Size(124, 48);
			this.pnlCrossbar.TabIndex = 23;
			this.pnlCrossbar.Visible = false;
			// 
			// cbxCrossbarInput
			// 
			this.cbxCrossbarInput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxCrossbarInput.FormattingEnabled = true;
			this.cbxCrossbarInput.Location = new System.Drawing.Point(7, 25);
			this.cbxCrossbarInput.Name = "cbxCrossbarInput";
			this.cbxCrossbarInput.Size = new System.Drawing.Size(115, 21);
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
			this.cbxVideoFormats.Location = new System.Drawing.Point(6, 84);
			this.cbxVideoFormats.Name = "cbxVideoFormats";
			this.cbxVideoFormats.Size = new System.Drawing.Size(210, 21);
			this.cbxVideoFormats.TabIndex = 26;
			this.cbxVideoFormats.SelectedIndexChanged += new System.EventHandler(this.cbxVideoFormats_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 66);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(166, 13);
			this.label1.TabIndex = 27;
			this.label1.Text = "Video Resolution and Frame Rate";
			// 
			// pnlFlipControls
			// 
			this.pnlFlipControls.Controls.Add(this.cbxFlipVertically);
			this.pnlFlipControls.Controls.Add(this.cbxFlipHorizontally);
			this.pnlFlipControls.Location = new System.Drawing.Point(227, 86);
			this.pnlFlipControls.Name = "pnlFlipControls";
			this.pnlFlipControls.Size = new System.Drawing.Size(108, 41);
			this.pnlFlipControls.TabIndex = 28;
			// 
			// cbxFlipVertically
			// 
			this.cbxFlipVertically.AutoSize = true;
			this.cbxFlipVertically.Location = new System.Drawing.Point(2, 22);
			this.cbxFlipVertically.Name = "cbxFlipVertically";
			this.cbxFlipVertically.Size = new System.Drawing.Size(87, 17);
			this.cbxFlipVertically.TabIndex = 27;
			this.cbxFlipVertically.Text = "Flip Vertically";
			this.cbxFlipVertically.UseVisualStyleBackColor = true;
			// 
			// cbxFlipHorizontally
			// 
			this.cbxFlipHorizontally.AutoSize = true;
			this.cbxFlipHorizontally.Location = new System.Drawing.Point(2, 1);
			this.cbxFlipHorizontally.Name = "cbxFlipHorizontally";
			this.cbxFlipHorizontally.Size = new System.Drawing.Size(99, 17);
			this.cbxFlipHorizontally.TabIndex = 26;
			this.cbxFlipHorizontally.Text = "Flip Horizontally";
			this.cbxFlipHorizontally.UseVisualStyleBackColor = true;
			// 
			// pnlSimpleFrameRate
			// 
			this.pnlSimpleFrameRate.Controls.Add(this.rbOtherMode);
			this.pnlSimpleFrameRate.Controls.Add(this.rbNTSC);
			this.pnlSimpleFrameRate.Controls.Add(this.rbPAL);
			this.pnlSimpleFrameRate.Location = new System.Drawing.Point(9, 85);
			this.pnlSimpleFrameRate.Name = "pnlSimpleFrameRate";
			this.pnlSimpleFrameRate.Size = new System.Drawing.Size(168, 22);
			this.pnlSimpleFrameRate.TabIndex = 29;
			// 
			// rbOtherMode
			// 
			this.rbOtherMode.AutoSize = true;
			this.rbOtherMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.rbOtherMode.ForeColor = System.Drawing.Color.Black;
			this.rbOtherMode.Location = new System.Drawing.Point(117, 2);
			this.rbOtherMode.Name = "rbOtherMode";
			this.rbOtherMode.Size = new System.Drawing.Size(51, 17);
			this.rbOtherMode.TabIndex = 31;
			this.rbOtherMode.Text = "Other";
			this.rbOtherMode.UseVisualStyleBackColor = true;
			this.rbOtherMode.CheckedChanged += new System.EventHandler(this.rbOtherMode_CheckedChanged);
			// 
			// rbNTSC
			// 
			this.rbNTSC.AutoSize = true;
			this.rbNTSC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.rbNTSC.ForeColor = System.Drawing.Color.OrangeRed;
			this.rbNTSC.Location = new System.Drawing.Point(53, 2);
			this.rbNTSC.Name = "rbNTSC";
			this.rbNTSC.Size = new System.Drawing.Size(58, 17);
			this.rbNTSC.TabIndex = 30;
			this.rbNTSC.Text = "NTSC";
			this.rbNTSC.UseVisualStyleBackColor = true;
			// 
			// rbPAL
			// 
			this.rbPAL.AutoSize = true;
			this.rbPAL.Checked = true;
			this.rbPAL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.rbPAL.ForeColor = System.Drawing.Color.Green;
			this.rbPAL.Location = new System.Drawing.Point(1, 2);
			this.rbPAL.Name = "rbPAL";
			this.rbPAL.Size = new System.Drawing.Size(48, 17);
			this.rbPAL.TabIndex = 0;
			this.rbPAL.TabStop = true;
			this.rbPAL.Text = "PAL";
			this.rbPAL.UseVisualStyleBackColor = true;
			// 
			// cbxIsIntegrating
			// 
			this.cbxIsIntegrating.AutoSize = true;
			this.cbxIsIntegrating.Checked = true;
			this.cbxIsIntegrating.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxIsIntegrating.Location = new System.Drawing.Point(229, 140);
			this.cbxIsIntegrating.Name = "cbxIsIntegrating";
			this.cbxIsIntegrating.Size = new System.Drawing.Size(76, 17);
			this.cbxIsIntegrating.TabIndex = 23;
			this.cbxIsIntegrating.Text = "Integrating";
			this.cbxIsIntegrating.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.cbFileSIM);
			this.groupBox1.Controls.Add(this.cbxCaptureDevices);
			this.groupBox1.Controls.Add(this.pnlSimpleFrameRate);
			this.groupBox1.Controls.Add(this.pnlCrossbar);
			this.groupBox1.Controls.Add(this.btnConfigureCameraDriver);
			this.groupBox1.Controls.Add(this.cbxCameraModel);
			this.groupBox1.Controls.Add(this.pnlFlipControls);
			this.groupBox1.Controls.Add(this.cbxVideoFormats);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.cbxIsIntegrating);
			this.groupBox1.Controls.Add(this.cbxCameraDriver);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(358, 215);
			this.groupBox1.TabIndex = 36;
			this.groupBox1.TabStop = false;
			// 
			// frmChooseCamera
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(381, 262);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmChooseCamera";
			this.Text = "Connect to camera";
			this.Load += new System.EventHandler(this.frmChooseCamera_Load);
			this.pnlCrossbar.ResumeLayout(false);
			this.pnlCrossbar.PerformLayout();
			this.pnlFlipControls.ResumeLayout(false);
			this.pnlFlipControls.PerformLayout();
			this.pnlSimpleFrameRate.ResumeLayout(false);
			this.pnlSimpleFrameRate.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbxCaptureDevices;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbxCameraModel;
        private System.Windows.Forms.Panel pnlCrossbar;
        private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cbxCrossbarInput;
        private System.Windows.Forms.CheckBox cbFileSIM;
        private System.Windows.Forms.ComboBox cbxVideoFormats;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel pnlFlipControls;
		private System.Windows.Forms.CheckBox cbxFlipVertically;
		private System.Windows.Forms.CheckBox cbxFlipHorizontally;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox cbxCameraDriver;
		private System.Windows.Forms.Button btnConfigureCameraDriver;
		private System.Windows.Forms.Panel pnlSimpleFrameRate;
		private System.Windows.Forms.RadioButton rbOtherMode;
		private System.Windows.Forms.RadioButton rbNTSC;
		private System.Windows.Forms.RadioButton rbPAL;
        private System.Windows.Forms.CheckBox cbxIsIntegrating;
        private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ToolTip toolTipControl;
    }
}