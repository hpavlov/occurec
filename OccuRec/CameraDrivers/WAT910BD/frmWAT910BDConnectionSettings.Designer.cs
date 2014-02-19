namespace OccuRec.CameraDrivers.WAT910BD
{
	partial class frmWAT910BDConnectionSettings
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
			this.cbxCOMPort = new System.Windows.Forms.ComboBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.llDriversLink = new System.Windows.Forms.LinkLabel();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.llDriverGuideUrl = new System.Windows.Forms.LinkLabel();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbxCOMPort
			// 
			this.cbxCOMPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxCOMPort.FormattingEnabled = true;
			this.cbxCOMPort.Location = new System.Drawing.Point(149, 164);
			this.cbxCOMPort.Name = "cbxCOMPort";
			this.cbxCOMPort.Size = new System.Drawing.Size(115, 21);
			this.cbxCOMPort.TabIndex = 3;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(222, 222);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(303, 222);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.llDriverGuideUrl);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.llDriversLink);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.cbxCOMPort);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(366, 204);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 167);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(134, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Configured VCP COM Port:";
			// 
			// llDriversLink
			// 
			this.llDriversLink.AutoSize = true;
			this.llDriversLink.Location = new System.Drawing.Point(9, 63);
			this.llDriversLink.Name = "llDriversLink";
			this.llDriversLink.Size = new System.Drawing.Size(206, 13);
			this.llDriversLink.TabIndex = 6;
			this.llDriversLink.TabStop = true;
			this.llDriversLink.Text = "http://www.ftdichip.com/Drivers/VCP.htm";
			this.llDriversLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llDriversLink_LinkClicked);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(9, 143);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(329, 17);
			this.label3.TabIndex = 5;
			this.label3.Text = "Once a COM port has been set up, specify the port name below:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(351, 49);
			this.label2.TabIndex = 4;
			this.label2.Text = "You need to install Virtual COM Port (VCP) drivers  and configure your COM port f" +
    "rom window\'s Device Manager before you can connect to the camera. Get the driver" +
    "s from the link below:";
			// 
			// llDriverGuideUrl
			// 
			this.llDriverGuideUrl.AutoSize = true;
			this.llDriverGuideUrl.Location = new System.Drawing.Point(9, 111);
			this.llDriverGuideUrl.Name = "llDriverGuideUrl";
			this.llDriverGuideUrl.Size = new System.Drawing.Size(308, 13);
			this.llDriverGuideUrl.TabIndex = 8;
			this.llDriverGuideUrl.TabStop = true;
			this.llDriverGuideUrl.Text = "http://www.ftdichip.com/Support/Documents/InstallGuides.htm";
			this.llDriverGuideUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llDriverGuideUrl_LinkClicked);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(9, 93);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(206, 15);
			this.label4.TabIndex = 7;
			this.label4.Text = "Driver setup guide is available here:";
			// 
			// frmWAT910BDConnectionSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(390, 257);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmWAT910BDConnectionSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "WAT910BD Nano Driver Connection Settings";
			this.Load += new System.EventHandler(this.frmWAT910BDConnectionSettings_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox cbxCOMPort;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.LinkLabel llDriversLink;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel llDriverGuideUrl;
		private System.Windows.Forms.Label label4;
	}
}