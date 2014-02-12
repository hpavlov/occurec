namespace OccuRec.Config.Panels
{
	partial class ucNTPTime
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucNTPTime));
			this.llblFindNTP = new System.Windows.Forms.LinkLabel();
			this.btnTestNTP = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tbxNTPServer = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.lblLatency1 = new System.Windows.Forms.Label();
			this.lblLatency2 = new System.Windows.Forms.Label();
			this.lblLatency3 = new System.Windows.Forms.Label();
			this.lblLatency4 = new System.Windows.Forms.Label();
			this.lblLatency5 = new System.Windows.Forms.Label();
			this.lblLatency6 = new System.Windows.Forms.Label();
			this.lblLatency7 = new System.Windows.Forms.Label();
			this.lblLatency8 = new System.Windows.Forms.Label();
			this.lblLatency9 = new System.Windows.Forms.Label();
			this.lblLatencyTitle = new System.Windows.Forms.Label();
			this.lblTestingIndicator = new System.Windows.Forms.Label();
			this.indicatorTimer = new System.Windows.Forms.Timer(this.components);
			this.label4 = new System.Windows.Forms.Label();
			this.nudHardwareLatencyCorrection = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.btnTestWindowsClock = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.nudHardwareLatencyCorrection)).BeginInit();
			this.SuspendLayout();
			// 
			// llblFindNTP
			// 
			this.llblFindNTP.AutoSize = true;
			this.llblFindNTP.Location = new System.Drawing.Point(3, 76);
			this.llblFindNTP.Name = "llblFindNTP";
			this.llblFindNTP.Size = new System.Drawing.Size(192, 13);
			this.llblFindNTP.TabIndex = 30;
			this.llblFindNTP.TabStop = true;
			this.llblFindNTP.Text = "Find an NTP Server that is close to you";
			this.llblFindNTP.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblFindNTP_LinkClicked);
			// 
			// btnTestNTP
			// 
			this.btnTestNTP.Location = new System.Drawing.Point(176, 125);
			this.btnTestNTP.Name = "btnTestNTP";
			this.btnTestNTP.Size = new System.Drawing.Size(119, 23);
			this.btnTestNTP.TabIndex = 29;
			this.btnTestNTP.Text = "Test Latency";
			this.btnTestNTP.UseVisualStyleBackColor = true;
			this.btnTestNTP.Click += new System.EventHandler(this.btnTestNTP_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 26);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(453, 49);
			this.label3.TabIndex = 28;
			this.label3.Text = resources.GetString("label3.Text");
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(453, 13);
			this.label1.TabIndex = 27;
			this.label1.Text = "NTP Server is used to adjust the Windows clock used by the scheduler to start/sto" +
    "p recording.";
			// 
			// tbxNTPServer
			// 
			this.tbxNTPServer.Location = new System.Drawing.Point(6, 127);
			this.tbxNTPServer.Name = "tbxNTPServer";
			this.tbxNTPServer.Size = new System.Drawing.Size(155, 20);
			this.tbxNTPServer.TabIndex = 26;
			this.tbxNTPServer.Text = "time.windows.com";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 111);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 13);
			this.label2.TabIndex = 25;
			this.label2.Text = "NTP Server";
			// 
			// lblLatency1
			// 
			this.lblLatency1.AutoSize = true;
			this.lblLatency1.Location = new System.Drawing.Point(3, 182);
			this.lblLatency1.Name = "lblLatency1";
			this.lblLatency1.Size = new System.Drawing.Size(44, 13);
			this.lblLatency1.TabIndex = 31;
			this.lblLatency1.Text = "1000ms";
			this.lblLatency1.Visible = false;
			// 
			// lblLatency2
			// 
			this.lblLatency2.AutoSize = true;
			this.lblLatency2.Location = new System.Drawing.Point(53, 182);
			this.lblLatency2.Name = "lblLatency2";
			this.lblLatency2.Size = new System.Drawing.Size(44, 13);
			this.lblLatency2.TabIndex = 32;
			this.lblLatency2.Text = "1000ms";
			this.lblLatency2.Visible = false;
			// 
			// lblLatency3
			// 
			this.lblLatency3.AutoSize = true;
			this.lblLatency3.Location = new System.Drawing.Point(103, 182);
			this.lblLatency3.Name = "lblLatency3";
			this.lblLatency3.Size = new System.Drawing.Size(44, 13);
			this.lblLatency3.TabIndex = 33;
			this.lblLatency3.Text = "1000ms";
			this.lblLatency3.Visible = false;
			// 
			// lblLatency4
			// 
			this.lblLatency4.AutoSize = true;
			this.lblLatency4.Location = new System.Drawing.Point(153, 182);
			this.lblLatency4.Name = "lblLatency4";
			this.lblLatency4.Size = new System.Drawing.Size(44, 13);
			this.lblLatency4.TabIndex = 34;
			this.lblLatency4.Text = "1000ms";
			this.lblLatency4.Visible = false;
			// 
			// lblLatency5
			// 
			this.lblLatency5.AutoSize = true;
			this.lblLatency5.Location = new System.Drawing.Point(203, 182);
			this.lblLatency5.Name = "lblLatency5";
			this.lblLatency5.Size = new System.Drawing.Size(44, 13);
			this.lblLatency5.TabIndex = 35;
			this.lblLatency5.Text = "1000ms";
			this.lblLatency5.Visible = false;
			// 
			// lblLatency6
			// 
			this.lblLatency6.AutoSize = true;
			this.lblLatency6.Location = new System.Drawing.Point(253, 182);
			this.lblLatency6.Name = "lblLatency6";
			this.lblLatency6.Size = new System.Drawing.Size(44, 13);
			this.lblLatency6.TabIndex = 36;
			this.lblLatency6.Text = "1000ms";
			this.lblLatency6.Visible = false;
			// 
			// lblLatency7
			// 
			this.lblLatency7.AutoSize = true;
			this.lblLatency7.Location = new System.Drawing.Point(303, 182);
			this.lblLatency7.Name = "lblLatency7";
			this.lblLatency7.Size = new System.Drawing.Size(44, 13);
			this.lblLatency7.TabIndex = 37;
			this.lblLatency7.Text = "1000ms";
			this.lblLatency7.Visible = false;
			// 
			// lblLatency8
			// 
			this.lblLatency8.AutoSize = true;
			this.lblLatency8.Location = new System.Drawing.Point(353, 182);
			this.lblLatency8.Name = "lblLatency8";
			this.lblLatency8.Size = new System.Drawing.Size(44, 13);
			this.lblLatency8.TabIndex = 38;
			this.lblLatency8.Text = "1000ms";
			this.lblLatency8.Visible = false;
			// 
			// lblLatency9
			// 
			this.lblLatency9.AutoSize = true;
			this.lblLatency9.Location = new System.Drawing.Point(403, 182);
			this.lblLatency9.Name = "lblLatency9";
			this.lblLatency9.Size = new System.Drawing.Size(44, 13);
			this.lblLatency9.TabIndex = 39;
			this.lblLatency9.Text = "1000ms";
			this.lblLatency9.Visible = false;
			// 
			// lblLatencyTitle
			// 
			this.lblLatencyTitle.AutoSize = true;
			this.lblLatencyTitle.Location = new System.Drawing.Point(3, 162);
			this.lblLatencyTitle.Name = "lblLatencyTitle";
			this.lblLatencyTitle.Size = new System.Drawing.Size(357, 13);
			this.lblLatencyTitle.TabIndex = 40;
			this.lblLatencyTitle.Text = "This will be a typical 3-Sigma timing error if this server is used to time video." +
    "";
			this.lblLatencyTitle.Visible = false;
			// 
			// lblTestingIndicator
			// 
			this.lblTestingIndicator.AutoSize = true;
			this.lblTestingIndicator.Location = new System.Drawing.Point(306, 130);
			this.lblTestingIndicator.Name = "lblTestingIndicator";
			this.lblTestingIndicator.Size = new System.Drawing.Size(54, 13);
			this.lblTestingIndicator.TabIndex = 41;
			this.lblTestingIndicator.Text = "Testing ...";
			this.lblTestingIndicator.Visible = false;
			// 
			// indicatorTimer
			// 
			this.indicatorTimer.Interval = 500;
			this.indicatorTimer.Tick += new System.EventHandler(this.indicatorTimer_Tick);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(3, 294);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(145, 13);
			this.label4.TabIndex = 42;
			this.label4.Text = "Hardware Latency Correction";
			// 
			// nudHardwareLatencyCorrection
			// 
			this.nudHardwareLatencyCorrection.Location = new System.Drawing.Point(154, 292);
			this.nudHardwareLatencyCorrection.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.nudHardwareLatencyCorrection.Name = "nudHardwareLatencyCorrection";
			this.nudHardwareLatencyCorrection.Size = new System.Drawing.Size(50, 20);
			this.nudHardwareLatencyCorrection.TabIndex = 43;
			this.nudHardwareLatencyCorrection.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(209, 297);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(20, 13);
			this.label5.TabIndex = 44;
			this.label5.Text = "ms";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(3, 216);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(453, 61);
			this.label6.TabIndex = 45;
			this.label6.Text = resources.GetString("label6.Text");
			// 
			// btnTestWindowsClock
			// 
			this.btnTestWindowsClock.Location = new System.Drawing.Point(258, 289);
			this.btnTestWindowsClock.Name = "btnTestWindowsClock";
			this.btnTestWindowsClock.Size = new System.Drawing.Size(198, 23);
			this.btnTestWindowsClock.TabIndex = 46;
			this.btnTestWindowsClock.Text = "Test Windows Clock Accuracy";
			this.btnTestWindowsClock.UseVisualStyleBackColor = true;
			this.btnTestWindowsClock.Click += new System.EventHandler(this.btnTestWindowsClock_Click);
			// 
			// ucNTPTime
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnTestWindowsClock);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.nudHardwareLatencyCorrection);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lblTestingIndicator);
			this.Controls.Add(this.lblLatencyTitle);
			this.Controls.Add(this.lblLatency9);
			this.Controls.Add(this.lblLatency8);
			this.Controls.Add(this.lblLatency7);
			this.Controls.Add(this.lblLatency6);
			this.Controls.Add(this.lblLatency5);
			this.Controls.Add(this.lblLatency4);
			this.Controls.Add(this.lblLatency3);
			this.Controls.Add(this.lblLatency2);
			this.Controls.Add(this.lblLatency1);
			this.Controls.Add(this.llblFindNTP);
			this.Controls.Add(this.btnTestNTP);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbxNTPServer);
			this.Controls.Add(this.label2);
			this.Name = "ucNTPTime";
			this.Size = new System.Drawing.Size(524, 366);
			((System.ComponentModel.ISupportInitialize)(this.nudHardwareLatencyCorrection)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbxNTPServer;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnTestNTP;
		private System.Windows.Forms.LinkLabel llblFindNTP;
		private System.Windows.Forms.Label lblLatency1;
		private System.Windows.Forms.Label lblLatency2;
		private System.Windows.Forms.Label lblLatency3;
		private System.Windows.Forms.Label lblLatency4;
		private System.Windows.Forms.Label lblLatency5;
		private System.Windows.Forms.Label lblLatency6;
		private System.Windows.Forms.Label lblLatency7;
		private System.Windows.Forms.Label lblLatency8;
		private System.Windows.Forms.Label lblLatency9;
		private System.Windows.Forms.Label lblLatencyTitle;
		private System.Windows.Forms.Label lblTestingIndicator;
		private System.Windows.Forms.Timer indicatorTimer;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown nudHardwareLatencyCorrection;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btnTestWindowsClock;
	}
}
