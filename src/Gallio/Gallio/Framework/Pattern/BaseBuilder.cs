// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using Gallio.Common;
using Gallio.Common.Reflection;

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
        /// <returns>The test model builder.</returns>
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
