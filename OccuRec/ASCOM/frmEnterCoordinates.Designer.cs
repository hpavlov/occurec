namespace OccuRec.ASCOM
{
    partial class frmEnterCoordinates
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEnterCoordinates));
            this.tbxRA = new System.Windows.Forms.TextBox();
            this.tbxDec = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.stmiPreviousPositions = new System.Windows.Forms.ToolStripDropDownButton();
            this.stmiCalSpec = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbxRA
            // 
            this.tbxRA.Location = new System.Drawing.Point(76, 45);
            this.tbxRA.Name = "tbxRA";
            this.tbxRA.Size = new System.Drawing.Size(100, 20);
            this.tbxRA.TabIndex = 7;
            // 
            // tbxDec
            // 
            this.tbxDec.Location = new System.Drawing.Point(76, 70);
            this.tbxDec.Name = "tbxDec";
            this.tbxDec.Size = new System.Drawing.Size(100, 20);
            this.tbxDec.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Symbol", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(58, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "a";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Symbol", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(58, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(15, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "d";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(45, 116);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "Slew";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(126, 116);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stmiPreviousPositions,
            this.stmiCalSpec});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(250, 25);
            this.toolStrip1.TabIndex = 13;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // stmiPreviousPositions
            // 
            this.stmiPreviousPositions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.stmiPreviousPositions.Image = ((System.Drawing.Image)(resources.GetObject("stmiPreviousPositions.Image")));
            this.stmiPreviousPositions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stmiPreviousPositions.Name = "stmiPreviousPositions";
            this.stmiPreviousPositions.Size = new System.Drawing.Size(106, 22);
            this.stmiPreviousPositions.Text = "Previous Positions";
            this.stmiPreviousPositions.DropDownOpening += new System.EventHandler(this.stmiPreviousPositions_DropDownOpening);
            // 
            // stmiCalSpec
            // 
            this.stmiCalSpec.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.stmiCalSpec.Image = ((System.Drawing.Image)(resources.GetObject("stmiCalSpec.Image")));
            this.stmiCalSpec.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stmiCalSpec.Name = "stmiCalSpec";
            this.stmiCalSpec.Size = new System.Drawing.Size(58, 22);
            this.stmiCalSpec.Text = "CalSpec";
            this.stmiCalSpec.DropDownOpening += new System.EventHandler(this.stmiCalSpec_DropDownOpening);
            // 
            // frmEnterCoordinates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(250, 151);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbxRA);
            this.Controls.Add(this.tbxDec);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmEnterCoordinates";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter Coordinates";
            this.Load += new System.EventHandler(this.frmEnterCoordinates_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbxRA;
        private System.Windows.Forms.TextBox tbxDec;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton stmiPreviousPositions;
        private System.Windows.Forms.ToolStripDropDownButton stmiCalSpec;
    }
}