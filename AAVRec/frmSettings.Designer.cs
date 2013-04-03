namespace AAVRec
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblArea1Config = new System.Windows.Forms.Label();
            this.lblArea2Config = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.tabIntegration = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.tabIOTAVTI = new System.Windows.Forms.TabPage();
            this.btnSave = new System.Windows.Forms.Button();
            this.nudSignDiffFactor = new System.Windows.Forms.NumericUpDown();
            this.cbxGraphDebugMode = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabIntegration.SuspendLayout();
            this.tabIOTAVTI.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSignDiffFactor)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(332, 248);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(23, 165);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "Update";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "TimeStamp Area 1:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(20, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "TimeStamp Area 2:";
            // 
            // lblArea1Config
            // 
            this.lblArea1Config.AutoSize = true;
            this.lblArea1Config.Location = new System.Drawing.Point(140, 28);
            this.lblArea1Config.Name = "lblArea1Config";
            this.lblArea1Config.Size = new System.Drawing.Size(75, 13);
            this.lblArea1Config.TabIndex = 5;
            this.lblArea1Config.Text = "lblArea1Config";
            // 
            // lblArea2Config
            // 
            this.lblArea2Config.AutoSize = true;
            this.lblArea2Config.Location = new System.Drawing.Point(140, 50);
            this.lblArea2Config.Name = "lblArea2Config";
            this.lblArea2Config.Size = new System.Drawing.Size(75, 13);
            this.lblArea2Config.TabIndex = 6;
            this.lblArea2Config.Text = "lblArea2Config";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabGeneral);
            this.tabControl1.Controls.Add(this.tabIntegration);
            this.tabControl1.Controls.Add(this.tabIOTAVTI);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(399, 230);
            this.tabControl1.TabIndex = 7;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.cbxGraphDebugMode);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(391, 204);
            this.tabGeneral.TabIndex = 1;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // tabIntegration
            // 
            this.tabIntegration.Controls.Add(this.nudSignDiffFactor);
            this.tabIntegration.Controls.Add(this.label3);
            this.tabIntegration.Location = new System.Drawing.Point(4, 22);
            this.tabIntegration.Name = "tabIntegration";
            this.tabIntegration.Size = new System.Drawing.Size(391, 204);
            this.tabIntegration.TabIndex = 2;
            this.tabIntegration.Text = "Integration Detection";
            this.tabIntegration.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(21, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(168, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Signature Difference Factor:";
            // 
            // tabIOTAVTI
            // 
            this.tabIOTAVTI.Controls.Add(this.label1);
            this.tabIOTAVTI.Controls.Add(this.btnOK);
            this.tabIOTAVTI.Controls.Add(this.lblArea2Config);
            this.tabIOTAVTI.Controls.Add(this.label2);
            this.tabIOTAVTI.Controls.Add(this.lblArea1Config);
            this.tabIOTAVTI.Location = new System.Drawing.Point(4, 22);
            this.tabIOTAVTI.Name = "tabIOTAVTI";
            this.tabIOTAVTI.Padding = new System.Windows.Forms.Padding(3);
            this.tabIOTAVTI.Size = new System.Drawing.Size(391, 204);
            this.tabIOTAVTI.TabIndex = 0;
            this.tabIOTAVTI.Text = "IOTA-VTI TimeStamp OCR";
            this.tabIOTAVTI.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSave.Location = new System.Drawing.Point(251, 248);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // nudSignDiffFactor
            // 
            this.nudSignDiffFactor.DecimalPlaces = 1;
            this.nudSignDiffFactor.Location = new System.Drawing.Point(195, 22);
            this.nudSignDiffFactor.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudSignDiffFactor.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSignDiffFactor.Name = "nudSignDiffFactor";
            this.nudSignDiffFactor.Size = new System.Drawing.Size(47, 20);
            this.nudSignDiffFactor.TabIndex = 5;
            this.nudSignDiffFactor.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // cbxGraphDebugMode
            // 
            this.cbxGraphDebugMode.AutoSize = true;
            this.cbxGraphDebugMode.Location = new System.Drawing.Point(23, 33);
            this.cbxGraphDebugMode.Name = "cbxGraphDebugMode";
            this.cbxGraphDebugMode.Size = new System.Drawing.Size(150, 17);
            this.cbxGraphDebugMode.TabIndex = 0;
            this.cbxGraphDebugMode.Text = "Video Graph Debug Mode";
            this.cbxGraphDebugMode.UseVisualStyleBackColor = true;
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 283);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AAVRec Settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.tabIntegration.ResumeLayout(false);
            this.tabIntegration.PerformLayout();
            this.tabIOTAVTI.ResumeLayout(false);
            this.tabIOTAVTI.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSignDiffFactor)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblArea1Config;
        private System.Windows.Forms.Label lblArea2Config;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabIOTAVTI;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TabPage tabIntegration;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudSignDiffFactor;
        private System.Windows.Forms.CheckBox cbxGraphDebugMode;
	}
}