The Common Compiler Infrastructure Metadata (http://ccimetadata.codeplex.com/) is intended to replace the use of System.Reflection in Gallio to parse the .PDB files and get the code location of any given method.

Unfortunately, the library does not expose publicly the method metadata tokens (internal property) that identify unambiguously each method.

The following workaround have been applied:
Recompiling the CCI Metadata project by making the internals visible to Gallio.
