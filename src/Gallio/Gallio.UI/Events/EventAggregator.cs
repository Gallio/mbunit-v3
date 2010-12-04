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

using Gallio.Runtime.Extensibility;

namespace Gallio.UI.Events
{
    /// <inheritdoc />
    public class EventAggregator : IEventAggregator
    {
        private readonly IServiceLocator serviceLocator;

        ///<summary>
        /// Ctor.
        ///</summary>
        ///<param name="serviceLocator">A service locator used to find registered handlers.</param>
        public EventAggregator(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        /// <inheritdoc />
        public void Send<T>(object sender, T message) where T : Event
        {
            foreach (var handler in serviceLocator.ResolveAll<Handles<T>>())
            {
                if (handler != sender)
                    handler.Handle(message);
            }
        }
    }
}
