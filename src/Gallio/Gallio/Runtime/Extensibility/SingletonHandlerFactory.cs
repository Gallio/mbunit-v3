using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Collections;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// A handler factory for singletons.
    /// </summary>
    public class SingletonHandlerFactory : IHandlerFactory
    {
        /// <inheritdoc />
        public IHandler CreateHandler(IServiceLocator serviceLocator, IResourceLocator resourceLocator, Type contractType, Type objectType, PropertySet properties)
        {
            throw new NotImplementedException();
        }
    }
}
