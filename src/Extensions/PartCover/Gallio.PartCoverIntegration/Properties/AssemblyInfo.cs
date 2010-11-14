// Copyright 2010 Nicolas Graziano 
// largely inspired by NCoverIntegration in Gallio Project - http://www.gallio.org/
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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Gallio.PartCoverIntegration")]
[assembly: AssemblyDescription("PartCover integration for Gallio.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Gallio")]
[assembly: AssemblyCopyright("Copyright � 2010 Nicolas Graziano")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("607fb49a-773c-4281-a7ab-1a5696a0589c")]

// Don't care about CLS compliance for this assembly.
[assembly: CLSCompliant(false)]

// The neutral resources language is US English.
// Telling the system that this is the case yields a small performance improvement during startup.
[assembly: NeutralResourcesLanguage("en-US")]

// Can't strong-name the task because NAnt isn't strong-named.
[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames")]

[assembly: InternalsVisibleTo("Gallio.PartCoverIntegration.Tests")]

[assembly: InternalsVisibleTo("Gallio.PartCoverIntegration.vs2010.Tests")]
[assembly: InternalsVisibleTo("Gallio.PartCoverIntegration.Explorables")]