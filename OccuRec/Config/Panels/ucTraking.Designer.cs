namespace OccuRec.Config.Panels
{
	partial class ucTraking
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
			this.label2 = new System.Windows.Forms.Label();
			this.cbxTrackingFrequency = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.nudMinCertGuidingStar = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.nudMinCertFixedObject = new System.Windows.Forms.NumericUpDown();
			this.label14 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.nudMinCertGuidingStar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinCertFixedObject)).BeginInit();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(18, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 13);
			this.label2.TabIndex = 64;
			this.label2.Text = "Track every ";
			// 
			// cbxTrackingFrequency
			// 
			this.cbxTrackingFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxTrackingFrequency.FormattingEnabled = true;
			this.cbxTrackingFrequency.Items.AddRange(new object[] {
            "frame",
            "2-nd frame",
            "3-rd frame",
            "4-th frame",
            "5-th frame",
            "10-th frame",
            "20-th frame"});
			this.cbxTrackingFrequency.Location = new System.Drawing.Point(91, 15);
			this.cbxTrackingFrequency.Name = "cbxTrackingFrequency";
			this.cbxTrackingFrequency.Size = new System.Drawing.Size(96, 21);
			this.cbxTrackingFrequency.TabIndex = 65;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(18, 74);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(171, 13);
			this.label4.TabIndex = 66;
			this.label4.Text = "Minimum Certainty for Guiding Star:";
			// 
			// nudMinCertGuidingStar
			// 
			this.nudMinCertGuidingStar.DecimalPlaces = 1;
			this.nudMinCertGuidingStar.Location = new System.Drawing.Point(195, 72);
			this.nudMinCertGuidingStar.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.nudMinCertGuidingStar.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.nudMinCertGuidingStar.Name = "nudMinCertGuidingStar";
			this.nudMinCertGuidingStar.Size = new System.Drawing.Size(51, 20);
			this.nudMinCertGuidingStar.TabIndex = 67;
			this.nudMinCertGuidingStar.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(18, 106);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(167, 13);
			this.label5.TabIndex = 68;
			this.label5.Text = "Force a Fixed Object for Certainty:";
			// 
			// nudMinCertFixedObject
			// 
			this.nudMinCertFixedObject.DecimalPlaces = 1;
			this.nudMinCertFixedObject.Location = new System.Drawing.Point(195, 104);
			this.nudMinCertFixedObject.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.nudMinCertFixedObject.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.nudMinCertFixedObject.Name = "nudMinCertFixedObject";
			this.nudMinCertFixedObject.Size = new System.Drawing.Size(51, 20);
			this.nudMinCertFixedObject.TabIndex = 69;
			this.nudMinCertFixedObject.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(249, 106);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(56, 13);
			this.label14.TabIndex = 78;
			this.label14.Text = "and below";
			// 
			// ucTraking
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label14);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.nudMinCertFixedObject);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.nudMinCertGuidingStar);
			this.Controls.Add(this.cbxTrackingFrequency);
			this.Controls.Add(this.label2);
			this.Name = "ucTraking";
			this.Size = new System.Drawing.Size(493, 289);
			((System.ComponentModel.ISupportInitialize)(this.nudMinCertGuidingStar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinCertFixedObject)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cbxTrackingFrequency;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown nudMinCertGuidingStar;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown nudMinCertFixedObject;
		private System.Windows.Forms.Label label14;
	}
}
