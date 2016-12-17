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
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.rbIntegrationBin = new System.Windows.Forms.RadioButton();
            this.cbxAavVersion = new System.Windows.Forms.ComboBox();
            this.nudLatitude = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.nudLongitude = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxTelescopeInfo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxObserverInfo = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cbxFrameProcessingMode = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbIntegrationAverage = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbxForceNewFrameOnLockedRate = new System.Windows.Forms.CheckBox();
            this.cbForceIntegrationRateRestrictions = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.nudCalibrIntegrRate = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.nudGammaDiff = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nudSignDiffRatio = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nudMinSignDiff = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudLatitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLongitude)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCalibrIntegrRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGammaDiff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSignDiffRatio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinSignDiff)).BeginInit();
            this.SuspendLayout();
            // 
            // rbIntegrationBin
            // 
            this.rbIntegrationBin.AutoSize = true;
            this.rbIntegrationBin.Location = new System.Drawing.Point(14, 19);
            this.rbIntegrationBin.Name = "rbIntegrationBin";
            this.rbIntegrationBin.Size = new System.Drawing.Size(95, 17);
            this.rbIntegrationBin.TabIndex = 29;
            this.rbIntegrationBin.Text = "Binning (16 bit)";
            this.toolTip1.SetToolTip(this.rbIntegrationBin, "x256 Max Integration");
            this.rbIntegrationBin.UseVisualStyleBackColor = true;
            // 
            // cbxAavVersion
            // 
            this.cbxAavVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxAavVersion.FormattingEnabled = true;
            this.cbxAavVersion.Items.AddRange(new object[] {
            "AAV2 (AdvLib)",
            "AAV1"});
            this.cbxAavVersion.Location = new System.Drawing.Point(106, 15);
            this.cbxAavVersion.Name = "cbxAavVersion";
            this.cbxAavVersion.Size = new System.Drawing.Size(123, 21);
            this.cbxAavVersion.TabIndex = 40;
            // 
            // nudLatitude
            // 
            this.nudLatitude.DecimalPlaces = 4;
            this.nudLatitude.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudLatitude.Location = new System.Drawing.Point(249, 305);
            this.nudLatitude.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.nudLatitude.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.nudLatitude.Name = "nudLatitude";
            this.nudLatitude.Size = new System.Drawing.Size(75, 20);
            this.nudLatitude.TabIndex = 39;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(196, 308);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 13);
            this.label6.TabIndex = 38;
            this.label6.Text = "Latitude:";
            // 
            // nudLongitude
            // 
            this.nudLongitude.DecimalPlaces = 4;
            this.nudLongitude.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudLongitude.Location = new System.Drawing.Point(118, 305);
            this.nudLongitude.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.nudLongitude.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.nudLongitude.Name = "nudLongitude";
            this.nudLongitude.Size = new System.Drawing.Size(75, 20);
            this.nudLongitude.TabIndex = 37;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 308);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 13);
            this.label5.TabIndex = 36;
            this.label5.Text = "Location Longitude:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 279);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "Telescope Info:";
            // 
            // tbxTelescopeInfo
            // 
            this.tbxTelescopeInfo.Location = new System.Drawing.Point(99, 276);
            this.tbxTelescopeInfo.Name = "tbxTelescopeInfo";
            this.tbxTelescopeInfo.Size = new System.Drawing.Size(207, 20);
            this.tbxTelescopeInfo.TabIndex = 34;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 253);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "Observer Name:";
            // 
            // tbxObserverInfo
            // 
            this.tbxObserverInfo.Location = new System.Drawing.Point(99, 250);
            this.tbxObserverInfo.Name = "tbxObserverInfo";
            this.tbxObserverInfo.Size = new System.Drawing.Size(207, 20);
            this.tbxObserverInfo.TabIndex = 32;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.cbxAavVersion);
            this.groupBox4.Controls.Add(this.cbxFrameProcessingMode);
            this.groupBox4.Location = new System.Drawing.Point(130, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(236, 65);
            this.groupBox4.TabIndex = 31;
            this.groupBox4.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(31, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 13);
            this.label8.TabIndex = 42;
            this.label8.Text = "AAV Version:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(7, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 13);
            this.label7.TabIndex = 41;
            this.label7.Text = "Frame Processing:";
            // 
            // cbxFrameProcessingMode
            // 
            this.cbxFrameProcessingMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxFrameProcessingMode.FormattingEnabled = true;
            this.cbxFrameProcessingMode.Items.AddRange(new object[] {
            "Buffered",
            "Synchronous"});
            this.cbxFrameProcessingMode.Location = new System.Drawing.Point(106, 38);
            this.cbxFrameProcessingMode.Name = "cbxFrameProcessingMode";
            this.cbxFrameProcessingMode.Size = new System.Drawing.Size(123, 21);
            this.cbxFrameProcessingMode.TabIndex = 22;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbIntegrationAverage);
            this.groupBox3.Controls.Add(this.rbIntegrationBin);
            this.groupBox3.Location = new System.Drawing.Point(4, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(120, 66);
            this.groupBox3.TabIndex = 30;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Pixel Integration";
            // 
            // rbIntegrationAverage
            // 
            this.rbIntegrationAverage.AutoSize = true;
            this.rbIntegrationAverage.Checked = true;
            this.rbIntegrationAverage.Location = new System.Drawing.Point(14, 43);
            this.rbIntegrationAverage.Name = "rbIntegrationAverage";
            this.rbIntegrationAverage.Size = new System.Drawing.Size(102, 17);
            this.rbIntegrationAverage.TabIndex = 28;
            this.rbIntegrationAverage.TabStop = true;
            this.rbIntegrationAverage.Text = "Averaging (8 bit)";
            this.rbIntegrationAverage.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbxForceNewFrameOnLockedRate);
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
            this.groupBox1.Location = new System.Drawing.Point(4, 76);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(362, 168);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Integration Detection";
            // 
            // cbxForceNewFrameOnLockedRate
            // 
            this.cbxForceNewFrameOnLockedRate.AutoSize = true;
            this.cbxForceNewFrameOnLockedRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbxForceNewFrameOnLockedRate.Location = new System.Drawing.Point(15, 145);
            this.cbxForceNewFrameOnLockedRate.Name = "cbxForceNewFrameOnLockedRate";
            this.cbxForceNewFrameOnLockedRate.Size = new System.Drawing.Size(264, 17);
            this.cbxForceNewFrameOnLockedRate.TabIndex = 17;
            this.cbxForceNewFrameOnLockedRate.Text = "Force New Frame On Locked Auto-Detected Rate";
            this.cbxForceNewFrameOnLockedRate.UseVisualStyleBackColor = true;
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
            // ucAAV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.nudLatitude);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.nudLongitude);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbxTelescopeInfo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbxObserverInfo);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "ucAAV";
            this.Size = new System.Drawing.Size(383, 338);
            ((System.ComponentModel.ISupportInitialize)(this.nudLatitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLongitude)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCalibrIntegrRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGammaDiff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSignDiffRatio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinSignDiff)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

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
		private System.Windows.Forms.RadioButton rbIntegrationAverage;
		private System.Windows.Forms.RadioButton rbIntegrationBin;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbxTelescopeInfo;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbxObserverInfo;
        private System.Windows.Forms.CheckBox cbxForceNewFrameOnLockedRate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudLongitude;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudLatitude;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox cbxAavVersion;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
	}
}
