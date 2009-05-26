namespace Gallio.UI.ControlPanel
{
    partial class ControlPanelDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlPanelDialog));
            this.controlPanelTabControl = new System.Windows.Forms.TabControl();
            this.outerTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.okCancelApplyFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.applyButton = new Gallio.UI.Controls.ShieldButton();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new Gallio.UI.Controls.ShieldButton();
            this.outerTableLayoutPanel.SuspendLayout();
            this.okCancelApplyFlowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // controlPanelTabControl
            // 
            this.controlPanelTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlPanelTabControl.Location = new System.Drawing.Point(0, 0);
            this.controlPanelTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.controlPanelTabControl.Name = "controlPanelTabControl";
            this.controlPanelTabControl.SelectedIndex = 0;
            this.controlPanelTabControl.Size = new System.Drawing.Size(482, 388);
            this.controlPanelTabControl.TabIndex = 0;
            this.controlPanelTabControl.SelectedIndexChanged += new System.EventHandler(this.controlPanelTabControl_SelectedIndexChanged);
            // 
            // outerTableLayoutPanel
            // 
            this.outerTableLayoutPanel.ColumnCount = 1;
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outerTableLayoutPanel.Controls.Add(this.controlPanelTabControl, 0, 0);
            this.outerTableLayoutPanel.Controls.Add(this.okCancelApplyFlowLayoutPanel, 0, 1);
            this.outerTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outerTableLayoutPanel.Location = new System.Drawing.Point(5, 5);
            this.outerTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.outerTableLayoutPanel.Name = "outerTableLayoutPanel";
            this.outerTableLayoutPanel.RowCount = 2;
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.Size = new System.Drawing.Size(482, 423);
            this.outerTableLayoutPanel.TabIndex = 0;
            // 
            // okCancelApplyFlowLayoutPanel
            // 
            this.okCancelApplyFlowLayoutPanel.AutoSize = true;
            this.okCancelApplyFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.okCancelApplyFlowLayoutPanel.Controls.Add(this.applyButton);
            this.okCancelApplyFlowLayoutPanel.Controls.Add(this.cancelButton);
            this.okCancelApplyFlowLayoutPanel.Controls.Add(this.okButton);
            this.okCancelApplyFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.okCancelApplyFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.okCancelApplyFlowLayoutPanel.Location = new System.Drawing.Point(0, 391);
            this.okCancelApplyFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.okCancelApplyFlowLayoutPanel.Name = "okCancelApplyFlowLayoutPanel";
            this.okCancelApplyFlowLayoutPanel.Size = new System.Drawing.Size(482, 29);
            this.okCancelApplyFlowLayoutPanel.TabIndex = 1;
            // 
            // applyButton
            // 
            this.applyButton.Enabled = false;
            this.applyButton.Location = new System.Drawing.Point(404, 3);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 2;
            this.applyButton.Text = "&Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(323, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(242, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // ControlPanelDialog
            // 
            this.AcceptButton = this.applyButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(492, 433);
            this.Controls.Add(this.outerTableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 460);
            this.Name = "ControlPanelDialog";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Gallio Control Panel";
            this.Load += new System.EventHandler(this.ControlPanelDialog_Load);
            this.outerTableLayoutPanel.ResumeLayout(false);
            this.outerTableLayoutPanel.PerformLayout();
            this.okCancelApplyFlowLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl controlPanelTabControl;
        private System.Windows.Forms.TableLayoutPanel outerTableLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel okCancelApplyFlowLayoutPanel;
        private Gallio.UI.Controls.ShieldButton okButton;
        private System.Windows.Forms.Button cancelButton;
        private Gallio.UI.Controls.ShieldButton applyButton;
    }
}