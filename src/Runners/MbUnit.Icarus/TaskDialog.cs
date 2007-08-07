using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MbUnit.Icarus.Properties;

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
            private const int MIN_DIALOG_HEIGHT = 100;
            private TaskButton[] activeButtons;
            private TaskButton[] buttons;
            private Label labelDescription;
            private Label labelTitle;
            private Panel panelCommands;
            private Panel panelHeader;
            private PictureBox picLogo;

            private TaskButton selectedButton;

            public TaskDialogForm()
            {
                InitializeComponent();
            }

            #region InitializeComponent

            private void InitializeComponent()
            {
                panelCommands = new Panel();
                picLogo = new PictureBox();
                labelDescription = new Label();
                panelHeader = new Panel();
                labelTitle = new Label();
                ((ISupportInitialize) (picLogo)).BeginInit();
                panelHeader.SuspendLayout();
                SuspendLayout();
                // 
                // panelCommands
                // 
                panelCommands.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom)
                                        | AnchorStyles.Left)
                                       | AnchorStyles.Right;
                panelCommands.Location = new Point(12, 88);
                panelCommands.Name = "panelCommands";
                panelCommands.Size = new Size(421, 144);
                // 
                // picLogo
                // 
                picLogo.Image = Resources.MbUnitLogo;
                picLogo.Location = new Point(324, -5);
                picLogo.Name = "picLogo";
                picLogo.Size = new Size(133, 82);
                // 
                // labelDescription
                // 
                labelDescription.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                          | AnchorStyles.Right;
                labelDescription.BackColor = Color.Transparent;
                labelDescription.Location = new Point(9, 33);
                labelDescription.Name = "labelDescription";
                labelDescription.Size = new Size(309, 30);
                // 
                // panelHeader
                // 
                panelHeader.BackColor = Color.White;
                panelHeader.Controls.Add(labelDescription);
                panelHeader.Controls.Add(labelTitle);
                panelHeader.Controls.Add(picLogo);
                panelHeader.Dock = DockStyle.Top;
                panelHeader.Location = new Point(0, 0);
                panelHeader.Name = "panelHeader";
                panelHeader.Size = new Size(445, 71);
                // 
                // labelTitle
                // 
                labelTitle.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                    | AnchorStyles.Right;
                labelTitle.Font =
                    new Font("Microsoft Sans Serif", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
                labelTitle.Location = new Point(9, 9);
                labelTitle.Name = "labelTitle";
                labelTitle.Size = new Size(309, 20);
                // 
                // TaskDialogForm
                // 
                AutoScaleDimensions = new SizeF(6F, 13F);
                AutoScaleMode = AutoScaleMode.Font;
                ClientSize = new Size(445, 100);
                Controls.Add(panelCommands);
                Controls.Add(panelHeader);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                Name = "TaskDialogForm";
                ShowInTaskbar = false;
                StartPosition = FormStartPosition.CenterParent;
                ((ISupportInitialize) (picLogo)).EndInit();
                panelHeader.ResumeLayout(false);
                ResumeLayout(false);
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
                set { labelDescription.Text = value; }
            }

            public TaskButton[] Buttons
            {
                set
                {
                    buttons = (TaskButton[]) value.Clone();
                    InitButtons();
                }
            }

            public TaskButton SelectedButton
            {
                get { return selectedButton; }
            }

            protected override void OnLayout(LayoutEventArgs levent)
            {
                int leftMargin = 8;
                int rightMargin = 8;

                int commandButtonVMargin = 8;

                int insetWidth = panelCommands.Width - leftMargin - rightMargin;

                int y = 0;

                if (activeButtons != null)
                {
                    for (int i = 0; i < activeButtons.Length; ++i)
                    {
                        activeButtons[i].Location = new Point(leftMargin, y);
                        activeButtons[i].Width = insetWidth;
                        activeButtons[i].PerformLayout();
                        y += activeButtons[i].Height + commandButtonVMargin;
                    }
                }

                ClientSize = new Size(ClientSize.Width, MIN_DIALOG_HEIGHT + y);

                base.OnLayout(levent);
            }

            private void InitButtons()
            {
                SuspendLayout();

                if (activeButtons != null)
                {
                    foreach (TaskButton taskButton in activeButtons)
                    {
                        panelCommands.Controls.Remove(taskButton);
                        taskButton.Click -= TaskButton_Click;
                        taskButton.Dispose();
                    }

                    activeButtons = null;
                }

                activeButtons = new TaskButton[buttons.Length];

                for (int i = 0; i < activeButtons.Length; ++i)
                {
                    TaskButton taskButton = buttons[i];
                    taskButton.Click += TaskButton_Click;

                    activeButtons[i] = taskButton;
                    panelCommands.Controls.Add(taskButton);
                }

                ResumeLayout();
            }

            private void TaskButton_Click(object sender, EventArgs e)
            {
                selectedButton = (TaskButton) sender;
                Close();
            }
        }

        #endregion
    }
}