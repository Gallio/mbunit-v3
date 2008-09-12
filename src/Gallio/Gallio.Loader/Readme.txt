Gallio.Loader
=============

The purpose of this assembly is to provide support for satellite applications to load
Gallio.dll despite not having a local copy of their own.

Please refer to the API documentation of the GallioLoader class for more details
on its usage.


DEVELOPER REMARK:
-----------------

Beware any references to Gallio assemblies until the loader is initialized.
The CLR JIT performs some work ahead of time that requires access to all
referenced assemblies.  So if we're not careful, the JIT will attempt to load
an assembly that cannot be resolved.

The loader is intended to be resolved either from the GAC or copy-local alongside
the application.
