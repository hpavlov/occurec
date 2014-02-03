namespace OccuRec.Config.Panels
{
	partial class ucGeneral
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
			this.cbxWarnForFileSystemIssues = new System.Windows.Forms.CheckBox();
			this.cbxTimeInUT = new System.Windows.Forms.CheckBox();
			this.btnBrowseOutputFolder = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.tbxOutputLocation = new System.Windows.Forms.TextBox();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.SuspendLayout();
			// 
			// cbxWarnForFileSystemIssues
			// 
			this.cbxWarnForFileSystemIssues.AutoSize = true;
			this.cbxWarnForFileSystemIssues.Location = new System.Drawing.Point(6, 42);
			this.cbxWarnForFileSystemIssues.Name = "cbxWarnForFileSystemIssues";
			this.cbxWarnForFileSystemIssues.Size = new System.Drawing.Size(230, 17);
			this.cbxWarnForFileSystemIssues.TabIndex = 25;
			this.cbxWarnForFileSystemIssues.Text = "Enable disk space and file system warnings";
			this.cbxWarnForFileSystemIssues.UseVisualStyleBackColor = true;
			// 
			// cbxTimeInUT
			// 
			this.cbxTimeInUT.AutoSize = true;
			this.cbxTimeInUT.Location = new System.Drawing.Point(6, 65);
			this.cbxTimeInUT.Name = "cbxTimeInUT";
			this.cbxTimeInUT.Size = new System.Drawing.Size(115, 17);
			this.cbxTimeInUT.TabIndex = 22;
			this.cbxTimeInUT.Text = "Display Time in UT";
			this.cbxTimeInUT.UseVisualStyleBackColor = true;
			// 
			// btnBrowseOutputFolder
			// 
			this.btnBrowseOutputFolder.Location = new System.Drawing.Point(334, 13);
			this.btnBrowseOutputFolder.Name = "btnBrowseOutputFolder";
			this.btnBrowseOutputFolder.Size = new System.Drawing.Size(30, 23);
			this.btnBrowseOutputFolder.TabIndex = 21;
			this.btnBrowseOutputFolder.Text = "...";
			this.btnBrowseOutputFolder.UseVisualStyleBackColor = true;
			this.btnBrowseOutputFolder.Click += new System.EventHandler(this.btnBrowseOutputFolder_Click);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(3, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(113, 13);
			this.label6.TabIndex = 20;
			this.label6.Text = "Output Video Location";
			// 
			// tbxOutputLocation
			// 
			this.tbxOutputLocation.Location = new System.Drawing.Point(6, 16);
			this.tbxOutputLocation.Name = "tbxOutputLocation";
			this.tbxOutputLocation.Size = new System.Drawing.Size(322, 20);
			this.tbxOutputLocation.TabIndex = 19;
			// 
			// ucGeneral
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cbxWarnForFileSystemIssues);
			this.Controls.Add(this.cbxTimeInUT);
			this.Controls.Add(this.btnBrowseOutputFolder);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.tbxOutputLocation);
			this.Name = "ucGeneral";
			this.Size = new System.Drawing.Size(433, 309);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox cbxWarnForFileSystemIssues;
		private System.Windows.Forms.CheckBox cbxTimeInUT;
		private System.Windows.Forms.Button btnBrowseOutputFolder;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbxOutputLocation;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
	}
}
