using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor;

namespace MbUnit.Icarus
{
    public partial class Scripting : Form
    {
        public Scripting()
        {
            InitializeComponent();

            textEditorControl1.ShowEOLMarkers = false;
            textEditorControl1.ShowInvalidLines = false;
            textEditorControl1.ShowSpaces = false;
            textEditorControl1.ShowTabs = false;

            textEditorControl1.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("C#");

            textEditorControl1.Text = @"
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MbUnit.GUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}";

        }
    }
}