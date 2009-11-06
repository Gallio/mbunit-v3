namespace Gallio.UI.ErrorReporting
{
    partial class ErrorDialog
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
            this.outerTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.showOrHideDetailsButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.errorMessageLabel = new System.Windows.Forms.Label();
            this.errorIcon = new System.Windows.Forms.PictureBox();
            this.errorDetailsTextBox = new System.Windows.Forms.TextBox();
            this.outerTableLayoutPanel.SuspendLayout();
            this.buttonFlowLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // outerTableLayoutPanel
            // 
            this.outerTableLayoutPanel.AutoSize = true;
            this.outerTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.outerTableLayoutPanel.ColumnCount = 2;
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outerTableLayoutPanel.Controls.Add(this.buttonFlowLayoutPanel, 0, 1);
            this.outerTableLayoutPanel.Controls.Add(this.errorMessageLabel, 1, 0);
            this.outerTableLayoutPanel.Controls.Add(this.errorIcon, 0, 0);
            this.outerTableLayoutPanel.Controls.Add(this.errorDetailsTextBox, 0, 2);
            this.outerTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outerTableLayoutPanel.Location = new System.Drawing.Point(8, 8);
            this.outerTableLayoutPanel.Name = "outerTableLayoutPanel";
            this.outerTableLayoutPanel.RowCount = 3;
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outerTableLayoutPanel.Size = new System.Drawing.Size(378, 258);
            this.outerTableLayoutPanel.TabIndex = 0;
            // 
            // buttonFlowLayoutPanel
            // 
            this.buttonFlowLayoutPanel.AutoSize = true;
            this.buttonFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.outerTableLayoutPanel.SetColumnSpan(this.buttonFlowLayoutPanel, 2);
            this.buttonFlowLayoutPanel.Controls.Add(this.showOrHideDetailsButton);
            this.buttonFlowLayoutPanel.Controls.Add(this.okButton);
            this.buttonFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonFlowLayoutPanel.Location = new System.Drawing.Point(3, 68);
            this.buttonFlowLayoutPanel.Name = "buttonFlowLayoutPanel";
            this.buttonFlowLayoutPanel.Size = new System.Drawing.Size(372, 29);
            this.buttonFlowLayoutPanel.TabIndex = 1;
            this.buttonFlowLayoutPanel.WrapContents = false;
            // 
            // showOrHideDetailsButton
            // 
            this.showOrHideDetailsButton.Location = new System.Drawing.Point(294, 3);
            this.showOrHideDetailsButton.Name = "showOrHideDetailsButton";
            this.showOrHideDetailsButton.Size = new System.Drawing.Size(75, 23);
            this.showOrHideDetailsButton.TabIndex = 1;
            this.showOrHideDetailsButton.Text = "Details >>";
            this.showOrHideDetailsButton.UseVisualStyleBackColor = true;
            this.showOrHideDetailsButton.Click += new System.EventHandler(this.showOrHideDetailsButton_Click);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(213, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // errorMessageLabel
            // 
            this.errorMessageLabel.AutoEllipsis = true;
            this.errorMessageLabel.AutoSize = true;
            this.errorMessageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorMessageLabel.Location = new System.Drawing.Point(76, 0);
            this.errorMessageLabel.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.errorMessageLabel.MinimumSize = new System.Drawing.Size(0, 64);
            this.errorMessageLabel.Name = "errorMessageLabel";
            this.errorMessageLabel.Size = new System.Drawing.Size(302, 65);
            this.errorMessageLabel.TabIndex = 0;
            this.errorMessageLabel.Text = "Error Message.";
            this.errorMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // errorIcon
            // 
            this.errorIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorIcon.Image = global::Gallio.UI.Properties.Resources.Pillar;
            this.errorIcon.Location = new System.Drawing.Point(0, 0);
            this.errorIcon.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.errorIcon.Name = "errorIcon";
            this.errorIcon.Size = new System.Drawing.Size(64, 65);
            this.errorIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.errorIcon.TabIndex = 4;
            this.errorIcon.TabStop = false;
            // 
            // errorDetailsTextBox
            // 
            this.outerTableLayoutPanel.SetColumnSpan(this.errorDetailsTextBox, 2);
            this.errorDetailsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorDetailsTextBox.Location = new System.Drawing.Point(3, 103);
            this.errorDetailsTextBox.MinimumSize = new System.Drawing.Size(4, 150);
            this.errorDetailsTextBox.Multiline = true;
            this.errorDetailsTextBox.Name = "errorDetailsTextBox";
            this.errorDetailsTextBox.ReadOnly = true;
            this.errorDetailsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.errorDetailsTextBox.Size = new System.Drawing.Size(372, 152);
            this.errorDetailsTextBox.TabIndex = 2;
            this.errorDetailsTextBox.Text = "Error Details.";
            // 
            // ErrorDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(394, 274);
            this.Controls.Add(this.outerTableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 25);
            this.Name = "ErrorDialog";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Error Title";
            this.Load += new System.EventHandler(this.ErrorDialog_Load);
            this.outerTableLayoutPanel.ResumeLayout(false);
            this.outerTableLayoutPanel.PerformLayout();
            this.buttonFlowLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel outerTableLayoutPanel;
        private System.Windows.Forms.Label errorMessageLabel;
        private System.Windows.Forms.FlowLayoutPanel buttonFlowLayoutPanel;
        private System.Windows.Forms.Button showOrHideDetailsButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox errorDetailsTextBox;
        private System.Windows.Forms.PictureBox errorIcon;
    }
}