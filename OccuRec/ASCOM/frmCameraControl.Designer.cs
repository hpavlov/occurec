namespace OccuRec.ASCOM
{
	partial class frmCameraControl
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
			this.tcControls = new System.Windows.Forms.TabControl();
			this.tabDirect = new System.Windows.Forms.TabPage();
			this.lblGamma = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.btnGammaUp = new System.Windows.Forms.Button();
			this.btnGammaDown = new System.Windows.Forms.Button();
			this.lblGain = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.btnGainUp = new System.Windows.Forms.Button();
			this.btnGainDown = new System.Windows.Forms.Button();
			this.lblExposure = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnExposureUp = new System.Windows.Forms.Button();
			this.btnExposureDown = new System.Windows.Forms.Button();
			this.tabOSDControl = new System.Windows.Forms.TabPage();
			this.btnOSDSet = new System.Windows.Forms.Button();
			this.btnOSDRight = new System.Windows.Forms.Button();
			this.btnOSDDown = new System.Windows.Forms.Button();
			this.btnOSDLeft = new System.Windows.Forms.Button();
			this.btnOSDUp = new System.Windows.Forms.Button();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.connectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miDisconnect = new System.Windows.Forms.ToolStripMenuItem();
			this.tcControls.SuspendLayout();
			this.tabDirect.SuspendLayout();
			this.tabOSDControl.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tcControls
			// 
			this.tcControls.Controls.Add(this.tabDirect);
			this.tcControls.Controls.Add(this.tabOSDControl);
			this.tcControls.Location = new System.Drawing.Point(12, 37);
			this.tcControls.Name = "tcControls";
			this.tcControls.SelectedIndex = 0;
			this.tcControls.Size = new System.Drawing.Size(363, 195);
			this.tcControls.TabIndex = 0;
			// 
			// tabDirect
			// 
			this.tabDirect.Controls.Add(this.lblGamma);
			this.tabDirect.Controls.Add(this.label6);
			this.tabDirect.Controls.Add(this.btnGammaUp);
			this.tabDirect.Controls.Add(this.btnGammaDown);
			this.tabDirect.Controls.Add(this.lblGain);
			this.tabDirect.Controls.Add(this.label4);
			this.tabDirect.Controls.Add(this.btnGainUp);
			this.tabDirect.Controls.Add(this.btnGainDown);
			this.tabDirect.Controls.Add(this.lblExposure);
			this.tabDirect.Controls.Add(this.label1);
			this.tabDirect.Controls.Add(this.btnExposureUp);
			this.tabDirect.Controls.Add(this.btnExposureDown);
			this.tabDirect.Location = new System.Drawing.Point(4, 22);
			this.tabDirect.Name = "tabDirect";
			this.tabDirect.Padding = new System.Windows.Forms.Padding(3);
			this.tabDirect.Size = new System.Drawing.Size(355, 169);
			this.tabDirect.TabIndex = 0;
			this.tabDirect.Text = "Direct Control";
			this.tabDirect.UseVisualStyleBackColor = true;
			// 
			// lblGamma
			// 
			this.lblGamma.AutoSize = true;
			this.lblGamma.Location = new System.Drawing.Point(180, 130);
			this.lblGamma.Name = "lblGamma";
			this.lblGamma.Size = new System.Drawing.Size(0, 13);
			this.lblGamma.TabIndex = 14;
			this.lblGamma.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(21, 130);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(43, 13);
			this.label6.TabIndex = 13;
			this.label6.Text = "Gamma";
			// 
			// btnGammaUp
			// 
			this.btnGammaUp.Location = new System.Drawing.Point(247, 125);
			this.btnGammaUp.Name = "btnGammaUp";
			this.btnGammaUp.Size = new System.Drawing.Size(55, 23);
			this.btnGammaUp.TabIndex = 12;
			this.btnGammaUp.Text = ">";
			this.btnGammaUp.UseVisualStyleBackColor = true;
			this.btnGammaUp.Click += new System.EventHandler(this.btnGammaUp_Click);
			// 
			// btnGammaDown
			// 
			this.btnGammaDown.Location = new System.Drawing.Point(117, 125);
			this.btnGammaDown.Name = "btnGammaDown";
			this.btnGammaDown.Size = new System.Drawing.Size(55, 23);
			this.btnGammaDown.TabIndex = 11;
			this.btnGammaDown.Text = "<";
			this.btnGammaDown.UseVisualStyleBackColor = true;
			this.btnGammaDown.Click += new System.EventHandler(this.btnGammaDown_Click);
			// 
			// lblGain
			// 
			this.lblGain.AutoSize = true;
			this.lblGain.Location = new System.Drawing.Point(180, 77);
			this.lblGain.Name = "lblGain";
			this.lblGain.Size = new System.Drawing.Size(0, 13);
			this.lblGain.TabIndex = 10;
			this.lblGain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(21, 77);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(29, 13);
			this.label4.TabIndex = 9;
			this.label4.Text = "Gain";
			// 
			// btnGainUp
			// 
			this.btnGainUp.Location = new System.Drawing.Point(247, 72);
			this.btnGainUp.Name = "btnGainUp";
			this.btnGainUp.Size = new System.Drawing.Size(55, 23);
			this.btnGainUp.TabIndex = 8;
			this.btnGainUp.Text = ">";
			this.btnGainUp.UseVisualStyleBackColor = true;
			this.btnGainUp.Click += new System.EventHandler(this.btnGainUp_Click);
			// 
			// btnGainDown
			// 
			this.btnGainDown.Location = new System.Drawing.Point(117, 72);
			this.btnGainDown.Name = "btnGainDown";
			this.btnGainDown.Size = new System.Drawing.Size(55, 23);
			this.btnGainDown.TabIndex = 7;
			this.btnGainDown.Text = "<";
			this.btnGainDown.UseVisualStyleBackColor = true;
			this.btnGainDown.Click += new System.EventHandler(this.btnGainDown_Click);
			// 
			// lblExposure
			// 
			this.lblExposure.AutoSize = true;
			this.lblExposure.Location = new System.Drawing.Point(180, 28);
			this.lblExposure.Name = "lblExposure";
			this.lblExposure.Size = new System.Drawing.Size(0, 13);
			this.lblExposure.TabIndex = 6;
			this.lblExposure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(21, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(51, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Exposure";
			// 
			// btnExposureUp
			// 
			this.btnExposureUp.Location = new System.Drawing.Point(247, 23);
			this.btnExposureUp.Name = "btnExposureUp";
			this.btnExposureUp.Size = new System.Drawing.Size(55, 23);
			this.btnExposureUp.TabIndex = 4;
			this.btnExposureUp.Text = ">";
			this.btnExposureUp.UseVisualStyleBackColor = true;
			this.btnExposureUp.Click += new System.EventHandler(this.btnExposureUp_Click);
			// 
			// btnExposureDown
			// 
			this.btnExposureDown.Location = new System.Drawing.Point(117, 23);
			this.btnExposureDown.Name = "btnExposureDown";
			this.btnExposureDown.Size = new System.Drawing.Size(55, 23);
			this.btnExposureDown.TabIndex = 3;
			this.btnExposureDown.Text = "<";
			this.btnExposureDown.UseVisualStyleBackColor = true;
			this.btnExposureDown.Click += new System.EventHandler(this.btnExposureDown_Click);
			// 
			// tabOSDControl
			// 
			this.tabOSDControl.Controls.Add(this.btnOSDSet);
			this.tabOSDControl.Controls.Add(this.btnOSDRight);
			this.tabOSDControl.Controls.Add(this.btnOSDDown);
			this.tabOSDControl.Controls.Add(this.btnOSDLeft);
			this.tabOSDControl.Controls.Add(this.btnOSDUp);
			this.tabOSDControl.Location = new System.Drawing.Point(4, 22);
			this.tabOSDControl.Name = "tabOSDControl";
			this.tabOSDControl.Padding = new System.Windows.Forms.Padding(3);
			this.tabOSDControl.Size = new System.Drawing.Size(355, 169);
			this.tabOSDControl.TabIndex = 1;
			this.tabOSDControl.Text = "5-Button OSD Control";
			this.tabOSDControl.UseVisualStyleBackColor = true;
			// 
			// btnOSDSet
			// 
			this.btnOSDSet.Location = new System.Drawing.Point(139, 72);
			this.btnOSDSet.Name = "btnOSDSet";
			this.btnOSDSet.Size = new System.Drawing.Size(65, 23);
			this.btnOSDSet.TabIndex = 9;
			this.btnOSDSet.Text = "Set";
			this.btnOSDSet.UseVisualStyleBackColor = true;
			// 
			// btnOSDRight
			// 
			this.btnOSDRight.Location = new System.Drawing.Point(219, 72);
			this.btnOSDRight.Name = "btnOSDRight";
			this.btnOSDRight.Size = new System.Drawing.Size(55, 23);
			this.btnOSDRight.TabIndex = 8;
			this.btnOSDRight.Text = "Right >";
			this.btnOSDRight.UseVisualStyleBackColor = true;
			// 
			// btnOSDDown
			// 
			this.btnOSDDown.Location = new System.Drawing.Point(139, 114);
			this.btnOSDDown.Name = "btnOSDDown";
			this.btnOSDDown.Size = new System.Drawing.Size(65, 23);
			this.btnOSDDown.TabIndex = 7;
			this.btnOSDDown.Text = "Down \\/";
			this.btnOSDDown.UseVisualStyleBackColor = true;
			// 
			// btnOSDLeft
			// 
			this.btnOSDLeft.Location = new System.Drawing.Point(70, 72);
			this.btnOSDLeft.Name = "btnOSDLeft";
			this.btnOSDLeft.Size = new System.Drawing.Size(55, 23);
			this.btnOSDLeft.TabIndex = 6;
			this.btnOSDLeft.Text = "< Left";
			this.btnOSDLeft.UseVisualStyleBackColor = true;
			// 
			// btnOSDUp
			// 
			this.btnOSDUp.Location = new System.Drawing.Point(139, 27);
			this.btnOSDUp.Name = "btnOSDUp";
			this.btnOSDUp.Size = new System.Drawing.Size(65, 23);
			this.btnOSDUp.TabIndex = 5;
			this.btnOSDUp.Text = "Up /\\";
			this.btnOSDUp.UseVisualStyleBackColor = true;
			this.btnOSDUp.Click += new System.EventHandler(this.btnOSDUp_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(388, 24);
			this.menuStrip1.TabIndex = 14;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// connectionToolStripMenuItem
			// 
			this.connectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDisconnect});
			this.connectionToolStripMenuItem.Name = "connectionToolStripMenuItem";
			this.connectionToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
			this.connectionToolStripMenuItem.Text = "&Connection";
			// 
			// miDisconnect
			// 
			this.miDisconnect.Name = "miDisconnect";
			this.miDisconnect.Size = new System.Drawing.Size(152, 22);
			this.miDisconnect.Text = "&Disconnect";
			this.miDisconnect.Click += new System.EventHandler(this.miDisconnect_Click);
			// 
			// frmCameraControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(388, 244);
			this.Controls.Add(this.tcControls);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "frmCameraControl";
			this.Text = "Camera Control";
			this.Load += new System.EventHandler(this.frmCameraControl_Load);
			this.tcControls.ResumeLayout(false);
			this.tabDirect.ResumeLayout(false);
			this.tabDirect.PerformLayout();
			this.tabOSDControl.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tcControls;
		private System.Windows.Forms.TabPage tabDirect;
		private System.Windows.Forms.TabPage tabOSDControl;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnExposureUp;
		private System.Windows.Forms.Button btnExposureDown;
		private System.Windows.Forms.Button btnOSDSet;
		private System.Windows.Forms.Button btnOSDRight;
		private System.Windows.Forms.Button btnOSDDown;
		private System.Windows.Forms.Button btnOSDLeft;
		private System.Windows.Forms.Button btnOSDUp;
		private System.Windows.Forms.Label lblExposure;
		private System.Windows.Forms.Label lblGamma;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btnGammaUp;
		private System.Windows.Forms.Button btnGammaDown;
		private System.Windows.Forms.Label lblGain;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnGainUp;
		private System.Windows.Forms.Button btnGainDown;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem connectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem miDisconnect;
	}
}