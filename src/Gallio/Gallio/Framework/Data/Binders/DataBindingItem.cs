// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

namespace Gallio.Framework.Data.Binders
{
    /// <summary>
    /// <para>
    /// A <see cref="DataBindingItem" /> contains an <see cref="IDataRow"/>
    /// that provides source information to be used by <see cref="IDataBindingAccessor"/>s
    /// to retrieve the value associated with a given data binding.
    /// </para>
    /// <para>
    /// The <see cref="DataBindingItem" /> must be disposed after use to ensure
    /// that resources used during data binding are reclaimed in a timely fashion.
    /// <seealso cref="Disposed"/>
    /// </para>
    /// </summary>
    public sealed class DataBindingItem : IDisposable
    {
        private EventHandler disposed;
        private IDataRow row;

        /// <summary>
        /// Creates a data binding item.
        /// </summary>
        /// <param name="row">The data row associated with the item</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="row"/> is null</exception>
        public DataBindingItem(IDataRow row)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            this.row = row;
        }

        /// <summary>
        /// Disposes the item and calls the <see cref="Disposed" /> event handlers.
        /// </summary>
        public void Dispose()
        {
            if (row != null)
            {
                row = null;

                if (disposed != null)
                    disposed(this, EventArgs.Empty);

                disposed = null;
            }
        }

        /// <summary>
        /// Adds or removes an event handler to be called when the item is disposed.
        /// </summary>
        /// <remarks>
        /// A consumer may enlist in the disposal process by registering an event handler
        /// for to the <see cref="Disposed" /> event.  These event handlers will be called
        /// in the reverse order from which they were added to ensure that more recently
        /// added consumers are provided a chance to clean up before any previously
        /// added consumers that they might depend on.
        /// </remarks>
        public event EventHandler Disposed
        {
            add
            {
                // Note: Compose delegates in reverse order from usual.
                disposed = (EventHandler) Delegate.Combine(value, disposed);
            }
            remove
            {
                disposed -= value;
            }
        }

        /// <summary>
        /// Gets the <see cref="IDataRow" /> associated with the item.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the item has been disposed</exception>
        public IDataRow GetRow()
        {
            if (row == null)
                throw new ObjectDisposedException(GetType().Name);
            return row;
        }
    }
}
