using System;
using System.Collections;
using System.Reflection;

namespace MbUnit.Core.Collections
{
    /// <summary>
    /// Summary description for AttributedMethodCollection.
    /// </summary>
    public sealed class AttributedMethodCollection : ICollection
    {
        private Type testedType;
        private Type customAttributeType;

        public AttributedMethodCollection(Type testedType, Type customAttributeType)
        {
            if (testedType == null)
                throw new ArgumentNullException("testedType");
            if (customAttributeType == null)
                throw new ArgumentNullException("customAttributeType");
            this.testedType = testedType;
            this.customAttributeType = customAttributeType;
        }

        public Object SyncRoot
        {
            get
            {
                return this;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public void CopyTo(Array array, int index)
        {
            int i = index;
            foreach (MethodInfo mi in this)
            {
                array.SetValue(mi, i++);
            }
        }

        public int Count
        {
            get
            {
                AttributedMethodEnumerator en = GetEnumerator();
                int n = 0;
                while (en.MoveNext())
                    ++n;
                return n;
            }
        }

        public AttributedMethodEnumerator GetEnumerator()
        {
            return new AttributedMethodEnumerator(
                this.testedType,
                this.customAttributeType
                );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
