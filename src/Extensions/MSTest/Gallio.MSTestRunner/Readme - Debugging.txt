How to Debug
============

1. Configure the Gallio.MSTestRunner project debugging options as follows:
   * Start External Program: devenv.exe

2. Compile the solution in Debug mode.  Note that if you just ran a build from
   the command-line, you may have Release mode binaries in the project bin folders.
   When in Debug mode, the Runtime applies special rules for locating plugin
   binaries within the source tree.  When in Release mode, these rules are not
   applied and you may encounter problems during debugging because important
   plugins, such as test frameworks, might not be loaded.

3. Run the "Install-MSTestRunner.bat" batch file from the command-line to copy
   the MSTestRunner files to the Visual Studio private assemblies folder.

4. Launch the Gallio.MSTestRunner project in debug mode.

5. Load a suitable solution for testing such as the MbUnit.Samples.sln.

6. Continue as usual.
