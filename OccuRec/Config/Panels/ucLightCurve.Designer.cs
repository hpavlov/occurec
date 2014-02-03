namespace OccuRec.Config.Panels
{
	partial class ucLightCurve
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
			this.nudInnerAnulusInApertures = new System.Windows.Forms.NumericUpDown();
			this.nudMinimumAnulusPixels = new System.Windows.Forms.NumericUpDown();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.nudApertureInFWHM = new System.Windows.Forms.NumericUpDown();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cbxDisplayGuidingPSF = new System.Windows.Forms.CheckBox();
			this.cbxDisplayTargetPSF = new System.Windows.Forms.CheckBox();
			this.cbxDisplayTargetLightCurve = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.nudInnerAnulusInApertures)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinimumAnulusPixels)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudApertureInFWHM)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// nudInnerAnulusInApertures
			// 
			this.nudInnerAnulusInApertures.DecimalPlaces = 1;
			this.nudInnerAnulusInApertures.Location = new System.Drawing.Point(203, 29);
			this.nudInnerAnulusInApertures.Name = "nudInnerAnulusInApertures";
			this.nudInnerAnulusInApertures.Size = new System.Drawing.Size(47, 20);
			this.nudInnerAnulusInApertures.TabIndex = 85;
			// 
			// nudMinimumAnulusPixels
			// 
			this.nudMinimumAnulusPixels.Location = new System.Drawing.Point(203, 56);
			this.nudMinimumAnulusPixels.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
			this.nudMinimumAnulusPixels.Name = "nudMinimumAnulusPixels";
			this.nudMinimumAnulusPixels.Size = new System.Drawing.Size(47, 20);
			this.nudMinimumAnulusPixels.TabIndex = 84;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(53, 59);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(144, 13);
			this.label11.TabIndex = 83;
			this.label11.Text = "Pixels in Background Anulus:";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(254, 31);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(81, 13);
			this.label12.TabIndex = 82;
			this.label12.Text = "signal apertures";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(19, 31);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(178, 13);
			this.label13.TabIndex = 81;
			this.label13.Text = "Inner Radius of Background Anulus:";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(254, 8);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(41, 13);
			this.label10.TabIndex = 80;
			this.label10.Text = "FWHM";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(129, 8);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(50, 13);
			this.label9.TabIndex = 78;
			this.label9.Text = "Aperture:";
			// 
			// nudApertureInFWHM
			// 
			this.nudApertureInFWHM.DecimalPlaces = 1;
			this.nudApertureInFWHM.Location = new System.Drawing.Point(203, 3);
			this.nudApertureInFWHM.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.nudApertureInFWHM.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
			this.nudApertureInFWHM.Name = "nudApertureInFWHM";
			this.nudApertureInFWHM.Size = new System.Drawing.Size(48, 20);
			this.nudApertureInFWHM.TabIndex = 79;
			this.nudApertureInFWHM.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cbxDisplayGuidingPSF);
			this.groupBox1.Controls.Add(this.cbxDisplayTargetPSF);
			this.groupBox1.Controls.Add(this.cbxDisplayTargetLightCurve);
			this.groupBox1.Location = new System.Drawing.Point(13, 93);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(334, 106);
			this.groupBox1.TabIndex = 86;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "On Screen Display";
			// 
			// cbxDisplayGuidingPSF
			// 
			this.cbxDisplayGuidingPSF.AutoSize = true;
			this.cbxDisplayGuidingPSF.Location = new System.Drawing.Point(22, 74);
			this.cbxDisplayGuidingPSF.Name = "cbxDisplayGuidingPSF";
			this.cbxDisplayGuidingPSF.Size = new System.Drawing.Size(140, 17);
			this.cbxDisplayGuidingPSF.TabIndex = 28;
			this.cbxDisplayGuidingPSF.Text = "Display guiding star PSF";
			this.cbxDisplayGuidingPSF.UseVisualStyleBackColor = true;
			// 
			// cbxDisplayTargetPSF
			// 
			this.cbxDisplayTargetPSF.AutoSize = true;
			this.cbxDisplayTargetPSF.Location = new System.Drawing.Point(22, 51);
			this.cbxDisplayTargetPSF.Name = "cbxDisplayTargetPSF";
			this.cbxDisplayTargetPSF.Size = new System.Drawing.Size(133, 17);
			this.cbxDisplayTargetPSF.TabIndex = 28;
			this.cbxDisplayTargetPSF.Text = "Display target star PSF";
			this.cbxDisplayTargetPSF.UseVisualStyleBackColor = true;
			// 
			// cbxDisplayTargetLightCurve
			// 
			this.cbxDisplayTargetLightCurve.AutoSize = true;
			this.cbxDisplayTargetLightCurve.Location = new System.Drawing.Point(22, 28);
			this.cbxDisplayTargetLightCurve.Name = "cbxDisplayTargetLightCurve";
			this.cbxDisplayTargetLightCurve.Size = new System.Drawing.Size(165, 17);
			this.cbxDisplayTargetLightCurve.TabIndex = 27;
			this.cbxDisplayTargetLightCurve.Text = "Display target star light curve ";
			this.cbxDisplayTargetLightCurve.UseVisualStyleBackColor = true;
			// 
			// ucLightCurve
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.nudInnerAnulusInApertures);
			this.Controls.Add(this.nudMinimumAnulusPixels);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.nudApertureInFWHM);
			this.Name = "ucLightCurve";
			this.Size = new System.Drawing.Size(497, 308);
			((System.ComponentModel.ISupportInitialize)(this.nudInnerAnulusInApertures)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinimumAnulusPixels)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudApertureInFWHM)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown nudInnerAnulusInApertures;
		private System.Windows.Forms.NumericUpDown nudMinimumAnulusPixels;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown nudApertureInFWHM;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox cbxDisplayGuidingPSF;
		private System.Windows.Forms.CheckBox cbxDisplayTargetPSF;
		private System.Windows.Forms.CheckBox cbxDisplayTargetLightCurve;
	}
}
