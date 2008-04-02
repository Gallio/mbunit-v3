using System;
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Overrides the name of a test or test parameter.
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.TestComponent, AllowMultiple = false, Inherited = true)]
    public class NameAttribute : PatternAttribute
    {
        private readonly string name;

        /// <summary>
        /// Overrides the name of a test or test parameter.
        /// </summary>
        /// <param name="name">The overridden name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public NameAttribute(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.name = name;
        }

        /// <summary>
        /// Gets the overridden name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <inheritdoc />
        public override void Process(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            Validate(scope, codeElement);

            scope.TestComponent.SetName(name);
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="scope">The scope</param>
        /// <param name="codeElement">The code element</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly</exception>
        protected virtual void Validate(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            if (!scope.IsTestDeclaration && !scope.IsTestParameterDeclaration)
                ThrowUsageErrorException("This attribute can only be used on a test or test parameter.");
        }
    }
}