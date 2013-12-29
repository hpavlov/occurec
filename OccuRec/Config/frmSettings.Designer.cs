namespace OccuRec.Config
{
	partial class frmSettings
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("General");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Astro Analogue Video (AAV)");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("NTP Time");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Telescope");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Focusing");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Observatory Control (ASCOM)", new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Tracking");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Debug");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnResetDefaults = new System.Windows.Forms.Button();
            this.pnlPropertyPage = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tvSettings = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(581, 376);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(500, 376);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnResetDefaults
            // 
            this.btnResetDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetDefaults.Location = new System.Drawing.Point(185, 376);
            this.btnResetDefaults.Name = "btnResetDefaults";
            this.btnResetDefaults.Size = new System.Drawing.Size(106, 23);
            this.btnResetDefaults.TabIndex = 20;
            this.btnResetDefaults.Text = "Reset Defaults";
            this.btnResetDefaults.Click += new System.EventHandler(this.btnResetDefaults_Click);
            // 
            // pnlPropertyPage
            // 
            this.pnlPropertyPage.Location = new System.Drawing.Point(185, 12);
            this.pnlPropertyPage.Name = "pnlPropertyPage";
            this.pnlPropertyPage.Size = new System.Drawing.Size(471, 349);
            this.pnlPropertyPage.TabIndex = 17;
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(185, 367);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(471, 3);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            // 
            // tvSettings
            // 
            this.tvSettings.Location = new System.Drawing.Point(3, 11);
            this.tvSettings.Name = "tvSettings";
            treeNode1.Name = "ndGeneral";
            treeNode1.Tag = "0";
            treeNode1.Text = "General";
            treeNode2.Name = "ndAAV";
            treeNode2.Tag = "1";
            treeNode2.Text = "Astro Analogue Video (AAV)";
            treeNode3.Name = "ndNTP";
            treeNode3.Tag = "2";
            treeNode3.Text = "NTP Time";
            treeNode4.Name = "ndTelescope";
            treeNode4.Tag = "6";
            treeNode4.Text = "Telescope";
            treeNode5.Name = "ndFocusing";
            treeNode5.Tag = "7";
            treeNode5.Text = "Focusing";
            treeNode6.Name = "ndObservatoryControl";
            treeNode6.Tag = "3";
            treeNode6.Text = "Observatory Control (ASCOM)";
            treeNode7.Name = "ndTracking";
            treeNode7.Tag = "4";
            treeNode7.Text = "Tracking";
            treeNode8.Name = "ndDebug";
            treeNode8.Tag = "5";
            treeNode8.Text = "Debug";
            this.tvSettings.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode6,
            treeNode7,
            treeNode8});
            this.tvSettings.Size = new System.Drawing.Size(176, 359);
            this.tvSettings.TabIndex = 15;
            this.tvSettings.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvSettings_BeforeSelect);
            this.tvSettings.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSettings_AfterSelect);
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 405);
            this.Controls.Add(this.btnResetDefaults);
            this.Controls.Add(this.pnlPropertyPage);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.tvSettings);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OccuRec Settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnResetDefaults;
		private System.Windows.Forms.Panel pnlPropertyPage;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TreeView tvSettings;
	}
}