namespace WindowsClock.Tester
{
	partial class frmPlotClockData
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miFileOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.pboxPlot = new System.Windows.Forms.PictureBox();
			this.pnlControls = new System.Windows.Forms.Panel();
			this.pnlClient = new System.Windows.Forms.Panel();
			this.nudMinPlot = new System.Windows.Forms.NumericUpDown();
			this.nudMaxPlot = new System.Windows.Forms.NumericUpDown();
			this.btnRePlot = new System.Windows.Forms.Button();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pboxPlot)).BeginInit();
			this.pnlControls.SuspendLayout();
			this.pnlClient.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMinPlot)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxPlot)).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(746, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFileOpen});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// miFileOpen
			// 
			this.miFileOpen.Name = "miFileOpen";
			this.miFileOpen.Size = new System.Drawing.Size(100, 22);
			this.miFileOpen.Text = "&Open";
			this.miFileOpen.Click += new System.EventHandler(this.miFileOpen_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "txt";
			this.openFileDialog.Filter = "Log Files (*.txt)|*.txt|All Files (*.*)|*.*";
			// 
			// pboxPlot
			// 
			this.pboxPlot.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pboxPlot.Location = new System.Drawing.Point(0, 0);
			this.pboxPlot.Name = "pboxPlot";
			this.pboxPlot.Size = new System.Drawing.Size(746, 396);
			this.pboxPlot.TabIndex = 1;
			this.pboxPlot.TabStop = false;
			// 
			// pnlControls
			// 
			this.pnlControls.Controls.Add(this.btnRePlot);
			this.pnlControls.Controls.Add(this.nudMaxPlot);
			this.pnlControls.Controls.Add(this.nudMinPlot);
			this.pnlControls.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlControls.Location = new System.Drawing.Point(0, 24);
			this.pnlControls.Name = "pnlControls";
			this.pnlControls.Size = new System.Drawing.Size(746, 41);
			this.pnlControls.TabIndex = 2;
			// 
			// pnlClient
			// 
			this.pnlClient.Controls.Add(this.pboxPlot);
			this.pnlClient.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlClient.Location = new System.Drawing.Point(0, 65);
			this.pnlClient.Name = "pnlClient";
			this.pnlClient.Size = new System.Drawing.Size(746, 396);
			this.pnlClient.TabIndex = 3;
			// 
			// nudMinPlot
			// 
			this.nudMinPlot.DecimalPlaces = 1;
			this.nudMinPlot.Location = new System.Drawing.Point(35, 10);
			this.nudMinPlot.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.nudMinPlot.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
			this.nudMinPlot.Name = "nudMinPlot";
			this.nudMinPlot.Size = new System.Drawing.Size(76, 20);
			this.nudMinPlot.TabIndex = 0;
			// 
			// nudMaxPlot
			// 
			this.nudMaxPlot.DecimalPlaces = 1;
			this.nudMaxPlot.Location = new System.Drawing.Point(157, 10);
			this.nudMaxPlot.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.nudMaxPlot.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
			this.nudMaxPlot.Name = "nudMaxPlot";
			this.nudMaxPlot.Size = new System.Drawing.Size(76, 20);
			this.nudMaxPlot.TabIndex = 1;
			// 
			// btnRePlot
			// 
			this.btnRePlot.Location = new System.Drawing.Point(254, 9);
			this.btnRePlot.Name = "btnRePlot";
			this.btnRePlot.Size = new System.Drawing.Size(75, 23);
			this.btnRePlot.TabIndex = 2;
			this.btnRePlot.Text = "Plot";
			this.btnRePlot.UseVisualStyleBackColor = true;
			this.btnRePlot.Click += new System.EventHandler(this.btnRePlot_Click);
			// 
			// frmPlotClockData
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(746, 461);
			this.Controls.Add(this.pnlClient);
			this.Controls.Add(this.pnlControls);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "frmPlotClockData";
			this.Text = "Time Quering Plot";
			this.Resize += new System.EventHandler(this.frmPlotClockData_Resize);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pboxPlot)).EndInit();
			this.pnlControls.ResumeLayout(false);
			this.pnlClient.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudMinPlot)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxPlot)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem miFileOpen;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.PictureBox pboxPlot;
		private System.Windows.Forms.Panel pnlControls;
		private System.Windows.Forms.Panel pnlClient;
		private System.Windows.Forms.NumericUpDown nudMaxPlot;
		private System.Windows.Forms.NumericUpDown nudMinPlot;
		private System.Windows.Forms.Button btnRePlot;
	}
}