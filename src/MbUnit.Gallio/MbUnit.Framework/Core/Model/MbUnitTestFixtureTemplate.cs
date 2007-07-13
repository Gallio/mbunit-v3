using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MbUnit.Core.Metadata;
using MbUnit.Core.Model;

namespace MbUnit.Framework.Core.Model
{
    /// <summary>
    /// Represents a template derived from an MbUnit test fixture.
    /// </summary>
    public class MbUnitTestFixtureTemplate : MbUnitTestTemplate
    {
        private MbUnitTestAssemblyTemplate assemblyTemplate;
        private List<MbUnitTestMethodTemplate> methodTemplates;

        /// <summary>
        /// Initializes an MbUnit test fixture template model object.
        /// </summary>
        /// <param name="assemblyTemplate">The containing assembly template</param>
        /// <param name="fixtureType">The test fixture type</param>
        public MbUnitTestFixtureTemplate(MbUnitTestAssemblyTemplate assemblyTemplate, Type fixtureType)
            : base(fixtureType.Name, CodeReference.CreateFromType(fixtureType))
        {
            this.assemblyTemplate = assemblyTemplate;

            methodTemplates = new List<MbUnitTestMethodTemplate>();
            Kind = TemplateKind.Fixture;
        }

        /// <inheritdoc />
        public override IEnumerable<ITestTemplate> Children
        {
            get
            {
                foreach (MbUnitTestMethodTemplate methodTemplate in methodTemplates)
                    yield return methodTemplate;
            }
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
        /// Gets the list of method templates.
        /// </summary>
        public IList<MbUnitTestMethodTemplate> MethodTemplates
        {
            get { return methodTemplates; }
        }

        /// <summary>
        /// Adds a test method template as a child of the fixture.
        /// </summary>
        /// <param name="methodTemplate">The test method model</param>
        public void AddMethodTemplate(MbUnitTestMethodTemplate methodTemplate)
        {
            ModelUtils.LinkTemplate(this, methodTemplates, methodTemplate);
        }
    }
}
