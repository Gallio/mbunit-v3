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

using System.ComponentModel;
using Gallio.UI.Common.Synchronization;

namespace Gallio.UI.DataBinding
{
    /// <summary>
    /// Wrapper for a field, that implements INotifyPropertyChanged.
    /// </summary>
    /// <typeparam name="T">The type to wrap.</typeparam>
    public class Observable<T> : INotifyPropertyChanged
    {
        private T value;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Observable()
        { }

        /// <summary>
        /// Constructor initializing field.
        /// </summary>
        /// <param name="value">The initial value to wrap.</param>
        public Observable(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// The value being wrapped.
        /// </summary>
        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
             
                if (PropertyChanged == null)
                    return;

                SynchronizationContext.Post(delegate
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                }, this);
            }
        }

        /// <summary>
        /// Implicit operator allowing use of value.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static implicit operator T(Observable<T> val)
        {
            return val.value;
        }

        /// <summary>
        /// Event fired when the value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}