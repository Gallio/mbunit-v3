using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Gallio.Icarus
{
    public class WindowCollection : IWindowCollection
    {
        private readonly Dictionary<string, Window> windows = new Dictionary<string, Window>();

        public IEnumerator GetEnumerator()
        {
            return windows.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return windows.Count; }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public IWindow Add(string identifier, Control content, string caption)
        {
            return Add(identifier, content, caption, null);
        }

        public IWindow Add(string identifier, Control content, string caption, Icon icon)
        {
            if (windows.ContainsKey(identifier))
                throw new Exception("Identifier is not unique");

            Window window = new Window(content, caption);
            if (icon != null)
                window.Icon = icon;

            windows.Add(identifier, window);
            return window;
        }

        public void Remove(string identifier)
        {
            if (!windows.ContainsKey(identifier))
                throw new Exception("Invalid identifier");

            windows.Remove(identifier);
        }

        public IWindow this[string identifier]
        {
            get { return windows[identifier]; }
        }
    }
}
