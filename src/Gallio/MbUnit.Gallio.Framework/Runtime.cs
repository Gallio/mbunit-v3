// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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

using MbUnit.Framework.Kernel.RuntimeSupport;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Provides functions for obtaining runtime services such as XML documentation
    /// resolution.
    /// </para>
    /// </summary>
    public static class Runtime
    {
        private static IRuntime instance;
        private static IXmlDocumentationResolver cachedXmlDocumentationResolver;
        private static IContextManager cachedContextManager;

        /// <summary>
        /// Gets the runtime instance.
        /// </summary>
        /// <remarks>
        /// This value is never null while tests are executing but it may be null at
        /// other times when the framework is not fully initialized.
        /// </remarks>
        public static IRuntime Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Sets the runtime instance.
        /// </summary>
        /// <remarks>
        /// This setter is deliberately hidden from normal user code which should have no business
        /// ever setting the runtime instance.  Refer to the CoreRuntimeHolder in MbUnit.Gallio.Core
        /// for a supported mechanism for setting the runtime instance.
        /// </remarks>
        /// <param name="instance">The runtime instance, or null if none</param>
        internal static void SetInstance(IRuntime instance)
        {
            Runtime.instance = instance;
            cachedXmlDocumentationResolver = null;
            cachedContextManager = null;
        }

        /// <summary>
        /// Gets the XML documentation resolver.
        /// </summary>
        public static IXmlDocumentationResolver XmlDocumentationResolver
        {
            get
            {
                if (cachedXmlDocumentationResolver == null)
                    cachedXmlDocumentationResolver = instance.Resolve<IXmlDocumentationResolver>();
                return cachedXmlDocumentationResolver;
            }
        }

        /// <summary>
        /// Gets the context manager.
        /// </summary>
        public static IContextManager ContextManager
        {
            get
            {
                if (cachedContextManager == null)
                    cachedContextManager = instance.Resolve<IContextManager>();
                return cachedContextManager;
            }
        }
    }
}
