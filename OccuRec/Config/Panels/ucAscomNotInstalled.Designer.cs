namespace OccuRec.Config.Panels
{
	partial class ucAscomNotInstalled
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucAscomNotInstalled));
			this.llAscomWebSite = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// llAscomWebSite
			// 
			this.llAscomWebSite.AutoSize = true;
			this.llAscomWebSite.Location = new System.Drawing.Point(13, 75);
			this.llAscomWebSite.Name = "llAscomWebSite";
			this.llAscomWebSite.Size = new System.Drawing.Size(168, 13);
			this.llAscomWebSite.TabIndex = 1;
			this.llAscomWebSite.TabStop = true;
			this.llAscomWebSite.Text = "http://www.ascom-standards.org/";
			this.llAscomWebSite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llAscomWebSite_LinkClicked);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(13, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(419, 47);
			this.label1.TabIndex = 0;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// ucAscomNotInstalled
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.llAscomWebSite);
			this.Controls.Add(this.label1);
			this.Name = "ucAscomNotInstalled";
			this.Size = new System.Drawing.Size(467, 193);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel llAscomWebSite;
	}
}
