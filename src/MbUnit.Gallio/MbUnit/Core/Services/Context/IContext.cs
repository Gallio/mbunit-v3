using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MbUnit.Core.Services.Reflection;

namespace MbUnit.Core.Services.Context
{
    /// <summary>
    /// A context is created to represent the scope of test execution.
    /// Contexts may be nested to capture composition relationships, such as
    /// test assemblies containing test fixtures containing test templates
    /// containing test instances.  Each nested level corresponds to the
    /// hierarchical scope of a test.
    /// </summary>
    /// <remarks>
    /// All context operations are thread-safe.
    /// </remarks>
    public interface IContext
    {
        /// <summary>
        /// Returns a monitor object that can be used to synchronize concurrent
        /// access to the context across multiple operations.
        /// </summary>
        object SyncRoot { get; }

        /// <summary>
        /// Gets the parent context or null if this is the root context.
        /// </summary>
        IContext Parent { get; }
        
        /// <summary>
        /// Returns true if the context is active.
        /// Becomes false when the context is exited.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Gets information about the test running within the context.
        /// </summary>
        TestInfo TestInfo { get; }

        /// <summary>
        /// The exited event is raised when the context is about to be exited.
        /// Clients may attach handlers to this event to perform cleanup
        /// activities and other tasks as needed.  If a new event handler is
        /// added after the context has been exited, it is immediately invoked.
        /// </summary>
        event EventHandler Exiting;

        /// <summary>
        /// Gets the value of the context data with the specified key and returns true
        /// if it exists.
        /// </summary>
        /// <typeparam name="T">The type of value to get</typeparam>
        /// <param name="key">The context data key</param>
        /// <param name="value">Set to the value that was retrieved or the default for the type if none</param>
        /// <returns>True if the context contained a value with the specified key, false otherwise</returns>
        bool TryGetData<T>(string key, out T value);

        /// <summary>
        /// Sets the value of the context data with the specified key, overwriting
        /// any that might previously have been present.
        /// </summary>
        /// <typeparam name="T">The type of value to store</typeparam>
        /// <param name="key">The context data key</param>
        /// <param name="value">The value to store</param>
        void SetData<T>(string key, T value);

        /// <summary>
        /// Removes the value in the context data with the specified key.
        /// Does nothing if there is no value with the specified key.
        /// </summary>
        /// <param name="key">The context data key</param>
        void RemoveData(string key);
    }
}
