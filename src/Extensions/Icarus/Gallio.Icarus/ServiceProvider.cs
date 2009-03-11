using System;
using Gallio.Icarus.Mediator.Interfaces;

namespace Gallio.Icarus
{
    internal class ServiceProvider : IServiceProvider
    {
        private readonly IMediator mediator;
        private readonly IWindowManager windowManager;

        public ServiceProvider(IMediator mediator, IWindowManager windowManager)
        {
            this.mediator = mediator;
            this.windowManager = windowManager;
        }

        public object GetService(Type serviceType)
        {
            switch (serviceType.FullName)
            {
                case "Gallio.Icarus.Mediator.Interfaces.IMediator":
                    return mediator;
                case "Gallio.Icarus.IWindowManager":
                    return windowManager;
                default:
                    return null;
            }
        }
    }
}
