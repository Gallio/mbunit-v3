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

using System.Threading;

namespace Gallio.UI.Common.Synchronization
{
    ///<summary>
    /// Default implementation of ISynchronizationContext.
    ///</summary>
    public class SynchronizationContext : ISynchronizationContext
    {
        private readonly System.Threading.SynchronizationContext synchronizationContext;

        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="synchronizationContext">The synchonization context to wrap.</param>
        public SynchronizationContext(System.Threading.SynchronizationContext synchronizationContext)
        {
            this.synchronizationContext = synchronizationContext;
        }

        /// <inheritdoc />
        public void Post(SendOrPostCallback sendOrPostCallback, object state)
        {
            synchronizationContext.Post(sendOrPostCallback, state);
        }

        /// <inheritdoc />
        public void Send(SendOrPostCallback sendOrPostCallback, object state)
        {
            synchronizationContext.Send(sendOrPostCallback, state);
        }

        ///<summary>
        /// Returns a shared ISynchronizationContext instance.
        ///</summary>
        /// <remarks>
        /// Usually used to stash a reference to the win forms synchronization 
        /// context (WindowsFormsSynchronizationContext.Current), used for 
        /// cross-thread data binding.
        /// </remarks>
        public static ISynchronizationContext Instance
        {
            get; set;
        }
    }
}