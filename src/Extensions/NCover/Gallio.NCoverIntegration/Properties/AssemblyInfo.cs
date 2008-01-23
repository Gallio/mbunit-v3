using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Gallio.NCoverIntegration")]
[assembly: AssemblyDescription("NCover integration for Gallio.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Gallio")]
[assembly: AssemblyCopyright("Copyright © 2008 MbUnit Project - http://www.mbunit.com/")]
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

[assembly: InternalsVisibleTo("Gallio.NCoverIntegration.Tests")]