using System;
using System.Reflection;
using Gallio.Model;
using Gallio.Model.Reflection;
using MbUnit.Model.Builder;
using MbUnit.Model.Patterns;

namespace MbUnit.Framework
{
    /// <summary>
    /// Creates a dependency from this test assembly, test fixture or test method on some other test
    /// fixture or test method.  If the other test fixture or test method fails then this test
    /// will not run.  Moreover, the dependency forces this test to run after those it depends upon.
    /// </summary>
    /// <remarks>
    /// This attribute can be repeated multiple times if there are multiple dependencies.
    /// </remarks>
    public class DependsOnAttribute : DependencyPatternAttribute
    {
        private readonly Type testFixtureType;
        private readonly string testMethodName;

        /// <summary>
        /// Creates a dependency from this test on another test fixture.
        /// </summary>
        /// <param name="testFixtureType">The dependent test fixture type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testFixtureType"/> is null</exception>
        public DependsOnAttribute(Type testFixtureType)
        {
            if (testFixtureType == null)
                throw new ArgumentNullException("testFixtureType");

            this.testFixtureType = testFixtureType;
        }

        /// <summary>
        /// Creates a dependency from this test on a particular test method within another test fixture.
        /// </summary>
        /// <param name="testFixtureType">The dependent test fixture type</param>
        /// <param name="testMethodName">The dependent test method name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testFixtureType"/> or <paramref name="testMethodName"/> is null</exception>
        public DependsOnAttribute(Type testFixtureType, string testMethodName)
        {
            if (testFixtureType == null)
                throw new ArgumentNullException("testFixtureType");
            if (testMethodName == null)
                throw new ArgumentNullException("testMethodName");

            this.testFixtureType = testFixtureType;
            this.testMethodName = testMethodName;
        }

        /// <summary>
        /// Creates a dependency from this test on another test method within this test fixture.
        /// </summary>
        /// <param name="testMethodName">The dependent test method name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testMethodName"/> is null</exception>
        public DependsOnAttribute(string testMethodName)
        {
            if (testMethodName == null)
                throw new ArgumentNullException("testMethodName");

            this.testMethodName = testMethodName;
        }

        /// <summary>
        /// Gets the dependent test fixture type, or null if the dependency is on another
        /// test method of this test fixture.
        /// </summary>
        public Type TestFixtureType
        {
            get { return testFixtureType; }
        }

        /// <summary>
        /// Gets the dependent test method name, or null if the dependency
        /// is on the whole test fixture.
        /// </summary>
        public string TestMethodName
        {
            get { return testMethodName; }
        }

        /// <inheritdoc />
        protected override ICodeElementInfo GetDependency(ITestBuilder testBuilder, ICodeElementInfo codeElement)
        {
            ITypeInfo resolvedFixtureType;
            if (testFixtureType != null)
            {
                resolvedFixtureType = Reflector.Wrap(testFixtureType);
            }
            else
            {
                resolvedFixtureType = ReflectionUtils.GetType(codeElement);
                if (resolvedFixtureType == null)
                    throw new TestDeclarationErrorException(
                        String.Format(
                            "Could not resolve dependency on test method '{0}' because the declaring fixture type is not known.",
                            testMethodName));
            }

            if (testMethodName == null)
                return resolvedFixtureType;

            IMethodInfo resolvedMethod = resolvedFixtureType.GetMethod(testMethodName, BindingFlags.Public | BindingFlags.Instance);
            if (resolvedMethod == null)
                throw new TestDeclarationErrorException(
                    String.Format("Could not resolve dependency on test method '{0}' of test fixture type '{1}' because the method was not found.",
                    testMethodName, resolvedFixtureType.CompoundName));
            return resolvedMethod;
        }
    }
}