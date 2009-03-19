// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
