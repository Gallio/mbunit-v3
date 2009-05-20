How to Debug
============

1. Configure the Gallio.VisualStudio project debugging options as follows:
   * Start External Program: devenv.exe

2. Compile the solution in Debug mode.

   Make sure that the Gallio.VisualStudio.Tip.Proxy assembly is NOT in the
   bin folder of Gallio.VisualStudio.Tip.  If it ends up in there then there may
   be assembly conflicts between it and the version loaded from the GAC.
   
   The Gallio.VisualStudio.Tip project is currently configured such that the
   proxy assembly is not "copy local".  So this should all be fine out of the box.

3. Run the "Install.bat" batch file from the command-line to
   set registry keys and install assemblies in the GAC.

4. Launch the Gallio.VisualStudio project in debug mode.

5. Load a suitable solution for testing such as the MbUnit.Samples.sln.

6. Continue as usual.

7. To debug certain scenarios you may need to attach to VSTestHost.exe
   which is the test hosting process that Visual Studio creates.
