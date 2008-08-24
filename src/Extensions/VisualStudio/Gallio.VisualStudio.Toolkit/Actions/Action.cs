using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.VisualStudio.Toolkit.Actions
{
    /// <summary>
    /// Represents an action that is associated with a button on a Visual Studio command bar.
    /// </summary>
    public abstract class Action
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="button">The button that was clicked to invoke the action</param>
        public abstract void Execute(ActionButton button);

        /// <summary>
        /// Provides an opportunity for the action to update the status, text and other
        /// properties of a button.
        /// </summary>
        /// <param name="button">The button to update</param>
        public virtual void Update(ActionButton button)
        {
        }
    }
}
