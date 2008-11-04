using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MbUnit.Framework.ContractVerifiers.Patterns.HasConstructor
{
    /// <summary>
    /// Required accessibility for the constructor searched
    /// by the <see cref="HasConstructorPattern"/>.
    /// </summary>
    internal class HasConstructorAccessibility
    {
        /// <summary>
        /// Gets the binding flags for reflection purpose.
        /// </summary>
        public BindingFlags BindingFlags
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the accessibility name.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        private HasConstructorAccessibility(BindingFlags bindingFlags, string name)
	    {
            this.BindingFlags = bindingFlags;
            this.Name = name;
	    }

        /// <summary>
        /// Public accessibility.
        /// </summary>
        public static HasConstructorAccessibility Public = 
            new HasConstructorAccessibility(BindingFlags.Public, "Public");
        
        /// <summary>
        /// Non-public accessibility (i.e. Protected or Internal)
        /// </summary>
        public static HasConstructorAccessibility NonPublic = 
            new HasConstructorAccessibility(BindingFlags.NonPublic, "Non-Public");
    }
}
