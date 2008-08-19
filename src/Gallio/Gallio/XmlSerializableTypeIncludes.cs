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

using System.Web.Services;
using System.Web.Services.Protocols;
using Gallio.Runner.Projects;
using Gallio.Runner.Reports;

namespace Gallio
{
    /// <summary>
    /// <para>
    /// This class is not intended to be instantiated or used in actual code.
    /// </para>
    /// <para>
    /// This class enumerates the Xml serializable types that are present in this assembly
    /// so that SGEN can generate exactly the required set of serializers based on reachability
    /// from this type.  This works by passing the name of this type to SGEN using the /proxytypes parameter
    /// which ordinarily does not allow multiple types to be specified.
    /// </para>
    /// <para>
    /// It turns out that due to a problem with SGEN we cannot simply generate Xml serializers for all
    /// types in the assembly.  It emits an error: "Unable to generate a temporary class (result=1)".
    /// Moreover, producing serializers for all types is wasteful so this hack suffices for now.
    /// </para>
    /// </summary>
    [WebServiceBinding]
    internal class XmlSerializableTypeIncludes : SoapHttpClientProtocol
    {
        [SoapDocumentMethod]
        public void Dummy(Report report, Project project)
        {
        }
    }
}
