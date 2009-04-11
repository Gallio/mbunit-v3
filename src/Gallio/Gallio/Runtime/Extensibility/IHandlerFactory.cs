using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Collections;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// A handler factory creates handlers for activating plugin, component and traits instances.
    /// </summary>
    public interface IHandlerFactory
    {
        /// <summary>
        /// Creates a handler.
        /// </summary>
        /// <param name="serviceLocator">The service locator for locating dependent services, not null</param>
        /// <param name="resourceLocator">The resource locator for locating dependent resources, not null</param>
        /// <param name="contractType">The contract type: the handler will ensure that the objects it produces are subclasses or implementations of the contract type</param>
        /// <param name="objectType">The object type: the handler will produce objects that are (possibly decorated) instances of the object type</param>
        /// <param name="properties">The configuration properties for the objects produced by the handler</param>
        /// <returns>The handler</returns>
        IHandler CreateHandler(IServiceLocator serviceLocator, IResourceLocator resourceLocator, Type contractType, Type objectType, PropertySet properties);
    }
}
