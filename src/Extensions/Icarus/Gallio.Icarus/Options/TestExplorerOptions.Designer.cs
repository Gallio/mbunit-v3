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
            this.unselectedTreeViewCategories = new System.Windows.Forms.ListBox();
            this.addButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.alwaysReloadAssemblies = new System.Windows.Forms.CheckBox();
            this.groupBoxTreeViewCategories = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel.SuspendLayout();
            this.groupBoxTreeViewCategories.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.groupBoxTreeViewCategories);
            this.panel.Controls.Add(this.alwaysReloadAssemblies);
            // 
            // selectedTreeViewCategories
            // 
            this.selectedTreeViewCategories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.selectedTreeViewCategories.FormattingEnabled = true;
            this.selectedTreeViewCategories.Location = new System.Drawing.Point(3, 3);
            this.selectedTreeViewCategories.Name = "selectedTreeViewCategories";
            this.selectedTreeViewCategories.Size = new System.Drawing.Size(190, 199);
            this.selectedTreeViewCategories.TabIndex = 0;
            // 
            // unselectedTreeViewCategories
            // 
            this.unselectedTreeViewCategories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.unselectedTreeViewCategories.FormattingEnabled = true;
            this.unselectedTreeViewCategories.Location = new System.Drawing.Point(244, 3);
            this.unselectedTreeViewCategories.Name = "unselectedTreeViewCategories";
            this.unselectedTreeViewCategories.Size = new System.Drawing.Size(191, 199);
            this.unselectedTreeViewCategories.TabIndex = 2;
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.addButton.Location = new System.Drawing.Point(1, 73);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(37, 26);
            this.addButton.TabIndex = 3;
            this.addButton.Text = "ç";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.removeButton.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.removeButton.Location = new System.Drawing.Point(1, 102);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(37, 26);
            this.removeButton.TabIndex = 4;
            this.removeButton.Text = "è";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // alwaysReloadAssemblies
            // 
            this.alwaysReloadAssemblies.AutoSize = true;
            this.alwaysReloadAssemblies.Location = new System.Drawing.Point(10, 234);
            this.alwaysReloadAssemblies.Name = "alwaysReloadAssemblies";
            this.alwaysReloadAssemblies.Size = new System.Drawing.Size(151, 17);
            this.alwaysReloadAssemblies.TabIndex = 5;
            this.alwaysReloadAssemblies.Text = "Always Reload Assemblies";
            this.alwaysReloadAssemblies.UseVisualStyleBackColor = true;
            // 
            // groupBoxTreeViewCategories
            // 
            this.groupBoxTreeViewCategories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxTreeViewCategories.Controls.Add(this.tableLayoutPanel1);
            this.groupBoxTreeViewCategories.Location = new System.Drawing.Point(3, 3);
            this.groupBoxTreeViewCategories.Name = "groupBoxTreeViewCategories";
            this.groupBoxTreeViewCategories.Size = new System.Drawing.Size(444, 225);
            this.groupBoxTreeViewCategories.TabIndex = 6;
            this.groupBoxTreeViewCategories.TabStop = false;
            this.groupBoxTreeViewCategories.Text = "Tree View Categories";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.selectedTreeViewCategories, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.unselectedTreeViewCategories, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(438, 206);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.addButton);
            this.panel1.Controls.Add(this.removeButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(199, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(39, 200);
            this.panel1.TabIndex = 3;
            // 
            // TestExplorerOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "TestExplorerOptions";
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.groupBoxTreeViewCategories.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ListBox unselectedTreeViewCategories;
        private System.Windows.Forms.ListBox selectedTreeViewCategories;
        private System.Windows.Forms.CheckBox alwaysReloadAssemblies;
        private System.Windows.Forms.GroupBox groupBoxTreeViewCategories;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
    }
}
