// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Collections.Generic;
using System.Text;

namespace Gallio.Model.Data
{
    /// <summary>
    /// A data binding context is used to track resources produced during
    /// the data binding process.  A new context is created each time data
    /// binding occurs.  It must be disposed when the data binding activity
    /// is finished.
    /// </summary>
    /// <remarks>
    /// Essentially the data binding context consists of a very limited
    /// form of a unit of work.
    /// </remarks>
    public interface IDataBindingContext : IDisposable
    {
        /// <summary>
        /// This event is raised when the data binding context is being
        /// disposed.  Data binding participants that must perform reclamation
        /// of bound objects should register to receive this event and
        /// execute their decommission concerns.
        /// </summary>
        event EventHandler Disposing;

        /// <summary>
        /// Gets the data binder used by the context.
        /// </summary>
        IDataBinder Binder { get; }
    }
}
