namespace ASCOM.GenericCCDCamera
{
	partial class SetupDialogForm
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
			this.cmdOK = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.picASCOM = new System.Windows.Forms.PictureBox();
			this.tbxCameraProgId = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btnChooseCCDCamera = new System.Windows.Forms.Button();
			this.chkTrace = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
			this.SuspendLayout();
			// 
			// cmdOK
			// 
			this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOK.Location = new System.Drawing.Point(289, 154);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(59, 24);
			this.cmdOK.TabIndex = 0;
			this.cmdOK.Text = "OK";
			this.cmdOK.UseVisualStyleBackColor = true;
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(354, 153);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(59, 25);
			this.cmdCancel.TabIndex = 1;
			this.cmdCancel.Text = "Cancel";
			this.cmdCancel.UseVisualStyleBackColor = true;
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// picASCOM
			// 
			this.picASCOM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.picASCOM.Cursor = System.Windows.Forms.Cursors.Hand;
			this.picASCOM.Image = global::ASCOM.GenericCCDCamera.Properties.Resources.ASCOM;
			this.picASCOM.Location = new System.Drawing.Point(365, 9);
			this.picASCOM.Name = "picASCOM";
			this.picASCOM.Size = new System.Drawing.Size(48, 56);
			this.picASCOM.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.picASCOM.TabIndex = 3;
			this.picASCOM.TabStop = false;
			this.picASCOM.Click += new System.EventHandler(this.BrowseToAscom);
			this.picASCOM.DoubleClick += new System.EventHandler(this.BrowseToAscom);
			// 
			// tbxCameraProgId
			// 
			this.tbxCameraProgId.Location = new System.Drawing.Point(15, 38);
			this.tbxCameraProgId.Name = "tbxCameraProgId";
			this.tbxCameraProgId.Size = new System.Drawing.Size(245, 20);
			this.tbxCameraProgId.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(74, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Camera Driver";
			// 
			// btnChooseCCDCamera
			// 
			this.btnChooseCCDCamera.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnChooseCCDCamera.Location = new System.Drawing.Point(263, 36);
			this.btnChooseCCDCamera.Name = "btnChooseCCDCamera";
			this.btnChooseCCDCamera.Size = new System.Drawing.Size(39, 24);
			this.btnChooseCCDCamera.TabIndex = 6;
			this.btnChooseCCDCamera.Text = "...";
			this.btnChooseCCDCamera.UseVisualStyleBackColor = true;
			this.btnChooseCCDCamera.Click += new System.EventHandler(this.btnChooseCCDCamera_Click);
			// 
			// chkTrace
			// 
			this.chkTrace.AutoSize = true;
			this.chkTrace.Location = new System.Drawing.Point(15, 153);
			this.chkTrace.Name = "chkTrace";
			this.chkTrace.Size = new System.Drawing.Size(96, 17);
			this.chkTrace.TabIndex = 7;
			this.chkTrace.Text = "Trace Enabled";
			this.chkTrace.UseVisualStyleBackColor = true;
			// 
			// SetupDialogForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(423, 186);
			this.Controls.Add(this.chkTrace);
			this.Controls.Add(this.btnChooseCCDCamera);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbxCameraProgId);
			this.Controls.Add(this.picASCOM);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SetupDialogForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Generic CCD Camera Video Driver Setup";
			((System.ComponentModel.ISupportInitialize)(this.picASCOM)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.PictureBox picASCOM;
		private System.Windows.Forms.TextBox tbxCameraProgId;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnChooseCCDCamera;
		private System.Windows.Forms.CheckBox chkTrace;
	}
}