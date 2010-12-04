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

namespace Gallio.UI.Events
{
    ///<summary>
    /// Proxy event handler.
    ///</summary>
    ///<typeparam name="T">The type of event.</typeparam>
    public class EventHandlerProxy<T> : Handles<T> where T : Event
    {
        private readonly Handles<T> target;

        ///<summary>
        /// Ctor.
        ///</summary>
        ///<param name="target">The handler to proxy for.</param>
        public EventHandlerProxy(Handles<T> target)
        {
            this.target = target;
        }

        /// <inheritdoc />
        public void Handle(T @event)
        {
            target.Handle(@event);
        }

        /// <inheritdoc />
        public bool Equals(EventHandlerProxy<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.target, target);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (EventHandlerProxy<T>)) return false;
            return Equals((EventHandlerProxy<T>) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (target != null ? target.GetHashCode() : 0);
        }

        /// <inheritdoc />
        public static bool operator ==(EventHandlerProxy<T> left, EventHandlerProxy<T> right)
        {
            return Equals(left, right);
        }

        /// <inheritdoc />
        public static bool operator !=(EventHandlerProxy<T> left, EventHandlerProxy<T> right)
        {
            return !Equals(left, right);
        }
    }
}
