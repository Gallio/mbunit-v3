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
using System.Reflection;

namespace Gallio.MSTestAdapter.Model
{
    /// <summary>
    /// Provides common MSTes metadata keys.
    /// </summary>
    internal static class MSTestMetadataKeys
    {
        /// <summary>
        /// The metadata key for the test's ASP.NET development server.
        /// </summary>
        public const string AspNetDevelopmentServer = "AspNetDevelopmentServer";

        /// <summary>
        /// The metadata key for the test's ASP.NET development server host.
        /// </summary>
        public const string AspNetDevelopmentServerHost = "AspNetDevelopmentServerHost";
        
        /// <summary>
        /// The metadata key for the test's credentials.
        /// </summary>
        public const string Credential = "Credential";
        
        /// <summary>
        /// The metadata key for the test's project iteration.
        /// </summary>
        public const string CssIteration = "CssIteration";

        /// <summary>
        /// The metadata key for the test's project feature identifier.
        /// </summary>
        public const string CssProjectStructure = "CssProjectStructure";

        /// <summary>
        /// The metadata key for the test's owner.
        /// </summary>
        public const string DataSource = "DataSource";

        /// <summary>
        /// The metadata key for the test's deployment item.
        /// </summary>
        public const string DeploymentItem = "DeploymentItem";        

        /// <summary>
        /// The metadata key for the type of host that the unit test will run in.
        /// </summary>
        public const string HostType = "HostType";

        /// <summary>
        /// The metadata key for the test's owner.
        /// </summary>
        public const string Owner = "Owner";

        /// <summary>
        /// The metadata key for the test's priority.
        /// </summary>
        public const string Priority = "Priority";

        /// <summary>
        /// The metadata key for the test's property.
        /// </summary>
        public const string TestProperty = "TestProperty";

        /// <summary>
        /// The metadata key for the test's timeout.
        /// </summary>
        public const string Timeout = "Timeout";
        
        /// <summary>
        /// The metadata key for the URL that will give context to the test.
        /// </summary>
        public const string UrlToTest = "UrlToTest";
        
        /// <summary>
        /// The metadata key for the test's work item.
        /// </summary>
        public const string WorkItem = "WorkItem";
    }
}