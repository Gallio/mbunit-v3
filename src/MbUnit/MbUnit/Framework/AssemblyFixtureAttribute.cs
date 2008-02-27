using System;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// The assembly fixture attribute is applied to a class that contains setup and
    /// teardown methods that are to be applied at the assembly level.  Conceptually,
    /// the <see cref="AssemblyFixtureAttribute" /> adds new behavior to an assembly-level
    /// test fixture that contains all of the test fixtures within the assembly.
    /// </para>
    /// <para>
    /// The following attributes are typically used within an assembly fixture:
    /// <list type="bullet">
    /// <item><see cref="FixtureSetUpAttribute" />: Performs setup activities before any
    /// test fixtures within the assembly are executed.</item>
    /// <item><see cref="FixtureTearDownAttribute" />: Performs teardown activities after all
    /// test fixtures within the assembly are executed.</item>
    /// <item><see cref="SetUpAttribute" />: Performs setup activities before each
    /// test fixture within the assembly is executed.</item>
    /// <item><see cref="TearDownAttribute" />: Performs teardown activities after eacj
    /// test fixture within the assembly is executed.</item>
    /// </list>
    /// </para>
    /// <para>
    /// It is also possible to use other attributes as with an ordinary <see cref="TestFixtureAttribute" />.
    /// An assembly fixture also supports data binding.  When data binding is used on an assembly
    /// fixture, it will cause all test fixtures within the assembly to run once for each combination
    /// of data values used.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The class must have a public default constructor.  The class may not be static.
    /// </para>
    /// <para>
    /// There must only be at most one class with an <see cref="AssemblyFixtureAttribute" />
    /// within any given assembly.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AssemblyFixtureAttribute : TestTypePatternAttribute
    {
        /// <inheritdoc />
        public override bool Consume(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
            ITypeInfo type = (ITypeInfo)codeElement;
            if (!ShouldConsume(type))
                return false;

            PatternTest assemblyTest = containingTestBuilder.Test;
            if (assemblyTest.Kind != TestKinds.Assembly)
                throw new ModelException("The [AssemblyFixture] attribute can only be used on a non-nested class.");

            InitializeTest(containingTestBuilder, type);
            SetTestSemantics(assemblyTest, type);
            return true;
        }

        /// <inheritdoc />
        protected override void SetTestSemantics(PatternTest test, ITypeInfo type)
        {
            base.SetTestSemantics(test, type);

            test.Actions.InitializeTestInstanceChain.Before(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    if (testInstanceState.FixtureType != null)
                        throw new ModelException("There appear to be multiple [AssemblyFixture] attributes within the assembly, or some other attribute has already defined a fixture for the assembly.");
                });

            test.Actions.DecorateChildTestChain.Clear();
        }
    }
}
