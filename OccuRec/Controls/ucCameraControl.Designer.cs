namespace OccuRec.Controls
{
	partial class ucCameraControl
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
			this.lblGamma = new System.Windows.Forms.Label();
			this.btnGammaUp = new System.Windows.Forms.Button();
			this.btnGammaDown = new System.Windows.Forms.Button();
			this.lblGain = new System.Windows.Forms.Label();
			this.btnGainUp = new System.Windows.Forms.Button();
			this.btnGainDown = new System.Windows.Forms.Button();
			this.lblExposure = new System.Windows.Forms.Label();
			this.btnExposureUp = new System.Windows.Forms.Button();
			this.btnExposureDown = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblGamma
			// 
			this.lblGamma.AutoSize = true;
			this.lblGamma.Location = new System.Drawing.Point(65, 66);
			this.lblGamma.Name = "lblGamma";
			this.lblGamma.Size = new System.Drawing.Size(0, 13);
			this.lblGamma.TabIndex = 23;
			this.lblGamma.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnGammaUp
			// 
			this.btnGammaUp.Location = new System.Drawing.Point(117, 61);
			this.btnGammaUp.Name = "btnGammaUp";
			this.btnGammaUp.Size = new System.Drawing.Size(38, 23);
			this.btnGammaUp.TabIndex = 22;
			this.btnGammaUp.Text = ">";
			this.btnGammaUp.UseVisualStyleBackColor = true;
			this.btnGammaUp.Click += new System.EventHandler(this.btnGammaUp_Click);
			// 
			// btnGammaDown
			// 
			this.btnGammaDown.Location = new System.Drawing.Point(17, 61);
			this.btnGammaDown.Name = "btnGammaDown";
			this.btnGammaDown.Size = new System.Drawing.Size(38, 23);
			this.btnGammaDown.TabIndex = 21;
			this.btnGammaDown.Text = "<";
			this.btnGammaDown.UseVisualStyleBackColor = true;
			this.btnGammaDown.Click += new System.EventHandler(this.btnGammaDown_Click);
			// 
			// lblGain
			// 
			this.lblGain.AutoSize = true;
			this.lblGain.Location = new System.Drawing.Point(65, 36);
			this.lblGain.Name = "lblGain";
			this.lblGain.Size = new System.Drawing.Size(0, 13);
			this.lblGain.TabIndex = 20;
			this.lblGain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnGainUp
			// 
			this.btnGainUp.Location = new System.Drawing.Point(117, 31);
			this.btnGainUp.Name = "btnGainUp";
			this.btnGainUp.Size = new System.Drawing.Size(38, 23);
			this.btnGainUp.TabIndex = 19;
			this.btnGainUp.Text = ">";
			this.btnGainUp.UseVisualStyleBackColor = true;
			this.btnGainUp.Click += new System.EventHandler(this.btnGainUp_Click);
			// 
			// btnGainDown
			// 
			this.btnGainDown.Location = new System.Drawing.Point(17, 31);
			this.btnGainDown.Name = "btnGainDown";
			this.btnGainDown.Size = new System.Drawing.Size(38, 23);
			this.btnGainDown.TabIndex = 18;
			this.btnGainDown.Text = "<";
			this.btnGainDown.UseVisualStyleBackColor = true;
			this.btnGainDown.Click += new System.EventHandler(this.btnGainDown_Click);
			// 
			// lblExposure
			// 
			this.lblExposure.AutoSize = true;
			this.lblExposure.Location = new System.Drawing.Point(65, 7);
			this.lblExposure.Name = "lblExposure";
			this.lblExposure.Size = new System.Drawing.Size(0, 13);
			this.lblExposure.TabIndex = 17;
			this.lblExposure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnExposureUp
			// 
			this.btnExposureUp.Location = new System.Drawing.Point(17, 2);
			this.btnExposureUp.Name = "btnExposureUp";
			this.btnExposureUp.Size = new System.Drawing.Size(38, 23);
			this.btnExposureUp.TabIndex = 16;
			this.btnExposureUp.Text = "<";
			this.btnExposureUp.UseVisualStyleBackColor = true;
			this.btnExposureUp.Click += new System.EventHandler(this.btnExposureUp_Click);
			// 
			// btnExposureDown
			// 
			this.btnExposureDown.Location = new System.Drawing.Point(117, 2);
			this.btnExposureDown.Name = "btnExposureDown";
			this.btnExposureDown.Size = new System.Drawing.Size(38, 23);
			this.btnExposureDown.TabIndex = 15;
			this.btnExposureDown.Text = ">";
			this.btnExposureDown.UseVisualStyleBackColor = true;
			this.btnExposureDown.Click += new System.EventHandler(this.btnExposureDown_Click);
			// 
			// ucCameraControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblGamma);
			this.Controls.Add(this.btnGammaUp);
			this.Controls.Add(this.btnGammaDown);
			this.Controls.Add(this.lblGain);
			this.Controls.Add(this.btnGainUp);
			this.Controls.Add(this.btnGainDown);
			this.Controls.Add(this.lblExposure);
			this.Controls.Add(this.btnExposureUp);
			this.Controls.Add(this.btnExposureDown);
			this.Name = "ucCameraControl";
			this.Size = new System.Drawing.Size(190, 86);
			this.Load += new System.EventHandler(this.ucCameraControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblGamma;
		private System.Windows.Forms.Button btnGammaUp;
		private System.Windows.Forms.Button btnGammaDown;
		private System.Windows.Forms.Label lblGain;
		private System.Windows.Forms.Button btnGainUp;
		private System.Windows.Forms.Button btnGainDown;
		private System.Windows.Forms.Label lblExposure;
		private System.Windows.Forms.Button btnExposureUp;
		private System.Windows.Forms.Button btnExposureDown;
	}
}
