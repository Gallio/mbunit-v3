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
            this.statusTextLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.pleaseWaitLabel = new System.Windows.Forms.Label();
            this.runInBackgroundButton = new System.Windows.Forms.Button();
            this.detailsButton = new System.Windows.Forms.Button();
            this.detailsTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 44);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(428, 23);
            this.progressBar.TabIndex = 0;
            // 
            // statusTextLabel
            // 
            this.statusTextLabel.AutoSize = true;
            this.statusTextLabel.Location = new System.Drawing.Point(12, 18);
            this.statusTextLabel.Name = "statusTextLabel";
            this.statusTextLabel.Size = new System.Drawing.Size(57, 13);
            this.statusTextLabel.TabIndex = 1;
            this.statusTextLabel.Text = "Status text";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(365, 104);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // pleaseWaitLabel
            // 
            this.pleaseWaitLabel.AutoSize = true;
            this.pleaseWaitLabel.Location = new System.Drawing.Point(12, 80);
            this.pleaseWaitLabel.Name = "pleaseWaitLabel";
            this.pleaseWaitLabel.Size = new System.Drawing.Size(70, 13);
            this.pleaseWaitLabel.TabIndex = 3;
            this.pleaseWaitLabel.Text = "Please wait...";
            // 
            // runInBackgroundButton
            // 
            this.runInBackgroundButton.AutoSize = true;
            this.runInBackgroundButton.Location = new System.Drawing.Point(242, 104);
            this.runInBackgroundButton.Name = "runInBackgroundButton";
            this.runInBackgroundButton.Size = new System.Drawing.Size(108, 23);
            this.runInBackgroundButton.TabIndex = 2;
            this.runInBackgroundButton.Text = "Run in background";
            this.runInBackgroundButton.UseVisualStyleBackColor = true;
            this.runInBackgroundButton.Click += new System.EventHandler(this.runInBackgroundButton_Click);
            // 
            // detailsButton
            // 
            this.detailsButton.AutoSize = true;
            this.detailsButton.Location = new System.Drawing.Point(119, 104);
            this.detailsButton.Name = "detailsButton";
            this.detailsButton.Size = new System.Drawing.Size(108, 23);
            this.detailsButton.TabIndex = 2;
            this.detailsButton.Text = "Details >>";
            this.detailsButton.UseVisualStyleBackColor = true;
            this.detailsButton.Click += new System.EventHandler(this.detailsButton_Click);
            // 
            // detailsTextBox
            // 
            this.detailsTextBox.Location = new System.Drawing.Point(12, 143);
            this.detailsTextBox.Name = "detailsTextBox";
            this.detailsTextBox.ReadOnly = true;
            this.detailsTextBox.Size = new System.Drawing.Size(428, 265);
            this.detailsTextBox.TabIndex = 4;
            this.detailsTextBox.Text = "";
            // 
            // ProgressMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 138);
            this.Controls.Add(this.detailsTextBox);
            this.Controls.Add(this.pleaseWaitLabel);
            this.Controls.Add(this.detailsButton);
            this.Controls.Add(this.runInBackgroundButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.statusTextLabel);
            this.Controls.Add(this.progressBar);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressMonitor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Progress information";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label statusTextLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label pleaseWaitLabel;
        private System.Windows.Forms.Button runInBackgroundButton;
        private System.Windows.Forms.Button detailsButton;
        private System.Windows.Forms.RichTextBox detailsTextBox;
    }
}