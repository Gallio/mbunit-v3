namespace Gallio.Icarus.Search
{
    partial class View
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
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.metadataComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // searchTextBox
            // 
            this.searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.searchTextBox.Location = new System.Drawing.Point(138, 12);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(217, 20);
            this.searchTextBox.TabIndex = 0;
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            // 
            // metadataComboBox
            // 
            this.metadataComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.metadataComboBox.FormattingEnabled = true;
            this.metadataComboBox.Location = new System.Drawing.Point(11, 12);
            this.metadataComboBox.Name = "metadataComboBox";
            this.metadataComboBox.Size = new System.Drawing.Size(121, 21);
            this.metadataComboBox.TabIndex = 1;
            this.metadataComboBox.SelectedIndexChanged += new System.EventHandler(this.metadataComboBox_SelectedIndexChanged);
            // 
            // View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.metadataComboBox);
            this.Controls.Add(this.searchTextBox);
            this.Name = "View";
            this.Size = new System.Drawing.Size(369, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.ComboBox metadataComboBox;
    }
}
