NVelocity does not work very well once it has been merged to Gallio.Reports. This is due to the built-in DI system which is initialized with types qualified with the assembly name "NVelocity". And unfortunately, that assembly does not exist any more after the merge.

In order to make it run, we just need to remove the assembly qualification in the following configuration files:
- nvelocity.properties
- directive.properties

And to rebuild NVelocity.

Yann.