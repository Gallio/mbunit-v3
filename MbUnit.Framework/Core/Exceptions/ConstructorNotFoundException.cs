namespace MbUnit.Core.Exceptions
{
    using System;
    using System.IO;

    public class ConstructorNotFoundException : System.Exception
    {
        private Type type;
        private Type[] parameters;

        public ConstructorNotFoundException(Type t, Type[] parameters)
        {
            this.type = t;
            this.parameters = parameters;
        }

        public ConstructorNotFoundException(
            Type t,
            Type[] parameters,
            string message
            )
            : base(message)
        {
            this.type = t;
            this.parameters = parameters;
        }

        public ConstructorNotFoundException(
            Type t,
            Type[] parameters,
            string message,
            Exception innerException
            )
            : base(message, innerException)
        {
            this.type = t;
            this.parameters = parameters;
        }

        public override string Message
        {
            get
            {
                StringWriter sw = new StringWriter();
                sw.WriteLine("Could not find a Constructor matching the desired signature");
                sw.WriteLine("Type: {0}", this.type.FullName);
                sw.WriteLine("Parameter types:");
                for (int i = 0; i < this.parameters.Length; ++i)
                    sw.WriteLine("\t{0}", this.parameters[i].FullName);

                return sw.ToString();
            }
        }
    }
}