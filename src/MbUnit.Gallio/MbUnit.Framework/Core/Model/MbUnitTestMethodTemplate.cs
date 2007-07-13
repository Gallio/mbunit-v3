using System;
using System.Collections.Generic;
using System.Reflection;
using MbUnit.Framework.Model.Metadata;
using MbUnit.Framework.Model;

namespace MbUnit.Framework.Core.Model
{
    /// <summary>
    /// Represents a template derived from an MbUnit test method.
    /// </summary>
    public class MbUnitTestMethodTemplate : MbUnitTestTemplate
    {
        private MbUnitTestFixtureTemplate fixtureTemplate;
        private MethodInfo method;

        /// <summary>
        /// Initializes an MbUnit test method template model object.
        /// </summary>
        /// <param name="fixtureTemplate">The containing fixture template</param>
        /// <param name="method">The test method</param>
        public MbUnitTestMethodTemplate(MbUnitTestFixtureTemplate fixtureTemplate, MethodInfo method)
            : base(method.Name, CodeReference.CreateFromMember(method))
        {
            this.fixtureTemplate = fixtureTemplate;
            this.method = method;

            Kind = TemplateKind.Test;
        }

        /// <summary>
        /// Gets the containing fixture template.
        /// </summary>
        public MbUnitTestFixtureTemplate FixtureTemplate
        {
            get { return fixtureTemplate; }
        }

        /// <summary>
        /// Gets the test method.
        /// </summary>
        public MethodInfo Method
        {
            get { return method; }
        }

        /// <summary>
        /// Adds a test method parameter.
        /// </summary>
        /// <param name="parameter">The parameter to add</param>
        public void AddParameter(MbUnitTestParameter parameter)
        {
        }
    }
}
