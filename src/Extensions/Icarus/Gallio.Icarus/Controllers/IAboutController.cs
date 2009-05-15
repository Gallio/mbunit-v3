using System.Collections.Generic;
using Gallio.Model;

namespace Gallio.Icarus.Controllers
{
    internal interface IAboutController
    {
        IList<TestFrameworkTraits> TestFrameworks { get; }
        string Version { get; }
    }
}