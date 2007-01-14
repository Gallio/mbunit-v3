using System;
using TestFu.Operations;
using System.Reflection;
using MbUnit.Core;

namespace MbUnit.Framework
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public abstract class UsingBaseAttribute : Attribute
    {
        public abstract void GetDomains(IDomainCollection domains, ParameterInfo parameter,
            object fixture);

    
        protected Object InvokeMethod(Type t, MethodInfo method)
        {
            Object fixture = null;
            try
            {
                fixture = TypeHelper.CreateInstance(t);
                return method.Invoke(fixture, null);
            }
            finally
            {
                IDisposable disposable = fixture as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }
       
    }
}
