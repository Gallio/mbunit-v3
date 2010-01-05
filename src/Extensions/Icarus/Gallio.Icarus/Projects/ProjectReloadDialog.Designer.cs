using System.ComponentModel;
using System.Windows.Forms;
using Gallio.Icarus.Properties;

namespace Gallio.Icarus.Projects
{
    partial class ProjectReloadDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectReloadDialog));
            this.warningIcon = new System.Windows.Forms.Label();
            this.projectModifiedLabel = new System.Windows.Forms.Label();
            this.ignoreButton = new System.Windows.Forms.Button();
            this.reloadButton = new System.Windows.Forms.Button();
            this.reloadOrIgnoreLabel = new System.Windows.Forms.Label();
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
            // projectModifiedLabel
            // 
            this.projectModifiedLabel.AutoSize = true;
            this.projectModifiedLabel.Location = new System.Drawing.Point(90, 19);
            this.projectModifiedLabel.Name = "projectModifiedLabel";
            this.projectModifiedLabel.Size = new System.Drawing.Size(153, 13);
            this.projectModifiedLabel.TabIndex = 1;
            this.projectModifiedLabel.Text = "Project \'{0}\' has been modified.";
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
            // reloadOrIgnoreLabel
            // 
            this.reloadOrIgnoreLabel.AutoSize = true;
            this.reloadOrIgnoreLabel.Location = new System.Drawing.Point(90, 58);
            this.reloadOrIgnoreLabel.Name = "reloadOrIgnoreLabel";
            this.reloadOrIgnoreLabel.Size = new System.Drawing.Size(275, 13);
            this.reloadOrIgnoreLabel.TabIndex = 3;
            this.reloadOrIgnoreLabel.Text = "Press Reload to update the project or Ignore to continue.";
            // 
            // ProjectReloadDialog
            // 
            this.AcceptButton = this.reloadButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.CancelButton = this.ignoreButton;
            this.ClientSize = new System.Drawing.Size(444, 143);
            this.Controls.Add(this.reloadOrIgnoreLabel);
            this.Controls.Add(this.reloadButton);
            this.Controls.Add(this.ignoreButton);
            this.Controls.Add(this.projectModifiedLabel);
            this.Controls.Add(this.warningIcon);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProjectReloadDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "File modification detected";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label warningIcon;
        private Label projectModifiedLabel;
        private Button ignoreButton;
        private Button reloadButton;
        private Label reloadOrIgnoreLabel;
    }
}