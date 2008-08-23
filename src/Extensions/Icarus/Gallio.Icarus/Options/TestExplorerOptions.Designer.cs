namespace Gallio.Icarus.Options
{
    internal partial class TestExplorerOptions
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
            this.selectedTreeViewCategories = new System.Windows.Forms.ListBox();
            this.treeViewCategoriesLabel = new System.Windows.Forms.Label();
            this.unselectedTreeViewCategories = new System.Windows.Forms.ListBox();
            this.addButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.alwaysReloadAssemblies = new System.Windows.Forms.CheckBox();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.alwaysReloadAssemblies);
            this.panel.Controls.Add(this.removeButton);
            this.panel.Controls.Add(this.addButton);
            this.panel.Controls.Add(this.unselectedTreeViewCategories);
            this.panel.Controls.Add(this.treeViewCategoriesLabel);
            this.panel.Controls.Add(this.selectedTreeViewCategories);
            // 
            // selectedTreeViewCategories
            // 
            this.selectedTreeViewCategories.FormattingEnabled = true;
            this.selectedTreeViewCategories.Location = new System.Drawing.Point(3, 33);
            this.selectedTreeViewCategories.Name = "selectedTreeViewCategories";
            this.selectedTreeViewCategories.Size = new System.Drawing.Size(120, 160);
            this.selectedTreeViewCategories.TabIndex = 0;
            // 
            // treeViewCategoriesLabel
            // 
            this.treeViewCategoriesLabel.AutoSize = true;
            this.treeViewCategoriesLabel.Location = new System.Drawing.Point(3, 5);
            this.treeViewCategoriesLabel.Name = "treeViewCategoriesLabel";
            this.treeViewCategoriesLabel.Size = new System.Drawing.Size(109, 13);
            this.treeViewCategoriesLabel.TabIndex = 1;
            this.treeViewCategoriesLabel.Text = "Tree view categories:";
            // 
            // unselectedTreeViewCategories
            // 
            this.unselectedTreeViewCategories.FormattingEnabled = true;
            this.unselectedTreeViewCategories.Location = new System.Drawing.Point(175, 33);
            this.unselectedTreeViewCategories.Name = "unselectedTreeViewCategories";
            this.unselectedTreeViewCategories.Size = new System.Drawing.Size(120, 160);
            this.unselectedTreeViewCategories.TabIndex = 2;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(138, 109);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(22, 23);
            this.addButton.TabIndex = 3;
            this.addButton.Text = "<";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(138, 80);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(22, 23);
            this.removeButton.TabIndex = 4;
            this.removeButton.Text = ">";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // alwaysReloadAssemblies
            // 
            this.alwaysReloadAssemblies.AutoSize = true;
            this.alwaysReloadAssemblies.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.alwaysReloadAssemblies.Location = new System.Drawing.Point(3, 210);
            this.alwaysReloadAssemblies.Name = "alwaysReloadAssemblies";
            this.alwaysReloadAssemblies.Size = new System.Drawing.Size(148, 17);
            this.alwaysReloadAssemblies.TabIndex = 5;
            this.alwaysReloadAssemblies.Text = "Always reload assemblies:";
            this.alwaysReloadAssemblies.UseVisualStyleBackColor = true;
            // 
            // TestExplorerOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "TestExplorerOptions";
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ListBox unselectedTreeViewCategories;
        private System.Windows.Forms.Label treeViewCategoriesLabel;
        private System.Windows.Forms.ListBox selectedTreeViewCategories;
        private System.Windows.Forms.CheckBox alwaysReloadAssemblies;
    }
}
