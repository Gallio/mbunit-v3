How to Debug
============

0. The Gallio Loader relies on a registry key to tell it where the Gallio install
	dir is. If this doesn't exist (HKLM\Software\Gallio.org\Gallio\0.0), 
	run the src\Install.bat script with a /x argument and choose option 1.
	Alternatively, set the GALLIO_RUNTIME_PATH environment variable to point to 
	your src dir (src\Gallio\Gallio).

1. Configure the Gallio.ReSharperRunner project debugging options as follows:
   * Start External Program: devenv.exe
   * Command Line Arguments: /ReSharper.Internal /ReSharper.Plugin "Gallio.ReSharperRunnerXX.dll"
       where XX is 31, 40, 41, 45, 50, etc...

2. Compile the solution in Debug mode.  Note that if you just ran a build from
   the command-line, you may have Release mode binaries in the project bin folders.
   When in Debug mode, the Runtime applies special rules for locating plugin
   binaries within the source tree.  When in Release mode, these rules are not
   applied and you may encounter problems during debugging because important
   plugins, such as test frameworks, might not be loaded.

3. Launch the Gallio.ReSharperRunner project in debug mode.

4. Load a suitable solution for testing such as the MbUnit.Samples.sln.

5. ReSharper runs tests using an external test runner process. 

6. Continue as usual.
