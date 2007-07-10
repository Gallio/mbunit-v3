namespace MbUnit.Core.Exceptions
{
    using System;
    using System.IO;

    public class MethodNotFoundException : System.Exception
    {
        private Type type;
        private string name;
        private Type[] parameters;

        public MethodNotFoundException(Type t, string name, Type[] parameters)
        {
            this.type = t;
            this.name = name;
            this.parameters = parameters;
        }

  
        public MethodNotFoundException(
            Type t,
            string name,
            Type[] parameters,
            string message,
            Exception innerException
            )
            : base(message, innerException)
        {
            this.type = t;
            this.name = name;
            this.parameters = parameters;
        }

        public override string Message
        {
            get
            {
                StringWriter sw = new StringWriter();
                sw.WriteLine("Could not find a Method matching the desired signature");
                sw.WriteLine("Type: {0}", this.type.FullName);
                sw.WriteLine("Name: {0}", this.name);
                sw.WriteLine("Parameter types:");
                for (int i = 0; i < this.parameters.Length; ++i)
                    sw.WriteLine("\t{0}", this.parameters[i].FullName);

                return sw.ToString();
            }
        }
    }
}