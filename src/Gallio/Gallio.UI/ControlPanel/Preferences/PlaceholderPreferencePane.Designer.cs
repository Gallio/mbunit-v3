namespace Gallio.UI.ControlPanel.Preferences
{
    partial class PlaceholderPreferencePane
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.placeholderLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // placeholderLabel
            // 
            this.placeholderLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.placeholderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.placeholderLabel.Location = new System.Drawing.Point(0, 0);
            this.placeholderLabel.Margin = new System.Windows.Forms.Padding(0);
            this.placeholderLabel.Name = "placeholderLabel";
            this.placeholderLabel.Size = new System.Drawing.Size(300, 300);
            this.placeholderLabel.TabIndex = 0;
            this.placeholderLabel.Text = "There are no items to show in this view.";
            this.placeholderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PlaceholderPreferencePane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.placeholderLabel);
            this.Name = "PlaceholderPreferencePane";
            this.Size = new System.Drawing.Size(300, 300);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label placeholderLabel;
    }
}
