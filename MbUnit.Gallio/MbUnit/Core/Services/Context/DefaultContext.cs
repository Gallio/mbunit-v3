using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Services.Reflection;

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
        private TestInfo testInfo;

        private event EventHandler exiting;

        public DefaultContext(IContext parent, TestInfo testInfo)
        {
            this.parent = parent;
            this.testInfo = testInfo;
            this.isActive = true;
        }

        public object SyncRoot
        {
            get { return syncRoot; }
        }

        public IContext Parent
        {
            get { return parent; }
        }

        public bool IsActive
        {
            get { return isActive; }
        }

        public TestInfo TestInfo
        {
            get { return testInfo; }
        }

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

        public void SetData<T>(string key, T value)
        {
            lock (syncRoot)
            {
                if (data == null)
                    data = new Dictionary<string, object>();

                data[key] = value;
            }
        }

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
