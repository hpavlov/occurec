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
			this.actionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miSlew = new System.Windows.Forms.ToolStripMenuItem();
			this.miSyncPosition = new System.Windows.Forms.ToolStripMenuItem();
			this.miCalibratePulseGuiding = new System.Windows.Forms.ToolStripMenuItem();
			this.rbSlowest = new System.Windows.Forms.RadioButton();
			this.rbFast = new System.Windows.Forms.RadioButton();
			this.rbSlow = new System.Windows.Forms.RadioButton();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssRA = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssDE = new System.Windows.Forms.ToolStripStatusLabel();
			this.rb1Min = new System.Windows.Forms.RadioButton();
			this.rb60min = new System.Windows.Forms.RadioButton();
			this.rb10Min = new System.Windows.Forms.RadioButton();
			this.rb30Min = new System.Windows.Forms.RadioButton();
			this.rb5Min = new System.Windows.Forms.RadioButton();
			this.btnGetPosition = new System.Windows.Forms.Button();
			this.menuStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnPulseNorth
			// 
			this.btnPulseNorth.Location = new System.Drawing.Point(92, 101);
			this.btnPulseNorth.Name = "btnPulseNorth";
			this.btnPulseNorth.Size = new System.Drawing.Size(25, 23);
			this.btnPulseNorth.TabIndex = 0;
			this.btnPulseNorth.Text = "/\\";
			this.btnPulseNorth.UseVisualStyleBackColor = true;
			this.btnPulseNorth.Click += new System.EventHandler(this.btnPulseNorth_Click);
			// 
			// btnPulseSouth
			// 
			this.btnPulseSouth.Location = new System.Drawing.Point(92, 159);
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
			this.btnPulseWest.Location = new System.Drawing.Point(59, 128);
			this.btnPulseWest.Name = "btnPulseWest";
			this.btnPulseWest.Size = new System.Drawing.Size(25, 23);
			this.btnPulseWest.TabIndex = 2;
			this.btnPulseWest.Text = "<";
			this.btnPulseWest.UseVisualStyleBackColor = true;
			this.btnPulseWest.Click += new System.EventHandler(this.btnPulseWest_Click);
			// 
			// btnPulseEast
			// 
			this.btnPulseEast.Location = new System.Drawing.Point(125, 128);
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
			this.menuStrip1.Size = new System.Drawing.Size(221, 24);
			this.menuStrip1.TabIndex = 13;
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
			// actionToolStripMenuItem
			// 
			this.actionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSlew,
            this.miSyncPosition,
            this.miCalibratePulseGuiding});
			this.actionToolStripMenuItem.Name = "actionToolStripMenuItem";
			this.actionToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
			this.actionToolStripMenuItem.Text = "&Action";
			// 
			// miSlew
			// 
			this.miSlew.Name = "miSlew";
			this.miSlew.Size = new System.Drawing.Size(197, 22);
			this.miSlew.Text = "&Slew To Coordinates";
			this.miSlew.Click += new System.EventHandler(this.miSlew_Click);
			// 
			// miSyncPosition
			// 
			this.miSyncPosition.Name = "miSyncPosition";
			this.miSyncPosition.Size = new System.Drawing.Size(197, 22);
			this.miSyncPosition.Text = "S&ync";
			this.miSyncPosition.Click += new System.EventHandler(this.miSyncPosition_Click);
			// 
			// miCalibratePulseGuiding
			// 
			this.miCalibratePulseGuiding.Name = "miCalibratePulseGuiding";
			this.miCalibratePulseGuiding.Size = new System.Drawing.Size(197, 22);
			this.miCalibratePulseGuiding.Text = "&Calibrate Pulse Guiding";
			this.miCalibratePulseGuiding.Click += new System.EventHandler(this.miCalibratePulseGuiding_Click);
			// 
			// rbSlowest
			// 
			this.rbSlowest.AutoSize = true;
			this.rbSlowest.Checked = true;
			this.rbSlowest.Location = new System.Drawing.Point(13, 33);
			this.rbSlowest.Name = "rbSlowest";
			this.rbSlowest.Size = new System.Drawing.Size(39, 17);
			this.rbSlowest.TabIndex = 14;
			this.rbSlowest.TabStop = true;
			this.rbSlowest.Text = "G1";
			this.rbSlowest.UseVisualStyleBackColor = true;
			// 
			// rbFast
			// 
			this.rbFast.AutoSize = true;
			this.rbFast.Location = new System.Drawing.Point(111, 33);
			this.rbFast.Name = "rbFast";
			this.rbFast.Size = new System.Drawing.Size(39, 17);
			this.rbFast.TabIndex = 15;
			this.rbFast.Text = "G3";
			this.rbFast.UseVisualStyleBackColor = true;
			// 
			// rbSlow
			// 
			this.rbSlow.AutoSize = true;
			this.rbSlow.Location = new System.Drawing.Point(60, 33);
			this.rbSlow.Name = "rbSlow";
			this.rbSlow.Size = new System.Drawing.Size(39, 17);
			this.rbSlow.TabIndex = 16;
			this.rbSlow.Text = "G2";
			this.rbSlow.UseVisualStyleBackColor = true;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tssRA,
            this.toolStripStatusLabel3,
            this.tssDE});
			this.statusStrip1.Location = new System.Drawing.Point(0, 210);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(221, 22);
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
			this.tssRA.Size = new System.Drawing.Size(66, 17);
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
			this.tssDE.Size = new System.Drawing.Size(64, 17);
			this.tssDE.Text = "+--° --\' --\"";
			// 
			// rb1Min
			// 
			this.rb1Min.AutoSize = true;
			this.rb1Min.Location = new System.Drawing.Point(156, 33);
			this.rb1Min.Name = "rb1Min";
			this.rb1Min.Size = new System.Drawing.Size(33, 17);
			this.rb1Min.TabIndex = 18;
			this.rb1Min.Text = "1\'";
			this.rb1Min.UseVisualStyleBackColor = true;
			// 
			// rb60min
			// 
			this.rb60min.AutoSize = true;
			this.rb60min.Location = new System.Drawing.Point(156, 57);
			this.rb60min.Name = "rb60min";
			this.rb60min.Size = new System.Drawing.Size(52, 17);
			this.rb60min.TabIndex = 22;
			this.rb60min.Text = "1 deg";
			this.rb60min.UseVisualStyleBackColor = true;
			// 
			// rb10Min
			// 
			this.rb10Min.AutoSize = true;
			this.rb10Min.Location = new System.Drawing.Point(60, 57);
			this.rb10Min.Name = "rb10Min";
			this.rb10Min.Size = new System.Drawing.Size(39, 17);
			this.rb10Min.TabIndex = 21;
			this.rb10Min.Text = "10\'";
			this.rb10Min.UseVisualStyleBackColor = true;
			// 
			// rb30Min
			// 
			this.rb30Min.AutoSize = true;
			this.rb30Min.Location = new System.Drawing.Point(111, 57);
			this.rb30Min.Name = "rb30Min";
			this.rb30Min.Size = new System.Drawing.Size(39, 17);
			this.rb30Min.TabIndex = 20;
			this.rb30Min.Text = "30\'";
			this.rb30Min.UseVisualStyleBackColor = true;
			// 
			// rb5Min
			// 
			this.rb5Min.AutoSize = true;
			this.rb5Min.Location = new System.Drawing.Point(13, 57);
			this.rb5Min.Name = "rb5Min";
			this.rb5Min.Size = new System.Drawing.Size(33, 17);
			this.rb5Min.TabIndex = 19;
			this.rb5Min.Text = "5\'";
			this.rb5Min.UseVisualStyleBackColor = true;
			// 
			// btnGetPosition
			// 
			this.btnGetPosition.Location = new System.Drawing.Point(92, 128);
			this.btnGetPosition.Name = "btnGetPosition";
			this.btnGetPosition.Size = new System.Drawing.Size(25, 23);
			this.btnGetPosition.TabIndex = 23;
			this.btnGetPosition.Text = "?";
			this.btnGetPosition.UseVisualStyleBackColor = true;
			this.btnGetPosition.Click += new System.EventHandler(this.btnGetPosition_Click);
			// 
			// frmTelescopeControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(221, 232);
			this.Controls.Add(this.btnGetPosition);
			this.Controls.Add(this.rb60min);
			this.Controls.Add(this.rb10Min);
			this.Controls.Add(this.rb30Min);
			this.Controls.Add(this.rb5Min);
			this.Controls.Add(this.rb1Min);
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
        private System.Windows.Forms.ToolStripMenuItem miSlew;
		private System.Windows.Forms.ToolStripMenuItem miSyncPosition;
		private System.Windows.Forms.RadioButton rb1Min;
		private System.Windows.Forms.RadioButton rb60min;
		private System.Windows.Forms.RadioButton rb10Min;
		private System.Windows.Forms.RadioButton rb30Min;
		private System.Windows.Forms.RadioButton rb5Min;
		private System.Windows.Forms.Button btnGetPosition;
    }
}