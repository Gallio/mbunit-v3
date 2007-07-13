using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Model;

namespace MbUnit.Core.Services.Context
{
    /// <summary>
    /// Default implementation of a context object.
    /// </summary>
    public sealed class DefaultContext : IContext
    {
        private readonly object syncRoot = new object();
        private IContext parent;
        private IDictionary<string, object> data;
        private bool isActive;
        private ITest currentTest;

        private event EventHandler exiting;

        /// <summary>
        /// Creates a new context.
        /// </summary>
        /// <param name="parent">The parent context</param>
        /// <param name="currentTest">The current test</param>
        public DefaultContext(IContext parent, ITest currentTest)
        {
            this.parent = parent;
            this.currentTest = currentTest;

            this.isActive = true;
        }

        /// <inheritdoc />
        public object SyncRoot
        {
            get { return syncRoot; }
        }

        /// <inheritdoc />
        public IContext Parent
        {
            get { return parent; }
        }

        /// <inheritdoc />
        public bool IsActive
        {
            get { return isActive; }
        }

        /// <inheritdoc />
        public ITest CurrentTest
        {
            get { return currentTest; }
        }

        /// <inheritdoc />
        public event EventHandler Exiting
        {
            add
            {
                lock (syncRoot)
                {
                    if (isActive)
                    {
                        exiting += value;
                        return;
                    }

                    value(this, EventArgs.Empty);
                }
            }
            remove
            {
                lock (syncRoot)
                    exiting -= value;
            }
        }

        /// <inheritdoc />
        public void Exit()
        {
            EventHandler handlers;

            lock (syncRoot)
            {
                if (!isActive)
                    return;

                handlers = exiting;
                exiting = null;
                isActive = false;
            }

            if (handlers != null)
                handlers(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public bool TryGetData<T>(string key, out T value)
        {
            lock (syncRoot)
            {
                if (data != null)
                {
                    object untypedValue;
                    if (data.TryGetValue(key, out untypedValue))
                    {
                        value = (T)untypedValue;
                        return true;
                    }
                }

                value = default(T);
                return false;
            }
        }

        /// <inheritdoc />
        public void SetData<T>(string key, T value)
        {
            lock (syncRoot)
            {
                if (data == null)
                    data = new Dictionary<string, object>();

                data[key] = value;
            }
        }

        /// <inheritdoc />
        public void RemoveData(string key)
        {
            lock (syncRoot)
            {
                if (data != null)
                    data.Remove(key);
            }
        }
    }
}
