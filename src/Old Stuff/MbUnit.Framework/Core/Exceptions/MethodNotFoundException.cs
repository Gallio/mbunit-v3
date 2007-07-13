// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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