namespace Gallio.Icarus
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
            this.progressLabel = new System.Windows.Forms.Label();
            this.runInBackgroundButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 36);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(428, 23);
            this.progressBar.TabIndex = 0;
            // 
            // subTaskNameLabel
            // 
            this.subTaskNameLabel.AutoSize = true;
            this.subTaskNameLabel.Location = new System.Drawing.Point(12, 10);
            this.subTaskNameLabel.Name = "subTaskNameLabel";
            this.subTaskNameLabel.Size = new System.Drawing.Size(78, 13);
            this.subTaskNameLabel.TabIndex = 1;
            this.subTaskNameLabel.Text = "Sub task name";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(365, 96);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(202, 71);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(48, 13);
            this.progressLabel.TabIndex = 3;
            this.progressLabel.Text = "Progress";
            // 
            // runInBackgroundButton
            // 
            this.runInBackgroundButton.AutoSize = true;
            this.runInBackgroundButton.Location = new System.Drawing.Point(242, 96);
            this.runInBackgroundButton.Name = "runInBackgroundButton";
            this.runInBackgroundButton.Size = new System.Drawing.Size(108, 23);
            this.runInBackgroundButton.TabIndex = 2;
            this.runInBackgroundButton.Text = "Run in background";
            this.runInBackgroundButton.UseVisualStyleBackColor = true;
            this.runInBackgroundButton.Click += new System.EventHandler(this.runInBackgroundButton_Click);
            // 
            // ProgressMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 133);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.runInBackgroundButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.subTaskNameLabel);
            this.Controls.Add(this.progressBar);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressMonitor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Task name";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label subTaskNameLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.Button runInBackgroundButton;
    }
}