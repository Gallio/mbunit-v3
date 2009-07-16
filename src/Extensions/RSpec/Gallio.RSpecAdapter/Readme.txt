Gallio RSpec Adapter
====================

This adapter enables Gallio to run RSpec tests.

Using it is straitforward, just tell Gallio to run your *_spec.rb files and
it will use this adapter to do so.


Passing Options to RSpec
------------------------

If you want to be a little fancier, you can also pass in options for RSpec
by setting the "RSpecOptions" test runner property.

The available options are the same as those normally available when running
RSpec from the command-line.

eg. Run "my_spec.rb" with an example timeout of 60 seconds.
    Also heckles passing examples in MyModule using code mutation to verify
    that at least one spec fails each time the code is changed.

Gallio.Echo.exe my_spec.rb /rp:RSpecOptions='--timeout 60 --heckle MyModule'
