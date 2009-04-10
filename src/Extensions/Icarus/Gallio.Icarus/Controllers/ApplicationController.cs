using System.Collections.Generic;
using System.IO;
using Gallio.Icarus.Mediator.Interfaces;

namespace Gallio.Icarus.Controllers
{
    public class ApplicationController : IApplicationController
    {
        private readonly IcarusArguments Arguments;
        public IMediator Mediator { get; private set; }

        public ApplicationController(IcarusArguments args, IMediator mediator)
        {
            Arguments = args;
            Mediator = mediator;
        }

        public void Load()
        {
            var assemblyFiles = new List<string>();
            if (Arguments != null && Arguments.Assemblies.Length > 0)
            {
                foreach (var assembly in Arguments.Assemblies)
                {
                    if (!File.Exists(assembly))
                        continue;

                    if (Path.GetExtension(assembly) == ".gallio")
                    {
                        Mediator.OpenProject(assembly);
                        break;
                    }
                    assemblyFiles.Add(assembly);
                }
                Mediator.AddAssemblies(assemblyFiles);
            }
            else if (Mediator.OptionsController.RestorePreviousSettings && Mediator.OptionsController.RecentProjects.Count > 0)
            {
                string projectName = Mediator.OptionsController.RecentProjects.Items[0];
                if (File.Exists(projectName))
                    Mediator.OpenProject(projectName);
                else
                    Mediator.OptionsController.RecentProjects.Items.Remove(projectName);
            }
        }
    }
}
