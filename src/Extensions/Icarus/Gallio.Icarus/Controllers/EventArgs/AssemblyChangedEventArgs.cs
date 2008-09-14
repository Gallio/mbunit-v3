namespace Gallio.Icarus.Controllers.EventArgs
{
    public class AssemblyChangedEventArgs : System.EventArgs
    {
        private readonly string assemblyName;

        public AssemblyChangedEventArgs(string assemblyName)
        {
            this.assemblyName = assemblyName;
        }

        public string AssemblyName
        {
            get { return assemblyName; }
        }
    }
}
