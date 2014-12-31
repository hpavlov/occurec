namespace OccuRec.ASCOM
{
    partial class frmTelescopeControl
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
            this.btnPulseNorth = new System.Windows.Forms.Button();
            this.btnPulseSouth = new System.Windows.Forms.Button();
            this.btnPulseWest = new System.Windows.Forms.Button();
            this.btnPulseEast = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.connectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.rbSlowest = new System.Windows.Forms.RadioButton();
            this.rbFast = new System.Windows.Forms.RadioButton();
            this.rbSlow = new System.Windows.Forms.RadioButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssRA = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssDE = new System.Windows.Forms.ToolStripStatusLabel();
            this.actionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miCalibratePulseGuiding = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPulseNorth
            // 
            this.btnPulseNorth.Location = new System.Drawing.Point(86, 69);
            this.btnPulseNorth.Name = "btnPulseNorth";
            this.btnPulseNorth.Size = new System.Drawing.Size(25, 23);
            this.btnPulseNorth.TabIndex = 0;
            this.btnPulseNorth.Text = "/\\";
            this.btnPulseNorth.UseVisualStyleBackColor = true;
            this.btnPulseNorth.Click += new System.EventHandler(this.btnPulseNorth_Click);
            // 
            // btnPulseSouth
            // 
            this.btnPulseSouth.Location = new System.Drawing.Point(86, 127);
            this.btnPulseSouth.Name = "btnPulseSouth";
            this.btnPulseSouth.Size = new System.Drawing.Size(25, 23);
            this.btnPulseSouth.TabIndex = 1;
            this.btnPulseSouth.Tag = "";
            this.btnPulseSouth.Text = "\\/";
            this.btnPulseSouth.UseVisualStyleBackColor = true;
            this.btnPulseSouth.Click += new System.EventHandler(this.btnPulseSouth_Click);
            // 
            // btnPulseWest
            // 
            this.btnPulseWest.Location = new System.Drawing.Point(53, 96);
            this.btnPulseWest.Name = "btnPulseWest";
            this.btnPulseWest.Size = new System.Drawing.Size(25, 23);
            this.btnPulseWest.TabIndex = 2;
            this.btnPulseWest.Text = "<";
            this.btnPulseWest.UseVisualStyleBackColor = true;
            this.btnPulseWest.Click += new System.EventHandler(this.btnPulseWest_Click);
            // 
            // btnPulseEast
            // 
            this.btnPulseEast.Location = new System.Drawing.Point(119, 96);
            this.btnPulseEast.Name = "btnPulseEast";
            this.btnPulseEast.Size = new System.Drawing.Size(25, 23);
            this.btnPulseEast.TabIndex = 3;
            this.btnPulseEast.Text = ">";
            this.btnPulseEast.UseVisualStyleBackColor = true;
            this.btnPulseEast.Click += new System.EventHandler(this.btnPulseEast_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionToolStripMenuItem,
            this.actionToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(199, 24);
            this.menuStrip1.TabIndex = 13;
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
            this.miDisconnect.Size = new System.Drawing.Size(152, 22);
            this.miDisconnect.Text = "&Disconnect";
            this.miDisconnect.Click += new System.EventHandler(this.miDisconnect_Click);
            // 
            // rbSlowest
            // 
            this.rbSlowest.AutoSize = true;
            this.rbSlowest.Checked = true;
            this.rbSlowest.Location = new System.Drawing.Point(12, 36);
            this.rbSlowest.Name = "rbSlowest";
            this.rbSlowest.Size = new System.Drawing.Size(62, 17);
            this.rbSlowest.TabIndex = 14;
            this.rbSlowest.TabStop = true;
            this.rbSlowest.Text = "Slowest";
            this.rbSlowest.UseVisualStyleBackColor = true;
            // 
            // rbFast
            // 
            this.rbFast.AutoSize = true;
            this.rbFast.Location = new System.Drawing.Point(125, 36);
            this.rbFast.Name = "rbFast";
            this.rbFast.Size = new System.Drawing.Size(45, 17);
            this.rbFast.TabIndex = 15;
            this.rbFast.Text = "Fast";
            this.rbFast.UseVisualStyleBackColor = true;
            // 
            // rbSlow
            // 
            this.rbSlow.AutoSize = true;
            this.rbSlow.Location = new System.Drawing.Point(74, 36);
            this.rbSlow.Name = "rbSlow";
            this.rbSlow.Size = new System.Drawing.Size(48, 17);
            this.rbSlow.TabIndex = 16;
            this.rbSlow.Text = "Slow";
            this.rbSlow.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tssRA,
            this.toolStripStatusLabel3,
            this.tssDE});
            this.statusStrip1.Location = new System.Drawing.Point(0, 170);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(199, 22);
            this.statusStrip1.TabIndex = 17;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Symbol", 8.25F);
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(20, 17);
            this.toolStripStatusLabel1.Text = "a=";
            // 
            // tssRA
            // 
            this.tssRA.Name = "tssRA";
            this.tssRA.Size = new System.Drawing.Size(56, 17);
            this.tssRA.Text = "--h --m --s";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Font = new System.Drawing.Font("Symbol", 8.25F);
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(21, 17);
            this.toolStripStatusLabel3.Text = " d=";
            // 
            // tssDE
            // 
            this.tssDE.Name = "tssDE";
            this.tssDE.Size = new System.Drawing.Size(56, 17);
            this.tssDE.Text = "+--° --\' --\"";
            // 
            // actionToolStripMenuItem
            // 
            this.actionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCalibratePulseGuiding});
            this.actionToolStripMenuItem.Name = "actionToolStripMenuItem";
            this.actionToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.actionToolStripMenuItem.Text = "&Action";
            // 
            // miCalibratePulseGuiding
            // 
            this.miCalibratePulseGuiding.Name = "miCalibratePulseGuiding";
            this.miCalibratePulseGuiding.Size = new System.Drawing.Size(183, 22);
            this.miCalibratePulseGuiding.Text = "&Calibrate Pulse Guiding";
            this.miCalibratePulseGuiding.Click += new System.EventHandler(this.miCalibratePulseGuiding_Click);
            // 
            // frmTelescopeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(199, 192);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.rbSlow);
            this.Controls.Add(this.rbFast);
            this.Controls.Add(this.rbSlowest);
            this.Controls.Add(this.btnPulseEast);
            this.Controls.Add(this.btnPulseWest);
            this.Controls.Add(this.btnPulseSouth);
            this.Controls.Add(this.btnPulseNorth);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmTelescopeControl";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Telescope Control";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.frmTelescopeControl_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPulseNorth;
        private System.Windows.Forms.Button btnPulseSouth;
        private System.Windows.Forms.Button btnPulseWest;
        private System.Windows.Forms.Button btnPulseEast;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem connectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miDisconnect;
        private System.Windows.Forms.RadioButton rbSlowest;
        private System.Windows.Forms.RadioButton rbFast;
        private System.Windows.Forms.RadioButton rbSlow;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem actionToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel tssRA;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel tssDE;
        private System.Windows.Forms.ToolStripMenuItem miCalibratePulseGuiding;
    }
}