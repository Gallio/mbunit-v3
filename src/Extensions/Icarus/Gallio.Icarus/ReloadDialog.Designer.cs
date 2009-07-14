namespace Gallio.Icarus
{
    partial class ReloadDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReloadDialog));
            this.warningIcon = new System.Windows.Forms.Label();
            this.fileModifiedLabel = new System.Windows.Forms.Label();
            this.ignoreButton = new System.Windows.Forms.Button();
            this.reloadButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.alwaysReload = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // warningIcon
            // 
            this.warningIcon.Image = ((System.Drawing.Image)(resources.GetObject("warningIcon.Image")));
            this.warningIcon.Location = new System.Drawing.Point(12, 29);
            this.warningIcon.Name = "warningIcon";
            this.warningIcon.Size = new System.Drawing.Size(62, 53);
            this.warningIcon.TabIndex = 0;
            // 
            // fileModifiedLabel
            // 
            this.fileModifiedLabel.AutoSize = true;
            this.fileModifiedLabel.Location = new System.Drawing.Point(90, 19);
            this.fileModifiedLabel.Name = "fileModifiedLabel";
            this.fileModifiedLabel.Size = new System.Drawing.Size(136, 13);
            this.fileModifiedLabel.TabIndex = 1;
            this.fileModifiedLabel.Text = "File \'{0}\' has been modified.";
            // 
            // ignoreButton
            // 
            this.ignoreButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ignoreButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ignoreButton.Location = new System.Drawing.Point(358, 109);
            this.ignoreButton.Name = "ignoreButton";
            this.ignoreButton.Size = new System.Drawing.Size(75, 23);
            this.ignoreButton.TabIndex = 2;
            this.ignoreButton.Text = "Ignore";
            this.ignoreButton.UseVisualStyleBackColor = true;
            // 
            // reloadButton
            // 
            this.reloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.reloadButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.reloadButton.Location = new System.Drawing.Point(277, 109);
            this.reloadButton.Name = "reloadButton";
            this.reloadButton.Size = new System.Drawing.Size(75, 23);
            this.reloadButton.TabIndex = 2;
            this.reloadButton.Text = "Reload";
            this.reloadButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(90, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(264, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Press Reload to refresh the tests or Ignore to continue.";
            // 
            // alwaysReload
            // 
            this.alwaysReload.AutoSize = true;
            this.alwaysReload.Location = new System.Drawing.Point(93, 114);
            this.alwaysReload.Name = "alwaysReload";
            this.alwaysReload.Size = new System.Drawing.Size(116, 17);
            this.alwaysReload.TabIndex = 4;
            this.alwaysReload.Text = "Always reload tests";
            this.alwaysReload.UseVisualStyleBackColor = true;
            // 
            // ReloadDialog
            // 
            this.AcceptButton = this.reloadButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.CancelButton = this.ignoreButton;
            this.ClientSize = new System.Drawing.Size(444, 143);
            this.Controls.Add(this.alwaysReload);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.reloadButton);
            this.Controls.Add(this.ignoreButton);
            this.Controls.Add(this.fileModifiedLabel);
            this.Controls.Add(this.warningIcon);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReloadDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "File modification detected";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label warningIcon;
        private System.Windows.Forms.Label fileModifiedLabel;
        private System.Windows.Forms.Button ignoreButton;
        private System.Windows.Forms.Button reloadButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox alwaysReload;
    }
}