// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Runtime.Remoting;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// <para>
    /// A host service enables a local client to interact with a remotely
    /// executing hosting environment.
    /// </para>
    /// <para>
    /// A host service implementation may choose to implement a keep-alive
    /// mechanism to automatically shut itself down when the service is disposed or
    /// when it has not received a ping within a set period of time.
    /// </para>
    /// </summary>
    public interface IHostService
    {
        /// <summary>
        /// Pings the host to verify and maintain connectivity.
        /// </summary>
        /// <exception cref="HostException">Thrown if the remote host is unreachable</exception>
        void Ping();

        /// <summary>
        /// <para>
        /// Asks the host to perform the specified action remotely.
        /// </para>
        /// <para>
        /// The action must be a serializable delegate so that it can be sent
        /// to the host and executed.  Generally speaking, this means it must either be
        /// a delegate for a static method or its target object must be serializable.
        /// The argument and result values must also be serializable (or null).
        /// </para>
        /// </summary>
        /// <param name="func">The action to perform</param>
        /// <param name="arg">The argument value, if any</param>
        /// <returns>The result value, if any</returns>
        /// <typeparam name="TArg">The argument type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null</exception>
        TResult Do<TArg, TResult>(Func<TArg, TResult> func, TArg arg);

        /// <summary>
        /// Creates an instance of a remote object given an assembly name and type name.
        /// </summary>
        /// <param name="assemblyName">The name of assembly that contains the type</param>
        /// <param name="typeName">The full name of the type</param>
        /// <returns>The object handle of the instance</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyName"/> or
        /// <paramref name="typeName"/> is null</exception>
        ObjectHandle CreateInstance(string assemblyName, string typeName);

        /// <summary>
        /// Creates an instance of a remote object given an assembly path and type name.
        /// </summary>
        /// <param name="assemblyPath">The path of assembly that contains the type</param>
        /// <param name="typeName">The full name of the type</param>
        /// <returns>The object handle of the instance</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyPath"/> or
        /// <paramref name="typeName"/> is null</exception>
        ObjectHandle CreateInstanceFrom(string assemblyPath, string typeName);
    }
}
