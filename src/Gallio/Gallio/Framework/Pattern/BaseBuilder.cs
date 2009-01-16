using System;
using System.Collections.Generic;
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Abstract base class for typical builder implementations.
    /// </summary>
    public abstract class BaseBuilder : ISupportDeferredActions
    {
        private List<Triple<ICodeElementInfo, int, Action>> deferredActions;

        /// <inheritdoc />
        public void AddDeferredAction(ICodeElementInfo codeElement, int order, Action deferredAction)
        {
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");
            if (deferredAction == null)
                throw new ArgumentNullException("deferredAction");

            if (deferredActions == null)
                deferredActions = new List<Triple<ICodeElementInfo, int, Action>>();

            deferredActions.Add(new Triple<ICodeElementInfo, int, Action>(codeElement, order, deferredAction));
        }

        /// <inheritdoc />
        public void ApplyDeferredActions()
        {
            if (deferredActions != null)
            {
                deferredActions.Sort((a, b) => a.Second.CompareTo(b.Second));

                foreach (var triple in deferredActions)
                    RunDeferredAction(triple.First, triple.Third);
            }
        }

        /// <summary>
        /// Gets the test model builder.
        /// </summary>
        /// <returns>The test model builder</returns>
        protected abstract ITestModelBuilder GetTestModelBuilder();

        private void RunDeferredAction(ICodeElementInfo codeElement, Action deferredAction)
        {
            try
            {
                deferredAction();
            }
            catch (Exception ex)
            {
                GetTestModelBuilder().PublishExceptionAsAnnotation(codeElement, ex);
            }
        }
    }
}
