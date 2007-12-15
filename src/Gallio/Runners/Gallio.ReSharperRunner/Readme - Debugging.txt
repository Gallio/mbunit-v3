How to Debug
============

1. Configure the Gallio.ReSharperRunner project debugging options as follows:
   * Start External Program: devenv.exe
   * Command Line Arguments: /ReSharper.Internal /ReSharper.Plugin "Gallio.ReSharperRunner.dll"

2. Compile the solution in Debug mode.  Note that if you just ran a build from
   the command-line, you may have Release mode binaries in the project bin folders.
   When in Debug mode, the Runtime applies special rules for locating plugin
   binaries within the source tree.  When in Release mode, these rules are not
   applied and you may encounter problems during debugging because important
   plugins, such as test frameworks, might not be loaded.

3. Launch the Gallio.ReSharperRunner project in debug mode.

4. Load a suitable solution for testing such as the MbUnit.Samples.sln.

5. Normally ReSharper runs tests using an external test runner process.  Unfortunately
   that hinders debugging.  To run tests within the same process, select the tests to
   run in the Unit Test Session tool window then click the "Run Thread" button.
   This is a special button that is enabled by the "/ReSharper.Internal" option mentioned
   above.

6. Continue as usual.
