namespace Viewer
{
    partial class Form2
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.displayedSlidePanel = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.elapsedTimeLabel = new System.Windows.Forms.Label();
            this.currentTrackLabel = new System.Windows.Forms.Label();
            this.backSlideButton = new System.Windows.Forms.Button();
            this.nextSlideButton = new System.Windows.Forms.Button();
            this.playAndPauseButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.slideTransitionTimer = new System.Windows.Forms.Timer(this.components);
            this.displayedSlidePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // displayedSlidePanel
            // 
            this.displayedSlidePanel.AutoSize = true;
            this.displayedSlidePanel.BackColor = System.Drawing.Color.Transparent;
            this.displayedSlidePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("displayedSlidePanel.BackgroundImage")));
            this.displayedSlidePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.displayedSlidePanel.Controls.Add(this.pictureBox2);
            this.displayedSlidePanel.Controls.Add(this.pictureBox1);
            this.displayedSlidePanel.Controls.Add(this.elapsedTimeLabel);
            this.displayedSlidePanel.Controls.Add(this.currentTrackLabel);
            this.displayedSlidePanel.Controls.Add(this.backSlideButton);
            this.displayedSlidePanel.Controls.Add(this.nextSlideButton);
            this.displayedSlidePanel.Controls.Add(this.playAndPauseButton);
            this.displayedSlidePanel.Controls.Add(this.progressBar);
            this.displayedSlidePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayedSlidePanel.Location = new System.Drawing.Point(0, 0);
            this.displayedSlidePanel.Name = "displayedSlidePanel";
            this.displayedSlidePanel.Size = new System.Drawing.Size(1088, 635);
            this.displayedSlidePanel.TabIndex = 0;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.Location = new System.Drawing.Point(11, 12);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(1066, 532);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(10, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1066, 532);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // elapsedTimeLabel
            // 
            this.elapsedTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.elapsedTimeLabel.AutoSize = true;
            this.elapsedTimeLabel.ForeColor = System.Drawing.Color.Lime;
            this.elapsedTimeLabel.Location = new System.Drawing.Point(1025, 609);
            this.elapsedTimeLabel.Name = "elapsedTimeLabel";
            this.elapsedTimeLabel.Size = new System.Drawing.Size(60, 13);
            this.elapsedTimeLabel.TabIndex = 5;
            this.elapsedTimeLabel.Text = "0:00 / 0:00";
            this.elapsedTimeLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // currentTrackLabel
            // 
            this.currentTrackLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.currentTrackLabel.AutoSize = true;
            this.currentTrackLabel.ForeColor = System.Drawing.Color.Lime;
            this.currentTrackLabel.Location = new System.Drawing.Point(7, 609);
            this.currentTrackLabel.Name = "currentTrackLabel";
            this.currentTrackLabel.Size = new System.Drawing.Size(96, 13);
            this.currentTrackLabel.TabIndex = 4;
            this.currentTrackLabel.Text = "No Tracks Loaded";
            // 
            // backSlideButton
            // 
            this.backSlideButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.backSlideButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("backSlideButton.BackgroundImage")));
            this.backSlideButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.backSlideButton.Location = new System.Drawing.Point(380, 590);
            this.backSlideButton.Name = "backSlideButton";
            this.backSlideButton.Size = new System.Drawing.Size(104, 30);
            this.backSlideButton.TabIndex = 3;
            this.backSlideButton.UseVisualStyleBackColor = true;
            // 
            // nextSlideButton
            // 
            this.nextSlideButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.nextSlideButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("nextSlideButton.BackgroundImage")));
            this.nextSlideButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.nextSlideButton.Location = new System.Drawing.Point(564, 589);
            this.nextSlideButton.Name = "nextSlideButton";
            this.nextSlideButton.Size = new System.Drawing.Size(101, 31);
            this.nextSlideButton.TabIndex = 2;
            this.nextSlideButton.UseVisualStyleBackColor = true;
            // 
            // playAndPauseButton
            // 
            this.playAndPauseButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.playAndPauseButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("playAndPauseButton.BackgroundImage")));
            this.playAndPauseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.playAndPauseButton.Location = new System.Drawing.Point(487, 566);
            this.playAndPauseButton.Name = "playAndPauseButton";
            this.playAndPauseButton.Size = new System.Drawing.Size(75, 55);
            this.playAndPauseButton.TabIndex = 1;
            this.playAndPauseButton.UseVisualStyleBackColor = true;
            this.playAndPauseButton.Click += new System.EventHandler(this.playAndPauseButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.Location = new System.Drawing.Point(0, 625);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1088, 10);
            this.progressBar.TabIndex = 0;
            // 
            // slideTransitionTimer
            // 
            this.slideTransitionTimer.Tick += new System.EventHandler(this.slideTransitionTimer_Tick);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1088, 635);
            this.Controls.Add(this.displayedSlidePanel);
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form2";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.displayedSlidePanel.ResumeLayout(false);
            this.displayedSlidePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel displayedSlidePanel;
        private System.Windows.Forms.Label elapsedTimeLabel;
        private System.Windows.Forms.Label currentTrackLabel;
        private System.Windows.Forms.Button backSlideButton;
        private System.Windows.Forms.Button nextSlideButton;
        private System.Windows.Forms.Button playAndPauseButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer slideTransitionTimer;
    }
}