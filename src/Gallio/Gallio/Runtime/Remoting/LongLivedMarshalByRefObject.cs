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

namespace Gallio.Runtime.Remoting
{
    /// <summary>
    /// <para>
    /// A <see cref="LongLivedMarshalByRefObject"/> represents a remote
    /// object whose lifetime is managed explicitly.  It ensures that long-lived
    /// services are not inadvertently disconnected by the remoting infrastructure
    /// and remain accessible until the application discards them.
    /// </para>
    /// <para>
    /// However, it is important to keep a reference to the marshalled object
    /// to ensure it does not get garbage collected prematurely.  A marshalled
    /// object can also be explicitly disconnected via <see cref="RemotingServices.Disconnect" />.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This class overrides <see cref="InitializeLifetimeService" />
    /// to return <c>null</c> which disables the automatic lease-management
    /// performed by the default lifetime service.
    /// </remarks>
    public abstract class LongLivedMarshalByRefObject : MarshalByRefObject
    {
        /// <inheritdoc />
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}