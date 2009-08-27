namespace Gallio.AutoCAD.UI.ControlPanel
{
    partial class StartupPreferencePane
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
            System.Windows.Forms.GroupBox startActionGroupBox;
            System.Windows.Forms.Button workingDirBrowse;
            System.Windows.Forms.Label workingDirLabel;
            System.Windows.Forms.Label argumentsLabel;
            this.executable = new System.Windows.Forms.TextBox();
            this.executableBrowse = new System.Windows.Forms.Button();
            this.startSpecified = new System.Windows.Forms.RadioButton();
            this.startAttach = new System.Windows.Forms.RadioButton();
            this.startRecent = new System.Windows.Forms.RadioButton();
            this.startOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.workingDir = new System.Windows.Forms.TextBox();
            this.arguments = new System.Windows.Forms.TextBox();
            startActionGroupBox = new System.Windows.Forms.GroupBox();
            workingDirBrowse = new System.Windows.Forms.Button();
            workingDirLabel = new System.Windows.Forms.Label();
            argumentsLabel = new System.Windows.Forms.Label();
            startActionGroupBox.SuspendLayout();
            this.startOptionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // startActionGroupBox
            // 
            startActionGroupBox.Controls.Add(this.executable);
            startActionGroupBox.Controls.Add(this.executableBrowse);
            startActionGroupBox.Controls.Add(this.startSpecified);
            startActionGroupBox.Controls.Add(this.startAttach);
            startActionGroupBox.Controls.Add(this.startRecent);
            startActionGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            startActionGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            startActionGroupBox.Location = new System.Drawing.Point(0, 0);
            startActionGroupBox.Name = "startActionGroupBox";
            startActionGroupBox.Size = new System.Drawing.Size(300, 95);
            startActionGroupBox.TabIndex = 0;
            startActionGroupBox.TabStop = false;
            startActionGroupBox.Text = "Start Action";
            // 
            // executable
            // 
            this.executable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.executable.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.executable.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.executable.Location = new System.Drawing.Point(140, 66);
            this.executable.Name = "executable";
            this.executable.Size = new System.Drawing.Size(117, 20);
            this.executable.TabIndex = 3;
            this.executable.TextChanged += new System.EventHandler(this.PreferenceChangedHandler);
            // 
            // executableBrowse
            // 
            this.executableBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.executableBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.executableBrowse.Location = new System.Drawing.Point(263, 65);
            this.executableBrowse.Name = "executableBrowse";
            this.executableBrowse.Size = new System.Drawing.Size(31, 23);
            this.executableBrowse.TabIndex = 4;
            this.executableBrowse.Text = "...";
            this.executableBrowse.UseVisualStyleBackColor = true;
            this.executableBrowse.Click += new System.EventHandler(this.executableBrowse_Click);
            // 
            // startSpecified
            // 
            this.startSpecified.AutoSize = true;
            this.startSpecified.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.startSpecified.Location = new System.Drawing.Point(6, 67);
            this.startSpecified.Name = "startSpecified";
            this.startSpecified.Size = new System.Drawing.Size(101, 18);
            this.startSpecified.TabIndex = 2;
            this.startSpecified.TabStop = true;
            this.startSpecified.Text = "Start specified:";
            this.startSpecified.UseVisualStyleBackColor = true;
            this.startSpecified.CheckedChanged += new System.EventHandler(this.PreferenceChangedHandler);
            // 
            // startAttach
            // 
            this.startAttach.AutoSize = true;
            this.startAttach.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.startAttach.Location = new System.Drawing.Point(6, 16);
            this.startAttach.Name = "startAttach";
            this.startAttach.Size = new System.Drawing.Size(214, 18);
            this.startAttach.TabIndex = 0;
            this.startAttach.TabStop = true;
            this.startAttach.Text = "Attach to an existing AutoCAD process";
            this.startAttach.UseVisualStyleBackColor = true;
            this.startAttach.CheckedChanged += new System.EventHandler(this.PreferenceChangedHandler);
            // 
            // startRecent
            // 
            this.startRecent.AutoSize = true;
            this.startRecent.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.startRecent.Location = new System.Drawing.Point(6, 41);
            this.startRecent.Name = "startRecent";
            this.startRecent.Size = new System.Drawing.Size(209, 18);
            this.startRecent.TabIndex = 1;
            this.startRecent.TabStop = true;
            this.startRecent.Text = "Start the most recently used AutoCAD";
            this.startRecent.UseVisualStyleBackColor = true;
            this.startRecent.CheckedChanged += new System.EventHandler(this.PreferenceChangedHandler);
            // 
            // workingDirBrowse
            // 
            workingDirBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            workingDirBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
            workingDirBrowse.Location = new System.Drawing.Point(263, 95);
            workingDirBrowse.Name = "workingDirBrowse";
            workingDirBrowse.Size = new System.Drawing.Size(31, 23);
            workingDirBrowse.TabIndex = 4;
            workingDirBrowse.Text = "...";
            workingDirBrowse.UseVisualStyleBackColor = true;
            workingDirBrowse.Click += new System.EventHandler(this.workingDirBrowse_Click);
            // 
            // workingDirLabel
            // 
            workingDirLabel.AutoSize = true;
            workingDirLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            workingDirLabel.Location = new System.Drawing.Point(9, 100);
            workingDirLabel.Name = "workingDirLabel";
            workingDirLabel.Size = new System.Drawing.Size(93, 13);
            workingDirLabel.TabIndex = 2;
            workingDirLabel.Text = "Working directory:";
            // 
            // argumentsLabel
            // 
            argumentsLabel.AutoSize = true;
            argumentsLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            argumentsLabel.Location = new System.Drawing.Point(6, 16);
            argumentsLabel.Name = "argumentsLabel";
            argumentsLabel.Size = new System.Drawing.Size(128, 13);
            argumentsLabel.TabIndex = 0;
            argumentsLabel.Text = "Command line arguments:";
            // 
            // startOptionsGroupBox
            // 
            this.startOptionsGroupBox.Controls.Add(workingDirBrowse);
            this.startOptionsGroupBox.Controls.Add(this.workingDir);
            this.startOptionsGroupBox.Controls.Add(workingDirLabel);
            this.startOptionsGroupBox.Controls.Add(this.arguments);
            this.startOptionsGroupBox.Controls.Add(argumentsLabel);
            this.startOptionsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startOptionsGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.startOptionsGroupBox.Location = new System.Drawing.Point(0, 95);
            this.startOptionsGroupBox.Name = "startOptionsGroupBox";
            this.startOptionsGroupBox.Size = new System.Drawing.Size(300, 225);
            this.startOptionsGroupBox.TabIndex = 1;
            this.startOptionsGroupBox.TabStop = false;
            this.startOptionsGroupBox.Text = "Start Options";
            // 
            // workingDir
            // 
            this.workingDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.workingDir.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.workingDir.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.workingDir.Location = new System.Drawing.Point(140, 96);
            this.workingDir.Name = "workingDir";
            this.workingDir.Size = new System.Drawing.Size(117, 20);
            this.workingDir.TabIndex = 3;
            this.workingDir.TextChanged += new System.EventHandler(this.PreferenceChangedHandler);
            // 
            // arguments
            // 
            this.arguments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.arguments.Location = new System.Drawing.Point(140, 13);
            this.arguments.Multiline = true;
            this.arguments.Name = "arguments";
            this.arguments.Size = new System.Drawing.Size(117, 76);
            this.arguments.TabIndex = 1;
            this.arguments.TextChanged += new System.EventHandler(this.PreferenceChangedHandler);
            // 
            // StartupPreferencePane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.startOptionsGroupBox);
            this.Controls.Add(startActionGroupBox);
            this.Name = "StartupPreferencePane";
            startActionGroupBox.ResumeLayout(false);
            startActionGroupBox.PerformLayout();
            this.startOptionsGroupBox.ResumeLayout(false);
            this.startOptionsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton startRecent;
        private System.Windows.Forms.RadioButton startAttach;
        private System.Windows.Forms.RadioButton startSpecified;
        private System.Windows.Forms.TextBox workingDir;
        private System.Windows.Forms.TextBox arguments;
        private System.Windows.Forms.Button executableBrowse;
        private System.Windows.Forms.GroupBox startOptionsGroupBox;
        private System.Windows.Forms.TextBox executable;

    }
}
