// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
    /// Holder for the current sync context (e.g. the winforms current).
    ///</summary>
    public static class SynchronizationContext
    {
        /// <summary>
        /// Wrapper for Send on the shared sync context.
        /// </summary>
        /// <param name="sendOrPostCallback">The SendOrPostCallback delegate to call.</param>
        /// <param name="state">The object passed to the delegate.</param>
        public static void Send(SendOrPostCallback sendOrPostCallback, object state)
        {
            if (Current != null)
                Current.Send(sendOrPostCallback, state);
        }

        /// <summary>
        /// Wrapper for Post on the shared sync context.
        /// </summary>
        /// <param name="sendOrPostCallback">The SendOrPostCallback delegate to call.</param>
        /// <param name="state">The object passed to the delegate.</param>
        public static void Post(SendOrPostCallback sendOrPostCallback, object state)
        {
            if (Current != null)
                Current.Post(sendOrPostCallback, state);
        }

        ///<summary>
        /// Returns a shared SynchronizationContext instance.
        ///</summary>
        /// <remarks>
        /// Usually used to stash a reference to the win forms synchronization 
        /// context (WindowsFormsSynchronizationContext.Current), used for 
        /// cross-thread data binding.
        /// </remarks>
        public static System.Threading.SynchronizationContext Current
        {
            get; set;
        }
    }
}