using System;

namespace Gallio.Icarus.ExtensionSample
{
    public interface IController
    {
        event EventHandler<UpdateEventArgs> Update;
    }
}