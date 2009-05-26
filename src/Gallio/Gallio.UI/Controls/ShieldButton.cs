using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Gallio.Common.Platform;

namespace Gallio.UI.Controls
{
    /// <summary>
    /// An extension of the Button class that displays the "Shield" icon when
    /// privilege elevation is required.
    /// </summary>
    public class ShieldButton : Button
    {
        private bool shield;

        /// <summary>
        /// Creates a shield button.
        /// </summary>
        public ShieldButton()
        {
            FlatStyle = FlatStyle.System;
        }

        /// <summary>
        /// An event raised when the value of <see cref="Shield" /> has changed.
        /// </summary>
        public event EventHandler ShieldChanged;

        /// <summary>
        /// Gets or sets whether the shield icon should be displayed.
        /// </summary>
        public bool Shield
        {
            get { return shield; }
            set
            {
                if (value != shield)
                {
                    shield = value;
                    UpdateShield();
                    OnShieldChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="ShieldChanged" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected virtual void OnShieldChanged(EventArgs e)
        {
            if (ShieldChanged != null)
                ShieldChanged(this, e);
        }

        private void UpdateShield()
        {
            if (! DotNetRuntimeSupport.IsUsingMono)
            {
                Native.SendMessage(new HandleRef(this, Handle), Native.BCM_SETSHIELD,
                    new IntPtr(0), new IntPtr(shield ? 1 : 0));
            }
        }
    }
}
