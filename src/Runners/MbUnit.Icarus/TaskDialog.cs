using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MbUnit.Icarus
{
    public abstract class TaskDialog
    {
        public static TaskButton Show(string title, string description, TaskButton[] buttons)
        {
            using (TaskDialogForm form = new TaskDialogForm())
            {
                form.Text = title;
                form.Description = description;
                form.Buttons = buttons;
                
                form.ShowDialog();
                return form.SelectedButton;
            }
        }

        #region Task Dialog

        private sealed class TaskDialogForm : Form
        {
            private System.Windows.Forms.Panel panelCommands;
            private System.Windows.Forms.PictureBox picLogo;
            private System.Windows.Forms.Label labelTitle;
            private System.Windows.Forms.Label labelDescription;
            private System.Windows.Forms.Panel panelHeader;

            private TaskButton[] buttons;
            private TaskButton[] activeButtons;

            private TaskButton selectedButton;

            private const int MIN_DIALOG_HEIGHT = 100;

            public TaskDialogForm()
            {
                InitializeComponent();
            }

            #region InitializeComponent

            private void InitializeComponent()
            {
                this.panelCommands = new System.Windows.Forms.Panel();
                this.picLogo = new System.Windows.Forms.PictureBox();
                this.labelDescription = new System.Windows.Forms.Label();
                this.panelHeader = new System.Windows.Forms.Panel();
                this.labelTitle = new System.Windows.Forms.Label();
                ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
                this.panelHeader.SuspendLayout();
                this.SuspendLayout();
                // 
                // panelCommands
                // 
                this.panelCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                            | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
                this.panelCommands.Location = new System.Drawing.Point(12, 88);
                this.panelCommands.Name = "panelCommands";
                this.panelCommands.Size = new System.Drawing.Size(421, 144);
                // 
                // picLogo
                // 
                this.picLogo.Image = global::MbUnit.Icarus.Properties.Resources.MbUnitLogo;
                this.picLogo.Location = new System.Drawing.Point(324, -5);
                this.picLogo.Name = "picLogo";
                this.picLogo.Size = new System.Drawing.Size(133, 82);
                // 
                // labelDescription
                // 
                this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
                this.labelDescription.BackColor = System.Drawing.Color.Transparent;
                this.labelDescription.Location = new System.Drawing.Point(9, 33);
                this.labelDescription.Name = "labelDescription";
                this.labelDescription.Size = new System.Drawing.Size(309, 30);
                // 
                // panelHeader
                // 
                this.panelHeader.BackColor = System.Drawing.Color.White;
                this.panelHeader.Controls.Add(this.labelDescription);
                this.panelHeader.Controls.Add(this.labelTitle);
                this.panelHeader.Controls.Add(this.picLogo);
                this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
                this.panelHeader.Location = new System.Drawing.Point(0, 0);
                this.panelHeader.Name = "panelHeader";
                this.panelHeader.Size = new System.Drawing.Size(445, 71);
                // 
                // labelTitle
                // 
                this.labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
                this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.labelTitle.Location = new System.Drawing.Point(9, 9);
                this.labelTitle.Name = "labelTitle";
                this.labelTitle.Size = new System.Drawing.Size(309, 20);
                // 
                // TaskDialogForm
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(445, 100);
                this.Controls.Add(this.panelCommands);
                this.Controls.Add(this.panelHeader);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "TaskDialogForm";
                this.ShowInTaskbar = false;
                this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
                this.panelHeader.ResumeLayout(false);
                this.ResumeLayout(false);
            }

            #endregion

            public override string Text
            {
                get { return base.Text; }
                set 
                {
                    base.Text = value;
                    labelTitle.Text = value;
                }
            }

            public string Description
            {
                get { return this.labelDescription.Text; }
                set { this.labelDescription.Text = value; }
            }

            public TaskButton[] Buttons
            {
                get { return (TaskButton[])this.buttons.Clone(); }
                set 
                { 
                    this.buttons = (TaskButton[])value.Clone();
                    InitButtons();
                }
            }

            protected override void OnLayout(LayoutEventArgs levent)
            {
                int leftMargin = 8;
                int rightMargin = 8;

                int commandButtonVMargin = 8;

                int insetWidth = panelCommands.Width - leftMargin - rightMargin;

                int y = 0;

                if (this.activeButtons != null)
                {
                    for (int i = 0; i < this.activeButtons.Length; ++i)
                    {
                        this.activeButtons[i].Location = new Point(leftMargin, y);
                        this.activeButtons[i].Width = insetWidth;
                        this.activeButtons[i].PerformLayout();
                        y += this.activeButtons[i].Height + commandButtonVMargin;
                    }
                }

                this.ClientSize = new Size(this.ClientSize.Width, MIN_DIALOG_HEIGHT + y);

                base.OnLayout(levent);
            }

            private void InitButtons()
            {
                SuspendLayout();

                if (this.activeButtons != null)
                {
                    foreach (TaskButton taskButton in this.activeButtons)
                    {
                        panelCommands.Controls.Remove(taskButton);
                        taskButton.Click -= TaskButton_Click;
                        taskButton.Dispose();
                    }

                    this.activeButtons = null;
                }

                this.activeButtons = new TaskButton[this.buttons.Length];

                for (int i = 0; i < this.activeButtons.Length; ++i)
                {
                    TaskButton taskButton = this.buttons[i];
                    taskButton.Click += TaskButton_Click;

                    this.activeButtons[i] = taskButton;
                    panelCommands.Controls.Add(taskButton);
                }

                ResumeLayout();
            }

            public TaskButton SelectedButton
            {
                get { return this.selectedButton; }
            }

            private void TaskButton_Click(object sender, EventArgs e)
            {
                this.selectedButton = (TaskButton)sender;
                this.Close();
            }
        }

        #endregion
    }
}
