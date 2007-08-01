MbUnit Gallio
Copyright 2005-2007 MbUnit Project - http://www.mbunit.com/
===========================================================

1. Build Instructions.

   To build all projects and run the unit tests, run the "Build.bat"
   batch file.  The resulting files are placed in a freshly cleaned
   folder called "build" in the project root directory.

   To do all of the the above and generate documentation, run the
   "Release.bat" batch file.

   Of course, you can run the "Build.msbuild" MSBuild project directly
   if you prefer.

   To also specify a particular version number for the build we need
   to define the $(Version) property of the MSBuild scripts.  This can
   be accomplished by adding the argument "/p:Version=x.y.z.w" to the
   command line like this:

      Release.bat /p:3.0.0.0

   If no version is specified, then a default of "0.0.0.0" will be used.

