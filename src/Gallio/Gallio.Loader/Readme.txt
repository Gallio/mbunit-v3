Gallio.Loader
=============

The purpose of this assembly is to provide support for satellite applications to load
Gallio.dll despite not having a local copy of their own.

Ordinarily, if an application is linked to Gallio, it is simpler for it to include
a copy of the required Gallio assemblies for bootstrapping purposes (Gallio plugins
may still be loaded from other locations dynamically).  However, in the case of
extensions to 3rd party applications, it may not be practical to copy the required
Gallio assemblies into the designated plugin folder or to install them in the GAC.

This library provides a workaround.

Please refer to the API documentation of the classes in this assembly for more details.


REMARK:
-------

This assembly should not link to other Gallio assemblies nor should it be linked
from other Gallio assemblies.  It is intended to be used standalone to immunize
it from version conflicts.

This code should someday be replaced by a version independent contract layer for
core services.
