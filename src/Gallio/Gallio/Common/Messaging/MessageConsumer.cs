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

namespace Gallio.Common.Messaging
{
    /// <summary>
    /// A message consumer provides a simple pattern for handling messages based on their type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A message consumer consists of a series of typed handler clauses which are traversed in
    /// order until a handler which matches the message type is found.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// MessageConsumer consumer = new MessageConsumer()
    ///     .Handle<FooMessage>(message => DoSomethingWithFooMessage(message))
    ///     .Handle<BarMessage>(message => DoSomethingWithBarMessage(message))
    ///     .Otherwise(message => throw new NotSupportedException());
    ///     
    /// consumer.Consume(new FooMessage());
    /// consumer.Consume(new BarMessage());
    /// ]]>
    /// </code>
    /// </example>
    public sealed class MessageConsumer : IMessageSink
    {
        private readonly Handler handler;

        private MessageConsumer(Handler handler)
        {
            this.handler = handler;
        }

        /// <summary>
        /// Creates a new message consumer with no handler clauses.
        /// </summary>
        public MessageConsumer()
        {
        }

        /// <summary>
        /// Consumes a message.
        /// </summary>
        /// <param name="message">The message to consume.</param>
        /// <returns>True if the message was consumed by a handler, false if no handler could consume it.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null.</exception>
        public bool Consume(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            return handler != null && handler.Consume(message);
        }

        void IMessageSink.Publish(Message message)
        {
            Consume(message);
        }

        /// <summary>
        /// Adds a handler for a particular message type.
        /// </summary>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="handlerAction">The message handler action.</param>
        /// <returns>The message consumer.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="handlerAction"/> is null.</exception>
        public MessageConsumer Handle<TMessage>(Action<TMessage> handlerAction)
            where TMessage : Message
        {
            if (handlerAction == null)
                throw new ArgumentNullException("handlerAction");

            return new MessageConsumer(new Handler<TMessage>(handler, handlerAction));
        }

        /// <summary>
        /// Adds a catch-all handler.
        /// </summary>
        /// <param name="handlerAction">The message handler action.</param>
        /// <returns>The message consumer.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="handlerAction"/> is null.</exception>
        public MessageConsumer Otherwise(Action<Message> handlerAction)
        {
            return Handle(handlerAction);
        }

        private abstract class Handler
        {
            public abstract bool Consume(Message message);
        }

        private sealed class Handler<TMessage> : Handler
            where TMessage : Message
        {
            private readonly Handler parent;
            private readonly Action<TMessage> handlerAction;

            public Handler(Handler parent, Action<TMessage> handlerAction)
            {
                this.parent = parent;
                this.handlerAction = handlerAction;
            }

            public override bool Consume(Message message)
            {
                TMessage typedMessage = message as TMessage;
                if (typedMessage != null)
                {
                    handlerAction(typedMessage);
                    return true;
                }

                return parent != null && parent.Consume(message);
            }
        }
    }
}
