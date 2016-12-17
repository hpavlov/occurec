namespace OccuRec.Drivers.QHYVideo
{
    partial class frmChooseQHYCamera
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
            this.cbxQHYCamera = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxBinning = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbxBPP = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxTiming = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbxCooling = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbxQHYCamera
            // 
            this.cbxQHYCamera.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxQHYCamera.FormattingEnabled = true;
            this.cbxQHYCamera.Location = new System.Drawing.Point(12, 38);
            this.cbxQHYCamera.Name = "cbxQHYCamera";
            this.cbxQHYCamera.Size = new System.Drawing.Size(328, 21);
            this.cbxQHYCamera.TabIndex = 0;
            this.cbxQHYCamera.SelectedIndexChanged += new System.EventHandler(this.cbxQHYCamera_SelectedIndexChanged);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(184, 141);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(265, 141);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Detected QHY CCD Cameras";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Binning:";
            // 
            // cbxBinning
            // 
            this.cbxBinning.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBinning.Enabled = false;
            this.cbxBinning.FormattingEnabled = true;
            this.cbxBinning.Location = new System.Drawing.Point(15, 95);
            this.cbxBinning.Name = "cbxBinning";
            this.cbxBinning.Size = new System.Drawing.Size(58, 21);
            this.cbxBinning.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(86, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Bpp:";
            // 
            // cbxBPP
            // 
            this.cbxBPP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBPP.Enabled = false;
            this.cbxBPP.FormattingEnabled = true;
            this.cbxBPP.Location = new System.Drawing.Point(89, 95);
            this.cbxBPP.Name = "cbxBPP";
            this.cbxBPP.Size = new System.Drawing.Size(58, 21);
            this.cbxBPP.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(160, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Timing:";
            // 
            // cbxTiming
            // 
            this.cbxTiming.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTiming.Enabled = false;
            this.cbxTiming.FormattingEnabled = true;
            this.cbxTiming.Location = new System.Drawing.Point(163, 95);
            this.cbxTiming.Name = "cbxTiming";
            this.cbxTiming.Size = new System.Drawing.Size(58, 21);
            this.cbxTiming.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(238, 79);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Cooling:";
            // 
            // cbxCooling
            // 
            this.cbxCooling.AutoSize = true;
            this.cbxCooling.Checked = true;
            this.cbxCooling.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxCooling.Location = new System.Drawing.Point(241, 99);
            this.cbxCooling.Name = "cbxCooling";
            this.cbxCooling.Size = new System.Drawing.Size(65, 17);
            this.cbxCooling.TabIndex = 18;
            this.cbxCooling.Text = "Enabled";
            this.cbxCooling.UseVisualStyleBackColor = true;
            // 
            // frmChooseQHYCamera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 176);
            this.Controls.Add(this.cbxCooling);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbxTiming);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbxBPP);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbxBinning);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.cbxQHYCamera);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmChooseQHYCamera";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose QHY Camera";
            this.Load += new System.EventHandler(this.frmChooseQHYCamera_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbxQHYCamera;
        private System.Windows.Forms.ComboBox cbxBinning;
        private System.Windows.Forms.ComboBox cbxBPP;
        private System.Windows.Forms.ComboBox cbxTiming;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbxCooling;
    }
}