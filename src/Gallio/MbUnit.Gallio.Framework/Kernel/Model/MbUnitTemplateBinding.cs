// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Text;
using MbUnit.Framework.Kernel.DataBinding;
using MbUnit.Framework.Kernel.Actions;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// An MbUnit template binding.  MbUnit attributes contribute to the
    /// test construction process by attaching appropriate behavior to the
    /// template binding.
    /// </summary>
    public class MbUnitTemplateBinding : BaseTemplateBinding
    {
        private ActionChain<MbUnitTest> processTestChain;

        /// <summary>
        /// Creates a template binding.
        /// </summary>
        /// <param name="template">The template that was bound</param>
        /// <param name="scope">The scope in which the binding occurred</param>
        /// <param name="arguments">The template arguments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="template"/>,
        /// <paramref name="scope"/> or <paramref name="arguments"/> is null</exception>
        public MbUnitTemplateBinding(MbUnitTemplate template, TemplateBindingScope scope,
            IDictionary<ITemplateParameter, IDataFactory> arguments)
            : base(template, scope, arguments)
        {
        }

        /// <summary>
        /// Gets the MbUnit template.
        /// </summary>
        /// <seealso cref="ITemplateBinding.Template"/>
        new public MbUnitTemplate Template
        {
            get { return (MbUnitTemplate) base.Template; }
        }

        /// <summary>
        /// This chain of actions is invoked when a test is built from a template binding
        /// derived from this template.  MbUnit framework attributes add behavior to
        /// this chain to apply contributions to the test.
        /// </summary>
        /// <remarks>
        /// The actions are invoked before the children of the test are constructed.
        /// To perform actions after all tests have been constructed, it's necessary
        /// to attach to the <see cref="ModelTreeBuilder{T}.PostProcess" /> event.
        /// </remarks>
        public ActionChain<MbUnitTest> ProcessTestChain
        {
            get
            {
                if (processTestChain == null)
                    processTestChain = new ActionChain<MbUnitTest>();
                return processTestChain;
            }
        }

        /// <inheritdoc />
        public override void BuildTests(TestTreeBuilder builder, ITest parent)
        {
            MbUnitTest test = Template.CreateMbUnitTest(Template.Name, Template.CodeReference, this);
            test.Kind = null;
            test.Metadata.AddAll(Template.Metadata);
            parent.AddChild(test);

            if (processTestChain != null)
                processTestChain.Action(test);

            BuildTestsForGenerativeChildren(builder, test);
        }
    }
}
