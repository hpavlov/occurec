namespace OccuRec
{
    partial class frmDetectIntegrationConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDetectIntegrationConfig));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn32FrameConfig = new System.Windows.Forms.Button();
            this.gbx2FrameConfig = new System.Windows.Forms.GroupBox();
            this.btn2FrameConfig = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.gbx2FrameConfig.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn32FrameConfig);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(351, 126);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "x32 Frame Configuration";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(17, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(328, 55);
            this.label1.TabIndex = 0;
            this.label1.Text = "Set the camera in mode to integrate 32 frames and press the begin configuration b" +
    "utton below";
            // 
            // btn32FrameConfig
            // 
            this.btn32FrameConfig.Location = new System.Drawing.Point(69, 88);
            this.btn32FrameConfig.Name = "btn32FrameConfig";
            this.btn32FrameConfig.Size = new System.Drawing.Size(187, 23);
            this.btn32FrameConfig.TabIndex = 1;
            this.btn32FrameConfig.Text = "Being x32 Frame Configuration";
            this.btn32FrameConfig.UseVisualStyleBackColor = true;
            // 
            // gbx2FrameConfig
            // 
            this.gbx2FrameConfig.Controls.Add(this.btn2FrameConfig);
            this.gbx2FrameConfig.Controls.Add(this.label2);
            this.gbx2FrameConfig.Enabled = false;
            this.gbx2FrameConfig.Location = new System.Drawing.Point(12, 159);
            this.gbx2FrameConfig.Name = "gbx2FrameConfig";
            this.gbx2FrameConfig.Size = new System.Drawing.Size(351, 126);
            this.gbx2FrameConfig.TabIndex = 1;
            this.gbx2FrameConfig.TabStop = false;
            this.gbx2FrameConfig.Text = "x2 Frame Configuration";
            // 
            // btn2FrameConfig
            // 
            this.btn2FrameConfig.Location = new System.Drawing.Point(69, 88);
            this.btn2FrameConfig.Name = "btn2FrameConfig";
            this.btn2FrameConfig.Size = new System.Drawing.Size(187, 23);
            this.btn2FrameConfig.TabIndex = 1;
            this.btn2FrameConfig.Text = "Being x2 Frame Configuration";
            this.btn2FrameConfig.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(17, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(328, 55);
            this.label2.TabIndex = 0;
            this.label2.Text = "Set the camera in mode to integrate 2 frames and press the begin configuration bu" +
    "tton below";
            // 
            // frmDetectIntegrationConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 336);
            this.Controls.Add(this.gbx2FrameConfig);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDetectIntegrationConfig";
            this.Text = "Detect Integration Configuration";
            this.groupBox1.ResumeLayout(false);
            this.gbx2FrameConfig.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn32FrameConfig;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbx2FrameConfig;
        private System.Windows.Forms.Button btn2FrameConfig;
        private System.Windows.Forms.Label label2;
    }
}