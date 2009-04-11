using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Collections;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// A handler factory for pre-manufactured instances.
    /// </summary>
    public class InstanceHandlerFactory : IHandlerFactory
    {
        private readonly object instance;

        /// <summary>
        /// Creates an instance handler factory given a particular pre-manufactured instance.
        /// </summary>
        /// <param name="instance">The instance to be returned by the handler</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="instance"/> is null</exception>
        public InstanceHandlerFactory(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            this.instance = instance;
        }

        /// <inheritdoc />
        public IHandler CreateHandler(IServiceLocator serviceLocator, IResourceLocator resourceLocator, Type contractType, Type objectType, PropertySet properties)
        {
            if (! contractType.IsInstanceOfType(instance))
                throw new RuntimeException(string.Format("Could not satisfy contract of type '{0}' using pre-manufactured instance of type '{1}'.",
                    contractType, instance.GetType()));

            return new Handler(instance);
        }

        private sealed class Handler : IHandler
        {
            private readonly object instance;

            public Handler(object instance)
            {
                this.instance = instance;
            }

            public object Activate()
            {
                return instance;
            }
        }
    }
}
