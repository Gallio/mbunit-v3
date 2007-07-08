using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MbUnit.Core.Model;

namespace MbUnit.Framework.Core.Model
{
    /// <summary>
    /// Represents a template derived from an MbUnit test fixture.
    /// </summary>
    public class MbUnitTestFixtureTemplate : MbUnitTestTemplate
    {
        private MbUnitTestAssemblyTemplate assemblyTemplate;

        /// <summary>
        /// Initializes an MbUnit test fixture template model object.
        /// </summary>
        /// <param name="assemblyTemplate">The containing assembly template</param>
        /// <param name="fixtureType">The test fixture type</param>
        public MbUnitTestFixtureTemplate(MbUnitTestAssemblyTemplate assemblyTemplate, Type fixtureType)
            : base(fixtureType.Name, CodeReference.CreateFromType(fixtureType))
        {
            this.assemblyTemplate = assemblyTemplate;
        }

        /// <summary>
        /// Gets the containing assembly template.
        /// </summary>
        public MbUnitTestAssemblyTemplate AssemblyTemplate
        {
            get { return assemblyTemplate; }
        }

        /// <summary>
        /// Gets the test fixture type.
        /// </summary>
        public Type FixtureType
        {
            get { return CodeReference.Type; }
        }

        /// <summary>
        /// Adds a test method template as a child of the fixture.
        /// </summary>
        /// <param name="methodTemplate">The test method model</param>
        public void AddMethod(MbUnitTestMethodTemplate methodTemplate)
        {
            Children.Add(methodTemplate);
        }

        public void AddConstructorParameters(ConstructorInfo constructor, IList<MbUnitTestParameter> parameters)
        {
            // FIXME: Currently we arbitrarily choose the first constructor and throw away the rest.
            //        This should be replaced by a more intelligent mechanism that can
            //        handle optional or alternative dependencies.  We might benefit from
            //        using an existing inversion of control framework like Castle
            //        to handle stuff like this.
        }

        public void AddFieldParameter(FieldInfo field, MbUnitTestParameter parameter)
        {
        }

        public void AddPropertyParameter(PropertyInfo property, MbUnitTestParameter parameter)
        {
        }
    }
}
