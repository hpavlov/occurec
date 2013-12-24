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
			((System.ComponentModel.ISupportInitialize)(this.nudDetectionCertainty)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxElongation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxFWHM)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinFWHM)).BeginInit();
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
			this.label7.Location = new System.Drawing.Point(191, 114);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(15, 13);
			this.label7.TabIndex = 61;
			this.label7.Text = "%";
			// 
			// cbxTestPSFElongation
			// 
			this.cbxTestPSFElongation.Location = new System.Drawing.Point(11, 86);
			this.cbxTestPSFElongation.Name = "cbxTestPSFElongation";
			this.cbxTestPSFElongation.Size = new System.Drawing.Size(187, 19);
			this.cbxTestPSFElongation.TabIndex = 60;
			this.cbxTestPSFElongation.Text = "Test PSF elongation";
			this.cbxTestPSFElongation.CheckedChanged += new System.EventHandler(this.cbxTestPSFElongation_CheckedChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(23, 113);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(83, 13);
			this.label6.TabIndex = 58;
			this.label6.Text = "Max Elongation:";
			// 
			// nudMaxElongation
			// 
			this.nudMaxElongation.Location = new System.Drawing.Point(130, 111);
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
			this.nudMaxElongation.Size = new System.Drawing.Size(57, 20);
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
			this.label3.Location = new System.Drawing.Point(23, 43);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(91, 13);
			this.label3.TabIndex = 56;
			this.label3.Text = "Maximum FWHM:";
			// 
			// nudMaxFWHM
			// 
			this.nudMaxFWHM.DecimalPlaces = 1;
			this.nudMaxFWHM.Location = new System.Drawing.Point(130, 41);
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
			// ucTraking
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
			this.Size = new System.Drawing.Size(460, 209);
			((System.ComponentModel.ISupportInitialize)(this.nudDetectionCertainty)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxElongation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxFWHM)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinFWHM)).EndInit();
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
	}
}
