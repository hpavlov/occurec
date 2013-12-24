namespace OccuRec.ASCOM
{
	partial class frmFocusControl
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
			this.cbxTempComp = new System.Windows.Forms.CheckBox();
			this.lblAbsRel = new System.Windows.Forms.Label();
			this.pnlFocuserControls = new System.Windows.Forms.Panel();
			this.lblTemp = new System.Windows.Forms.Label();
			this.btnMoveIn = new System.Windows.Forms.Button();
			this.btnMoveOut = new System.Windows.Forms.Button();
			this.lblPosition = new System.Windows.Forms.Label();
			this.pnlFocuserControls.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbxTempComp
			// 
			this.cbxTempComp.AutoSize = true;
			this.cbxTempComp.Location = new System.Drawing.Point(83, 8);
			this.cbxTempComp.Name = "cbxTempComp";
			this.cbxTempComp.Size = new System.Drawing.Size(156, 17);
			this.cbxTempComp.TabIndex = 0;
			this.cbxTempComp.Text = "Temperature Compensation";
			this.cbxTempComp.UseVisualStyleBackColor = true;
			// 
			// lblAbsRel
			// 
			this.lblAbsRel.AutoSize = true;
			this.lblAbsRel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblAbsRel.Location = new System.Drawing.Point(9, 9);
			this.lblAbsRel.Name = "lblAbsRel";
			this.lblAbsRel.Size = new System.Drawing.Size(56, 13);
			this.lblAbsRel.TabIndex = 1;
			this.lblAbsRel.Text = "Absolute";
			// 
			// pnlFocuserControls
			// 
			this.pnlFocuserControls.Controls.Add(this.lblPosition);
			this.pnlFocuserControls.Controls.Add(this.btnMoveOut);
			this.pnlFocuserControls.Controls.Add(this.btnMoveIn);
			this.pnlFocuserControls.Controls.Add(this.lblTemp);
			this.pnlFocuserControls.Controls.Add(this.cbxTempComp);
			this.pnlFocuserControls.Controls.Add(this.lblAbsRel);
			this.pnlFocuserControls.Location = new System.Drawing.Point(3, 12);
			this.pnlFocuserControls.Name = "pnlFocuserControls";
			this.pnlFocuserControls.Size = new System.Drawing.Size(313, 166);
			this.pnlFocuserControls.TabIndex = 2;
			// 
			// lblTemp
			// 
			this.lblTemp.AutoSize = true;
			this.lblTemp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblTemp.Location = new System.Drawing.Point(257, 10);
			this.lblTemp.Name = "lblTemp";
			this.lblTemp.Size = new System.Drawing.Size(36, 13);
			this.lblTemp.TabIndex = 2;
			this.lblTemp.Text = "12.3 ";
			// 
			// btnMoveIn
			// 
			this.btnMoveIn.Location = new System.Drawing.Point(72, 125);
			this.btnMoveIn.Name = "btnMoveIn";
			this.btnMoveIn.Size = new System.Drawing.Size(38, 23);
			this.btnMoveIn.TabIndex = 3;
			this.btnMoveIn.Tag = "";
			this.btnMoveIn.Text = "In";
			this.btnMoveIn.UseVisualStyleBackColor = true;
			this.btnMoveIn.Click += new System.EventHandler(this.btnMoveIn_Click);
			// 
			// btnMoveOut
			// 
			this.btnMoveOut.Location = new System.Drawing.Point(181, 125);
			this.btnMoveOut.Name = "btnMoveOut";
			this.btnMoveOut.Size = new System.Drawing.Size(38, 23);
			this.btnMoveOut.TabIndex = 4;
			this.btnMoveOut.Tag = "\t\t\t\t\t{\r\n\t\t\t\t\t\t\ttry\r\n\t\t\t\t\t\t\t{\r\n\t\t\t\t\t\t\t\tcallback(state);\r\n\t\t\t\t\t\t\t}\r\n\t\t\t\t\t\t\tcatch (E" +
    "xception ex)\r\n\t\t\t\t\t\t\t{\r\n\t\t\t\t\t\t\t\tTrace.WriteLine(ex.GetFullStackTrace());\r\n\t\t\t\t\t\t" +
    "\t}\t\t\t\t\t\t\t\r\n\t\t\t\t\t\t}";
			this.btnMoveOut.Text = "Out";
			this.btnMoveOut.UseVisualStyleBackColor = true;
			this.btnMoveOut.Click += new System.EventHandler(this.btnMoveOut_Click);
			// 
			// lblPosition
			// 
			this.lblPosition.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblPosition.ForeColor = System.Drawing.Color.Blue;
			this.lblPosition.Location = new System.Drawing.Point(116, 130);
			this.lblPosition.Name = "lblPosition";
			this.lblPosition.Size = new System.Drawing.Size(59, 13);
			this.lblPosition.TabIndex = 5;
			this.lblPosition.Text = "123456";
			this.lblPosition.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// frmFocusControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(325, 190);
			this.Controls.Add(this.pnlFocuserControls);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmFocusControl";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Focus Control";
			this.TopMost = true;
			this.Shown += new System.EventHandler(this.frmFocusControl_Shown);
			this.pnlFocuserControls.ResumeLayout(false);
			this.pnlFocuserControls.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox cbxTempComp;
		private System.Windows.Forms.Label lblAbsRel;
		private System.Windows.Forms.Panel pnlFocuserControls;
		private System.Windows.Forms.Label lblTemp;
		private System.Windows.Forms.Button btnMoveOut;
		private System.Windows.Forms.Button btnMoveIn;
		private System.Windows.Forms.Label lblPosition;
	}
}