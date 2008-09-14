using System.Drawing;

namespace Gallio.Icarus.Controllers.EventArgs
{
    public class RuntimeLogEventArgs : System.EventArgs
    {
        private readonly string message;
        private readonly Color color;

        public RuntimeLogEventArgs(string message, Color color)
        {
            this.message = message;
            this.color = color;
        }

        public string Message
        {
            get { return message; }
        }

        public Color Color
        {
            get { return color; }
        }
    }
}
