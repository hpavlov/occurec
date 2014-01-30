namespace OccuRec.Config.Panels
{
	partial class ucNTPTime
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucNTPTime));
			this.tbxNTPServer = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btnTestNTP = new System.Windows.Forms.Button();
			this.llblFindNTP = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// tbxNTPServer
			// 
			this.tbxNTPServer.Location = new System.Drawing.Point(6, 97);
			this.tbxNTPServer.Name = "tbxNTPServer";
			this.tbxNTPServer.Size = new System.Drawing.Size(155, 20);
			this.tbxNTPServer.TabIndex = 26;
			this.tbxNTPServer.Text = "time.windows.com";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 81);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 13);
			this.label2.TabIndex = 25;
			this.label2.Text = "NTP Server";
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
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 26);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(453, 49);
			this.label3.TabIndex = 28;
			this.label3.Text = resources.GetString("label3.Text");
			// 
			// btnTestNTP
			// 
			this.btnTestNTP.Location = new System.Drawing.Point(176, 95);
			this.btnTestNTP.Name = "btnTestNTP";
			this.btnTestNTP.Size = new System.Drawing.Size(119, 23);
			this.btnTestNTP.TabIndex = 29;
			this.btnTestNTP.Text = "Test Latency";
			this.btnTestNTP.UseVisualStyleBackColor = true;
			// 
			// llblFindNTP
			// 
			this.llblFindNTP.AutoSize = true;
			this.llblFindNTP.Location = new System.Drawing.Point(3, 146);
			this.llblFindNTP.Name = "llblFindNTP";
			this.llblFindNTP.Size = new System.Drawing.Size(149, 13);
			this.llblFindNTP.TabIndex = 30;
			this.llblFindNTP.TabStop = true;
			this.llblFindNTP.Text = "Find NTP Server Close to You";
			this.llblFindNTP.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblFindNTP_LinkClicked);
			// 
			// ucNTPTime
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.llblFindNTP);
			this.Controls.Add(this.btnTestNTP);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbxNTPServer);
			this.Controls.Add(this.label2);
			this.Name = "ucNTPTime";
			this.Size = new System.Drawing.Size(524, 291);
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
	}
}
