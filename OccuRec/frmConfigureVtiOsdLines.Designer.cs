namespace OccuRec
{
	partial class frmConfigureVtiOsdLines
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
			this.cbxUserPreserveOSDLines = new System.Windows.Forms.CheckBox();
			this.nudPreserveVTIBottomRow = new System.Windows.Forms.NumericUpDown();
			this.nudPreserveVTITopRow = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.nudPreserveVTIBottomRow)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPreserveVTITopRow)).BeginInit();
			this.SuspendLayout();
			// 
			// cbxUserPreserveOSDLines
			// 
			this.cbxUserPreserveOSDLines.AutoSize = true;
			this.cbxUserPreserveOSDLines.Checked = true;
			this.cbxUserPreserveOSDLines.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxUserPreserveOSDLines.Enabled = false;
			this.cbxUserPreserveOSDLines.Location = new System.Drawing.Point(12, 12);
			this.cbxUserPreserveOSDLines.Name = "cbxUserPreserveOSDLines";
			this.cbxUserPreserveOSDLines.Size = new System.Drawing.Size(116, 17);
			this.cbxUserPreserveOSDLines.TabIndex = 35;
			this.cbxUserPreserveOSDLines.Text = "VTI Preserve Lines";
			this.cbxUserPreserveOSDLines.UseVisualStyleBackColor = true;
			// 
			// nudPreserveVTIBottomRow
			// 
			this.nudPreserveVTIBottomRow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.nudPreserveVTIBottomRow.Location = new System.Drawing.Point(136, 41);
			this.nudPreserveVTIBottomRow.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
			this.nudPreserveVTIBottomRow.Name = "nudPreserveVTIBottomRow";
			this.nudPreserveVTIBottomRow.Size = new System.Drawing.Size(49, 20);
			this.nudPreserveVTIBottomRow.TabIndex = 25;
			this.nudPreserveVTIBottomRow.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
			this.nudPreserveVTIBottomRow.ValueChanged += new System.EventHandler(this.nudPreserveVTIBottomRow_ValueChanged);
			// 
			// nudPreserveVTITopRow
			// 
			this.nudPreserveVTITopRow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.nudPreserveVTITopRow.Location = new System.Drawing.Point(60, 41);
			this.nudPreserveVTITopRow.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
			this.nudPreserveVTITopRow.Name = "nudPreserveVTITopRow";
			this.nudPreserveVTITopRow.Size = new System.Drawing.Size(50, 20);
			this.nudPreserveVTITopRow.TabIndex = 24;
			this.nudPreserveVTITopRow.Value = new decimal(new int[] {
            542,
            0,
            0,
            0});
			this.nudPreserveVTITopRow.ValueChanged += new System.EventHandler(this.nudPreserveVTITopRow_ValueChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(117, 44);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(16, 13);
			this.label3.TabIndex = 27;
			this.label3.Text = "to";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(33, 43);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(27, 13);
			this.label1.TabIndex = 36;
			this.label1.Text = "from";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(62, 84);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 37;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(143, 84);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 38;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// frmConfigureVtiOsdLines
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(285, 128);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.nudPreserveVTIBottomRow);
			this.Controls.Add(this.nudPreserveVTITopRow);
			this.Controls.Add(this.cbxUserPreserveOSDLines);
			this.Controls.Add(this.label3);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmConfigureVtiOsdLines";
			this.Text = "Configure VTI-OSD Lines";
			this.Load += new System.EventHandler(this.frmConfigureVtiOsdLines_Load);
			((System.ComponentModel.ISupportInitialize)(this.nudPreserveVTIBottomRow)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPreserveVTITopRow)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox cbxUserPreserveOSDLines;
		private System.Windows.Forms.NumericUpDown nudPreserveVTIBottomRow;
		private System.Windows.Forms.NumericUpDown nudPreserveVTITopRow;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
	}
}