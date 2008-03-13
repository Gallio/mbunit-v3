using System;
using Gallio.Model;
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// Declares that a constructor is used to provide paramters to a <see cref="PatternTest" />.
    /// Subclasses of this attribute can control what happens with the method.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given constructor.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple=false, Inherited=true)]
    public abstract class TestConstructorPatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Gets a default instance of the constructor pattern attribute to use
        /// when no other pattern consumes a contructor.
        /// </summary>
        public static readonly TestConstructorPatternAttribute DefaultInstance = new DefaultImpl();

        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override void Consume(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement, bool skipChildren)
        {
            IConstructorInfo constructor = (IConstructorInfo)codeElement;
            Validate(constructor);

            InitializeContainingTest(containingTestBuilder, constructor);
        }

        /// <summary>
        /// Validates whether the attribute has been applied to a valid <see cref="IConstructorInfo" />.
        /// Called by <see cref="Consume" />.
        /// </summary>
        /// <remarks>
        /// The default implementation throws an exception if <paramref name="constructor"/> is static.
        /// </remarks>
        /// <param name="constructor">The constructor</param>
        /// <exception cref="ModelException">Thrown if the attribute is applied to an inappropriate constructor</exception>
        protected virtual void Validate(IConstructorInfo constructor)
        {
            if (constructor.IsStatic)
                throw new ModelException(String.Format("The {0} attribute is not valid for use on constructor '{1}'.  The constructor must not be static.", GetType().Name, constructor));
        }

        /// <summary>
        /// <para>
        /// Initializes the containing <see cref="PatternTest" />.
        /// </para>
        /// </summary>
        /// <param name="containingTestBuilder">The containing test builder</param>
        /// <param name="constructor">The constructor</param>
        protected virtual void InitializeContainingTest(IPatternTestBuilder containingTestBuilder, IConstructorInfo constructor)
        {
            foreach (IParameterInfo parameter in constructor.Parameters)
                ProcessConstructorParameter(containingTestBuilder, parameter);
        }

        /// <summary>
        /// Gets the default pattern to apply to constructor parameters that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <see cref="TestParameterPatternAttribute.DefaultInstance" />.
        /// </remarks>
        protected virtual IPattern DefaultConstructorParameterPattern
        {
            get { return TestParameterPatternAttribute.DefaultInstance; }
        }

        /// <summary>
        /// Gets the primary pattern of a constructor parameter, or null if none.
        /// </summary>
        /// <param name="patternResolver">The pattern resolver</param>
        /// <param name="constructorParameter">The constructor parameter</param>
        /// <returns>The primary pattern, or null if none</returns>
        protected IPattern GetPrimaryConstructorMethodParameterPattern(IPatternResolver patternResolver, IParameterInfo constructorParameter)
        {
            return PatternUtils.GetPrimaryPattern(patternResolver, constructorParameter) ?? DefaultConstructorParameterPattern;
        }

        /// <summary>
        /// Processes a constructor parameter.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="constructorParameter">The constructor parameter</param>
        protected virtual void ProcessConstructorParameter(IPatternTestBuilder typeTestBuilder, IParameterInfo constructorParameter)
        {
            IPattern pattern = GetPrimaryConstructorMethodParameterPattern(typeTestBuilder.TestModelBuilder.PatternResolver, constructorParameter);
            if (pattern != null)
                pattern.Consume(typeTestBuilder, constructorParameter, false);
        }

        private sealed class DefaultImpl : TestConstructorPatternAttribute
        {
        }
    }
}