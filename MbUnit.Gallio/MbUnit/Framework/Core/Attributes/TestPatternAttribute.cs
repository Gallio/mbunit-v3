using System;
using System.Reflection;
using MbUnit.Core.Model;
using MbUnit.Core.Utilities;
using MbUnit.Framework.Core.Model;

namespace MbUnit.Framework.Core.Attributes
{
    /// <summary>
    /// <para>
    /// Declares that a method in a fixture class represents an MbUnit test.
    /// Subclasses of this attribute can customize how test template enumeration takes
    /// place within a fixture.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given class.
    /// </para>

    /// Declares that a method in a fixture is an MbUnit test method.
    /// At most one attribute of this type may appear on any given test method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public abstract class TestPatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Gets a default instance of the test pattern attribute to use
        /// when none was specified.
        /// </summary>
        public static readonly TestPatternAttribute DefaultInstance = new DefaultImpl();

        /// <summary>
        /// Creates a test method template.
        /// This method is called when a test method is discovered via reflection to
        /// create a new model object to represent it.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="fixtureTemplate">The containing fixture template</param>
        /// <param name="methodInfo">The test method</param>
        /// <returns>The test method template</returns>
        public virtual MbUnitTestMethodTemplate CreateTemplate(TestTemplateTreeBuilder builder,
            MbUnitTestFixtureTemplate fixtureTemplate, MethodInfo methodInfo)
        {
            return new MbUnitTestMethodTemplate(fixtureTemplate, methodInfo);
        }

        /// <summary>
        /// Applies contributions to a method template.
        /// This method is called after the test method template is linked to the template tree.
        /// </summary>
        /// <remarks>
        /// A typical use of this method is to apply additional metadata to model
        /// objects in the test template tree and to further expand the tree using
        /// declarative metadata derived via reflection.
        /// </remarks>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="methodTemplate">The method template</param>
        public virtual void Apply(TestTemplateTreeBuilder builder, MbUnitTestMethodTemplate methodTemplate)
        {
            // FIXME: Should we apply decorators in two separate sorted batches or merge them
            //        together to produce a single ordered collection?
            TestDecoratorPatternAttribute.ProcessDecorators(builder, methodTemplate, methodTemplate.Method);
            TestDecoratorPatternAttribute.ProcessDecorators(builder, methodTemplate, methodTemplate.Method.ReflectedType);
            MetadataPatternAttribute.ProcessMetadata(builder, methodTemplate, methodTemplate.Method);

            ProcessParameters(builder, methodTemplate);
        }

        /// <summary>
        /// Processes a method using reflection to populate tests and other executable components.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="fixtureTemplate">The fixture template</param>
        /// <param name="method">The method to process</param>
        public static void ProcessMethod(TestTemplateTreeBuilder builder, MbUnitTestFixtureTemplate fixtureTemplate, MethodInfo method)
        {
            TestPatternAttribute testPatternAttribute = ReflectionUtils.GetAttribute<TestPatternAttribute>(method);
            if (testPatternAttribute == null)
                return;

            MbUnitTestMethodTemplate methodTemplate = testPatternAttribute.CreateTemplate(builder, fixtureTemplate, method);
            fixtureTemplate.AddMethod(methodTemplate);
            testPatternAttribute.Apply(builder, methodTemplate);
        }

        /// <summary>
        /// Processes all parameters using reflection to populate method parameters.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="methodTemplate">The method template</param>
        protected virtual void ProcessParameters(TestTemplateTreeBuilder builder, MbUnitTestMethodTemplate methodTemplate)
        {
            MbUnitTestParameterSet parameterSet = methodTemplate.CreateAnonymousParameterSet();
            foreach (ParameterInfo parameter in methodTemplate.Method.GetParameters())
            {
                ProcessParameter(builder, methodTemplate, parameterSet, parameter);
            }
        }

        /// <summary>
        /// Processes a parameter using reflection to populate method parameters.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="methodTemplate">The method template</param>
        /// <param name="parameterSet">The default parameter set to use unless overridden</param>
        /// <param name="parameter">The parameter</param>
        protected virtual void ProcessParameter(TestTemplateTreeBuilder builder, MbUnitTestMethodTemplate methodTemplate,
            MbUnitTestParameterSet parameterSet, ParameterInfo parameter)
        {
            ParameterPatternAttribute.ProcessSlot(builder, parameterSet, new Slot(parameter));
        }

        private class DefaultImpl : TestPatternAttribute
        {
        }
    }
}
