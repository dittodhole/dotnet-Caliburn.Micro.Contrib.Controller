var artifactsDirectory = MakeAbsolute(Directory("../artifacts"));

Task("Build")
  .IsDependentOn("Clean")
  .Does(() =>
{
  var filePath = MakeAbsolute(File(Argument("file", "../src/Caliburn.Micro.Contrib.Controller.sln")));
  var configurations = Argument("configurations", "Release");

  foreach (var configuration in configurations.Split(';'))
  {
    Information($"Building {filePath} with {configuration}");

    MSBuild(filePath,
            settings => settings.SetConfiguration(configuration)
                                .SetVerbosity(Verbosity.Minimal)
                                .WithRestore()
                                .WithProperty("PackageOutputPath", artifactsDirectory.FullPath));
  }
});

Task("Clean")
  .Does(() =>
{
  Information($"Cleaning {artifactsDirectory}");

  if (DirectoryExists(artifactsDirectory))
  {
    DeleteDirectory(artifactsDirectory,
                    new DeleteDirectorySettings
                    {
                      Recursive = true
                    });
  }

  CreateDirectory(artifactsDirectory);
});

var targetArgument = Argument("target", "Build");
RunTarget(targetArgument);
