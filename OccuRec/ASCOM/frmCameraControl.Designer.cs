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
			this.tabControl1 = new System.Windows.Forms.TabControl();
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
			this.button5 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.btnDisconnect = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabDirect.SuspendLayout();
			this.tabOSDControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabDirect);
			this.tabControl1.Controls.Add(this.tabOSDControl);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(363, 195);
			this.tabControl1.TabIndex = 0;
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
			this.lblGamma.Size = new System.Drawing.Size(43, 13);
			this.lblGamma.TabIndex = 14;
			this.lblGamma.Text = "Gamma";
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
			// 
			// btnGammaDown
			// 
			this.btnGammaDown.Location = new System.Drawing.Point(117, 125);
			this.btnGammaDown.Name = "btnGammaDown";
			this.btnGammaDown.Size = new System.Drawing.Size(55, 23);
			this.btnGammaDown.TabIndex = 11;
			this.btnGammaDown.Text = "<";
			this.btnGammaDown.UseVisualStyleBackColor = true;
			// 
			// lblGain
			// 
			this.lblGain.AutoSize = true;
			this.lblGain.Location = new System.Drawing.Point(180, 77);
			this.lblGain.Name = "lblGain";
			this.lblGain.Size = new System.Drawing.Size(29, 13);
			this.lblGain.TabIndex = 10;
			this.lblGain.Text = "Gain";
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
			// 
			// btnGainDown
			// 
			this.btnGainDown.Location = new System.Drawing.Point(117, 72);
			this.btnGainDown.Name = "btnGainDown";
			this.btnGainDown.Size = new System.Drawing.Size(55, 23);
			this.btnGainDown.TabIndex = 7;
			this.btnGainDown.Text = "<";
			this.btnGainDown.UseVisualStyleBackColor = true;
			// 
			// lblExposure
			// 
			this.lblExposure.AutoSize = true;
			this.lblExposure.Location = new System.Drawing.Point(180, 28);
			this.lblExposure.Name = "lblExposure";
			this.lblExposure.Size = new System.Drawing.Size(51, 13);
			this.lblExposure.TabIndex = 6;
			this.lblExposure.Text = "Exposure";
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
			// 
			// btnExposureDown
			// 
			this.btnExposureDown.Location = new System.Drawing.Point(117, 23);
			this.btnExposureDown.Name = "btnExposureDown";
			this.btnExposureDown.Size = new System.Drawing.Size(55, 23);
			this.btnExposureDown.TabIndex = 3;
			this.btnExposureDown.Text = "<";
			this.btnExposureDown.UseVisualStyleBackColor = true;
			// 
			// tabOSDControl
			// 
			this.tabOSDControl.Controls.Add(this.button5);
			this.tabOSDControl.Controls.Add(this.button4);
			this.tabOSDControl.Controls.Add(this.button3);
			this.tabOSDControl.Controls.Add(this.button1);
			this.tabOSDControl.Controls.Add(this.button2);
			this.tabOSDControl.Location = new System.Drawing.Point(4, 22);
			this.tabOSDControl.Name = "tabOSDControl";
			this.tabOSDControl.Padding = new System.Windows.Forms.Padding(3);
			this.tabOSDControl.Size = new System.Drawing.Size(355, 169);
			this.tabOSDControl.TabIndex = 1;
			this.tabOSDControl.Text = "5-Button OSD Control";
			this.tabOSDControl.UseVisualStyleBackColor = true;
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(139, 72);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(65, 23);
			this.button5.TabIndex = 9;
			this.button5.Text = "Set";
			this.button5.UseVisualStyleBackColor = true;
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(219, 72);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(55, 23);
			this.button4.TabIndex = 8;
			this.button4.Text = "Right >";
			this.button4.UseVisualStyleBackColor = true;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(139, 114);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(65, 23);
			this.button3.TabIndex = 7;
			this.button3.Text = "Down \\/";
			this.button3.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(70, 72);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(55, 23);
			this.button1.TabIndex = 6;
			this.button1.Text = "< Left";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(139, 27);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(65, 23);
			this.button2.TabIndex = 5;
			this.button2.Text = "Up /\\";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// btnDisconnect
			// 
			this.btnDisconnect.Location = new System.Drawing.Point(300, 216);
			this.btnDisconnect.Name = "btnDisconnect";
			this.btnDisconnect.Size = new System.Drawing.Size(75, 23);
			this.btnDisconnect.TabIndex = 13;
			this.btnDisconnect.Text = "Disconnect";
			this.btnDisconnect.UseVisualStyleBackColor = true;
			// 
			// frmCameraControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(388, 246);
			this.Controls.Add(this.btnDisconnect);
			this.Controls.Add(this.tabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmCameraControl";
			this.Text = "Camera Control";
			this.tabControl1.ResumeLayout(false);
			this.tabDirect.ResumeLayout(false);
			this.tabDirect.PerformLayout();
			this.tabOSDControl.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabDirect;
		private System.Windows.Forms.TabPage tabOSDControl;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnExposureUp;
		private System.Windows.Forms.Button btnExposureDown;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label lblExposure;
		private System.Windows.Forms.Label lblGamma;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btnGammaUp;
		private System.Windows.Forms.Button btnGammaDown;
		private System.Windows.Forms.Label lblGain;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnGainUp;
		private System.Windows.Forms.Button btnGainDown;
		private System.Windows.Forms.Button btnDisconnect;
	}
}