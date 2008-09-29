using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;

namespace Gallio.VisualStudio.Shell.ToolWindows
{
    /// <summary>
    /// 
    /// </summary>
    [Guid("9C9191A4-CDFB-4931-9B96-6CC7CD0BC203")]
    public class GallioToolWindow : ToolWindowPane
    {
    	/// <summary>
		/// Default constructor. Calls base class with Package Instance
		/// </summary>
        public GallioToolWindow()
            : base(ShellAccessor.Shell.Package as IServiceProvider)
		{
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        public void SetContent(Control content)
        {
            Control.SetContent(content);
        }

        private GallioToolWindowControl control;

        /// <summary>
        /// Gets the window that is associated with this window pane.
        /// </summary>
        public GallioToolWindowControl Control
        {
            get
            {
                if (control == null)
                {
                    control = new GallioToolWindowControl();
                    control.Visible = true;
                    control.ToolWindow = this;
                }

                return control;
            }
        }

        /// <summary>
        /// Gets the window that is associated with this window pane.
        /// </summary>
        public override IWin32Window Window
        {
            get
            {
                return Control;
            }
        }

        /// <summary>
        /// Gets/sets the window caption.
        /// Due to a bug in the VS2008 SDK, <see cref="ToolWindowPane.Caption"/> does not work properly.
        /// </summary>
        public new string Caption
        {
            get
            {
                object caption;
                Frame.GetProperty((int)__VSFPROPID.VSFPROPID_Caption, out caption);
                return (string)caption;
            }
            
            set
            {
                Frame.SetProperty((int)__VSFPROPID.VSFPROPID_Caption, value);
            }
        }

        /// <summary>
        /// Gets/sets the frame hosting the window.
        /// </summary>
        public new IVsWindowFrame Frame
        {
            get
            {
                return (IVsWindowFrame)base.Frame;
            }

            set
            {
                base.Frame = value;
            }
        }

        /// <summary>
        /// Shows the window frame.
        /// </summary>
        public void Show()
        {
            Frame.Show();
        }

        /// <summary>
        /// Hides the window frame.
        /// </summary>
        public void Hide()
        {
            Frame.Hide();
        }

        /// <summary>
        /// Closes the window frame.
        /// </summary>
        public void Close()
        {
            Frame.CloseFrame(0);
        }


    }
}
