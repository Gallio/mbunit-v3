using System;
using System.Collections;
using System.Reflection;

namespace MbUnit.Core.Collections
{
    /// <summary>
    /// Summary description for AttributedMethodEnumerator.
    /// </summary>
    public sealed class AttributedMethodEnumerator : IEnumerator
    {
        private MethodInfo[] methods;
        private IEnumerator methodEnumerator;
        private Type customAttributeType;

        public AttributedMethodEnumerator(Type testedType, Type customAttributeType)
        {
            if (testedType == null)
                throw new ArgumentNullException("testedType");
            if (customAttributeType == null)
                throw new ArgumentNullException("customAttributeType");

            this.methods = testedType.GetMethods();
            this.customAttributeType = customAttributeType;
            this.methodEnumerator = methods.GetEnumerator();
        }

        public void Reset()
        {
            this.methodEnumerator.Reset();
        }

        public MethodInfo Current
        {
            get
            {
                return (MethodInfo)this.methodEnumerator.Current;
            }
        }

        Object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        public bool MoveNext()
        {
            bool success = false;
            while (true)
            {
                success = this.methodEnumerator.MoveNext();
                if (!success)
                    break;

                if (TypeHelper.HasCustomAttribute(
                    (MethodInfo)this.methodEnumerator.Current,
                    this.customAttributeType
                    ))
                {
                    success = true;
                    break;
                }
                else
                    success = false;
            }

            return success;
        }
    }
}
