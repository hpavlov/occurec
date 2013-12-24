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
			this.tbxNTPServer = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tbxNTPServer
			// 
			this.tbxNTPServer.Location = new System.Drawing.Point(6, 57);
			this.tbxNTPServer.Name = "tbxNTPServer";
			this.tbxNTPServer.Size = new System.Drawing.Size(155, 20);
			this.tbxNTPServer.TabIndex = 26;
			this.tbxNTPServer.Text = "time.windows.com";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 41);
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
			// ucNTPTime
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
	}
}
