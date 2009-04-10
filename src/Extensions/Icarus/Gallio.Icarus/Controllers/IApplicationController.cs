using Gallio.Icarus.Mediator.Interfaces;

namespace Gallio.Icarus.Controllers
{
    public interface IApplicationController
    {
        IMediator Mediator { get; }
        void Load();
    }
}