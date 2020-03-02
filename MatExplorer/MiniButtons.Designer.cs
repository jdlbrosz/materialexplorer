namespace MatExplorer
{
    partial class MiniButtons
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MiniButtons));
            this.sizeBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.sizeBox)).BeginInit();
            this.SuspendLayout();
            // 
            // sizeBox
            // 
            this.sizeBox.BackColor = System.Drawing.Color.SlateBlue;
            this.sizeBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sizeBox.Image = ((System.Drawing.Image)(resources.GetObject("sizeBox.Image")));
            this.sizeBox.Location = new System.Drawing.Point(80, 0);
            this.sizeBox.Name = "sizeBox";
            this.sizeBox.Size = new System.Drawing.Size(20, 15);
            this.sizeBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.sizeBox.TabIndex = 0;
            this.sizeBox.TabStop = false;
            // 
            // MiniButtons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(225)))), ((int)(((byte)(210)))));
            this.Controls.Add(this.sizeBox);
            this.Name = "MiniButtons";
            this.Size = new System.Drawing.Size(100, 15);
            ((System.ComponentModel.ISupportInitialize)(this.sizeBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox sizeBox;
    }
}
