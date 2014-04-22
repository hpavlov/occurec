namespace OccuRec
{
	partial class frmOneStacking
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOneStacking));
			this.label1 = new System.Windows.Forms.Label();
			this.cbxStackRate = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lblEffectiveTimeResolution = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnRemoveStacking = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(13, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(293, 86);
			this.label1.TabIndex = 0;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// cbxStackRate
			// 
			this.cbxStackRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxStackRate.FormattingEnabled = true;
			this.cbxStackRate.Items.AddRange(new object[] {
            "2",
            "4",
            "8",
            "16",
            "32",
            "64",
            "128",
            "256"});
			this.cbxStackRate.Location = new System.Drawing.Point(52, 111);
			this.cbxStackRate.Name = "cbxStackRate";
			this.cbxStackRate.Size = new System.Drawing.Size(70, 21);
			this.cbxStackRate.TabIndex = 1;
			this.cbxStackRate.SelectedIndexChanged += new System.EventHandler(this.cbxStackRate_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 115);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Stack ";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 142);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(168, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Effective maximum time resolution:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(129, 115);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(185, 13);
			this.label4.TabIndex = 4;
			this.label4.Text = "video frame(s) into one recorded fame";
			// 
			// lblEffectiveTimeResolution
			// 
			this.lblEffectiveTimeResolution.AutoSize = true;
			this.lblEffectiveTimeResolution.ForeColor = System.Drawing.Color.Navy;
			this.lblEffectiveTimeResolution.Location = new System.Drawing.Point(181, 143);
			this.lblEffectiveTimeResolution.Name = "lblEffectiveTimeResolution";
			this.lblEffectiveTimeResolution.Size = new System.Drawing.Size(48, 13);
			this.lblEffectiveTimeResolution.TabIndex = 5;
			this.lblEffectiveTimeResolution.Text = "0.16 sec";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(154, 186);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 6;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(235, 186);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnRemoveStacking
			// 
			this.btnRemoveStacking.DialogResult = System.Windows.Forms.DialogResult.Ignore;
			this.btnRemoveStacking.Location = new System.Drawing.Point(16, 186);
			this.btnRemoveStacking.Name = "btnRemoveStacking";
			this.btnRemoveStacking.Size = new System.Drawing.Size(106, 23);
			this.btnRemoveStacking.TabIndex = 8;
			this.btnRemoveStacking.Text = "Remove Stacking";
			this.btnRemoveStacking.UseVisualStyleBackColor = true;
			this.btnRemoveStacking.Visible = false;
			// 
			// frmOneStacking
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(324, 221);
			this.Controls.Add(this.btnRemoveStacking);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.lblEffectiveTimeResolution);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cbxStackRate);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmOneStacking";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "x1 Integration Rate - Stacking";
			this.Load += new System.EventHandler(this.frmOneStacking_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cbxStackRate;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lblEffectiveTimeResolution;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnRemoveStacking;
	}
}