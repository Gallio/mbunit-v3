using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Gallio.Icarus.Mediator.Interfaces;

namespace Gallio.Icarus.Controllers
{
    public class ApplicationController : IApplicationController
    {
        private readonly IcarusArguments arguments;
        private string projectFileName = string.Empty;
        
        public IMediator Mediator { get; private set; }
        
        public string ProjectFileName
        {
            get
            {
                return string.IsNullOrEmpty(projectFileName) ? "Gallio Icarus" :
                    string.Format("{0} - Gallio Icarus", Path.GetFileNameWithoutExtension(projectFileName));
            }
            set
            {
                projectFileName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ProjectFileName"));
            }
        }

        public ApplicationController(IcarusArguments args, IMediator mediator)
        {
            arguments = args;
            Mediator = mediator;
        }

        public void Load()
        {
            var assemblyFiles = new List<string>();
            if (arguments != null && arguments.Assemblies.Length > 0)
            {
                foreach (var assembly in arguments.Assemblies)
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
                ProjectFileName = projectName;
                Mediator.OpenProject(projectName);
            }
        }

        public void OpenProject(string projectName)
        {
            ProjectFileName = projectName;
            Mediator.OpenProject(projectName);
        }

        public void SaveProject()
        {
            Mediator.SaveProject(projectFileName);
        }

        public void NewProject()
        {
            ProjectFileName = string.Empty;
            Mediator.NewProject();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }
}
