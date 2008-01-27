// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Gallio.Icarus
{
    partial class TestResults
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
            this.resultsFilterPanel = new System.Windows.Forms.Panel();
            this.testProgressStatusBar = new Gallio.Icarus.Controls.TestStatusBar();
            this.filterTestResultsCombo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.testResultsList = new Gallio.Icarus.Controls.TestResultsList();
            this.TestCol = new System.Windows.Forms.ColumnHeader();
            this.ResultCol = new System.Windows.Forms.ColumnHeader();
            this.DurationCol = new System.Windows.Forms.ColumnHeader();
            this.TypeCol = new System.Windows.Forms.ColumnHeader();
            this.NamespaceCol = new System.Windows.Forms.ColumnHeader();
            this.AssemblyCol = new System.Windows.Forms.ColumnHeader();
            this.resultsFilterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // resultsFilterPanel
            // 
            this.resultsFilterPanel.Controls.Add(this.testProgressStatusBar);
            this.resultsFilterPanel.Controls.Add(this.filterTestResultsCombo);
            this.resultsFilterPanel.Controls.Add(this.label3);
            this.resultsFilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.resultsFilterPanel.Location = new System.Drawing.Point(0, 0);
            this.resultsFilterPanel.Name = "resultsFilterPanel";
            this.resultsFilterPanel.Size = new System.Drawing.Size(292, 99);
            this.resultsFilterPanel.TabIndex = 2;
            // 
            // testProgressStatusBar
            // 
            this.testProgressStatusBar.BackColor = System.Drawing.Color.White;
            this.testProgressStatusBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.testProgressStatusBar.ElapsedTime = 0;
            this.testProgressStatusBar.Failed = 0;
            this.testProgressStatusBar.FailedColor = System.Drawing.Color.Red;
            this.testProgressStatusBar.Font = new System.Drawing.Font("Verdana", 8F);
            this.testProgressStatusBar.Inconclusive = 0;
            this.testProgressStatusBar.InconclusiveColor = System.Drawing.Color.Gold;
            this.testProgressStatusBar.Location = new System.Drawing.Point(0, 0);
            this.testProgressStatusBar.Name = "testProgressStatusBar";
            this.testProgressStatusBar.Passed = 0;
            this.testProgressStatusBar.PassedColor = System.Drawing.Color.Green;
            this.testProgressStatusBar.Size = new System.Drawing.Size(292, 56);
            this.testProgressStatusBar.TabIndex = 5;
            this.testProgressStatusBar.Text = "{0} tests - {1} successes - {2} inconclusive - {3} failures - {4:0.0}s";
            this.testProgressStatusBar.Total = 0;
            // 
            // filterTestResultsCombo
            // 
            this.filterTestResultsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filterTestResultsCombo.FormattingEnabled = true;
            this.filterTestResultsCombo.Items.AddRange(new object[] {
            "All tests",
            "Passed tests",
            "Failed tests",
            "Inconclusive tests"});
            this.filterTestResultsCombo.Location = new System.Drawing.Point(74, 68);
            this.filterTestResultsCombo.Name = "filterTestResultsCombo";
            this.filterTestResultsCombo.Size = new System.Drawing.Size(189, 21);
            this.filterTestResultsCombo.TabIndex = 1;
            this.filterTestResultsCombo.SelectedIndexChanged += new System.EventHandler(this.filterTestResultsCombo_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Show only";
            // 
            // testResultsList
            // 
            this.testResultsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.TestCol,
            this.ResultCol,
            this.DurationCol,
            this.TypeCol,
            this.NamespaceCol,
            this.AssemblyCol});
            this.testResultsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testResultsList.Location = new System.Drawing.Point(0, 99);
            this.testResultsList.Name = "testResultsList";
            this.testResultsList.Size = new System.Drawing.Size(292, 174);
            this.testResultsList.TabIndex = 3;
            this.testResultsList.UseCompatibleStateImageBehavior = false;
            this.testResultsList.View = System.Windows.Forms.View.Details;
            // 
            // TestCol
            // 
            this.TestCol.Text = "Test";
            this.TestCol.Width = 200;
            // 
            // ResultCol
            // 
            this.ResultCol.Text = "Result";
            this.ResultCol.Width = 100;
            // 
            // DurationCol
            // 
            this.DurationCol.Text = "Duration (ms)";
            this.DurationCol.Width = 100;
            // 
            // TypeCol
            // 
            this.TypeCol.Text = "Type";
            // 
            // NamespaceCol
            // 
            this.NamespaceCol.Text = "Namespace";
            // 
            // AssemblyCol
            // 
            this.AssemblyCol.Text = "Assembly";
            // 
            // TestResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.testResultsList);
            this.Controls.Add(this.resultsFilterPanel);
            this.Name = "TestResults";
            this.TabText = "Test Results";
            this.Text = "Test Results";
            this.resultsFilterPanel.ResumeLayout(false);
            this.resultsFilterPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel resultsFilterPanel;
        private System.Windows.Forms.ComboBox filterTestResultsCombo;
        private System.Windows.Forms.Label label3;
        private Gallio.Icarus.Controls.TestResultsList testResultsList;
        private System.Windows.Forms.ColumnHeader TestCol;
        private System.Windows.Forms.ColumnHeader ResultCol;
        private System.Windows.Forms.ColumnHeader DurationCol;
        private System.Windows.Forms.ColumnHeader TypeCol;
        private System.Windows.Forms.ColumnHeader NamespaceCol;
        private System.Windows.Forms.ColumnHeader AssemblyCol;
        private Gallio.Icarus.Controls.TestStatusBar testProgressStatusBar;
    }
}