// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio
{
    /// <summary>
    /// An action chain captures a sequence of actions to be performed as
    /// part of a complex multi-part process.
    /// </summary>
    /// <remarks>
    /// Later actions in a chain will not run if prior ones throw an exception.
    /// To catch and handle exceptions, use <see cref="Around" /> to register a
    /// decorator that runs around the current action whose execution is then
    /// under your control.
    /// </remarks>
    /// <typeparam name="T">The action argument type</typeparam>
    public class ActionChain<T>
    {
        private Action<T> action;

        /// <summary>
        /// Gets a singleton action that does nothing when invoked.
        /// </summary>
        public static readonly Action<T> NoOp = delegate { };

        /// <summary>
        /// <para>
        /// Gets or sets a representation of the chain as a single action.
        /// </para>
        /// <para>
        /// The action is progressively augmented as new contributions are
        /// registered using <see cref="Before" />, <see cref="After" /> and
        /// <see cref="Around" />.  By default the action is <see cref="NoOp" />.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public Action<T> Action
        {
            get
            {
                return action ?? NoOp;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                action = value == NoOp ? null : value;
            }
        }

        /// <summary>
        /// Registers an action to perform before all other actions
        /// currently in the chain.
        /// </summary>
        /// <remarks>
        /// The value of <see cref="Action" /> will be set to a new instance
        /// that performs the specified <paramref name="action"/> before
        /// the current <see cref="Action" />.
        /// </remarks>
        /// <param name="action">The action to register</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public void Before(Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (this.action == null)
                this.action = action;
            else
                this.action = (Action<T>) Delegate.Combine(action, this.action);
        }

        /// <summary>
        /// Registers an action to perform after all other actions
        /// currently in the chain.
        /// </summary>
        /// <remarks>
        /// The value of <see cref="Action" /> will be set to a new instance
        /// that performs the specified <paramref name="action"/> after
        /// the current <see cref="Action" />.
        /// </remarks>
        /// <param name="action">The action to register</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public void After(Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (this.action == null)
                this.action = action;
            else
                this.action = (Action<T>)Delegate.Combine(this.action, action);
        }

        /// <summary>
        /// Registers an action to perform around all other actions
        /// currently in the chain.  The contained part of the chain
        /// is passed in as an action to the decorator that the decorator
        /// can choose to run (or not) as needed.
        /// </summary>
        /// <remarks>
        /// The value of <see cref="Action" /> will be set to a new instance
        /// that performs the specified <paramref name="decorator"/> around
        /// the current <see cref="Action" />.
        /// </remarks>
        /// <param name="decorator">The decorator to register</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="decorator"/> is null</exception>
        public void Around(ActionDecorator<T> decorator)
        {
            if (decorator == null)
                throw new ArgumentNullException("decorator");

            Action<T> innerAction = Action;
            action = delegate(T obj)
            {
                decorator(obj, innerAction);
            };
        }

        /// <summary>
        /// Clears the chain and sets it action to <see cref="NoOp" />.
        /// </summary>
        public void Clear()
        {
            action = NoOp;
        }
    }

    /// <summary>
    /// An action chain captures a sequence of actions to be performed as
    /// part of a complex multi-part process.
    /// </summary>
    /// <remarks>
    /// Later actions in a chain will not run if prior ones throw an exception.
    /// To catch and handle exceptions, use <see cref="Around" /> to register a
    /// decorator that runs around the current action whose execution is then
    /// under your control.
    /// </remarks>
    /// <typeparam name="T1">The first argument type</typeparam>
    /// <typeparam name="T2">The second argument type</typeparam>
    public class ActionChain<T1, T2>
    {
        private Action<T1, T2> action;

        /// <summary>
        /// Gets a singleton action that does nothing when invoked.
        /// </summary>
        public static readonly Action<T1, T2> NoOp = delegate { };

        /// <summary>
        /// <para>
        /// Gets or sets a representation of the chain as a single action.
        /// </para>
        /// <para>
        /// The action is progressively augmented as new contributions are
        /// registered using <see cref="Before" />, <see cref="After" /> and
        /// <see cref="Around" />.  By default the action is <see cref="NoOp" />.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public Action<T1, T2> Action
        {
            get
            {
                return action ?? NoOp;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                action = value == NoOp ? null : value;
            }
        }

        /// <summary>
        /// Registers an action to perform before all other actions
        /// currently in the chain.
        /// </summary>
        /// <remarks>
        /// The value of <see cref="Action" /> will be set to a new instance
        /// that performs the specified <paramref name="action"/> before
        /// the current <see cref="Action" />.
        /// </remarks>
        /// <param name="action">The action to register</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public void Before(Action<T1, T2> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (this.action == null)
                this.action = action;
            else
                this.action = (Action<T1, T2>)Delegate.Combine(action, this.action);
        }

        /// <summary>
        /// Registers an action to perform after all other actions
        /// currently in the chain.
        /// </summary>
        /// <remarks>
        /// The value of <see cref="Action" /> will be set to a new instance
        /// that performs the specified <paramref name="action"/> after
        /// the current <see cref="Action" />.
        /// </remarks>
        /// <param name="action">The action to register</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public void After(Action<T1, T2> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (this.action == null)
                this.action = action;
            else
                this.action = (Action<T1, T2>)Delegate.Combine(this.action, action);
        }

        /// <summary>
        /// Registers an action to perform around all other actions
        /// currently in the chain.  The contained part of the chain
        /// is passed in as an action to the decorator that the decorator
        /// can choose to run (or not) as needed.
        /// </summary>
        /// <remarks>
        /// The value of <see cref="Action" /> will be set to a new instance
        /// that performs the specified <paramref name="decorator"/> around
        /// the current <see cref="Action" />.
        /// </remarks>
        /// <param name="decorator">The decorator to register</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="decorator"/> is null</exception>
        public void Around(ActionDecorator<T1, T2> decorator)
        {
            if (decorator == null)
                throw new ArgumentNullException("decorator");

            Action<T1, T2> innerAction = Action;
            action = delegate(T1 arg1, T2 arg2)
            {
                decorator(arg1, arg2, innerAction);
            };
        }

        /// <summary>
        /// Clears the chain and sets it action to <see cref="NoOp" />.
        /// </summary>
        public void Clear()
        {
            action = NoOp;
        }
    }
}
