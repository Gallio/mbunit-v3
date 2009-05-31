namespace Gallio.UI.Progress
{
    partial class ProgressMonitor
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.subTaskNameLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.runInBackgroundButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.percentLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                                            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(82, 28);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(315, 23);
            this.progressBar.TabIndex = 0;
            // 
            // subTaskNameLabel
            // 
            this.subTaskNameLabel.AutoSize = true;
            this.subTaskNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.subTaskNameLabel.Location = new System.Drawing.Point(3, 0);
            this.subTaskNameLabel.Name = "subTaskNameLabel";
            this.subTaskNameLabel.Size = new System.Drawing.Size(91, 13);
            this.subTaskNameLabel.TabIndex = 1;
            this.subTaskNameLabel.Text = "Sub task name";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(284, 59);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(113, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // runInBackgroundButton
            // 
            this.runInBackgroundButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.runInBackgroundButton.AutoSize = true;
            this.runInBackgroundButton.Location = new System.Drawing.Point(165, 59);
            this.runInBackgroundButton.Name = "runInBackgroundButton";
            this.runInBackgroundButton.Size = new System.Drawing.Size(113, 23);
            this.runInBackgroundButton.TabIndex = 2;
            this.runInBackgroundButton.Text = "Run in background";
            this.runInBackgroundButton.UseVisualStyleBackColor = true;
            this.runInBackgroundButton.Click += new System.EventHandler(this.runInBackgroundButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = Properties.Resources.Pillar;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 65);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                                                 | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.subTaskNameLabel);
            this.flowLayoutPanel1.Controls.Add(this.percentLabel);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(82, 9);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(315, 16);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // percentLabel
            // 
            this.percentLabel.AutoSize = true;
            this.percentLabel.Location = new System.Drawing.Point(100, 0);
            this.percentLabel.Name = "percentLabel";
            this.percentLabel.Size = new System.Drawing.Size(36, 13);
            this.percentLabel.TabIndex = 2;
            this.percentLabel.Text = "(88 %)";
            // 
            // ProgressMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(409, 94);
            this.ControlBox = false;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.runInBackgroundButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.progressBar);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressMonitor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Task name";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label subTaskNameLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button runInBackgroundButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label percentLabel;
    }
}