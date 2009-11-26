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
            this.splashView1 = new Gallio.Common.Splash.SplashView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.verticalSplitter = new System.Windows.Forms.SplitContainer();
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
            this.horizontalSplitter.Panel2.Controls.Add(this.textBox1);
            this.horizontalSplitter.Size = new System.Drawing.Size(784, 562);
            this.horizontalSplitter.SplitterDistance = 450;
            this.horizontalSplitter.TabIndex = 0;
            // 
            // splashView1
            // 
            this.splashView1.AutoScroll = true;
            this.splashView1.AutoScrollMinSize = new System.Drawing.Size(100, 0);
            this.splashView1.BackColor = System.Drawing.SystemColors.Window;
            this.splashView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splashView1.Location = new System.Drawing.Point(0, 0);
            this.splashView1.MinimumTextLayoutWidth = 1;
            this.splashView1.Name = "splashView1";
            this.splashView1.Padding = new System.Windows.Forms.Padding(3);
            this.splashView1.Size = new System.Drawing.Size(755, 450);
            this.splashView1.TabIndex = 0;
            this.splashView1.Text = "splashView1";
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(784, 108);
            this.textBox1.TabIndex = 0;
            // 
            // verticalSplitter
            // 
            this.verticalSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.verticalSplitter.Location = new System.Drawing.Point(0, 0);
            this.verticalSplitter.Name = "verticalSplitter";
            // 
            // verticalSplitter.Panel1
            // 
            this.verticalSplitter.Panel1.Controls.Add(this.splashView1);
            this.verticalSplitter.Panel1MinSize = 1;
            this.verticalSplitter.Size = new System.Drawing.Size(784, 450);
            this.verticalSplitter.SplitterDistance = 755;
            this.verticalSplitter.TabIndex = 1;
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
        private System.Windows.Forms.TextBox textBox1;
        private SplashView splashView1;
        private System.Windows.Forms.SplitContainer verticalSplitter;
    }
}