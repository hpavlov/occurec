namespace WindowsClock.Tester
{
	partial class frmTestWindowsClock
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
			this.components = new System.ComponentModel.Container();
			this.btnStartStopTest = new System.Windows.Forms.Button();
			this.nudFrequency = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.label3 = new System.Windows.Forms.Label();
			this.lblAverageDiff = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.tbxNTPServer = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.tbxMeasurements = new System.Windows.Forms.TextBox();
			this.lblAverageDiffOccuRec = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.btnPlotData = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.cbxCOMPort = new System.Windows.Forms.ComboBox();
			this.cbxHTCC = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.nudFrequency)).BeginInit();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnStartStopTest
			// 
			this.btnStartStopTest.Location = new System.Drawing.Point(564, 20);
			this.btnStartStopTest.Name = "btnStartStopTest";
			this.btnStartStopTest.Size = new System.Drawing.Size(75, 23);
			this.btnStartStopTest.TabIndex = 0;
			this.btnStartStopTest.Text = "Run Test";
			this.btnStartStopTest.UseVisualStyleBackColor = true;
			this.btnStartStopTest.Click += new System.EventHandler(this.btnStartStopTest_Click);
			// 
			// nudFrequency
			// 
			this.nudFrequency.Location = new System.Drawing.Point(431, 23);
			this.nudFrequency.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudFrequency.Name = "nudFrequency";
			this.nudFrequency.Size = new System.Drawing.Size(44, 20);
			this.nudFrequency.TabIndex = 1;
			this.nudFrequency.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(368, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(57, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Test every";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(481, 26);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(47, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "seconds";
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyAllToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(114, 26);
			// 
			// copyAllToolStripMenuItem
			// 
			this.copyAllToolStripMenuItem.Name = "copyAllToolStripMenuItem";
			this.copyAllToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
			this.copyAllToolStripMenuItem.Text = "&Copy All";
			this.copyAllToolStripMenuItem.Click += new System.EventHandler(this.copyAllToolStripMenuItem_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 58);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(185, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Average Difference (Windows Clock):";
			// 
			// lblAverageDiff
			// 
			this.lblAverageDiff.AutoSize = true;
			this.lblAverageDiff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblAverageDiff.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
			this.lblAverageDiff.Location = new System.Drawing.Point(200, 58);
			this.lblAverageDiff.Name = "lblAverageDiff";
			this.lblAverageDiff.Size = new System.Drawing.Size(0, 13);
			this.lblAverageDiff.TabIndex = 6;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 104);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(76, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Measurements";
			// 
			// tbxNTPServer
			// 
			this.tbxNTPServer.Location = new System.Drawing.Point(15, 25);
			this.tbxNTPServer.Name = "tbxNTPServer";
			this.tbxNTPServer.Size = new System.Drawing.Size(155, 20);
			this.tbxNTPServer.TabIndex = 28;
			this.tbxNTPServer.Text = "ntp0.cs.mu.OZ.AU";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 9);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(63, 13);
			this.label5.TabIndex = 27;
			this.label5.Text = "NTP Server";
			// 
			// tbxMeasurements
			// 
			this.tbxMeasurements.BackColor = System.Drawing.SystemColors.Info;
			this.tbxMeasurements.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tbxMeasurements.Location = new System.Drawing.Point(15, 120);
			this.tbxMeasurements.Multiline = true;
			this.tbxMeasurements.Name = "tbxMeasurements";
			this.tbxMeasurements.ReadOnly = true;
			this.tbxMeasurements.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbxMeasurements.Size = new System.Drawing.Size(624, 182);
			this.tbxMeasurements.TabIndex = 29;
			this.tbxMeasurements.Text = "ntp0.cs.mu.OZ.AU";
			// 
			// lblAverageDiffOccuRec
			// 
			this.lblAverageDiffOccuRec.AutoSize = true;
			this.lblAverageDiffOccuRec.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblAverageDiffOccuRec.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
			this.lblAverageDiffOccuRec.Location = new System.Drawing.Point(199, 75);
			this.lblAverageDiffOccuRec.Name = "lblAverageDiffOccuRec";
			this.lblAverageDiffOccuRec.Size = new System.Drawing.Size(0, 13);
			this.lblAverageDiffOccuRec.TabIndex = 31;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(12, 75);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(187, 13);
			this.label7.TabIndex = 30;
			this.label7.Text = "Average Difference (OccuRec Clock):";
			// 
			// btnPlotData
			// 
			this.btnPlotData.Location = new System.Drawing.Point(564, 48);
			this.btnPlotData.Name = "btnPlotData";
			this.btnPlotData.Size = new System.Drawing.Size(75, 23);
			this.btnPlotData.TabIndex = 32;
			this.btnPlotData.Text = "Plot Data";
			this.btnPlotData.UseVisualStyleBackColor = true;
			this.btnPlotData.Click += new System.EventHandler(this.btnPlotData_Click);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(221, 7);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(103, 13);
			this.label6.TabIndex = 33;
			this.label6.Text = "HTCCv38 COM Port";
			// 
			// cbxCOMPort
			// 
			this.cbxCOMPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxCOMPort.Enabled = false;
			this.cbxCOMPort.FormattingEnabled = true;
			this.cbxCOMPort.Location = new System.Drawing.Point(224, 22);
			this.cbxCOMPort.Name = "cbxCOMPort";
			this.cbxCOMPort.Size = new System.Drawing.Size(112, 21);
			this.cbxCOMPort.TabIndex = 34;
			// 
			// cbxHTCC
			// 
			this.cbxHTCC.AutoSize = true;
			this.cbxHTCC.Location = new System.Drawing.Point(203, 26);
			this.cbxHTCC.Name = "cbxHTCC";
			this.cbxHTCC.Size = new System.Drawing.Size(15, 14);
			this.cbxHTCC.TabIndex = 35;
			this.cbxHTCC.UseVisualStyleBackColor = true;
			this.cbxHTCC.CheckedChanged += new System.EventHandler(this.cbxHTCC_CheckedChanged);
			// 
			// frmTestWindowsClock
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(651, 325);
			this.Controls.Add(this.cbxHTCC);
			this.Controls.Add(this.cbxCOMPort);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.btnPlotData);
			this.Controls.Add(this.lblAverageDiffOccuRec);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.tbxMeasurements);
			this.Controls.Add(this.tbxNTPServer);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lblAverageDiff);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.nudFrequency);
			this.Controls.Add(this.btnStartStopTest);
			this.Name = "frmTestWindowsClock";
			this.Text = "Test Windows Clock Accuracy v2";
			((System.ComponentModel.ISupportInitialize)(this.nudFrequency)).EndInit();
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnStartStopTest;
		private System.Windows.Forms.NumericUpDown nudFrequency;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblAverageDiff;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem copyAllToolStripMenuItem;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbxNTPServer;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbxMeasurements;
		private System.Windows.Forms.Label lblAverageDiffOccuRec;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button btnPlotData;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox cbxCOMPort;
		private System.Windows.Forms.CheckBox cbxHTCC;
	}
}