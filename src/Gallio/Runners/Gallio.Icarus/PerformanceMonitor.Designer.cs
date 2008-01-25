namespace Gallio.Icarus
{
    partial class PerformanceMonitor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PerformanceMonitor));
            this.panel2 = new System.Windows.Forms.Panel();
            this.graphFilter = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.testResultsGraph = new Gallio.Icarus.Controls.TestResultsGraph();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.graphFilter);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(292, 42);
            this.panel2.TabIndex = 2;
            // 
            // graphFilter
            // 
            this.graphFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.graphFilter.FormattingEnabled = true;
            this.graphFilter.Items.AddRange(new object[] {
            "Test results by Type",
            "Test results by Namespace",
            "Test results by Assembly"});
            this.graphFilter.Location = new System.Drawing.Point(58, 10);
            this.graphFilter.Name = "graphFilter";
            this.graphFilter.Size = new System.Drawing.Size(189, 21);
            this.graphFilter.TabIndex = 2;
            this.graphFilter.SelectedIndexChanged += new System.EventHandler(this.graphFilter_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Graphs:";
            // 
            // testResultsGraph
            // 
            this.testResultsGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testResultsGraph.Location = new System.Drawing.Point(0, 42);
            this.testResultsGraph.Name = "testResultsGraph";
            this.testResultsGraph.ScrollMaxX = 0;
            this.testResultsGraph.ScrollMaxY = 0;
            this.testResultsGraph.ScrollMaxY2 = 0;
            this.testResultsGraph.ScrollMinX = 0;
            this.testResultsGraph.ScrollMinY = 0;
            this.testResultsGraph.ScrollMinY2 = 0;
            this.testResultsGraph.Size = new System.Drawing.Size(292, 231);
            this.testResultsGraph.TabIndex = 3;
            // 
            // PerformanceMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.testResultsGraph);
            this.Controls.Add(this.panel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PerformanceMonitor";
            this.TabText = "Performance Monitor";
            this.Text = "Performance Monitor";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox graphFilter;
        private System.Windows.Forms.Label label4;
        private Gallio.Icarus.Controls.TestResultsGraph testResultsGraph;
    }
}