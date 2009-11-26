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
            this.leftToRightButton = new System.Windows.Forms.Button();
            this.rightToLeftButton = new System.Windows.Forms.Button();
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
            this.splashView.AutoScrollMinSize = new System.Drawing.Size(1, 1);
            this.splashView.BackColor = System.Drawing.SystemColors.Window;
            this.splashView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splashView.Location = new System.Drawing.Point(0, 0);
            this.splashView.MinimumTextLayoutWidth = 1;
            this.splashView.Name = "splashView";
            this.splashView.Padding = new System.Windows.Forms.Padding(3);
            this.splashView.Size = new System.Drawing.Size(755, 450);
            this.splashView.TabIndex = 0;
            this.splashView.Text = "splashView1";
            // 
            // leftToRightButton
            // 
            this.leftToRightButton.Location = new System.Drawing.Point(13, 4);
            this.leftToRightButton.Name = "leftToRightButton";
            this.leftToRightButton.Size = new System.Drawing.Size(75, 23);
            this.leftToRightButton.TabIndex = 0;
            this.leftToRightButton.Text = "Left to Right";
            this.leftToRightButton.UseVisualStyleBackColor = true;
            this.leftToRightButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // rightToLeftButton
            // 
            this.rightToLeftButton.Location = new System.Drawing.Point(95, 3);
            this.rightToLeftButton.Name = "rightToLeftButton";
            this.rightToLeftButton.Size = new System.Drawing.Size(75, 23);
            this.rightToLeftButton.TabIndex = 1;
            this.rightToLeftButton.Text = "Right to Left";
            this.rightToLeftButton.UseVisualStyleBackColor = true;
            this.rightToLeftButton.Click += new System.EventHandler(this.button2_Click);
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
    }
}