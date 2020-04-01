namespace Viewer
{
    partial class Form1
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
            this.fileOpenButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.slideDisplayPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // fileOpenButton
            // 
            this.fileOpenButton.Location = new System.Drawing.Point(13, 13);
            this.fileOpenButton.Name = "fileOpenButton";
            this.fileOpenButton.Size = new System.Drawing.Size(103, 23);
            this.fileOpenButton.TabIndex = 0;
            this.fileOpenButton.Text = "Open File";
            this.fileOpenButton.UseVisualStyleBackColor = true;
            // 
            // playButton
            // 
            this.playButton.Location = new System.Drawing.Point(136, 12);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(103, 23);
            this.playButton.TabIndex = 1;
            this.playButton.Text = "Play Slide Show";
            this.playButton.UseVisualStyleBackColor = true;
            // 
            // slideDisplayPanel
            // 
            this.slideDisplayPanel.AutoScroll = true;
            this.slideDisplayPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.slideDisplayPanel.Location = new System.Drawing.Point(13, 42);
            this.slideDisplayPanel.Name = "slideDisplayPanel";
            this.slideDisplayPanel.Size = new System.Drawing.Size(103, 396);
            this.slideDisplayPanel.TabIndex = 2;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(136, 42);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(561, 396);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.slideDisplayPanel);
            this.Controls.Add(this.playButton);
            this.Controls.Add(this.fileOpenButton);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button fileOpenButton;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.FlowLayoutPanel slideDisplayPanel;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

