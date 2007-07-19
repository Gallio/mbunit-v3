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

namespace MbUnit.Framework.Kernel.Utilities
{
    /// <summary>
    /// A variation on <see cref="EventHandler{T}" /> with a typed sender
    /// parameter to eliminate redundant casts.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="e">The event arguments</param>
    /// <typeparam name="TSender">The sender type</typeparam>
    /// <typeparam name="TEventArgs">The event args type</typeparam>
    public delegate void TypedEventHandler<TSender, TEventArgs>(TSender sender, TEventArgs e)
        where TEventArgs : EventArgs;
}
