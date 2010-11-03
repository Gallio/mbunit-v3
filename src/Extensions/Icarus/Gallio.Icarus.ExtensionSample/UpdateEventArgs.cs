using System;

namespace Gallio.Icarus.ExtensionSample
{
    public class UpdateEventArgs : EventArgs
    {
        public string Text { get; private set; }

        public UpdateEventArgs(string text)
        {
            Text = text;
        }
    }
}