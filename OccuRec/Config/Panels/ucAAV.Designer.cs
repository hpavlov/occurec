namespace OccuRec.Config.Panels
{
	partial class ucAAV
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
			this.cbDebugIntegration = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cbForceIntegrationRateRestrictions = new System.Windows.Forms.CheckBox();
			this.label15 = new System.Windows.Forms.Label();
			this.nudCalibrIntegrRate = new System.Windows.Forms.NumericUpDown();
			this.label14 = new System.Windows.Forms.Label();
			this.nudGammaDiff = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.nudSignDiffRatio = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.nudMinSignDiff = new System.Windows.Forms.NumericUpDown();
			this.cbxFrameProcessingMode = new System.Windows.Forms.ComboBox();
			this.label12 = new System.Windows.Forms.Label();
			this.cbxStatusSectionOnly = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.nudNTPDebugValue1 = new System.Windows.Forms.NumericUpDown();
			this.nudNTPDebugValue2 = new System.Windows.Forms.NumericUpDown();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCalibrIntegrRate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudGammaDiff)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSignDiffRatio)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinSignDiff)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudNTPDebugValue1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudNTPDebugValue2)).BeginInit();
			this.SuspendLayout();
			// 
			// cbDebugIntegration
			// 
			this.cbDebugIntegration.AutoSize = true;
			this.cbDebugIntegration.Location = new System.Drawing.Point(13, 22);
			this.cbDebugIntegration.Name = "cbDebugIntegration";
			this.cbDebugIntegration.Size = new System.Drawing.Size(161, 17);
			this.cbDebugIntegration.TabIndex = 24;
			this.cbDebugIntegration.Text = "Integration Detection Tuning";
			this.cbDebugIntegration.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cbForceIntegrationRateRestrictions);
			this.groupBox1.Controls.Add(this.label15);
			this.groupBox1.Controls.Add(this.nudCalibrIntegrRate);
			this.groupBox1.Controls.Add(this.label14);
			this.groupBox1.Controls.Add(this.nudGammaDiff);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.nudSignDiffRatio);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.nudMinSignDiff);
			this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(362, 152);
			this.groupBox1.TabIndex = 25;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Integration Detection";
			// 
			// cbForceIntegrationRateRestrictions
			// 
			this.cbForceIntegrationRateRestrictions.AutoSize = true;
			this.cbForceIntegrationRateRestrictions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.cbForceIntegrationRateRestrictions.Location = new System.Drawing.Point(15, 125);
			this.cbForceIntegrationRateRestrictions.Name = "cbForceIntegrationRateRestrictions";
			this.cbForceIntegrationRateRestrictions.Size = new System.Drawing.Size(190, 17);
			this.cbForceIntegrationRateRestrictions.TabIndex = 16;
			this.cbForceIntegrationRateRestrictions.Text = "Force Integration Rate Restrictions";
			this.cbForceIntegrationRateRestrictions.UseVisualStyleBackColor = true;
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label15.Location = new System.Drawing.Point(12, 94);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(138, 13);
			this.label15.TabIndex = 14;
			this.label15.Text = "Calibration Integration Rate:";
			// 
			// nudCalibrIntegrRate
			// 
			this.nudCalibrIntegrRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.nudCalibrIntegrRate.Location = new System.Drawing.Point(192, 91);
			this.nudCalibrIntegrRate.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this.nudCalibrIntegrRate.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
			this.nudCalibrIntegrRate.Name = "nudCalibrIntegrRate";
			this.nudCalibrIntegrRate.Size = new System.Drawing.Size(49, 20);
			this.nudCalibrIntegrRate.TabIndex = 15;
			this.nudCalibrIntegrRate.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label14.Location = new System.Drawing.Point(212, 32);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(79, 13);
			this.label14.TabIndex = 12;
			this.label14.Text = "Gamma Factor:";
			// 
			// nudGammaDiff
			// 
			this.nudGammaDiff.DecimalPlaces = 2;
			this.nudGammaDiff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.nudGammaDiff.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.nudGammaDiff.Location = new System.Drawing.Point(296, 28);
			this.nudGammaDiff.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.nudGammaDiff.Name = "nudGammaDiff";
			this.nudGammaDiff.Size = new System.Drawing.Size(49, 20);
			this.nudGammaDiff.TabIndex = 13;
			this.nudGammaDiff.Value = new decimal(new int[] {
            100,
            0,
            0,
            131072});
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(12, 31);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(118, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Minimal Signature Ratio";
			// 
			// nudSignDiffRatio
			// 
			this.nudSignDiffRatio.DecimalPlaces = 1;
			this.nudSignDiffRatio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.nudSignDiffRatio.Location = new System.Drawing.Point(136, 28);
			this.nudSignDiffRatio.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
			this.nudSignDiffRatio.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudSignDiffRatio.Name = "nudSignDiffRatio";
			this.nudSignDiffRatio.Size = new System.Drawing.Size(49, 20);
			this.nudSignDiffRatio.TabIndex = 5;
			this.nudSignDiffRatio.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(11, 63);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(145, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Minimal Signature Difference:";
			// 
			// nudMinSignDiff
			// 
			this.nudMinSignDiff.DecimalPlaces = 2;
			this.nudMinSignDiff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.nudMinSignDiff.Location = new System.Drawing.Point(191, 60);
			this.nudMinSignDiff.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudMinSignDiff.Name = "nudMinSignDiff";
			this.nudMinSignDiff.Size = new System.Drawing.Size(49, 20);
			this.nudMinSignDiff.TabIndex = 7;
			this.nudMinSignDiff.Value = new decimal(new int[] {
            2,
            0,
            0,
            65536});
			// 
			// cbxFrameProcessingMode
			// 
			this.cbxFrameProcessingMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxFrameProcessingMode.FormattingEnabled = true;
			this.cbxFrameProcessingMode.Items.AddRange(new object[] {
            "Buffered",
            "Synchronous"});
			this.cbxFrameProcessingMode.Location = new System.Drawing.Point(3, 183);
			this.cbxFrameProcessingMode.Name = "cbxFrameProcessingMode";
			this.cbxFrameProcessingMode.Size = new System.Drawing.Size(144, 21);
			this.cbxFrameProcessingMode.TabIndex = 22;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(0, 167);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(91, 13);
			this.label12.TabIndex = 23;
			this.label12.Text = "Frame Processing";
			// 
			// cbxStatusSectionOnly
			// 
			this.cbxStatusSectionOnly.AutoSize = true;
			this.cbxStatusSectionOnly.Location = new System.Drawing.Point(13, 51);
			this.cbxStatusSectionOnly.Name = "cbxStatusSectionOnly";
			this.cbxStatusSectionOnly.Size = new System.Drawing.Size(157, 17);
			this.cbxStatusSectionOnly.TabIndex = 26;
			this.cbxStatusSectionOnly.Text = "Record Status Section Only";
			this.cbxStatusSectionOnly.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.nudNTPDebugValue2);
			this.groupBox2.Controls.Add(this.nudNTPDebugValue1);
			this.groupBox2.Controls.Add(this.cbDebugIntegration);
			this.groupBox2.Controls.Add(this.cbxStatusSectionOnly);
			this.groupBox2.Location = new System.Drawing.Point(4, 211);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(361, 100);
			this.groupBox2.TabIndex = 27;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Debug";
			// 
			// nudNTPDebugValue1
			// 
			this.nudNTPDebugValue1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.nudNTPDebugValue1.Location = new System.Drawing.Point(176, 50);
			this.nudNTPDebugValue1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudNTPDebugValue1.Name = "nudNTPDebugValue1";
			this.nudNTPDebugValue1.Size = new System.Drawing.Size(43, 20);
			this.nudNTPDebugValue1.TabIndex = 27;
			this.nudNTPDebugValue1.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// nudNTPDebugValue2
			// 
			this.nudNTPDebugValue2.DecimalPlaces = 1;
			this.nudNTPDebugValue2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.nudNTPDebugValue2.Location = new System.Drawing.Point(225, 50);
			this.nudNTPDebugValue2.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
			this.nudNTPDebugValue2.Name = "nudNTPDebugValue2";
			this.nudNTPDebugValue2.Size = new System.Drawing.Size(49, 20);
			this.nudNTPDebugValue2.TabIndex = 28;
			this.nudNTPDebugValue2.Value = new decimal(new int[] {
            50,
            0,
            0,
            65536});
			// 
			// ucAAV
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.cbxFrameProcessingMode);
			this.Controls.Add(this.label12);
			this.Name = "ucAAV";
			this.Size = new System.Drawing.Size(401, 327);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCalibrIntegrRate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudGammaDiff)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSignDiffRatio)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinSignDiff)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudNTPDebugValue1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudNTPDebugValue2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox cbDebugIntegration;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox cbForceIntegrationRateRestrictions;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.NumericUpDown nudCalibrIntegrRate;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.NumericUpDown nudGammaDiff;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown nudSignDiffRatio;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown nudMinSignDiff;
		private System.Windows.Forms.ComboBox cbxFrameProcessingMode;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.CheckBox cbxStatusSectionOnly;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.NumericUpDown nudNTPDebugValue2;
		private System.Windows.Forms.NumericUpDown nudNTPDebugValue1;
	}
}
