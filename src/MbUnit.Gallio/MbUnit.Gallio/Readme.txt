MbUnit.Gallio Project
=====================

IMPORTANT NOTE: This isn't a REAL C# project!

This project is used to run ILMerge on MbUnit.Core.dll, MbUnit.Framework.dll and
their dependencies to produce the MbUnit.Gallio.dll you know and love.  The
project file (an MSBuild script) has been hacked to give the appearance that
MbUnit.Gallio is a real C# project.  Don't be fooled!

The hacked project does the following:

1. Compiles everything in this project (just the AssemblyInfo) to
   MbUnit.Gallio.Attributes.dll.  This intermediate assembly is only used
   to supply the attributes for the final merged assembly.

2. Grab all of the references and combine them with ILMerge including the
   XML documentation files.  Places the results in the output path as
   MbUnit.Gallio.dll and MbUnit.Gallio.xml.

To ensure that all project references are set correctly, all projects that
depend on MbUnit.Core and MbUnit.Framework should be modified as described
in the comments at the beginning of the following file:
  --> src\MbUnit.Gallio.ProjectReferences.targets.

Also refer to the following file to explain how ILMerge is integrated:
  --> src\MbUnit.Gallio.ILMerge.targets.
