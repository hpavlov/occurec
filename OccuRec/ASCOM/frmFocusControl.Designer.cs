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
			this.pnlFocuserControls = new System.Windows.Forms.Panel();
			this.btnInLargest = new System.Windows.Forms.Button();
			this.btnOutLargest = new System.Windows.Forms.Button();
			this.btnOutLarge = new System.Windows.Forms.Button();
			this.btnOutSmall = new System.Windows.Forms.Button();
			this.btnInSmall = new System.Windows.Forms.Button();
			this.btnInLarge = new System.Windows.Forms.Button();
			this.nudMove = new System.Windows.Forms.NumericUpDown();
			this.btnMove = new System.Windows.Forms.Button();
			this.lblPosition = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.gbxTargetControl = new System.Windows.Forms.GroupBox();
			this.btnFocusTarget = new System.Windows.Forms.Button();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.connectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miDisconnect = new System.Windows.Forms.ToolStripMenuItem();
			this.pnlFocuserControls.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMove)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.gbxTargetControl.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbxTempComp
			// 
			this.cbxTempComp.AutoSize = true;
			this.cbxTempComp.Location = new System.Drawing.Point(170, 29);
			this.cbxTempComp.Name = "cbxTempComp";
			this.cbxTempComp.Size = new System.Drawing.Size(156, 17);
			this.cbxTempComp.TabIndex = 0;
			this.cbxTempComp.Text = "Temperature Compensation";
			this.cbxTempComp.UseVisualStyleBackColor = true;
			this.cbxTempComp.CheckedChanged += new System.EventHandler(this.cbxTempComp_CheckedChanged);
			// 
			// pnlFocuserControls
			// 
			this.pnlFocuserControls.Controls.Add(this.btnInLargest);
			this.pnlFocuserControls.Controls.Add(this.btnOutLargest);
			this.pnlFocuserControls.Controls.Add(this.btnOutLarge);
			this.pnlFocuserControls.Controls.Add(this.btnOutSmall);
			this.pnlFocuserControls.Controls.Add(this.btnInSmall);
			this.pnlFocuserControls.Controls.Add(this.btnInLarge);
			this.pnlFocuserControls.Controls.Add(this.nudMove);
			this.pnlFocuserControls.Controls.Add(this.btnMove);
			this.pnlFocuserControls.Controls.Add(this.lblPosition);
			this.pnlFocuserControls.Location = new System.Drawing.Point(6, 13);
			this.pnlFocuserControls.Name = "pnlFocuserControls";
			this.pnlFocuserControls.Size = new System.Drawing.Size(332, 35);
			this.pnlFocuserControls.TabIndex = 2;
			// 
			// btnInLargest
			// 
			this.btnInLargest.Location = new System.Drawing.Point(10, 6);
			this.btnInLargest.Name = "btnInLargest";
			this.btnInLargest.Size = new System.Drawing.Size(38, 23);
			this.btnInLargest.TabIndex = 14;
			this.btnInLargest.Tag = "";
			this.btnInLargest.Text = "<<<";
			this.btnInLargest.UseVisualStyleBackColor = true;
			this.btnInLargest.Click += new System.EventHandler(this.btnInLargest_Click);
			// 
			// btnOutLargest
			// 
			this.btnOutLargest.Location = new System.Drawing.Point(282, 6);
			this.btnOutLargest.Name = "btnOutLargest";
			this.btnOutLargest.Size = new System.Drawing.Size(38, 23);
			this.btnOutLargest.TabIndex = 13;
			this.btnOutLargest.Tag = "";
			this.btnOutLargest.Text = ">>>";
			this.btnOutLargest.UseVisualStyleBackColor = true;
			this.btnOutLargest.Click += new System.EventHandler(this.btnOutLargest_Click);
			// 
			// btnOutLarge
			// 
			this.btnOutLarge.Location = new System.Drawing.Point(238, 6);
			this.btnOutLarge.Name = "btnOutLarge";
			this.btnOutLarge.Size = new System.Drawing.Size(38, 23);
			this.btnOutLarge.TabIndex = 12;
			this.btnOutLarge.Tag = "";
			this.btnOutLarge.Text = ">>";
			this.btnOutLarge.UseVisualStyleBackColor = true;
			this.btnOutLarge.Click += new System.EventHandler(this.btnOutLarge_Click);
			// 
			// btnOutSmall
			// 
			this.btnOutSmall.Location = new System.Drawing.Point(194, 6);
			this.btnOutSmall.Name = "btnOutSmall";
			this.btnOutSmall.Size = new System.Drawing.Size(38, 23);
			this.btnOutSmall.TabIndex = 11;
			this.btnOutSmall.Tag = "";
			this.btnOutSmall.Text = ">";
			this.btnOutSmall.UseVisualStyleBackColor = true;
			this.btnOutSmall.Click += new System.EventHandler(this.btnOutSmall_Click);
			// 
			// btnInSmall
			// 
			this.btnInSmall.Location = new System.Drawing.Point(96, 6);
			this.btnInSmall.Name = "btnInSmall";
			this.btnInSmall.Size = new System.Drawing.Size(38, 23);
			this.btnInSmall.TabIndex = 10;
			this.btnInSmall.Tag = "";
			this.btnInSmall.Text = "<";
			this.btnInSmall.UseVisualStyleBackColor = true;
			this.btnInSmall.Click += new System.EventHandler(this.btnInSmall_Click);
			// 
			// btnInLarge
			// 
			this.btnInLarge.Location = new System.Drawing.Point(52, 6);
			this.btnInLarge.Name = "btnInLarge";
			this.btnInLarge.Size = new System.Drawing.Size(38, 23);
			this.btnInLarge.TabIndex = 9;
			this.btnInLarge.Tag = "";
			this.btnInLarge.Text = "<<";
			this.btnInLarge.UseVisualStyleBackColor = true;
			this.btnInLarge.Click += new System.EventHandler(this.btnInLarge_Click);
			// 
			// nudMove
			// 
			this.nudMove.Location = new System.Drawing.Point(12, 74);
			this.nudMove.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.nudMove.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
			this.nudMove.Name = "nudMove";
			this.nudMove.Size = new System.Drawing.Size(54, 20);
			this.nudMove.TabIndex = 8;
			this.nudMove.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
			// 
			// btnMove
			// 
			this.btnMove.Location = new System.Drawing.Point(72, 74);
			this.btnMove.Name = "btnMove";
			this.btnMove.Size = new System.Drawing.Size(48, 23);
			this.btnMove.TabIndex = 7;
			this.btnMove.Tag = "";
			this.btnMove.Text = "Move";
			this.btnMove.UseVisualStyleBackColor = true;
			this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
			// 
			// lblPosition
			// 
			this.lblPosition.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblPosition.ForeColor = System.Drawing.Color.Blue;
			this.lblPosition.Location = new System.Drawing.Point(135, 11);
			this.lblPosition.Name = "lblPosition";
			this.lblPosition.Size = new System.Drawing.Size(59, 13);
			this.lblPosition.TabIndex = 5;
			this.lblPosition.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.pnlFocuserControls);
			this.groupBox1.Location = new System.Drawing.Point(12, 27);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(344, 53);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Manual Focus";
			// 
			// gbxTargetControl
			// 
			this.gbxTargetControl.Controls.Add(this.btnFocusTarget);
			this.gbxTargetControl.Controls.Add(this.cbxTempComp);
			this.gbxTargetControl.Location = new System.Drawing.Point(12, 86);
			this.gbxTargetControl.Name = "gbxTargetControl";
			this.gbxTargetControl.Size = new System.Drawing.Size(344, 62);
			this.gbxTargetControl.TabIndex = 4;
			this.gbxTargetControl.TabStop = false;
			this.gbxTargetControl.Text = "Automatic Target Control";
			// 
			// btnFocusTarget
			// 
			this.btnFocusTarget.Location = new System.Drawing.Point(16, 25);
			this.btnFocusTarget.Name = "btnFocusTarget";
			this.btnFocusTarget.Size = new System.Drawing.Size(114, 23);
			this.btnFocusTarget.TabIndex = 8;
			this.btnFocusTarget.Tag = "";
			this.btnFocusTarget.Text = "Refocus Target";
			this.btnFocusTarget.UseVisualStyleBackColor = true;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(369, 24);
			this.menuStrip1.TabIndex = 14;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// connectionToolStripMenuItem
			// 
			this.connectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDisconnect});
			this.connectionToolStripMenuItem.Name = "connectionToolStripMenuItem";
			this.connectionToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
			this.connectionToolStripMenuItem.Text = "&Connection";
			// 
			// miDisconnect
			// 
			this.miDisconnect.Name = "miDisconnect";
			this.miDisconnect.Size = new System.Drawing.Size(126, 22);
			this.miDisconnect.Text = "&Disconnect";
			this.miDisconnect.Click += new System.EventHandler(this.miDisconnect_Click);
			// 
			// frmFocusControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(369, 158);
			this.Controls.Add(this.gbxTargetControl);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "frmFocusControl";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Focuser Control";
			this.TopMost = true;
			this.Shown += new System.EventHandler(this.frmFocusControl_Shown);
			this.pnlFocuserControls.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudMove)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.gbxTargetControl.ResumeLayout(false);
			this.gbxTargetControl.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.CheckBox cbxTempComp;
		private System.Windows.Forms.Panel pnlFocuserControls;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.NumericUpDown nudMove;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Button btnOutLarge;
        private System.Windows.Forms.Button btnOutSmall;
        private System.Windows.Forms.Button btnInSmall;
        private System.Windows.Forms.Button btnInLarge;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox gbxTargetControl;
        private System.Windows.Forms.Button btnFocusTarget;
        private System.Windows.Forms.Button btnInLargest;
		private System.Windows.Forms.Button btnOutLargest;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem connectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem miDisconnect;
	}
}