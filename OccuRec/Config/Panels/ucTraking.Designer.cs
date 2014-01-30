namespace OccuRec.Config.Panels
{
	partial class ucTraking
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
			this.label8 = new System.Windows.Forms.Label();
			this.nudDetectionCertainty = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.cbxTestPSFElongation = new System.Windows.Forms.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.nudMaxElongation = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.nudMaxFWHM = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.nudMinFWHM = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.cbxTrackingFrequency = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.nudMinCertGuidingStar = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.nudMinCertFixedObject = new System.Windows.Forms.NumericUpDown();
			this.label9 = new System.Windows.Forms.Label();
			this.nudApertureInFWHM = new System.Windows.Forms.NumericUpDown();
			this.label10 = new System.Windows.Forms.Label();
			this.nudInnerAnulusInApertures = new System.Windows.Forms.NumericUpDown();
			this.nudMinimumAnulusPixels = new System.Windows.Forms.NumericUpDown();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.nudDetectionCertainty)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxElongation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxFWHM)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinFWHM)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinCertGuidingStar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinCertFixedObject)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudApertureInFWHM)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudInnerAnulusInApertures)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinimumAnulusPixels)).BeginInit();
			this.SuspendLayout();
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(225, 18);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(144, 13);
			this.label8.TabIndex = 62;
			this.label8.Text = "Minimum Detection Certainty:";
			// 
			// nudDetectionCertainty
			// 
			this.nudDetectionCertainty.DecimalPlaces = 1;
			this.nudDetectionCertainty.Location = new System.Drawing.Point(379, 15);
			this.nudDetectionCertainty.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.nudDetectionCertainty.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.nudDetectionCertainty.Name = "nudDetectionCertainty";
			this.nudDetectionCertainty.Size = new System.Drawing.Size(51, 20);
			this.nudDetectionCertainty.TabIndex = 63;
			this.nudDetectionCertainty.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(432, 71);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(15, 13);
			this.label7.TabIndex = 61;
			this.label7.Text = "%";
			// 
			// cbxTestPSFElongation
			// 
			this.cbxTestPSFElongation.Location = new System.Drawing.Point(228, 47);
			this.cbxTestPSFElongation.Name = "cbxTestPSFElongation";
			this.cbxTestPSFElongation.Size = new System.Drawing.Size(187, 19);
			this.cbxTestPSFElongation.TabIndex = 60;
			this.cbxTestPSFElongation.Text = "Test PSF elongation";
			this.cbxTestPSFElongation.CheckedChanged += new System.EventHandler(this.cbxTestPSFElongation_CheckedChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(286, 71);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(83, 13);
			this.label6.TabIndex = 58;
			this.label6.Text = "Max Elongation:";
			// 
			// nudMaxElongation
			// 
			this.nudMaxElongation.Location = new System.Drawing.Point(379, 68);
			this.nudMaxElongation.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
			this.nudMaxElongation.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
			this.nudMaxElongation.Name = "nudMaxElongation";
			this.nudMaxElongation.Size = new System.Drawing.Size(51, 20);
			this.nudMaxElongation.TabIndex = 59;
			this.nudMaxElongation.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(23, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(91, 13);
			this.label3.TabIndex = 56;
			this.label3.Text = "Maximum FWHM:";
			// 
			// nudMaxFWHM
			// 
			this.nudMaxFWHM.DecimalPlaces = 1;
			this.nudMaxFWHM.Location = new System.Drawing.Point(130, 46);
			this.nudMaxFWHM.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
			this.nudMaxFWHM.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.nudMaxFWHM.Name = "nudMaxFWHM";
			this.nudMaxFWHM.Size = new System.Drawing.Size(57, 20);
			this.nudMaxFWHM.TabIndex = 57;
			this.nudMaxFWHM.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(23, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 13);
			this.label1.TabIndex = 54;
			this.label1.Text = "Minimum FWHM:";
			// 
			// nudMinFWHM
			// 
			this.nudMinFWHM.DecimalPlaces = 1;
			this.nudMinFWHM.Location = new System.Drawing.Point(130, 15);
			this.nudMinFWHM.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.nudMinFWHM.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
			this.nudMinFWHM.Name = "nudMinFWHM";
			this.nudMinFWHM.Size = new System.Drawing.Size(57, 20);
			this.nudMinFWHM.TabIndex = 55;
			this.nudMinFWHM.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(18, 123);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 13);
			this.label2.TabIndex = 64;
			this.label2.Text = "Track every ";
			// 
			// cbxTrackingFrequency
			// 
			this.cbxTrackingFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxTrackingFrequency.FormattingEnabled = true;
			this.cbxTrackingFrequency.Items.AddRange(new object[] {
            "frame",
            "2-nd frame",
            "3-rd frame",
            "4-th frame",
            "5-th frame",
            "10-th frame",
            "20-th frame"});
			this.cbxTrackingFrequency.Location = new System.Drawing.Point(91, 120);
			this.cbxTrackingFrequency.Name = "cbxTrackingFrequency";
			this.cbxTrackingFrequency.Size = new System.Drawing.Size(96, 21);
			this.cbxTrackingFrequency.TabIndex = 65;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(202, 123);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(171, 13);
			this.label4.TabIndex = 66;
			this.label4.Text = "Minimum Certainty for Guiding Star:";
			// 
			// nudMinCertGuidingStar
			// 
			this.nudMinCertGuidingStar.DecimalPlaces = 1;
			this.nudMinCertGuidingStar.Location = new System.Drawing.Point(379, 121);
			this.nudMinCertGuidingStar.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.nudMinCertGuidingStar.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.nudMinCertGuidingStar.Name = "nudMinCertGuidingStar";
			this.nudMinCertGuidingStar.Size = new System.Drawing.Size(51, 20);
			this.nudMinCertGuidingStar.TabIndex = 67;
			this.nudMinCertGuidingStar.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(201, 154);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(172, 13);
			this.label5.TabIndex = 68;
			this.label5.Text = "Minimum Certainty for Fixed Object:";
			// 
			// nudMinCertFixedObject
			// 
			this.nudMinCertFixedObject.DecimalPlaces = 1;
			this.nudMinCertFixedObject.Location = new System.Drawing.Point(378, 152);
			this.nudMinCertFixedObject.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.nudMinCertFixedObject.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.nudMinCertFixedObject.Name = "nudMinCertFixedObject";
			this.nudMinCertFixedObject.Size = new System.Drawing.Size(51, 20);
			this.nudMinCertFixedObject.TabIndex = 69;
			this.nudMinCertFixedObject.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(18, 154);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(50, 13);
			this.label9.TabIndex = 70;
			this.label9.Text = "Aperture:";
			// 
			// nudApertureInFWHM
			// 
			this.nudApertureInFWHM.DecimalPlaces = 1;
			this.nudApertureInFWHM.Location = new System.Drawing.Point(91, 149);
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
			this.nudApertureInFWHM.TabIndex = 71;
			this.nudApertureInFWHM.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(143, 154);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(41, 13);
			this.label10.TabIndex = 72;
			this.label10.Text = "FWHM";
			// 
			// nudInnerAnulusInApertures
			// 
			this.nudInnerAnulusInApertures.DecimalPlaces = 1;
			this.nudInnerAnulusInApertures.Location = new System.Drawing.Point(204, 193);
			this.nudInnerAnulusInApertures.Name = "nudInnerAnulusInApertures";
			this.nudInnerAnulusInApertures.Size = new System.Drawing.Size(47, 20);
			this.nudInnerAnulusInApertures.TabIndex = 77;
			// 
			// nudMinimumAnulusPixels
			// 
			this.nudMinimumAnulusPixels.Location = new System.Drawing.Point(204, 225);
			this.nudMinimumAnulusPixels.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
			this.nudMinimumAnulusPixels.Name = "nudMinimumAnulusPixels";
			this.nudMinimumAnulusPixels.Size = new System.Drawing.Size(47, 20);
			this.nudMinimumAnulusPixels.TabIndex = 76;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(54, 228);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(144, 13);
			this.label11.TabIndex = 75;
			this.label11.Text = "Pixels in Background Anulus:";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(260, 196);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(81, 13);
			this.label12.TabIndex = 74;
			this.label12.Text = "signal apertures";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(20, 195);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(178, 13);
			this.label13.TabIndex = 73;
			this.label13.Text = "Inner Radius of Background Anulus:";
			// 
			// ucTraking
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.nudInnerAnulusInApertures);
			this.Controls.Add(this.nudMinimumAnulusPixels);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.nudApertureInFWHM);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.nudMinCertFixedObject);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.nudMinCertGuidingStar);
			this.Controls.Add(this.cbxTrackingFrequency);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.nudDetectionCertainty);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.cbxTestPSFElongation);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.nudMaxElongation);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.nudMaxFWHM);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.nudMinFWHM);
			this.Name = "ucTraking";
			this.Size = new System.Drawing.Size(460, 271);
			((System.ComponentModel.ISupportInitialize)(this.nudDetectionCertainty)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxElongation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxFWHM)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinFWHM)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinCertGuidingStar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinCertFixedObject)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudApertureInFWHM)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudInnerAnulusInApertures)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinimumAnulusPixels)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown nudDetectionCertainty;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox cbxTestPSFElongation;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown nudMaxElongation;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown nudMaxFWHM;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nudMinFWHM;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cbxTrackingFrequency;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown nudMinCertGuidingStar;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown nudMinCertFixedObject;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown nudApertureInFWHM;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.NumericUpDown nudInnerAnulusInApertures;
		private System.Windows.Forms.NumericUpDown nudMinimumAnulusPixels;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
	}
}
