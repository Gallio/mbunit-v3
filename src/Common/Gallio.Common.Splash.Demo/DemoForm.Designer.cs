namespace Gallio.Common.Splash.Demo
{
    partial class DemoForm
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
            this.horizontalSplitter = new System.Windows.Forms.SplitContainer();
            this.verticalSplitter = new System.Windows.Forms.SplitContainer();
            this.splashView = new Gallio.Common.Splash.SplashView();
            this.selectionStatusLabel = new System.Windows.Forms.Label();
            this.snapPositionStatusLabel = new System.Windows.Forms.Label();
            this.rightToLeftButton = new System.Windows.Forms.Button();
            this.leftToRightButton = new System.Windows.Forms.Button();
            this.horizontalSplitter.Panel1.SuspendLayout();
            this.horizontalSplitter.Panel2.SuspendLayout();
            this.horizontalSplitter.SuspendLayout();
            this.verticalSplitter.Panel1.SuspendLayout();
            this.verticalSplitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // horizontalSplitter
            // 
            this.horizontalSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.horizontalSplitter.Location = new System.Drawing.Point(0, 0);
            this.horizontalSplitter.Name = "horizontalSplitter";
            this.horizontalSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // horizontalSplitter.Panel1
            // 
            this.horizontalSplitter.Panel1.Controls.Add(this.verticalSplitter);
            this.horizontalSplitter.Panel1MinSize = 1;
            // 
            // horizontalSplitter.Panel2
            // 
            this.horizontalSplitter.Panel2.Controls.Add(this.selectionStatusLabel);
            this.horizontalSplitter.Panel2.Controls.Add(this.snapPositionStatusLabel);
            this.horizontalSplitter.Panel2.Controls.Add(this.rightToLeftButton);
            this.horizontalSplitter.Panel2.Controls.Add(this.leftToRightButton);
            this.horizontalSplitter.Size = new System.Drawing.Size(784, 562);
            this.horizontalSplitter.SplitterDistance = 450;
            this.horizontalSplitter.TabIndex = 0;
            // 
            // verticalSplitter
            // 
            this.verticalSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.verticalSplitter.Location = new System.Drawing.Point(0, 0);
            this.verticalSplitter.Name = "verticalSplitter";
            // 
            // verticalSplitter.Panel1
            // 
            this.verticalSplitter.Panel1.Controls.Add(this.splashView);
            this.verticalSplitter.Panel1MinSize = 1;
            this.verticalSplitter.Size = new System.Drawing.Size(784, 450);
            this.verticalSplitter.SplitterDistance = 755;
            this.verticalSplitter.TabIndex = 1;
            // 
            // splashView
            // 
            this.splashView.AutoScroll = true;
            this.splashView.AutoScrollMinSize = new System.Drawing.Size(140, 41);
            this.splashView.BackColor = System.Drawing.SystemColors.Window;
            this.splashView.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.splashView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splashView.Location = new System.Drawing.Point(0, 0);
            this.splashView.MinimumTextLayoutWidth = 100;
            this.splashView.Name = "splashView";
            this.splashView.Padding = new System.Windows.Forms.Padding(20);
            this.splashView.SelectedBackgroundColor = System.Drawing.SystemColors.Highlight;
            this.splashView.SelectedTextColor = System.Drawing.SystemColors.HighlightText;
            this.splashView.Size = new System.Drawing.Size(755, 450);
            this.splashView.TabIndex = 0;
            this.splashView.Text = "splashView1";
            this.splashView.SelectionChanged += new System.EventHandler(this.splashView_SelectionChanged);
            this.splashView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.splashView_MouseClick);
            this.splashView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.splashView_MouseMove);
            // 
            // selectionStatusLabel
            // 
            this.selectionStatusLabel.AutoSize = true;
            this.selectionStatusLabel.Location = new System.Drawing.Point(13, 56);
            this.selectionStatusLabel.Name = "selectionStatusLabel";
            this.selectionStatusLabel.Size = new System.Drawing.Size(84, 13);
            this.selectionStatusLabel.TabIndex = 3;
            this.selectionStatusLabel.Text = "Selection Status";
            // 
            // snapPositionStatusLabel
            // 
            this.snapPositionStatusLabel.AutoSize = true;
            this.snapPositionStatusLabel.Location = new System.Drawing.Point(13, 34);
            this.snapPositionStatusLabel.Name = "snapPositionStatusLabel";
            this.snapPositionStatusLabel.Size = new System.Drawing.Size(72, 13);
            this.snapPositionStatusLabel.TabIndex = 2;
            this.snapPositionStatusLabel.Text = "Snap Position";
            // 
            // rightToLeftButton
            // 
            this.rightToLeftButton.Location = new System.Drawing.Point(95, 3);
            this.rightToLeftButton.Name = "rightToLeftButton";
            this.rightToLeftButton.Size = new System.Drawing.Size(75, 23);
            this.rightToLeftButton.TabIndex = 1;
            this.rightToLeftButton.Text = "Right to Left";
            this.rightToLeftButton.UseVisualStyleBackColor = true;
            this.rightToLeftButton.Click += new System.EventHandler(this.rightToLeftButton_Click);
            // 
            // leftToRightButton
            // 
            this.leftToRightButton.Location = new System.Drawing.Point(13, 4);
            this.leftToRightButton.Name = "leftToRightButton";
            this.leftToRightButton.Size = new System.Drawing.Size(75, 23);
            this.leftToRightButton.TabIndex = 0;
            this.leftToRightButton.Text = "Left to Right";
            this.leftToRightButton.UseVisualStyleBackColor = true;
            this.leftToRightButton.Click += new System.EventHandler(this.leftToRightButton_Click);
            // 
            // DemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.horizontalSplitter);
            this.Name = "DemoForm";
            this.Text = "Form1";
            this.horizontalSplitter.Panel1.ResumeLayout(false);
            this.horizontalSplitter.Panel2.ResumeLayout(false);
            this.horizontalSplitter.Panel2.PerformLayout();
            this.horizontalSplitter.ResumeLayout(false);
            this.verticalSplitter.Panel1.ResumeLayout(false);
            this.verticalSplitter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer horizontalSplitter;
        private SplashView splashView;
        private System.Windows.Forms.SplitContainer verticalSplitter;
        private System.Windows.Forms.Button leftToRightButton;
        private System.Windows.Forms.Button rightToLeftButton;
        private System.Windows.Forms.Label selectionStatusLabel;
        private System.Windows.Forms.Label snapPositionStatusLabel;
    }
}