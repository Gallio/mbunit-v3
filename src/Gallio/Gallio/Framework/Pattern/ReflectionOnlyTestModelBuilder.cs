using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Runtime.Loader;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// An implementation of a test model builder that is used for reflection only and
    /// does not actually support building tests.
    /// </summary>
    public class ReflectionOnlyTestModelBuilder : ITestModelBuilder
    {
        private readonly IReflectionPolicy reflectionPolicy;

        /// <summary>
        /// Creates a test model builder.
        /// </summary>
        /// <param name="reflectionPolicy">The reflection policy</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reflectionPolicy"/> is null</exception>
        public ReflectionOnlyTestModelBuilder(IReflectionPolicy reflectionPolicy)
        {
            if (reflectionPolicy == null)
                throw new ArgumentNullException("reflectionPolicy");

            this.reflectionPolicy = reflectionPolicy;
        }

        /// <inheritdoc />
        public IReflectionPolicy ReflectionPolicy
        {
            get { return reflectionPolicy; }
        }

        void ITestModelBuilder.AddAnnotation(Annotation annotation)
        {
            throw new NotSupportedException();
        }

        void ITestModelBuilder.AddAssemblyResolver(IAssemblyResolver resolver)
        {
            throw new NotSupportedException();
        }

        void ITestModelBuilder.PublishExceptionAsAnnotation(ICodeElementInfo codeElement, Exception ex)
        {
            throw new NotSupportedException();
        }

        ITestBuilder ITestModelBuilder.CreateTopLevelTest(string name, ICodeElementInfo codeElement, ITestDataContextBuilder dataContextBuilder)
        {
            throw new NotSupportedException();
        }

        TestModel ITestModelBuilder.ToTestModel()
        {
            throw new NotSupportedException();
        }

        void ISupportDeferredActions.AddDeferredAction(ICodeElementInfo codeElement, int order, Action deferredAction)
        {
            throw new NotSupportedException();
        }

        void ISupportDeferredActions.ApplyDeferredActions()
        {
            throw new NotSupportedException();
        }
    }
}
