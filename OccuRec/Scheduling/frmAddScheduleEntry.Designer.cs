namespace OccuRec.Scheduling
{
    partial class frmAddScheduleEntry
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
			this.cbxOperations = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cbxAutoPulseGuiding = new System.Windows.Forms.CheckBox();
			this.cbxAutoFocusing = new System.Windows.Forms.CheckBox();
			this.tcEntryType = new System.Windows.Forms.TabControl();
			this.tabMidTimeWings = new System.Windows.Forms.TabPage();
			this.label6 = new System.Windows.Forms.Label();
			this.nudMidMins = new System.Windows.Forms.NumericUpDown();
			this.nudWingsMinutes = new System.Windows.Forms.NumericUpDown();
			this.nudWingsSeconds = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.nudMidHours = new System.Windows.Forms.NumericUpDown();
			this.lblUT2 = new System.Windows.Forms.Label();
			this.nudMidSecs = new System.Windows.Forms.NumericUpDown();
			this.label10 = new System.Windows.Forms.Label();
			this.tabStartDuration = new System.Windows.Forms.TabPage();
			this.label5 = new System.Windows.Forms.Label();
			this.nudSchMinutes = new System.Windows.Forms.NumericUpDown();
			this.nudDurMinutes = new System.Windows.Forms.NumericUpDown();
			this.nudDurSeconds = new System.Windows.Forms.NumericUpDown();
			this.nudSchHours = new System.Windows.Forms.NumericUpDown();
			this.lblUT = new System.Windows.Forms.Label();
			this.nudSchSeconds = new System.Windows.Forms.NumericUpDown();
			this.groupBox1.SuspendLayout();
			this.tcEntryType.SuspendLayout();
			this.tabMidTimeWings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMidMins)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudWingsMinutes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudWingsSeconds)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMidHours)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMidSecs)).BeginInit();
			this.tabStartDuration.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudSchMinutes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudDurMinutes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudDurSeconds)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSchHours)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSchSeconds)).BeginInit();
			this.SuspendLayout();
			// 
			// cbxOperations
			// 
			this.cbxOperations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxOperations.FormattingEnabled = true;
			this.cbxOperations.Items.AddRange(new object[] {
            "Record Video"});
			this.cbxOperations.Location = new System.Drawing.Point(85, 19);
			this.cbxOperations.Name = "cbxOperations";
			this.cbxOperations.Size = new System.Drawing.Size(175, 21);
			this.cbxOperations.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(23, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Operation:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(37, 25);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Start:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(19, 60);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(50, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Duration:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(197, 60);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(24, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "sec";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(171, 245);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 7;
			this.button1.Text = "Schedule";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(255, 245);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 8;
			this.button2.Text = "Cancel";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cbxAutoPulseGuiding);
			this.groupBox1.Controls.Add(this.cbxAutoFocusing);
			this.groupBox1.Controls.Add(this.tcEntryType);
			this.groupBox1.Controls.Add(this.cbxOperations);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(318, 215);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			// 
			// cbxAutoPulseGuiding
			// 
			this.cbxAutoPulseGuiding.AutoSize = true;
			this.cbxAutoPulseGuiding.Location = new System.Drawing.Point(26, 184);
			this.cbxAutoPulseGuiding.Name = "cbxAutoPulseGuiding";
			this.cbxAutoPulseGuiding.Size = new System.Drawing.Size(116, 17);
			this.cbxAutoPulseGuiding.TabIndex = 11;
			this.cbxAutoPulseGuiding.Text = "Auto Pulse Guiding";
			this.cbxAutoPulseGuiding.UseVisualStyleBackColor = true;
			// 
			// cbxAutoFocusing
			// 
			this.cbxAutoFocusing.AutoSize = true;
			this.cbxAutoFocusing.Location = new System.Drawing.Point(181, 185);
			this.cbxAutoFocusing.Name = "cbxAutoFocusing";
			this.cbxAutoFocusing.Size = new System.Drawing.Size(94, 17);
			this.cbxAutoFocusing.TabIndex = 10;
			this.cbxAutoFocusing.Text = "Auto Focusing";
			this.cbxAutoFocusing.UseVisualStyleBackColor = true;
			// 
			// tcEntryType
			// 
			this.tcEntryType.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.tcEntryType.Controls.Add(this.tabMidTimeWings);
			this.tcEntryType.Controls.Add(this.tabStartDuration);
			this.tcEntryType.Location = new System.Drawing.Point(20, 54);
			this.tcEntryType.Name = "tcEntryType";
			this.tcEntryType.SelectedIndex = 0;
			this.tcEntryType.Size = new System.Drawing.Size(265, 122);
			this.tcEntryType.TabIndex = 9;
			// 
			// tabMidTimeWings
			// 
			this.tabMidTimeWings.Controls.Add(this.label6);
			this.tabMidTimeWings.Controls.Add(this.nudMidMins);
			this.tabMidTimeWings.Controls.Add(this.nudWingsMinutes);
			this.tabMidTimeWings.Controls.Add(this.nudWingsSeconds);
			this.tabMidTimeWings.Controls.Add(this.label7);
			this.tabMidTimeWings.Controls.Add(this.label8);
			this.tabMidTimeWings.Controls.Add(this.nudMidHours);
			this.tabMidTimeWings.Controls.Add(this.lblUT2);
			this.tabMidTimeWings.Controls.Add(this.nudMidSecs);
			this.tabMidTimeWings.Controls.Add(this.label10);
			this.tabMidTimeWings.Location = new System.Drawing.Point(4, 25);
			this.tabMidTimeWings.Name = "tabMidTimeWings";
			this.tabMidTimeWings.Padding = new System.Windows.Forms.Padding(3);
			this.tabMidTimeWings.Size = new System.Drawing.Size(257, 93);
			this.tabMidTimeWings.TabIndex = 1;
			this.tabMidTimeWings.Text = "Mid Time + Wings";
			this.tabMidTimeWings.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(125, 60);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(23, 13);
			this.label6.TabIndex = 19;
			this.label6.Text = "min";
			// 
			// nudMidMins
			// 
			this.nudMidMins.Location = new System.Drawing.Point(115, 23);
			this.nudMidMins.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
			this.nudMidMins.Name = "nudMidMins";
			this.nudMidMins.Size = new System.Drawing.Size(39, 20);
			this.nudMidMins.TabIndex = 21;
			this.nudMidMins.Value = new decimal(new int[] {
            59,
            0,
            0,
            0});
			// 
			// nudWingsMinutes
			// 
			this.nudWingsMinutes.Location = new System.Drawing.Point(75, 58);
			this.nudWingsMinutes.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.nudWingsMinutes.Name = "nudWingsMinutes";
			this.nudWingsMinutes.Size = new System.Drawing.Size(48, 20);
			this.nudWingsMinutes.TabIndex = 24;
			this.nudWingsMinutes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// nudWingsSeconds
			// 
			this.nudWingsSeconds.Location = new System.Drawing.Point(157, 58);
			this.nudWingsSeconds.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.nudWingsSeconds.Name = "nudWingsSeconds";
			this.nudWingsSeconds.Size = new System.Drawing.Size(39, 20);
			this.nudWingsSeconds.TabIndex = 23;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(197, 60);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(24, 13);
			this.label7.TabIndex = 17;
			this.label7.Text = "sec";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(29, 60);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(40, 13);
			this.label8.TabIndex = 16;
			this.label8.Text = "Wings:";
			// 
			// nudMidHours
			// 
			this.nudMidHours.Location = new System.Drawing.Point(75, 23);
			this.nudMidHours.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
			this.nudMidHours.Name = "nudMidHours";
			this.nudMidHours.Size = new System.Drawing.Size(39, 20);
			this.nudMidHours.TabIndex = 20;
			this.nudMidHours.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
			// 
			// lblUT2
			// 
			this.lblUT2.AutoSize = true;
			this.lblUT2.Location = new System.Drawing.Point(197, 25);
			this.lblUT2.Name = "lblUT2";
			this.lblUT2.Size = new System.Drawing.Size(22, 13);
			this.lblUT2.TabIndex = 18;
			this.lblUT2.Text = "UT";
			this.lblUT2.Visible = false;
			// 
			// nudMidSecs
			// 
			this.nudMidSecs.Location = new System.Drawing.Point(156, 23);
			this.nudMidSecs.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
			this.nudMidSecs.Name = "nudMidSecs";
			this.nudMidSecs.Size = new System.Drawing.Size(39, 20);
			this.nudMidSecs.TabIndex = 22;
			this.nudMidSecs.Value = new decimal(new int[] {
            59,
            0,
            0,
            0});
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(19, 25);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(53, 13);
			this.label10.TabIndex = 15;
			this.label10.Text = "Mid Time:";
			// 
			// tabStartDuration
			// 
			this.tabStartDuration.Controls.Add(this.label5);
			this.tabStartDuration.Controls.Add(this.nudSchMinutes);
			this.tabStartDuration.Controls.Add(this.nudDurMinutes);
			this.tabStartDuration.Controls.Add(this.nudDurSeconds);
			this.tabStartDuration.Controls.Add(this.label4);
			this.tabStartDuration.Controls.Add(this.label3);
			this.tabStartDuration.Controls.Add(this.nudSchHours);
			this.tabStartDuration.Controls.Add(this.lblUT);
			this.tabStartDuration.Controls.Add(this.nudSchSeconds);
			this.tabStartDuration.Controls.Add(this.label2);
			this.tabStartDuration.Location = new System.Drawing.Point(4, 25);
			this.tabStartDuration.Name = "tabStartDuration";
			this.tabStartDuration.Padding = new System.Windows.Forms.Padding(3);
			this.tabStartDuration.Size = new System.Drawing.Size(257, 93);
			this.tabStartDuration.TabIndex = 0;
			this.tabStartDuration.Text = "Start + Duration";
			this.tabStartDuration.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(125, 60);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(23, 13);
			this.label5.TabIndex = 8;
			this.label5.Text = "min";
			// 
			// nudSchMinutes
			// 
			this.nudSchMinutes.Location = new System.Drawing.Point(115, 23);
			this.nudSchMinutes.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
			this.nudSchMinutes.Name = "nudSchMinutes";
			this.nudSchMinutes.Size = new System.Drawing.Size(39, 20);
			this.nudSchMinutes.TabIndex = 11;
			this.nudSchMinutes.Value = new decimal(new int[] {
            59,
            0,
            0,
            0});
			// 
			// nudDurMinutes
			// 
			this.nudDurMinutes.Location = new System.Drawing.Point(75, 58);
			this.nudDurMinutes.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.nudDurMinutes.Name = "nudDurMinutes";
			this.nudDurMinutes.Size = new System.Drawing.Size(48, 20);
			this.nudDurMinutes.TabIndex = 14;
			this.nudDurMinutes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// nudDurSeconds
			// 
			this.nudDurSeconds.Location = new System.Drawing.Point(157, 58);
			this.nudDurSeconds.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.nudDurSeconds.Name = "nudDurSeconds";
			this.nudDurSeconds.Size = new System.Drawing.Size(39, 20);
			this.nudDurSeconds.TabIndex = 13;
			// 
			// nudSchHours
			// 
			this.nudSchHours.Location = new System.Drawing.Point(75, 23);
			this.nudSchHours.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
			this.nudSchHours.Name = "nudSchHours";
			this.nudSchHours.Size = new System.Drawing.Size(39, 20);
			this.nudSchHours.TabIndex = 10;
			this.nudSchHours.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
			// 
			// lblUT
			// 
			this.lblUT.AutoSize = true;
			this.lblUT.Location = new System.Drawing.Point(197, 25);
			this.lblUT.Name = "lblUT";
			this.lblUT.Size = new System.Drawing.Size(22, 13);
			this.lblUT.TabIndex = 7;
			this.lblUT.Text = "UT";
			this.lblUT.Visible = false;
			// 
			// nudSchSeconds
			// 
			this.nudSchSeconds.Location = new System.Drawing.Point(156, 23);
			this.nudSchSeconds.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
			this.nudSchSeconds.Name = "nudSchSeconds";
			this.nudSchSeconds.Size = new System.Drawing.Size(39, 20);
			this.nudSchSeconds.TabIndex = 12;
			this.nudSchSeconds.Value = new decimal(new int[] {
            59,
            0,
            0,
            0});
			// 
			// frmAddScheduleEntry
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(342, 280);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmAddScheduleEntry";
			this.Text = "Add Schedule Entry";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tcEntryType.ResumeLayout(false);
			this.tabMidTimeWings.ResumeLayout(false);
			this.tabMidTimeWings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMidMins)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudWingsMinutes)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudWingsSeconds)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMidHours)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMidSecs)).EndInit();
			this.tabStartDuration.ResumeLayout(false);
			this.tabStartDuration.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudSchMinutes)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudDurMinutes)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudDurSeconds)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSchHours)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSchSeconds)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxOperations;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblUT;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown nudSchHours;
		private System.Windows.Forms.NumericUpDown nudSchMinutes;
		private System.Windows.Forms.NumericUpDown nudSchSeconds;
		private System.Windows.Forms.NumericUpDown nudDurSeconds;
		private System.Windows.Forms.NumericUpDown nudDurMinutes;
		private System.Windows.Forms.TabControl tcEntryType;
		private System.Windows.Forms.TabPage tabStartDuration;
		private System.Windows.Forms.TabPage tabMidTimeWings;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown nudMidMins;
		private System.Windows.Forms.NumericUpDown nudWingsMinutes;
		private System.Windows.Forms.NumericUpDown nudWingsSeconds;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown nudMidHours;
		private System.Windows.Forms.Label lblUT2;
		private System.Windows.Forms.NumericUpDown nudMidSecs;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox cbxAutoPulseGuiding;
		private System.Windows.Forms.CheckBox cbxAutoFocusing;
    }
}