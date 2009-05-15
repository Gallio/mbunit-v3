using System.ComponentModel;

namespace Gallio.Icarus.Options
{
    internal partial class TestRunnerFactoryOptions
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private const IContainer components = null;

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
            this.testRunnerFactories = new System.Windows.Forms.ComboBox();
            this.groupBoxGeneral = new System.Windows.Forms.GroupBox();
            this.panel.SuspendLayout();
            this.groupBoxGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.groupBoxGeneral);
            // 
            // testRunnerFactories
            // 
            this.testRunnerFactories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.testRunnerFactories.DropDownWidth = 200;
            this.testRunnerFactories.FormattingEnabled = true;
            this.testRunnerFactories.Location = new System.Drawing.Point(16, 19);
            this.testRunnerFactories.Name = "testRunnerFactories";
            this.testRunnerFactories.Size = new System.Drawing.Size(233, 21);
            this.testRunnerFactories.TabIndex = 2;
            // 
            // groupBoxGeneral
            // 
            this.groupBoxGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxGeneral.Controls.Add(this.testRunnerFactories);
            this.groupBoxGeneral.Location = new System.Drawing.Point(3, 3);
            this.groupBoxGeneral.Name = "groupBoxGeneral";
            this.groupBoxGeneral.Size = new System.Drawing.Size(444, 57);
            this.groupBoxGeneral.TabIndex = 3;
            this.groupBoxGeneral.TabStop = false;
            this.groupBoxGeneral.Text = "Test Runner Factory";
            // 
            // TestRunnerFactoryOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "TestRunnerFactoryOptions";
            this.panel.ResumeLayout(false);
            this.groupBoxGeneral.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox testRunnerFactories;
        private System.Windows.Forms.GroupBox groupBoxGeneral;
    }
}
