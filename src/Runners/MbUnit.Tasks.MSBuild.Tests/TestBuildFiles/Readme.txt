These .XML files are build files. They are fed to MSBuild to perform integration tests.
We should probably add them as embedded resources and use Phil Haack's ExtractResource
attribute to manipulate them.