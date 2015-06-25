namespace OccuRec.Config.Panels
{
	partial class ucSpectroscopy
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
			this.label6 = new System.Windows.Forms.Label();
			this.nudFocusingBand = new System.Windows.Forms.NumericUpDown();
			this.cbxFocusingBand = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.nudBlurFWHM = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.nudFrameStackSize = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.nudDispersion = new System.Windows.Forms.NumericUpDown();
			this.cbxUseSpectroscopyAids = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.nudFocusingBand)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudBlurFWHM)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudFrameStackSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudDispersion)).BeginInit();
			this.SuspendLayout();
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(299, 149);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(14, 13);
			this.label6.TabIndex = 11;
			this.label6.Text = "A";
			// 
			// nudFocusingBand
			// 
			this.nudFocusingBand.Enabled = false;
			this.nudFocusingBand.Location = new System.Drawing.Point(239, 144);
			this.nudFocusingBand.Maximum = new decimal(new int[] {
            9000,
            0,
            0,
            0});
			this.nudFocusingBand.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.nudFocusingBand.Name = "nudFocusingBand";
			this.nudFocusingBand.Size = new System.Drawing.Size(57, 20);
			this.nudFocusingBand.TabIndex = 10;
			this.nudFocusingBand.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			// 
			// cbxFocusingBand
			// 
			this.cbxFocusingBand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxFocusingBand.FormattingEnabled = true;
			this.cbxFocusingBand.Items.AddRange(new object[] {
            "A O2 (7605)",
            "B O2 (6869)",
            "C Hα (6563)",
            "F Hβ (4861)",
            "G\' Hγ (4340)",
            "Other"});
			this.cbxFocusingBand.Location = new System.Drawing.Point(142, 144);
			this.cbxFocusingBand.Name = "cbxFocusingBand";
			this.cbxFocusingBand.Size = new System.Drawing.Size(91, 21);
			this.cbxFocusingBand.TabIndex = 9;
			this.cbxFocusingBand.SelectedIndexChanged += new System.EventHandler(this.cbxFocusingBand_SelectedIndexChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(43, 147);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(93, 13);
			this.label5.TabIndex = 8;
			this.label5.Text = "Band for Focusing";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(43, 117);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(109, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "Gaussian Blur FWHM";
			// 
			// nudBlurFWHM
			// 
			this.nudBlurFWHM.DecimalPlaces = 1;
			this.nudBlurFWHM.Location = new System.Drawing.Point(158, 113);
			this.nudBlurFWHM.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudBlurFWHM.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudBlurFWHM.Name = "nudBlurFWHM";
			this.nudBlurFWHM.Size = new System.Drawing.Size(44, 20);
			this.nudBlurFWHM.TabIndex = 6;
			this.nudBlurFWHM.Value = new decimal(new int[] {
            20,
            0,
            0,
            65536});
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(43, 86);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(118, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Averaging Frame Stack";
			// 
			// nudFrameStackSize
			// 
			this.nudFrameStackSize.Location = new System.Drawing.Point(166, 83);
			this.nudFrameStackSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudFrameStackSize.Name = "nudFrameStackSize";
			this.nudFrameStackSize.Size = new System.Drawing.Size(44, 20);
			this.nudFrameStackSize.TabIndex = 4;
			this.nudFrameStackSize.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(161, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "A/pix";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(43, 57);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Dispersion";
			// 
			// nudDispersion
			// 
			this.nudDispersion.DecimalPlaces = 2;
			this.nudDispersion.Location = new System.Drawing.Point(105, 55);
			this.nudDispersion.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudDispersion.Name = "nudDispersion";
			this.nudDispersion.Size = new System.Drawing.Size(52, 20);
			this.nudDispersion.TabIndex = 1;
			this.nudDispersion.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// cbxUseSpectroscopyAids
			// 
			this.cbxUseSpectroscopyAids.AutoSize = true;
			this.cbxUseSpectroscopyAids.Location = new System.Drawing.Point(23, 23);
			this.cbxUseSpectroscopyAids.Name = "cbxUseSpectroscopyAids";
			this.cbxUseSpectroscopyAids.Size = new System.Drawing.Size(173, 17);
			this.cbxUseSpectroscopyAids.TabIndex = 0;
			this.cbxUseSpectroscopyAids.Text = "Use Grating Spectroscopy Aids";
			this.cbxUseSpectroscopyAids.UseVisualStyleBackColor = true;
			// 
			// ucSpectroscopy
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label6);
			this.Controls.Add(this.nudFocusingBand);
			this.Controls.Add(this.cbxFocusingBand);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.nudBlurFWHM);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.nudFrameStackSize);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.nudDispersion);
			this.Controls.Add(this.cbxUseSpectroscopyAids);
			this.Name = "ucSpectroscopy";
			this.Size = new System.Drawing.Size(493, 289);
			((System.ComponentModel.ISupportInitialize)(this.nudFocusingBand)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudBlurFWHM)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudFrameStackSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudDispersion)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox cbxUseSpectroscopyAids;
		private System.Windows.Forms.NumericUpDown nudDispersion;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown nudFrameStackSize;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown nudBlurFWHM;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cbxFocusingBand;
		private System.Windows.Forms.NumericUpDown nudFocusingBand;
		private System.Windows.Forms.Label label6;


	}
}
