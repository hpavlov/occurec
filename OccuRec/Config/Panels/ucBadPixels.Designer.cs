
namespace OccuRec.Config.Panels
{
    partial class ucBadPixels
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
            this.components = new System.ComponentModel.Container();
            this.cbxEnableBadPixelsControl = new System.Windows.Forms.CheckBox();
            this.tbxBadPixelsFile = new System.Windows.Forms.TextBox();
            this.lblBadPixelsFile = new System.Windows.Forms.Label();
            this.btnBrowseBadPixelsFile = new System.Windows.Forms.Button();
            this.nupSize = new System.Windows.Forms.NumericUpDown();
            this.lblSize = new System.Windows.Forms.Label();
            this.cbxBlinking = new System.Windows.Forms.CheckBox();
            this.gbxOverlayMarker = new System.Windows.Forms.GroupBox();
            this.gbxShape = new System.Windows.Forms.GroupBox();
            this.rbCircle = new System.Windows.Forms.RadioButton();
            this.rbCross = new System.Windows.Forms.RadioButton();
            this.rbPlus = new System.Windows.Forms.RadioButton();
            this.gbxBadPixels = new System.Windows.Forms.GroupBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ofdBrowseBadPixelsFile = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.nupSize)).BeginInit();
            this.gbxOverlayMarker.SuspendLayout();
            this.gbxShape.SuspendLayout();
            this.gbxBadPixels.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbxEnableBadPixelsControl
            // 
            this.cbxEnableBadPixelsControl.AutoSize = true;
            this.cbxEnableBadPixelsControl.Location = new System.Drawing.Point(17, 14);
            this.cbxEnableBadPixelsControl.Name = "cbxEnableBadPixelsControl";
            this.cbxEnableBadPixelsControl.Size = new System.Drawing.Size(185, 17);
            this.cbxEnableBadPixelsControl.TabIndex = 0;
            this.cbxEnableBadPixelsControl.Text = "Enable \'bad pixels overlay\' control";
            this.toolTip1.SetToolTip(this.cbxEnableBadPixelsControl, "If ticked, a control for turning on/off the display of the bad pixel markers will" +
        " be enabled on the main menu.");
            this.cbxEnableBadPixelsControl.UseVisualStyleBackColor = true;
            this.cbxEnableBadPixelsControl.CheckedChanged += new System.EventHandler(this.cbxUseBadPixelsAids_CheckedChanged);
            // 
            // tbxBadPixelsFile
            // 
            this.tbxBadPixelsFile.Location = new System.Drawing.Point(7, 34);
            this.tbxBadPixelsFile.Name = "tbxBadPixelsFile";
            this.tbxBadPixelsFile.Size = new System.Drawing.Size(350, 20);
            this.tbxBadPixelsFile.TabIndex = 1;
            this.toolTip1.SetToolTip(this.tbxBadPixelsFile, "Enter the directory and name of the file containing the  bad pixels coordinates d" +
        "ata.");
            // 
            // lblBadPixelsFile
            // 
            this.lblBadPixelsFile.AutoSize = true;
            this.lblBadPixelsFile.Location = new System.Drawing.Point(7, 14);
            this.lblBadPixelsFile.Name = "lblBadPixelsFile";
            this.lblBadPixelsFile.Size = new System.Drawing.Size(75, 13);
            this.lblBadPixelsFile.TabIndex = 2;
            this.lblBadPixelsFile.Text = "Bad Pixels File";
            // 
            // btnBrowseBadPixelsFile
            // 
            this.btnBrowseBadPixelsFile.Location = new System.Drawing.Point(372, 28);
            this.btnBrowseBadPixelsFile.Name = "btnBrowseBadPixelsFile";
            this.btnBrowseBadPixelsFile.Size = new System.Drawing.Size(50, 30);
            this.btnBrowseBadPixelsFile.TabIndex = 3;
            this.btnBrowseBadPixelsFile.Text = "...";
            this.toolTip1.SetToolTip(this.btnBrowseBadPixelsFile, "Browse to select the bad pixels file");
            this.btnBrowseBadPixelsFile.UseVisualStyleBackColor = true;
            this.btnBrowseBadPixelsFile.Click += new System.EventHandler(this.btnBrowseBadPixelsFile_Click);
            // 
            // nupSize
            // 
            this.nupSize.Location = new System.Drawing.Point(127, 36);
            this.nupSize.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nupSize.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nupSize.Name = "nupSize";
            this.nupSize.Size = new System.Drawing.Size(56, 20);
            this.nupSize.TabIndex = 7;
            this.toolTip1.SetToolTip(this.nupSize, "choose the length or diameter (as appropriate) of the bad pixel overlay markers");
            this.nupSize.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(127, 21);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(85, 13);
            this.lblSize.TabIndex = 8;
            this.lblSize.Text = "Length/diameter";
            // 
            // cbxBlinking
            // 
            this.cbxBlinking.AutoSize = true;
            this.cbxBlinking.Location = new System.Drawing.Point(249, 36);
            this.cbxBlinking.Name = "cbxBlinking";
            this.cbxBlinking.Size = new System.Drawing.Size(63, 17);
            this.cbxBlinking.TabIndex = 9;
            this.cbxBlinking.Text = "Blinking";
            this.toolTip1.SetToolTip(this.cbxBlinking, "tick if you want the bad pixel markers to blink");
            this.cbxBlinking.UseVisualStyleBackColor = true;
            // 
            // gbxOverlayMarker
            // 
            this.gbxOverlayMarker.Controls.Add(this.gbxShape);
            this.gbxOverlayMarker.Controls.Add(this.cbxBlinking);
            this.gbxOverlayMarker.Controls.Add(this.lblSize);
            this.gbxOverlayMarker.Controls.Add(this.nupSize);
            this.gbxOverlayMarker.Location = new System.Drawing.Point(7, 83);
            this.gbxOverlayMarker.Name = "gbxOverlayMarker";
            this.gbxOverlayMarker.Size = new System.Drawing.Size(349, 137);
            this.gbxOverlayMarker.TabIndex = 10;
            this.gbxOverlayMarker.TabStop = false;
            this.gbxOverlayMarker.Text = "Overlay marker";
            // 
            // gbxShape
            // 
            this.gbxShape.Controls.Add(this.rbCircle);
            this.gbxShape.Controls.Add(this.rbCross);
            this.gbxShape.Controls.Add(this.rbPlus);
            this.gbxShape.Location = new System.Drawing.Point(10, 20);
            this.gbxShape.Name = "gbxShape";
            this.gbxShape.Size = new System.Drawing.Size(99, 97);
            this.gbxShape.TabIndex = 10;
            this.gbxShape.TabStop = false;
            this.gbxShape.Text = "Shape";
            this.toolTip1.SetToolTip(this.gbxShape, "Choose the shape of the bad pixels overlay marker");
            // 
            // rbCircle
            // 
            this.rbCircle.AutoSize = true;
            this.rbCircle.Location = new System.Drawing.Point(12, 59);
            this.rbCircle.Name = "rbCircle";
            this.rbCircle.Size = new System.Drawing.Size(51, 17);
            this.rbCircle.TabIndex = 2;
            this.rbCircle.TabStop = true;
            this.rbCircle.Text = "Circle";
            this.rbCircle.UseVisualStyleBackColor = true;
            // 
            // rbCross
            // 
            this.rbCross.AutoSize = true;
            this.rbCross.Location = new System.Drawing.Point(12, 39);
            this.rbCross.Name = "rbCross";
            this.rbCross.Size = new System.Drawing.Size(51, 17);
            this.rbCross.TabIndex = 1;
            this.rbCross.TabStop = true;
            this.rbCross.Text = "Cross";
            this.rbCross.UseVisualStyleBackColor = true;
            // 
            // rbPlus
            // 
            this.rbPlus.AutoSize = true;
            this.rbPlus.Location = new System.Drawing.Point(12, 19);
            this.rbPlus.Name = "rbPlus";
            this.rbPlus.Size = new System.Drawing.Size(45, 17);
            this.rbPlus.TabIndex = 0;
            this.rbPlus.TabStop = true;
            this.rbPlus.Text = "Plus";
            this.rbPlus.UseVisualStyleBackColor = true;
            // 
            // gbxBadPixels
            // 
            this.gbxBadPixels.Controls.Add(this.gbxOverlayMarker);
            this.gbxBadPixels.Controls.Add(this.btnBrowseBadPixelsFile);
            this.gbxBadPixels.Controls.Add(this.lblBadPixelsFile);
            this.gbxBadPixels.Controls.Add(this.tbxBadPixelsFile);
            this.gbxBadPixels.Location = new System.Drawing.Point(10, 37);
            this.gbxBadPixels.Name = "gbxBadPixels";
            this.gbxBadPixels.Size = new System.Drawing.Size(465, 241);
            this.gbxBadPixels.TabIndex = 11;
            this.gbxBadPixels.TabStop = false;
            // 
            // ofdBrowseBadPixelsFile
            // 
            this.ofdBrowseBadPixelsFile.FileName = "C:\\ProgramData\\OccuRec\\BadPixels.txt";
            this.ofdBrowseBadPixelsFile.Filter = "txt files (*.txt)|*.txt";
            this.ofdBrowseBadPixelsFile.Title = "Select the Bad Pixels file";
            // 
            // ucBadPixels
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxBadPixels);
            this.Controls.Add(this.cbxEnableBadPixelsControl);
            this.Name = "ucBadPixels";
            this.Size = new System.Drawing.Size(493, 289);
            ((System.ComponentModel.ISupportInitialize)(this.nupSize)).EndInit();
            this.gbxOverlayMarker.ResumeLayout(false);
            this.gbxOverlayMarker.PerformLayout();
            this.gbxShape.ResumeLayout(false);
            this.gbxShape.PerformLayout();
            this.gbxBadPixels.ResumeLayout(false);
            this.gbxBadPixels.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbxEnableBadPixelsControl;
        private System.Windows.Forms.TextBox tbxBadPixelsFile;
        private System.Windows.Forms.Label lblBadPixelsFile;
        private System.Windows.Forms.Button btnBrowseBadPixelsFile;
        private System.Windows.Forms.NumericUpDown nupSize;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.CheckBox cbxBlinking;
        private System.Windows.Forms.GroupBox gbxOverlayMarker;
        private System.Windows.Forms.GroupBox gbxBadPixels;
        private System.Windows.Forms.GroupBox gbxShape;
        private System.Windows.Forms.RadioButton rbCircle;
        private System.Windows.Forms.RadioButton rbCross;
        private System.Windows.Forms.RadioButton rbPlus;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.OpenFileDialog ofdBrowseBadPixelsFile;
    }
}
