// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Diagnostics;
using Gallio.Loader.Isolation;
using Gallio.Loader.SharedEnvironment;
using Microsoft.VisualStudio.TestTools.TestAdapter;

namespace Gallio.VisualStudio.Tip
{
    /// <summary>
    /// Proxies the <see cref="ITestAdapter" /> interface over to the actual implementation
    /// after initializing the loader.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This implementation actually loads up Gallio within a fresh AppDomain because the
    /// test adapter is called from a context that includes the test assemblies within
    /// its ApplicationBase.  That becomes a problem if the test assemblies are linked to
    /// a different version of Gallio.dll than that of the installed runtime.  The resulting
    /// conflict produces hard to debug problems related to types being loaded
    /// multiple times from different locations.
    /// </para>
    /// <para>
    /// So we sidestep this whole mess at least while trying to get Gallio off the ground.
    /// Then Gallio takes its own ordinary measures to cope with any version conflicts.
    /// </para>
    /// </remarks>
    public class GallioTestAdapterProxy : MarshalByRefTestAdapterProxy
    {
        public GallioTestAdapterProxy()
            : base(CreateRemoteShim())
        {
        }

        private static ITestAdapter CreateRemoteShim()
        {
            IIsolatedEnvironment environment = SharedEnvironmentManager.GetSharedEnvironment();

            Type shimType = typeof(Shim);
            ITestAdapter shim = (ITestAdapter)environment.AppDomain.CreateInstanceAndUnwrap(shimType.Assembly.FullName, shimType.FullName);
            return shim;
        }

        private sealed class Shim : MarshalByRefTestAdapterProxy
        {
            public Shim()
                : base(ProxyHelper.GetTargetFactory().CreateTestAdapter())
            {
            }
        }
    }
}
