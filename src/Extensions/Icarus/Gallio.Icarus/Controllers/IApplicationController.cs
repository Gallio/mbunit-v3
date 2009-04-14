using System.ComponentModel;
using Gallio.Icarus.Mediator.Interfaces;

namespace Gallio.Icarus.Controllers
{
    public interface IApplicationController
    {
        IMediator Mediator { get; }
        string ProjectFileName { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
        
        void Load();
        void NewProject();
        void OpenProject(string projectName);
        void SaveProject();
    }
}