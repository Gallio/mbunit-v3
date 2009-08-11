This program generates a little assembly called Gallio.ReflectionShim that
contains two types, AssemblyShim and ModuleShim.

These types are special subclasses of the Assembly and Module class that do
not call the constructor of their supertype (which is internal).  We use them
to generate custom subclasses that represent unresolved assemblies or modules
so as to make it easier to integrate unmodified test frameworks.

This is of course an ugly hack.