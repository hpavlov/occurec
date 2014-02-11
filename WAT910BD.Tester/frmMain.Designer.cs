namespace WAT910BD.Tester
{
	partial class frmMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.serialPort = new System.IO.Ports.SerialPort(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.cbxCOMPort = new System.Windows.Forms.ComboBox();
			this.btnConnect = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.gbxCameraControl = new System.Windows.Forms.GroupBox();
			this.button5 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.lblGamma = new System.Windows.Forms.Label();
			this.btnGammaUp = new System.Windows.Forms.Button();
			this.btnGammaDown = new System.Windows.Forms.Button();
			this.lblGain = new System.Windows.Forms.Label();
			this.btnGainUp = new System.Windows.Forms.Button();
			this.btnGainDown = new System.Windows.Forms.Button();
			this.lblExposure = new System.Windows.Forms.Label();
			this.btnExposureUp = new System.Windows.Forms.Button();
			this.btnExposureDown = new System.Windows.Forms.Button();
			this.btnInitialise = new System.Windows.Forms.Button();
			this.btnReadSettings = new System.Windows.Forms.Button();
			this.btnReadState = new System.Windows.Forms.Button();
			this.gbxCameraControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "COM Port";
			// 
			// cbxCOMPort
			// 
			this.cbxCOMPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxCOMPort.FormattingEnabled = true;
			this.cbxCOMPort.Location = new System.Drawing.Point(69, 8);
			this.cbxCOMPort.Name = "cbxCOMPort";
			this.cbxCOMPort.Size = new System.Drawing.Size(98, 21);
			this.cbxCOMPort.TabIndex = 1;
			this.cbxCOMPort.SelectedIndexChanged += new System.EventHandler(this.cbxCOMPort_SelectedIndexChanged);
			// 
			// btnConnect
			// 
			this.btnConnect.Location = new System.Drawing.Point(174, 7);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(75, 23);
			this.btnConnect.TabIndex = 2;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.SystemColors.Info;
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.textBox1.Location = new System.Drawing.Point(0, 307);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox1.Size = new System.Drawing.Size(543, 138);
			this.textBox1.TabIndex = 3;
			// 
			// gbxCameraControl
			// 
			this.gbxCameraControl.Controls.Add(this.btnReadState);
			this.gbxCameraControl.Controls.Add(this.btnReadSettings);
			this.gbxCameraControl.Controls.Add(this.button5);
			this.gbxCameraControl.Controls.Add(this.button4);
			this.gbxCameraControl.Controls.Add(this.button3);
			this.gbxCameraControl.Controls.Add(this.button1);
			this.gbxCameraControl.Controls.Add(this.button2);
			this.gbxCameraControl.Controls.Add(this.lblGamma);
			this.gbxCameraControl.Controls.Add(this.btnGammaUp);
			this.gbxCameraControl.Controls.Add(this.btnGammaDown);
			this.gbxCameraControl.Controls.Add(this.lblGain);
			this.gbxCameraControl.Controls.Add(this.btnGainUp);
			this.gbxCameraControl.Controls.Add(this.btnGainDown);
			this.gbxCameraControl.Controls.Add(this.lblExposure);
			this.gbxCameraControl.Controls.Add(this.btnExposureUp);
			this.gbxCameraControl.Controls.Add(this.btnExposureDown);
			this.gbxCameraControl.Controls.Add(this.btnInitialise);
			this.gbxCameraControl.Enabled = false;
			this.gbxCameraControl.Location = new System.Drawing.Point(10, 47);
			this.gbxCameraControl.Name = "gbxCameraControl";
			this.gbxCameraControl.Size = new System.Drawing.Size(521, 234);
			this.gbxCameraControl.TabIndex = 4;
			this.gbxCameraControl.TabStop = false;
			this.gbxCameraControl.Text = "Camera Control";
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(343, 134);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(65, 23);
			this.button5.TabIndex = 28;
			this.button5.Text = "Set";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(423, 134);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(55, 23);
			this.button4.TabIndex = 27;
			this.button4.Text = "Right >";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(343, 176);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(65, 23);
			this.button3.TabIndex = 26;
			this.button3.Text = "Down \\/";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(274, 134);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(55, 23);
			this.button1.TabIndex = 25;
			this.button1.Text = "< Left";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(343, 89);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(65, 23);
			this.button2.TabIndex = 24;
			this.button2.Text = "Up /\\";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// lblGamma
			// 
			this.lblGamma.AutoSize = true;
			this.lblGamma.Location = new System.Drawing.Point(84, 191);
			this.lblGamma.Name = "lblGamma";
			this.lblGamma.Size = new System.Drawing.Size(0, 13);
			this.lblGamma.TabIndex = 23;
			this.lblGamma.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnGammaUp
			// 
			this.btnGammaUp.Location = new System.Drawing.Point(151, 186);
			this.btnGammaUp.Name = "btnGammaUp";
			this.btnGammaUp.Size = new System.Drawing.Size(55, 23);
			this.btnGammaUp.TabIndex = 22;
			this.btnGammaUp.Text = ">";
			this.btnGammaUp.UseVisualStyleBackColor = true;
			this.btnGammaUp.Click += new System.EventHandler(this.btnGammaUp_Click);
			// 
			// btnGammaDown
			// 
			this.btnGammaDown.Location = new System.Drawing.Point(21, 186);
			this.btnGammaDown.Name = "btnGammaDown";
			this.btnGammaDown.Size = new System.Drawing.Size(55, 23);
			this.btnGammaDown.TabIndex = 21;
			this.btnGammaDown.Text = "<";
			this.btnGammaDown.UseVisualStyleBackColor = true;
			this.btnGammaDown.Click += new System.EventHandler(this.btnGammaDown_Click);
			// 
			// lblGain
			// 
			this.lblGain.AutoSize = true;
			this.lblGain.Location = new System.Drawing.Point(84, 138);
			this.lblGain.Name = "lblGain";
			this.lblGain.Size = new System.Drawing.Size(0, 13);
			this.lblGain.TabIndex = 20;
			this.lblGain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnGainUp
			// 
			this.btnGainUp.Location = new System.Drawing.Point(151, 133);
			this.btnGainUp.Name = "btnGainUp";
			this.btnGainUp.Size = new System.Drawing.Size(55, 23);
			this.btnGainUp.TabIndex = 19;
			this.btnGainUp.Text = ">";
			this.btnGainUp.UseVisualStyleBackColor = true;
			this.btnGainUp.Click += new System.EventHandler(this.btnGainUp_Click);
			// 
			// btnGainDown
			// 
			this.btnGainDown.Location = new System.Drawing.Point(21, 133);
			this.btnGainDown.Name = "btnGainDown";
			this.btnGainDown.Size = new System.Drawing.Size(55, 23);
			this.btnGainDown.TabIndex = 18;
			this.btnGainDown.Text = "<";
			this.btnGainDown.UseVisualStyleBackColor = true;
			this.btnGainDown.Click += new System.EventHandler(this.btnGainDown_Click);
			// 
			// lblExposure
			// 
			this.lblExposure.AutoSize = true;
			this.lblExposure.Location = new System.Drawing.Point(84, 89);
			this.lblExposure.Name = "lblExposure";
			this.lblExposure.Size = new System.Drawing.Size(0, 13);
			this.lblExposure.TabIndex = 17;
			this.lblExposure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnExposureUp
			// 
			this.btnExposureUp.Location = new System.Drawing.Point(151, 84);
			this.btnExposureUp.Name = "btnExposureUp";
			this.btnExposureUp.Size = new System.Drawing.Size(55, 23);
			this.btnExposureUp.TabIndex = 16;
			this.btnExposureUp.Text = ">";
			this.btnExposureUp.UseVisualStyleBackColor = true;
			this.btnExposureUp.Click += new System.EventHandler(this.btnExposureUp_Click);
			// 
			// btnExposureDown
			// 
			this.btnExposureDown.Location = new System.Drawing.Point(21, 84);
			this.btnExposureDown.Name = "btnExposureDown";
			this.btnExposureDown.Size = new System.Drawing.Size(55, 23);
			this.btnExposureDown.TabIndex = 15;
			this.btnExposureDown.Text = "<";
			this.btnExposureDown.UseVisualStyleBackColor = true;
			this.btnExposureDown.Click += new System.EventHandler(this.btnExposureDown_Click);
			// 
			// btnInitialise
			// 
			this.btnInitialise.Location = new System.Drawing.Point(21, 31);
			this.btnInitialise.Name = "btnInitialise";
			this.btnInitialise.Size = new System.Drawing.Size(104, 23);
			this.btnInitialise.TabIndex = 5;
			this.btnInitialise.Text = "Initialise";
			this.btnInitialise.UseVisualStyleBackColor = true;
			this.btnInitialise.Click += new System.EventHandler(this.btnInitialise_Click);
			// 
			// btnReadSettings
			// 
			this.btnReadSettings.Location = new System.Drawing.Point(131, 31);
			this.btnReadSettings.Name = "btnReadSettings";
			this.btnReadSettings.Size = new System.Drawing.Size(104, 23);
			this.btnReadSettings.TabIndex = 29;
			this.btnReadSettings.Text = "Read All Settings";
			this.btnReadSettings.UseVisualStyleBackColor = true;
			this.btnReadSettings.Click += new System.EventHandler(this.btnReadSettings_Click);
			// 
			// btnReadState
			// 
			this.btnReadState.Location = new System.Drawing.Point(241, 31);
			this.btnReadState.Name = "btnReadState";
			this.btnReadState.Size = new System.Drawing.Size(104, 23);
			this.btnReadState.TabIndex = 30;
			this.btnReadState.Text = "Read State";
			this.btnReadState.UseVisualStyleBackColor = true;
			this.btnReadState.Click += new System.EventHandler(this.btnReadState_Click);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(543, 445);
			this.Controls.Add(this.gbxCameraControl);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.btnConnect);
			this.Controls.Add(this.cbxCOMPort);
			this.Controls.Add(this.label1);
			this.Name = "frmMain";
			this.Text = "WAT-910BD Control Tester, v0.2";
			this.gbxCameraControl.ResumeLayout(false);
			this.gbxCameraControl.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.IO.Ports.SerialPort serialPort;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cbxCOMPort;
		private System.Windows.Forms.Button btnConnect;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.GroupBox gbxCameraControl;
		private System.Windows.Forms.Button btnInitialise;
		private System.Windows.Forms.Label lblGamma;
		private System.Windows.Forms.Button btnGammaUp;
		private System.Windows.Forms.Button btnGammaDown;
		private System.Windows.Forms.Label lblGain;
		private System.Windows.Forms.Button btnGainUp;
		private System.Windows.Forms.Button btnGainDown;
		private System.Windows.Forms.Label lblExposure;
		private System.Windows.Forms.Button btnExposureUp;
		private System.Windows.Forms.Button btnExposureDown;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button btnReadSettings;
		private System.Windows.Forms.Button btnReadState;
	}
}

