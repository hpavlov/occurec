namespace OccuRec.ASCOM
{
    partial class frmTelescopeControl
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
            this.btnPulseNorth = new System.Windows.Forms.Button();
            this.btnPulseSouth = new System.Windows.Forms.Button();
            this.btnPulseWest = new System.Windows.Forms.Button();
            this.btnPulseEast = new System.Windows.Forms.Button();
            this.btnPulseEast2 = new System.Windows.Forms.Button();
            this.btnPulseEast3 = new System.Windows.Forms.Button();
            this.btnPulseWest2 = new System.Windows.Forms.Button();
            this.btnPulseWest3 = new System.Windows.Forms.Button();
            this.btnPulseNorth2 = new System.Windows.Forms.Button();
            this.btnPulseNorth3 = new System.Windows.Forms.Button();
            this.btnPulseSouth2 = new System.Windows.Forms.Button();
            this.btnPulseSouth3 = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPulseNorth
            // 
            this.btnPulseNorth.Location = new System.Drawing.Point(133, 69);
            this.btnPulseNorth.Name = "btnPulseNorth";
            this.btnPulseNorth.Size = new System.Drawing.Size(30, 23);
            this.btnPulseNorth.TabIndex = 0;
            this.btnPulseNorth.Text = "/\\";
            this.btnPulseNorth.UseVisualStyleBackColor = true;
            this.btnPulseNorth.Click += new System.EventHandler(this.btnPulseNorth_Click);
            // 
            // btnPulseSouth
            // 
            this.btnPulseSouth.Location = new System.Drawing.Point(133, 145);
            this.btnPulseSouth.Name = "btnPulseSouth";
            this.btnPulseSouth.Size = new System.Drawing.Size(30, 23);
            this.btnPulseSouth.TabIndex = 1;
            this.btnPulseSouth.Tag = "";
            this.btnPulseSouth.Text = "\\/";
            this.btnPulseSouth.UseVisualStyleBackColor = true;
            this.btnPulseSouth.Click += new System.EventHandler(this.btnPulseSouth_Click);
            // 
            // btnPulseWest
            // 
            this.btnPulseWest.Location = new System.Drawing.Point(90, 107);
            this.btnPulseWest.Name = "btnPulseWest";
            this.btnPulseWest.Size = new System.Drawing.Size(30, 23);
            this.btnPulseWest.TabIndex = 2;
            this.btnPulseWest.Text = "<";
            this.btnPulseWest.UseVisualStyleBackColor = true;
            this.btnPulseWest.Click += new System.EventHandler(this.btnPulseWest_Click);
            // 
            // btnPulseEast
            // 
            this.btnPulseEast.Location = new System.Drawing.Point(169, 107);
            this.btnPulseEast.Name = "btnPulseEast";
            this.btnPulseEast.Size = new System.Drawing.Size(30, 23);
            this.btnPulseEast.TabIndex = 3;
            this.btnPulseEast.Text = ">";
            this.btnPulseEast.UseVisualStyleBackColor = true;
            this.btnPulseEast.Click += new System.EventHandler(this.btnPulseEast_Click);
            // 
            // btnPulseEast2
            // 
            this.btnPulseEast2.Location = new System.Drawing.Point(205, 104);
            this.btnPulseEast2.Name = "btnPulseEast2";
            this.btnPulseEast2.Size = new System.Drawing.Size(30, 27);
            this.btnPulseEast2.TabIndex = 4;
            this.btnPulseEast2.Text = ">>";
            this.btnPulseEast2.UseVisualStyleBackColor = true;
            this.btnPulseEast2.Click += new System.EventHandler(this.btnPulseEast2_Click);
            // 
            // btnPulseEast3
            // 
            this.btnPulseEast3.Location = new System.Drawing.Point(241, 99);
            this.btnPulseEast3.Name = "btnPulseEast3";
            this.btnPulseEast3.Size = new System.Drawing.Size(36, 36);
            this.btnPulseEast3.TabIndex = 5;
            this.btnPulseEast3.Text = ">>>";
            this.btnPulseEast3.UseVisualStyleBackColor = true;
            this.btnPulseEast3.Click += new System.EventHandler(this.btnPulseEast3_Click);
            // 
            // btnPulseWest2
            // 
            this.btnPulseWest2.Location = new System.Drawing.Point(54, 105);
            this.btnPulseWest2.Name = "btnPulseWest2";
            this.btnPulseWest2.Size = new System.Drawing.Size(30, 27);
            this.btnPulseWest2.TabIndex = 6;
            this.btnPulseWest2.Text = "<<";
            this.btnPulseWest2.UseVisualStyleBackColor = true;
            this.btnPulseWest2.Click += new System.EventHandler(this.btnPulseWest2_Click);
            // 
            // btnPulseWest3
            // 
            this.btnPulseWest3.Location = new System.Drawing.Point(12, 100);
            this.btnPulseWest3.Name = "btnPulseWest3";
            this.btnPulseWest3.Size = new System.Drawing.Size(36, 36);
            this.btnPulseWest3.TabIndex = 7;
            this.btnPulseWest3.Text = "<<<";
            this.btnPulseWest3.UseVisualStyleBackColor = true;
            this.btnPulseWest3.Click += new System.EventHandler(this.btnPulseWest3_Click);
            // 
            // btnPulseNorth2
            // 
            this.btnPulseNorth2.Location = new System.Drawing.Point(130, 40);
            this.btnPulseNorth2.Name = "btnPulseNorth2";
            this.btnPulseNorth2.Size = new System.Drawing.Size(38, 23);
            this.btnPulseNorth2.TabIndex = 8;
            this.btnPulseNorth2.Text = "/\\/\\";
            this.btnPulseNorth2.UseVisualStyleBackColor = true;
            this.btnPulseNorth2.Click += new System.EventHandler(this.btnPulseNorth2_Click);
            // 
            // btnPulseNorth3
            // 
            this.btnPulseNorth3.Location = new System.Drawing.Point(124, 11);
            this.btnPulseNorth3.Name = "btnPulseNorth3";
            this.btnPulseNorth3.Size = new System.Drawing.Size(50, 23);
            this.btnPulseNorth3.TabIndex = 9;
            this.btnPulseNorth3.Text = "/\\/\\/\\";
            this.btnPulseNorth3.UseVisualStyleBackColor = true;
            this.btnPulseNorth3.Click += new System.EventHandler(this.btnPulseNorth3_Click);
            // 
            // btnPulseSouth2
            // 
            this.btnPulseSouth2.Location = new System.Drawing.Point(130, 174);
            this.btnPulseSouth2.Name = "btnPulseSouth2";
            this.btnPulseSouth2.Size = new System.Drawing.Size(38, 23);
            this.btnPulseSouth2.TabIndex = 10;
            this.btnPulseSouth2.Text = "\\/\\/";
            this.btnPulseSouth2.UseVisualStyleBackColor = true;
            this.btnPulseSouth2.Click += new System.EventHandler(this.btnPulseSouth2_Click);
            // 
            // btnPulseSouth3
            // 
            this.btnPulseSouth3.Location = new System.Drawing.Point(124, 203);
            this.btnPulseSouth3.Name = "btnPulseSouth3";
            this.btnPulseSouth3.Size = new System.Drawing.Size(50, 23);
            this.btnPulseSouth3.TabIndex = 11;
            this.btnPulseSouth3.Text = "\\/\\/\\/";
            this.btnPulseSouth3.UseVisualStyleBackColor = true;
            this.btnPulseSouth3.Click += new System.EventHandler(this.btnPulseSouth3_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(12, 278);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(75, 23);
            this.btnDisconnect.TabIndex = 12;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // frmTelescopeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 313);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.btnPulseSouth3);
            this.Controls.Add(this.btnPulseSouth2);
            this.Controls.Add(this.btnPulseNorth3);
            this.Controls.Add(this.btnPulseNorth2);
            this.Controls.Add(this.btnPulseWest3);
            this.Controls.Add(this.btnPulseWest2);
            this.Controls.Add(this.btnPulseEast3);
            this.Controls.Add(this.btnPulseEast2);
            this.Controls.Add(this.btnPulseEast);
            this.Controls.Add(this.btnPulseWest);
            this.Controls.Add(this.btnPulseSouth);
            this.Controls.Add(this.btnPulseNorth);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmTelescopeControl";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Telescope Control";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPulseNorth;
        private System.Windows.Forms.Button btnPulseSouth;
        private System.Windows.Forms.Button btnPulseWest;
        private System.Windows.Forms.Button btnPulseEast;
        private System.Windows.Forms.Button btnPulseEast2;
        private System.Windows.Forms.Button btnPulseEast3;
        private System.Windows.Forms.Button btnPulseWest2;
        private System.Windows.Forms.Button btnPulseWest3;
        private System.Windows.Forms.Button btnPulseNorth2;
        private System.Windows.Forms.Button btnPulseNorth3;
        private System.Windows.Forms.Button btnPulseSouth2;
        private System.Windows.Forms.Button btnPulseSouth3;
        private System.Windows.Forms.Button btnDisconnect;
    }
}