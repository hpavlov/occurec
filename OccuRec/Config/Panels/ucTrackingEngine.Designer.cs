namespace OccuRec.Config.Panels
{
	partial class ucTrackingEngine
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
			this.nudGuidingStarDetectionCertainty = new System.Windows.Forms.NumericUpDown();
			this.label14 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.nudMinCertFixedObject = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.nudMinCertGuidingStar = new System.Windows.Forms.NumericUpDown();
			this.cbxTrackingFrequency = new System.Windows.Forms.ComboBox();
			this.label9 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.nudDetectionCertainty)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxElongation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxFWHM)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinFWHM)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudGuidingStarDetectionCertainty)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinCertFixedObject)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinCertGuidingStar)).BeginInit();
			this.SuspendLayout();
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(237, 138);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(144, 13);
			this.label8.TabIndex = 72;
			this.label8.Text = "Minimum Detection Certainty:";
			// 
			// nudDetectionCertainty
			// 
			this.nudDetectionCertainty.DecimalPlaces = 1;
			this.nudDetectionCertainty.Location = new System.Drawing.Point(387, 136);
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
			this.nudDetectionCertainty.TabIndex = 73;
			this.nudDetectionCertainty.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(230, 235);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(15, 13);
			this.label7.TabIndex = 71;
			this.label7.Text = "%";
			// 
			// cbxTestPSFElongation
			// 
			this.cbxTestPSFElongation.Location = new System.Drawing.Point(26, 211);
			this.cbxTestPSFElongation.Name = "cbxTestPSFElongation";
			this.cbxTestPSFElongation.Size = new System.Drawing.Size(187, 19);
			this.cbxTestPSFElongation.TabIndex = 70;
			this.cbxTestPSFElongation.Text = "Test PSF elongation";
			this.cbxTestPSFElongation.CheckedChanged += new System.EventHandler(this.cbxTestPSFElongation_CheckedChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(84, 235);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(83, 13);
			this.label6.TabIndex = 68;
			this.label6.Text = "Max Elongation:";
			// 
			// nudMaxElongation
			// 
			this.nudMaxElongation.Location = new System.Drawing.Point(177, 232);
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
			this.nudMaxElongation.TabIndex = 69;
			this.nudMaxElongation.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(23, 169);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(91, 13);
			this.label3.TabIndex = 66;
			this.label3.Text = "Maximum FWHM:";
			// 
			// nudMaxFWHM
			// 
			this.nudMaxFWHM.DecimalPlaces = 1;
			this.nudMaxFWHM.Location = new System.Drawing.Point(130, 167);
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
			this.nudMaxFWHM.TabIndex = 67;
			this.nudMaxFWHM.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(23, 138);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 13);
			this.label1.TabIndex = 64;
			this.label1.Text = "Minimum FWHM:";
			// 
			// nudMinFWHM
			// 
			this.nudMinFWHM.DecimalPlaces = 1;
			this.nudMinFWHM.Location = new System.Drawing.Point(130, 136);
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
			this.nudMinFWHM.TabIndex = 65;
			this.nudMinFWHM.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(225, 167);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(156, 13);
			this.label2.TabIndex = 74;
			this.label2.Text = "Minimum Guiding Star Certainty:";
			// 
			// nudGuidingStarDetectionCertainty
			// 
			this.nudGuidingStarDetectionCertainty.DecimalPlaces = 1;
			this.nudGuidingStarDetectionCertainty.Location = new System.Drawing.Point(387, 164);
			this.nudGuidingStarDetectionCertainty.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.nudGuidingStarDetectionCertainty.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.nudGuidingStarDetectionCertainty.Name = "nudGuidingStarDetectionCertainty";
			this.nudGuidingStarDetectionCertainty.Size = new System.Drawing.Size(51, 20);
			this.nudGuidingStarDetectionCertainty.TabIndex = 75;
			this.nudGuidingStarDetectionCertainty.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(254, 90);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(56, 13);
			this.label14.TabIndex = 85;
			this.label14.Text = "and below";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(23, 90);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(167, 13);
			this.label5.TabIndex = 83;
			this.label5.Text = "Force a Fixed Object for Certainty:";
			// 
			// nudMinCertFixedObject
			// 
			this.nudMinCertFixedObject.DecimalPlaces = 1;
			this.nudMinCertFixedObject.Location = new System.Drawing.Point(200, 88);
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
			this.nudMinCertFixedObject.TabIndex = 84;
			this.nudMinCertFixedObject.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(23, 58);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(171, 13);
			this.label4.TabIndex = 81;
			this.label4.Text = "Minimum Certainty for Guiding Star:";
			// 
			// nudMinCertGuidingStar
			// 
			this.nudMinCertGuidingStar.DecimalPlaces = 1;
			this.nudMinCertGuidingStar.Location = new System.Drawing.Point(200, 56);
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
			this.nudMinCertGuidingStar.TabIndex = 82;
			this.nudMinCertGuidingStar.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
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
			this.cbxTrackingFrequency.Location = new System.Drawing.Point(96, 20);
			this.cbxTrackingFrequency.Name = "cbxTrackingFrequency";
			this.cbxTrackingFrequency.Size = new System.Drawing.Size(96, 21);
			this.cbxTrackingFrequency.TabIndex = 80;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(23, 23);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(67, 13);
			this.label9.TabIndex = 79;
			this.label9.Text = "Track every ";
			// 
			// ucTrackingEngine
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label14);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.nudMinCertFixedObject);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.nudMinCertGuidingStar);
			this.Controls.Add(this.cbxTrackingFrequency);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.nudGuidingStarDetectionCertainty);
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
			this.Name = "ucTrackingEngine";
			this.Size = new System.Drawing.Size(498, 459);
			((System.ComponentModel.ISupportInitialize)(this.nudDetectionCertainty)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxElongation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxFWHM)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinFWHM)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudGuidingStarDetectionCertainty)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinCertFixedObject)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinCertGuidingStar)).EndInit();
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
		private System.Windows.Forms.NumericUpDown nudGuidingStarDetectionCertainty;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown nudMinCertFixedObject;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown nudMinCertGuidingStar;
		private System.Windows.Forms.ComboBox cbxTrackingFrequency;
		private System.Windows.Forms.Label label9;
	}
}
