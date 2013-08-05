namespace AAVRec.Scheduling
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
			this.label5 = new System.Windows.Forms.Label();
			this.lblUT = new System.Windows.Forms.Label();
			this.nudSchHours = new System.Windows.Forms.NumericUpDown();
			this.nudSchMinutes = new System.Windows.Forms.NumericUpDown();
			this.nudSchSeconds = new System.Windows.Forms.NumericUpDown();
			this.nudDurSeconds = new System.Windows.Forms.NumericUpDown();
			this.nudDurMinutes = new System.Windows.Forms.NumericUpDown();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudSchHours)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSchMinutes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSchSeconds)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudDurSeconds)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudDurMinutes)).BeginInit();
			this.SuspendLayout();
			// 
			// cbxOperations
			// 
			this.cbxOperations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxOperations.FormattingEnabled = true;
			this.cbxOperations.Items.AddRange(new object[] {
            "Record Video"});
			this.cbxOperations.Location = new System.Drawing.Point(77, 19);
			this.cbxOperations.Name = "cbxOperations";
			this.cbxOperations.Size = new System.Drawing.Size(175, 21);
			this.cbxOperations.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(15, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Operation:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(39, 55);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Start:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(21, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(50, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Duration:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(199, 88);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(24, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "sec";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(123, 147);
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
			this.button2.Location = new System.Drawing.Point(207, 147);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 8;
			this.button2.Text = "Cancel";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.lblUT);
			this.groupBox1.Controls.Add(this.cbxOperations);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(271, 119);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(127, 88);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(23, 13);
			this.label5.TabIndex = 8;
			this.label5.Text = "min";
			// 
			// lblUT
			// 
			this.lblUT.AutoSize = true;
			this.lblUT.Location = new System.Drawing.Point(200, 55);
			this.lblUT.Name = "lblUT";
			this.lblUT.Size = new System.Drawing.Size(22, 13);
			this.lblUT.TabIndex = 7;
			this.lblUT.Text = "UT";
			this.lblUT.Visible = false;
			// 
			// nudSchHours
			// 
			this.nudSchHours.Location = new System.Drawing.Point(89, 62);
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
			// nudSchMinutes
			// 
			this.nudSchMinutes.Location = new System.Drawing.Point(129, 62);
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
			// nudSchSeconds
			// 
			this.nudSchSeconds.Location = new System.Drawing.Point(170, 62);
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
			// nudDurSeconds
			// 
			this.nudDurSeconds.Location = new System.Drawing.Point(170, 95);
			this.nudDurSeconds.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.nudDurSeconds.Name = "nudDurSeconds";
			this.nudDurSeconds.Size = new System.Drawing.Size(39, 20);
			this.nudDurSeconds.TabIndex = 13;
			// 
			// nudDurMinutes
			// 
			this.nudDurMinutes.Location = new System.Drawing.Point(88, 95);
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
			// frmAddScheduleEntry
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(294, 182);
			this.Controls.Add(this.nudDurMinutes);
			this.Controls.Add(this.nudDurSeconds);
			this.Controls.Add(this.nudSchSeconds);
			this.Controls.Add(this.nudSchMinutes);
			this.Controls.Add(this.nudSchHours);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmAddScheduleEntry";
			this.Text = "Add Schedule Entry";
			this.Load += new System.EventHandler(this.frmAddScheduleEntry_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudSchHours)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSchMinutes)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSchSeconds)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudDurSeconds)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudDurMinutes)).EndInit();
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
    }
}