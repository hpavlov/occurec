namespace OccuRec.Config.Panels
{
	partial class ucObservatoryControl
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
            this.btnConfigureTelescope = new System.Windows.Forms.Button();
            this.btnConfigureFocuser = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.btnClearTelescope = new System.Windows.Forms.Button();
            this.btnClearFocuser = new System.Windows.Forms.Button();
            this.btnTestTelescopeConnection = new System.Windows.Forms.Button();
            this.btnTestFocuserConnection = new System.Windows.Forms.Button();
            this.lblConnectedTelescopeInfo = new System.Windows.Forms.Label();
            this.tbxTelescope = new System.Windows.Forms.TextBox();
            this.Telescope = new System.Windows.Forms.Label();
            this.btnSelectTelescope = new System.Windows.Forms.Button();
            this.lblConnectedFocuserInfo = new System.Windows.Forms.Label();
            this.tbxFocuser = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelectFocuser = new System.Windows.Forms.Button();
            this.cbxUseAppDomainIsolation = new System.Windows.Forms.CheckBox();
            this.btnDisconnectFocuser = new System.Windows.Forms.Button();
            this.btnDisconnectTelescope = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.nudTelescopePingRate = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudTelescopePingRate)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConfigureTelescope
            // 
            this.btnConfigureTelescope.Location = new System.Drawing.Point(376, 120);
            this.btnConfigureTelescope.Name = "btnConfigureTelescope";
            this.btnConfigureTelescope.Size = new System.Drawing.Size(79, 23);
            this.btnConfigureTelescope.TabIndex = 29;
            this.btnConfigureTelescope.Text = "Configure";
            this.btnConfigureTelescope.UseVisualStyleBackColor = true;
            this.btnConfigureTelescope.Click += new System.EventHandler(this.btnConfigureTelescope_Click);
            // 
            // btnConfigureFocuser
            // 
            this.btnConfigureFocuser.Location = new System.Drawing.Point(376, 13);
            this.btnConfigureFocuser.Name = "btnConfigureFocuser";
            this.btnConfigureFocuser.Size = new System.Drawing.Size(79, 23);
            this.btnConfigureFocuser.TabIndex = 28;
            this.btnConfigureFocuser.Text = "Configure";
            this.btnConfigureFocuser.UseVisualStyleBackColor = true;
            this.btnConfigureFocuser.Click += new System.EventHandler(this.btnConfigureFocuser_Click);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(6, 298);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(161, 17);
            this.checkBox2.TabIndex = 27;
            this.checkBox2.Text = "Re-focuse Before Recording";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.Visible = false;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(6, 275);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(148, 17);
            this.checkBox1.TabIndex = 26;
            this.checkBox1.Text = "Guide on Selected Target";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Visible = false;
            // 
            // btnClearTelescope
            // 
            this.btnClearTelescope.Location = new System.Drawing.Point(271, 149);
            this.btnClearTelescope.Name = "btnClearTelescope";
            this.btnClearTelescope.Size = new System.Drawing.Size(99, 23);
            this.btnClearTelescope.TabIndex = 25;
            this.btnClearTelescope.Text = "Clear Telescope";
            this.btnClearTelescope.UseVisualStyleBackColor = true;
            this.btnClearTelescope.Click += new System.EventHandler(this.btnClearTelescope_Click);
            // 
            // btnClearFocuser
            // 
            this.btnClearFocuser.Location = new System.Drawing.Point(271, 42);
            this.btnClearFocuser.Name = "btnClearFocuser";
            this.btnClearFocuser.Size = new System.Drawing.Size(99, 23);
            this.btnClearFocuser.TabIndex = 24;
            this.btnClearFocuser.Text = "Clear Focuser";
            this.btnClearFocuser.UseVisualStyleBackColor = true;
            this.btnClearFocuser.Click += new System.EventHandler(this.btnClearFocuser_Click);
            // 
            // btnTestTelescopeConnection
            // 
            this.btnTestTelescopeConnection.Location = new System.Drawing.Point(6, 156);
            this.btnTestTelescopeConnection.Name = "btnTestTelescopeConnection";
            this.btnTestTelescopeConnection.Size = new System.Drawing.Size(99, 23);
            this.btnTestTelescopeConnection.TabIndex = 22;
            this.btnTestTelescopeConnection.Text = "Test Connection";
            this.btnTestTelescopeConnection.UseVisualStyleBackColor = true;
            this.btnTestTelescopeConnection.Click += new System.EventHandler(this.btnTestTelescopeConnection_Click);
            // 
            // btnTestFocuserConnection
            // 
            this.btnTestFocuserConnection.Location = new System.Drawing.Point(6, 47);
            this.btnTestFocuserConnection.Name = "btnTestFocuserConnection";
            this.btnTestFocuserConnection.Size = new System.Drawing.Size(99, 23);
            this.btnTestFocuserConnection.TabIndex = 21;
            this.btnTestFocuserConnection.Text = "Test Connection";
            this.btnTestFocuserConnection.UseVisualStyleBackColor = true;
            this.btnTestFocuserConnection.Click += new System.EventHandler(this.btnTestFocuserConnection_Click);
            // 
            // lblConnectedTelescopeInfo
            // 
            this.lblConnectedTelescopeInfo.AutoSize = true;
            this.lblConnectedTelescopeInfo.ForeColor = System.Drawing.Color.Green;
            this.lblConnectedTelescopeInfo.Location = new System.Drawing.Point(3, 182);
            this.lblConnectedTelescopeInfo.Name = "lblConnectedTelescopeInfo";
            this.lblConnectedTelescopeInfo.Size = new System.Drawing.Size(57, 13);
            this.lblConnectedTelescopeInfo.TabIndex = 20;
            this.lblConnectedTelescopeInfo.Text = "Telescope";
            this.lblConnectedTelescopeInfo.Visible = false;
            // 
            // tbxTelescope
            // 
            this.tbxTelescope.Location = new System.Drawing.Point(6, 123);
            this.tbxTelescope.Name = "tbxTelescope";
            this.tbxTelescope.ReadOnly = true;
            this.tbxTelescope.Size = new System.Drawing.Size(259, 20);
            this.tbxTelescope.TabIndex = 19;
            this.tbxTelescope.TextChanged += new System.EventHandler(this.tbxTelescope_TextChanged);
            // 
            // Telescope
            // 
            this.Telescope.AutoSize = true;
            this.Telescope.Location = new System.Drawing.Point(3, 107);
            this.Telescope.Name = "Telescope";
            this.Telescope.Size = new System.Drawing.Size(57, 13);
            this.Telescope.TabIndex = 18;
            this.Telescope.Text = "Telescope";
            // 
            // btnSelectTelescope
            // 
            this.btnSelectTelescope.Location = new System.Drawing.Point(271, 120);
            this.btnSelectTelescope.Name = "btnSelectTelescope";
            this.btnSelectTelescope.Size = new System.Drawing.Size(99, 23);
            this.btnSelectTelescope.TabIndex = 17;
            this.btnSelectTelescope.Text = "Select Telescope";
            this.btnSelectTelescope.UseVisualStyleBackColor = true;
            this.btnSelectTelescope.Click += new System.EventHandler(this.btnSelectTelescope_Click);
            // 
            // lblConnectedFocuserInfo
            // 
            this.lblConnectedFocuserInfo.AutoSize = true;
            this.lblConnectedFocuserInfo.ForeColor = System.Drawing.Color.Green;
            this.lblConnectedFocuserInfo.Location = new System.Drawing.Point(3, 73);
            this.lblConnectedFocuserInfo.Name = "lblConnectedFocuserInfo";
            this.lblConnectedFocuserInfo.Size = new System.Drawing.Size(45, 13);
            this.lblConnectedFocuserInfo.TabIndex = 16;
            this.lblConnectedFocuserInfo.Text = "Focuser";
            this.lblConnectedFocuserInfo.Visible = false;
            // 
            // tbxFocuser
            // 
            this.tbxFocuser.Location = new System.Drawing.Point(6, 16);
            this.tbxFocuser.Name = "tbxFocuser";
            this.tbxFocuser.ReadOnly = true;
            this.tbxFocuser.Size = new System.Drawing.Size(259, 20);
            this.tbxFocuser.TabIndex = 15;
            this.tbxFocuser.TextChanged += new System.EventHandler(this.tbxFocuser_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Focuser";
            // 
            // btnSelectFocuser
            // 
            this.btnSelectFocuser.Location = new System.Drawing.Point(271, 13);
            this.btnSelectFocuser.Name = "btnSelectFocuser";
            this.btnSelectFocuser.Size = new System.Drawing.Size(99, 23);
            this.btnSelectFocuser.TabIndex = 13;
            this.btnSelectFocuser.Text = "Select Focuser";
            this.btnSelectFocuser.UseVisualStyleBackColor = true;
            this.btnSelectFocuser.Click += new System.EventHandler(this.btnSelectFocuser_Click);
            // 
            // cbxUseAppDomainIsolation
            // 
            this.cbxUseAppDomainIsolation.AutoSize = true;
            this.cbxUseAppDomainIsolation.Location = new System.Drawing.Point(6, 213);
            this.cbxUseAppDomainIsolation.Name = "cbxUseAppDomainIsolation";
            this.cbxUseAppDomainIsolation.Size = new System.Drawing.Size(163, 17);
            this.cbxUseAppDomainIsolation.TabIndex = 30;
            this.cbxUseAppDomainIsolation.Text = "Run Drivers in Isolated Mode";
            this.cbxUseAppDomainIsolation.UseVisualStyleBackColor = true;
            // 
            // btnDisconnectFocuser
            // 
            this.btnDisconnectFocuser.Location = new System.Drawing.Point(111, 47);
            this.btnDisconnectFocuser.Name = "btnDisconnectFocuser";
            this.btnDisconnectFocuser.Size = new System.Drawing.Size(99, 23);
            this.btnDisconnectFocuser.TabIndex = 31;
            this.btnDisconnectFocuser.Text = "Disconnect";
            this.btnDisconnectFocuser.UseVisualStyleBackColor = true;
            this.btnDisconnectFocuser.Click += new System.EventHandler(this.btnDisconnectFocuser_Click);
            // 
            // btnDisconnectTelescope
            // 
            this.btnDisconnectTelescope.Location = new System.Drawing.Point(111, 156);
            this.btnDisconnectTelescope.Name = "btnDisconnectTelescope";
            this.btnDisconnectTelescope.Size = new System.Drawing.Size(99, 23);
            this.btnDisconnectTelescope.TabIndex = 32;
            this.btnDisconnectTelescope.Text = "Disconnect";
            this.btnDisconnectTelescope.UseVisualStyleBackColor = true;
            this.btnDisconnectTelescope.Click += new System.EventHandler(this.btnDisconnectTelescope_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(165, 240);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 13);
            this.label2.TabIndex = 35;
            this.label2.Text = "sec";
            // 
            // nudTelescopePingRate
            // 
            this.nudTelescopePingRate.Location = new System.Drawing.Point(107, 238);
            this.nudTelescopePingRate.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudTelescopePingRate.Name = "nudTelescopePingRate";
            this.nudTelescopePingRate.Size = new System.Drawing.Size(52, 20);
            this.nudTelescopePingRate.TabIndex = 34;
            this.nudTelescopePingRate.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 240);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 33;
            this.label3.Text = "Check status every";
            // 
            // ucObservatoryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudTelescopePingRate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnDisconnectTelescope);
            this.Controls.Add(this.btnDisconnectFocuser);
            this.Controls.Add(this.cbxUseAppDomainIsolation);
            this.Controls.Add(this.btnConfigureTelescope);
            this.Controls.Add(this.btnConfigureFocuser);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.btnClearTelescope);
            this.Controls.Add(this.btnClearFocuser);
            this.Controls.Add(this.btnTestTelescopeConnection);
            this.Controls.Add(this.btnTestFocuserConnection);
            this.Controls.Add(this.lblConnectedTelescopeInfo);
            this.Controls.Add(this.tbxTelescope);
            this.Controls.Add(this.Telescope);
            this.Controls.Add(this.btnSelectTelescope);
            this.Controls.Add(this.lblConnectedFocuserInfo);
            this.Controls.Add(this.tbxFocuser);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSelectFocuser);
            this.Name = "ucObservatoryControl";
            this.Size = new System.Drawing.Size(570, 328);
            ((System.ComponentModel.ISupportInitialize)(this.nudTelescopePingRate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnClearTelescope;
        private System.Windows.Forms.Button btnClearFocuser;
		private System.Windows.Forms.Button btnTestTelescopeConnection;
		private System.Windows.Forms.Button btnTestFocuserConnection;
		private System.Windows.Forms.Label lblConnectedTelescopeInfo;
		private System.Windows.Forms.TextBox tbxTelescope;
		private System.Windows.Forms.Label Telescope;
		private System.Windows.Forms.Button btnSelectTelescope;
		private System.Windows.Forms.Label lblConnectedFocuserInfo;
		private System.Windows.Forms.TextBox tbxFocuser;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnSelectFocuser;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Button btnConfigureFocuser;
        private System.Windows.Forms.Button btnConfigureTelescope;
		private System.Windows.Forms.CheckBox cbxUseAppDomainIsolation;
        private System.Windows.Forms.Button btnDisconnectFocuser;
        private System.Windows.Forms.Button btnDisconnectTelescope;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudTelescopePingRate;
        private System.Windows.Forms.Label label3;
	}
}
