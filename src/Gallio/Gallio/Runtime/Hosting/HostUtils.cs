// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using System.Reflection;
using Gallio.Reflection;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// Provides utilities for working with hosts.
    /// </summary>
    public static class HostUtils
    {
        /// <summary>
        /// Creates an instance of an object within a host.
        /// </summary>
        /// <param name="host">The host in which to create the object</param>
        /// <typeparam name="T">The type of object to create</typeparam>
        /// <returns>The object instance</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="host" /> is null</exception>
        public static T CreateInstance<T>(IHost host)
        {
            return (T) CreateInstance(host, typeof(T));
        }

        /// <summary>
        /// Creates an instance of an object within 
        /// </summary>
        /// <param name="host">The host in which to create the object</param>
        /// <param name="type">The type of object to create</param>
        /// <returns>The object instance</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="host" />
        /// or <paramref name="type"/> is null</exception>
        public static object CreateInstance(IHost host, Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // FIXME: Will need to take into account times when we might wish to redirect
            //        the assembly loading to a different path, eg. when DevelopmentRuntimePath
            //        is set.
            Assembly assembly = type.Assembly;
            IHostService hostService = host.GetHostService();
            try
            {
                return hostService.CreateInstance(assembly.FullName, type.FullName).Unwrap();
            }
            catch (Exception)
            {
                return hostService.CreateInstanceFrom(AssemblyUtils.GetAssemblyLocalPath(assembly), type.FullName).Unwrap();
            }
        }
    }
}
