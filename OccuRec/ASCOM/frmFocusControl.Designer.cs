namespace OccuRec.ASCOM
{
	partial class frmFocusControl
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
			this.cbxTempComp = new System.Windows.Forms.CheckBox();
			this.btnInLargest = new System.Windows.Forms.Button();
			this.btnOutLargest = new System.Windows.Forms.Button();
			this.btnOutLarge = new System.Windows.Forms.Button();
			this.btnOutSmall = new System.Windows.Forms.Button();
			this.btnInSmall = new System.Windows.Forms.Button();
			this.btnInLarge = new System.Windows.Forms.Button();
			this.gbxTargetControl = new System.Windows.Forms.GroupBox();
			this.btnFocusTarget = new System.Windows.Forms.Button();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.connectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miDisconnect = new System.Windows.Forms.ToolStripMenuItem();
			this.pnlFocuserControls = new System.Windows.Forms.Panel();
			this.gbxTargetControl.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.pnlFocuserControls.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbxTempComp
			// 
			this.cbxTempComp.AutoSize = true;
			this.cbxTempComp.Location = new System.Drawing.Point(11, 22);
			this.cbxTempComp.Name = "cbxTempComp";
			this.cbxTempComp.Size = new System.Drawing.Size(83, 17);
			this.cbxTempComp.TabIndex = 0;
			this.cbxTempComp.Text = "Temp Comp";
			this.cbxTempComp.UseVisualStyleBackColor = true;
			this.cbxTempComp.CheckedChanged += new System.EventHandler(this.cbxTempComp_CheckedChanged);
			// 
			// btnInLargest
			// 
			this.btnInLargest.Location = new System.Drawing.Point(7, 4);
			this.btnInLargest.Name = "btnInLargest";
			this.btnInLargest.Size = new System.Drawing.Size(33, 23);
			this.btnInLargest.TabIndex = 14;
			this.btnInLargest.Tag = "";
			this.btnInLargest.Text = "<<<";
			this.btnInLargest.UseVisualStyleBackColor = true;
			this.btnInLargest.Click += new System.EventHandler(this.btnInLargest_Click);
			// 
			// btnOutLargest
			// 
			this.btnOutLargest.Location = new System.Drawing.Point(70, 4);
			this.btnOutLargest.Name = "btnOutLargest";
			this.btnOutLargest.Size = new System.Drawing.Size(33, 23);
			this.btnOutLargest.TabIndex = 13;
			this.btnOutLargest.Tag = "";
			this.btnOutLargest.Text = ">>>";
			this.btnOutLargest.UseVisualStyleBackColor = true;
			this.btnOutLargest.Click += new System.EventHandler(this.btnOutLargest_Click);
			// 
			// btnOutLarge
			// 
			this.btnOutLarge.Location = new System.Drawing.Point(70, 33);
			this.btnOutLarge.Name = "btnOutLarge";
			this.btnOutLarge.Size = new System.Drawing.Size(33, 23);
			this.btnOutLarge.TabIndex = 12;
			this.btnOutLarge.Tag = "";
			this.btnOutLarge.Text = ">>";
			this.btnOutLarge.UseVisualStyleBackColor = true;
			this.btnOutLarge.Click += new System.EventHandler(this.btnOutLarge_Click);
			// 
			// btnOutSmall
			// 
			this.btnOutSmall.Location = new System.Drawing.Point(70, 62);
			this.btnOutSmall.Name = "btnOutSmall";
			this.btnOutSmall.Size = new System.Drawing.Size(33, 23);
			this.btnOutSmall.TabIndex = 11;
			this.btnOutSmall.Tag = "";
			this.btnOutSmall.Text = ">";
			this.btnOutSmall.UseVisualStyleBackColor = true;
			this.btnOutSmall.Click += new System.EventHandler(this.btnOutSmall_Click);
			// 
			// btnInSmall
			// 
			this.btnInSmall.Location = new System.Drawing.Point(7, 62);
			this.btnInSmall.Name = "btnInSmall";
			this.btnInSmall.Size = new System.Drawing.Size(33, 23);
			this.btnInSmall.TabIndex = 10;
			this.btnInSmall.Tag = "";
			this.btnInSmall.Text = "<";
			this.btnInSmall.UseVisualStyleBackColor = true;
			this.btnInSmall.Click += new System.EventHandler(this.btnInSmall_Click);
			// 
			// btnInLarge
			// 
			this.btnInLarge.Location = new System.Drawing.Point(7, 33);
			this.btnInLarge.Name = "btnInLarge";
			this.btnInLarge.Size = new System.Drawing.Size(33, 23);
			this.btnInLarge.TabIndex = 9;
			this.btnInLarge.Tag = "";
			this.btnInLarge.Text = "<<";
			this.btnInLarge.UseVisualStyleBackColor = true;
			this.btnInLarge.Click += new System.EventHandler(this.btnInLarge_Click);
			// 
			// gbxTargetControl
			// 
			this.gbxTargetControl.Controls.Add(this.btnFocusTarget);
			this.gbxTargetControl.Controls.Add(this.cbxTempComp);
			this.gbxTargetControl.Location = new System.Drawing.Point(5, 121);
			this.gbxTargetControl.Name = "gbxTargetControl";
			this.gbxTargetControl.Size = new System.Drawing.Size(119, 87);
			this.gbxTargetControl.TabIndex = 4;
			this.gbxTargetControl.TabStop = false;
			this.gbxTargetControl.Text = "Automatic Control";
			// 
			// btnFocusTarget
			// 
			this.btnFocusTarget.Enabled = false;
			this.btnFocusTarget.Location = new System.Drawing.Point(11, 54);
			this.btnFocusTarget.Name = "btnFocusTarget";
			this.btnFocusTarget.Size = new System.Drawing.Size(96, 23);
			this.btnFocusTarget.TabIndex = 8;
			this.btnFocusTarget.Tag = "";
			this.btnFocusTarget.Text = "Refocus Target";
			this.btnFocusTarget.UseVisualStyleBackColor = true;
			this.btnFocusTarget.Click += new System.EventHandler(this.btnFocusTarget_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(129, 24);
			this.menuStrip1.TabIndex = 14;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// connectionToolStripMenuItem
			// 
			this.connectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDisconnect});
			this.connectionToolStripMenuItem.Name = "connectionToolStripMenuItem";
			this.connectionToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
			this.connectionToolStripMenuItem.Text = "&Connection";
			// 
			// miDisconnect
			// 
			this.miDisconnect.Name = "miDisconnect";
			this.miDisconnect.Size = new System.Drawing.Size(133, 22);
			this.miDisconnect.Text = "&Disconnect";
			this.miDisconnect.Click += new System.EventHandler(this.miDisconnect_Click);
			// 
			// pnlFocuserControls
			// 
			this.pnlFocuserControls.Controls.Add(this.btnOutLargest);
			this.pnlFocuserControls.Controls.Add(this.btnInLarge);
			this.pnlFocuserControls.Controls.Add(this.btnInLargest);
			this.pnlFocuserControls.Controls.Add(this.btnInSmall);
			this.pnlFocuserControls.Controls.Add(this.btnOutLarge);
			this.pnlFocuserControls.Controls.Add(this.btnOutSmall);
			this.pnlFocuserControls.Location = new System.Drawing.Point(9, 27);
			this.pnlFocuserControls.Name = "pnlFocuserControls";
			this.pnlFocuserControls.Size = new System.Drawing.Size(115, 92);
			this.pnlFocuserControls.TabIndex = 15;
			// 
			// frmFocusControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(129, 212);
			this.Controls.Add(this.pnlFocuserControls);
			this.Controls.Add(this.gbxTargetControl);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "frmFocusControl";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Focuser Control";
			this.TopMost = true;
			this.Shown += new System.EventHandler(this.frmFocusControl_Shown);
			this.gbxTargetControl.ResumeLayout(false);
			this.gbxTargetControl.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.pnlFocuserControls.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox cbxTempComp;
        private System.Windows.Forms.Button btnOutLarge;
        private System.Windows.Forms.Button btnOutSmall;
        private System.Windows.Forms.Button btnInSmall;
		private System.Windows.Forms.Button btnInLarge;
        private System.Windows.Forms.GroupBox gbxTargetControl;
        private System.Windows.Forms.Button btnFocusTarget;
        private System.Windows.Forms.Button btnInLargest;
		private System.Windows.Forms.Button btnOutLargest;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem connectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem miDisconnect;
		private System.Windows.Forms.Panel pnlFocuserControls;
	}
}