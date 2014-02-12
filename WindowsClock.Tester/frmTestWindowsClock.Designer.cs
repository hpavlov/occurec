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
			this.nudFrequency.Location = new System.Drawing.Point(323, 22);
			this.nudFrequency.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudFrequency.Name = "nudFrequency";
			this.nudFrequency.Size = new System.Drawing.Size(44, 20);
			this.nudFrequency.TabIndex = 1;
			this.nudFrequency.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(260, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(57, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Test every";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(373, 25);
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
			this.label3.Size = new System.Drawing.Size(102, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Average Difference:";
			// 
			// lblAverageDiff
			// 
			this.lblAverageDiff.AutoSize = true;
			this.lblAverageDiff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblAverageDiff.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
			this.lblAverageDiff.Location = new System.Drawing.Point(121, 58);
			this.lblAverageDiff.Name = "lblAverageDiff";
			this.lblAverageDiff.Size = new System.Drawing.Size(0, 13);
			this.lblAverageDiff.TabIndex = 6;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(16, 95);
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
			this.tbxMeasurements.Location = new System.Drawing.Point(19, 120);
			this.tbxMeasurements.Multiline = true;
			this.tbxMeasurements.Name = "tbxMeasurements";
			this.tbxMeasurements.ReadOnly = true;
			this.tbxMeasurements.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbxMeasurements.Size = new System.Drawing.Size(620, 182);
			this.tbxMeasurements.TabIndex = 29;
			this.tbxMeasurements.Text = "ntp0.cs.mu.OZ.AU";
			// 
			// frmTestWindowsClock
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(651, 325);
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
			this.Text = "Test Windows Clock Accuracy";
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
	}
}