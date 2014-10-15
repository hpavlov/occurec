/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

namespace WindowsClock.Tester
{
	partial class frmTestWindowsClock
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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
			this.tbxNTPServer2 = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.tbxNTPServer4 = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.tbxNTPServer3 = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.nudFrequencyNTP = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.nudFrequency)).BeginInit();
			this.contextMenuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudFrequencyNTP)).BeginInit();
			this.SuspendLayout();
			// 
			// btnStartStopTest
			// 
			this.btnStartStopTest.Location = new System.Drawing.Point(564, 106);
			this.btnStartStopTest.Name = "btnStartStopTest";
			this.btnStartStopTest.Size = new System.Drawing.Size(75, 23);
			this.btnStartStopTest.TabIndex = 0;
			this.btnStartStopTest.Text = "Run Test";
			this.btnStartStopTest.UseVisualStyleBackColor = true;
			this.btnStartStopTest.Click += new System.EventHandler(this.btnStartStopTest_Click);
			// 
			// nudFrequency
			// 
			this.nudFrequency.Location = new System.Drawing.Point(513, 79);
			this.nudFrequency.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudFrequency.Name = "nudFrequency";
			this.nudFrequency.Size = new System.Drawing.Size(44, 20);
			this.nudFrequency.TabIndex = 1;
			this.nudFrequency.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(450, 82);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(57, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Test every";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(563, 82);
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
			this.label3.Location = new System.Drawing.Point(13, 111);
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
			this.lblAverageDiff.Location = new System.Drawing.Point(200, 111);
			this.lblAverageDiff.Name = "lblAverageDiff";
			this.lblAverageDiff.Size = new System.Drawing.Size(0, 13);
			this.lblAverageDiff.TabIndex = 6;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 157);
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
			this.tbxNTPServer.Text = "ntp2.tpg.com.au";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 9);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(72, 13);
			this.label5.TabIndex = 27;
			this.label5.Text = "NTP Server 1";
			// 
			// tbxMeasurements
			// 
			this.tbxMeasurements.BackColor = System.Drawing.SystemColors.Info;
			this.tbxMeasurements.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tbxMeasurements.Location = new System.Drawing.Point(15, 173);
			this.tbxMeasurements.Multiline = true;
			this.tbxMeasurements.Name = "tbxMeasurements";
			this.tbxMeasurements.ReadOnly = true;
			this.tbxMeasurements.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbxMeasurements.Size = new System.Drawing.Size(624, 198);
			this.tbxMeasurements.TabIndex = 29;
			// 
			// lblAverageDiffOccuRec
			// 
			this.lblAverageDiffOccuRec.AutoSize = true;
			this.lblAverageDiffOccuRec.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblAverageDiffOccuRec.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
			this.lblAverageDiffOccuRec.Location = new System.Drawing.Point(199, 128);
			this.lblAverageDiffOccuRec.Name = "lblAverageDiffOccuRec";
			this.lblAverageDiffOccuRec.Size = new System.Drawing.Size(0, 13);
			this.lblAverageDiffOccuRec.TabIndex = 31;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(12, 128);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(187, 13);
			this.label7.TabIndex = 30;
			this.label7.Text = "Average Difference (OccuRec Clock):";
			// 
			// btnPlotData
			// 
			this.btnPlotData.Location = new System.Drawing.Point(564, 144);
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
			this.label6.Location = new System.Drawing.Point(404, 28);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(103, 13);
			this.label6.TabIndex = 33;
			this.label6.Text = "HTCCv38 COM Port";
			// 
			// cbxCOMPort
			// 
			this.cbxCOMPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxCOMPort.FormattingEnabled = true;
			this.cbxCOMPort.Location = new System.Drawing.Point(513, 22);
			this.cbxCOMPort.Name = "cbxCOMPort";
			this.cbxCOMPort.Size = new System.Drawing.Size(112, 21);
			this.cbxCOMPort.TabIndex = 34;
			// 
			// tbxNTPServer2
			// 
			this.tbxNTPServer2.Location = new System.Drawing.Point(176, 25);
			this.tbxNTPServer2.Name = "tbxNTPServer2";
			this.tbxNTPServer2.Size = new System.Drawing.Size(155, 20);
			this.tbxNTPServer2.TabIndex = 37;
			this.tbxNTPServer2.Text = "ntp0.cs.mu.OZ.AU";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(173, 9);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(72, 13);
			this.label8.TabIndex = 36;
			this.label8.Text = "NTP Server 2";
			// 
			// tbxNTPServer4
			// 
			this.tbxNTPServer4.Location = new System.Drawing.Point(176, 68);
			this.tbxNTPServer4.Name = "tbxNTPServer4";
			this.tbxNTPServer4.Size = new System.Drawing.Size(155, 20);
			this.tbxNTPServer4.TabIndex = 41;
			this.tbxNTPServer4.Text = "ntp1.tpg.com.au";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(173, 52);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(72, 13);
			this.label9.TabIndex = 40;
			this.label9.Text = "NTP Server 4";
			// 
			// tbxNTPServer3
			// 
			this.tbxNTPServer3.Location = new System.Drawing.Point(15, 68);
			this.tbxNTPServer3.Name = "tbxNTPServer3";
			this.tbxNTPServer3.Size = new System.Drawing.Size(155, 20);
			this.tbxNTPServer3.TabIndex = 39;
			this.tbxNTPServer3.Text = "ntp.tourism.wa.gov.au";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(12, 52);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(72, 13);
			this.label10.TabIndex = 38;
			this.label10.Text = "NTP Server 3";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(563, 53);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(47, 13);
			this.label11.TabIndex = 44;
			this.label11.Text = "seconds";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(413, 53);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(94, 13);
			this.label12.TabIndex = 43;
			this.label12.Text = "NTP update every";
			// 
			// nudFrequencyNTP
			// 
			this.nudFrequencyNTP.Location = new System.Drawing.Point(513, 50);
			this.nudFrequencyNTP.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudFrequencyNTP.Name = "nudFrequencyNTP";
			this.nudFrequencyNTP.Size = new System.Drawing.Size(44, 20);
			this.nudFrequencyNTP.TabIndex = 42;
			this.nudFrequencyNTP.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
			// 
			// frmTestWindowsClock
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(651, 383);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.nudFrequencyNTP);
			this.Controls.Add(this.tbxNTPServer4);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.tbxNTPServer3);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.tbxNTPServer2);
			this.Controls.Add(this.label8);
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
			this.Text = "Test Windows Clock Accuracy v5";
			((System.ComponentModel.ISupportInitialize)(this.nudFrequency)).EndInit();
			this.contextMenuStrip1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudFrequencyNTP)).EndInit();
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
        private System.Windows.Forms.TextBox tbxNTPServer2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbxNTPServer4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbxNTPServer3;
        private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.NumericUpDown nudFrequencyNTP;
	}
}