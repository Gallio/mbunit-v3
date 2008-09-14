using Gallio.Reflection;

namespace Gallio.Icarus.Controllers.EventArgs
{
    public class ShowSourceCodeEventArgs : System.EventArgs
    {
        private readonly CodeLocation codeLocation;

        public CodeLocation CodeLocation
        {
            get { return codeLocation; }
        }

        public ShowSourceCodeEventArgs(CodeLocation codeLocation)
        {
            this.codeLocation = codeLocation;
        }
    }
}
