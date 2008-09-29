using System.ComponentModel;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface IAnnotationsController
    {
        BindingList<AnnotationData> Annotations { get; }
        bool ShowErrors { get; set; }
        bool ShowWarnings { get; set; }
        bool ShowInfo { get; set; }
        string ErrorsText { get; }
        string WarningsText { get; }
        string InfoText { get; }
    }
}
