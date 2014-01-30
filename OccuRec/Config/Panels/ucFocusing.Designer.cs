namespace OccuRec.Config.Panels
{
    partial class ucFocusing
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudFocuserSmallestStep = new System.Windows.Forms.NumericUpDown();
            this.nudFocuserLargeStep = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nudFocuserSmallStep = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxTempIn = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFocuserSmallestStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFocuserLargeStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFocuserSmallStep)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.nudFocuserSmallestStep);
            this.groupBox1.Controls.Add(this.nudFocuserLargeStep);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.nudFocuserSmallStep);
            this.groupBox1.Location = new System.Drawing.Point(18, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 103);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Focus Control Steps";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Smallest Step";
            // 
            // nudFocuserSmallestStep
            // 
            this.nudFocuserSmallestStep.Location = new System.Drawing.Point(94, 77);
            this.nudFocuserSmallestStep.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudFocuserSmallestStep.Name = "nudFocuserSmallestStep";
            this.nudFocuserSmallestStep.Size = new System.Drawing.Size(58, 20);
            this.nudFocuserSmallestStep.TabIndex = 5;
            // 
            // nudFocuserLargeStep
            // 
            this.nudFocuserLargeStep.Location = new System.Drawing.Point(94, 25);
            this.nudFocuserLargeStep.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudFocuserLargeStep.Name = "nudFocuserLargeStep";
            this.nudFocuserLargeStep.Size = new System.Drawing.Size(58, 20);
            this.nudFocuserLargeStep.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Small Step";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Large Step";
            // 
            // nudFocuserSmallStep
            // 
            this.nudFocuserSmallStep.Location = new System.Drawing.Point(94, 51);
            this.nudFocuserSmallStep.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudFocuserSmallStep.Name = "nudFocuserSmallStep";
            this.nudFocuserSmallStep.Size = new System.Drawing.Size(58, 20);
            this.nudFocuserSmallStep.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(224, 13);
            this.label4.TabIndex = 33;
            this.label4.Text = "The current driver returns the temperature in ° ";
            // 
            // cbxTempIn
            // 
            this.cbxTempIn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTempIn.FormattingEnabled = true;
            this.cbxTempIn.Items.AddRange(new object[] {
            "C",
            "F"});
            this.cbxTempIn.Location = new System.Drawing.Point(237, 120);
            this.cbxTempIn.Name = "cbxTempIn";
            this.cbxTempIn.Size = new System.Drawing.Size(39, 21);
            this.cbxTempIn.TabIndex = 34;
            // 
            // ucFocusing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbxTempIn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Name = "ucFocusing";
            this.Size = new System.Drawing.Size(435, 252);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFocuserSmallestStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFocuserLargeStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFocuserSmallStep)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown nudFocuserLargeStep;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudFocuserSmallStep;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudFocuserSmallestStep;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbxTempIn;
    }
}
