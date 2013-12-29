namespace OccuRec.Config.Panels
{
    partial class ucTelescope
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
            this.label1 = new System.Windows.Forms.Label();
            this.nudTelescopePingRate = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudTelescopePingRate)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(230, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "sec";
            // 
            // nudTelescopePingRate
            // 
            this.nudTelescopePingRate.Location = new System.Drawing.Point(172, 22);
            this.nudTelescopePingRate.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudTelescopePingRate.Name = "nudTelescopePingRate";
            this.nudTelescopePingRate.Size = new System.Drawing.Size(52, 20);
            this.nudTelescopePingRate.TabIndex = 6;
            this.nudTelescopePingRate.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Position Check Frequency";
            // 
            // ucTelescope
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudTelescopePingRate);
            this.Controls.Add(this.label2);
            this.Name = "ucTelescope";
            this.Size = new System.Drawing.Size(411, 248);
            ((System.ComponentModel.ISupportInitialize)(this.nudTelescopePingRate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudTelescopePingRate;
        private System.Windows.Forms.Label label2;
    }
}
