namespace OccuRec.Config.Panels
{
    partial class ucTelescope
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
            this.label1 = new System.Windows.Forms.Label();
            this.nudTelescopePingRate = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.gbxPulseGuiding = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.nudPulseDuration = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nudPulseGuideFast = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.nudPulseSlowRate = new System.Windows.Forms.NumericUpDown();
            this.nudPulseSlowestRate = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.cbxPulseRatesTied = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudTelescopePingRate)).BeginInit();
            this.gbxPulseGuiding.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPulseDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPulseGuideFast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPulseSlowRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPulseSlowestRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(219, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "sec";
            // 
            // nudTelescopePingRate
            // 
            this.nudTelescopePingRate.Location = new System.Drawing.Point(161, 14);
            this.nudTelescopePingRate.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudTelescopePingRate.Name = "nudTelescopePingRate";
            this.nudTelescopePingRate.Size = new System.Drawing.Size(52, 20);
            this.nudTelescopePingRate.TabIndex = 6;
            this.nudTelescopePingRate.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Position Check Frequency";
            // 
            // gbxPulseGuiding
            // 
            this.gbxPulseGuiding.Controls.Add(this.label15);
            this.gbxPulseGuiding.Controls.Add(this.label14);
            this.gbxPulseGuiding.Controls.Add(this.numericUpDown1);
            this.gbxPulseGuiding.Controls.Add(this.cbxPulseRatesTied);
            this.gbxPulseGuiding.Controls.Add(this.label13);
            this.gbxPulseGuiding.Controls.Add(this.label12);
            this.gbxPulseGuiding.Controls.Add(this.label11);
            this.gbxPulseGuiding.Controls.Add(this.label10);
            this.gbxPulseGuiding.Controls.Add(this.label9);
            this.gbxPulseGuiding.Controls.Add(this.label8);
            this.gbxPulseGuiding.Controls.Add(this.nudPulseSlowestRate);
            this.gbxPulseGuiding.Controls.Add(this.label7);
            this.gbxPulseGuiding.Controls.Add(this.nudPulseSlowRate);
            this.gbxPulseGuiding.Controls.Add(this.label6);
            this.gbxPulseGuiding.Controls.Add(this.nudPulseGuideFast);
            this.gbxPulseGuiding.Controls.Add(this.label5);
            this.gbxPulseGuiding.Controls.Add(this.label3);
            this.gbxPulseGuiding.Controls.Add(this.nudPulseDuration);
            this.gbxPulseGuiding.Controls.Add(this.label4);
            this.gbxPulseGuiding.Location = new System.Drawing.Point(18, 52);
            this.gbxPulseGuiding.Name = "gbxPulseGuiding";
            this.gbxPulseGuiding.Size = new System.Drawing.Size(349, 212);
            this.gbxPulseGuiding.TabIndex = 8;
            this.gbxPulseGuiding.TabStop = false;
            this.gbxPulseGuiding.Text = "Pulse Guiding";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(172, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "ms";
            // 
            // nudPulseDuration
            // 
            this.nudPulseDuration.Location = new System.Drawing.Point(114, 113);
            this.nudPulseDuration.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudPulseDuration.Name = "nudPulseDuration";
            this.nudPulseDuration.Size = new System.Drawing.Size(52, 20);
            this.nudPulseDuration.TabIndex = 9;
            this.nudPulseDuration.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Pulse Duration";
            // 
            // nudPulseGuideFast
            // 
            this.nudPulseGuideFast.DecimalPlaces = 1;
            this.nudPulseGuideFast.Location = new System.Drawing.Point(114, 33);
            this.nudPulseGuideFast.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudPulseGuideFast.Name = "nudPulseGuideFast";
            this.nudPulseGuideFast.Size = new System.Drawing.Size(52, 20);
            this.nudPulseGuideFast.TabIndex = 12;
            this.nudPulseGuideFast.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Fast Rate";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Slow Rate";
            // 
            // nudPulseSlowRate
            // 
            this.nudPulseSlowRate.DecimalPlaces = 1;
            this.nudPulseSlowRate.Location = new System.Drawing.Point(114, 59);
            this.nudPulseSlowRate.Name = "nudPulseSlowRate";
            this.nudPulseSlowRate.Size = new System.Drawing.Size(52, 20);
            this.nudPulseSlowRate.TabIndex = 14;
            this.nudPulseSlowRate.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // nudPulseSlowestRate
            // 
            this.nudPulseSlowestRate.DecimalPlaces = 1;
            this.nudPulseSlowestRate.Location = new System.Drawing.Point(114, 85);
            this.nudPulseSlowestRate.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudPulseSlowestRate.Name = "nudPulseSlowestRate";
            this.nudPulseSlowestRate.Size = new System.Drawing.Size(52, 20);
            this.nudPulseSlowestRate.TabIndex = 16;
            this.nudPulseSlowestRate.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 89);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Slowest Rate";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(101, 63);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(12, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "x";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(101, 36);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(12, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "x";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(101, 87);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(12, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "x";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(172, 89);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(60, 13);
            this.label11.TabIndex = 20;
            this.label11.Text = "default rate";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(172, 63);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(60, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "default rate";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(172, 37);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(60, 13);
            this.label13.TabIndex = 22;
            this.label13.Text = "default rate";
            // 
            // cbxPulseRatesTied
            // 
            this.cbxPulseRatesTied.AutoSize = true;
            this.cbxPulseRatesTied.Enabled = false;
            this.cbxPulseRatesTied.Location = new System.Drawing.Point(20, 154);
            this.cbxPulseRatesTied.Name = "cbxPulseRatesTied";
            this.cbxPulseRatesTied.Size = new System.Drawing.Size(229, 17);
            this.cbxPulseRatesTied.TabIndex = 23;
            this.cbxPulseRatesTied.Text = "RA and DEC Guide Rates are tied together";
            this.cbxPulseRatesTied.UseVisualStyleBackColor = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(17, 183);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(87, 13);
            this.label14.TabIndex = 24;
            this.label14.Text = "Max Guide Rate:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(172, 183);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(34, 13);
            this.label15.TabIndex = 12;
            this.label15.Text = "\"/sec";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(114, 181);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(52, 20);
            this.numericUpDown1.TabIndex = 11;
            this.numericUpDown1.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // ucTelescope
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxPulseGuiding);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudTelescopePingRate);
            this.Controls.Add(this.label2);
            this.Name = "ucTelescope";
            this.Size = new System.Drawing.Size(411, 305);
            ((System.ComponentModel.ISupportInitialize)(this.nudTelescopePingRate)).EndInit();
            this.gbxPulseGuiding.ResumeLayout(false);
            this.gbxPulseGuiding.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPulseDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPulseGuideFast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPulseSlowRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPulseSlowestRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudTelescopePingRate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox gbxPulseGuiding;
        private System.Windows.Forms.NumericUpDown nudPulseSlowestRate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nudPulseSlowRate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudPulseGuideFast;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudPulseDuration;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox cbxPulseRatesTied;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
    }
}
