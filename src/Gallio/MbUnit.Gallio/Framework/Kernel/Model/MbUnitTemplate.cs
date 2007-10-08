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
using MbUnit.Model.Data;
using MbUnit.Model.Actions;
using MbUnit.Model;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// <para>
    /// Abstract base class for MbUnit templates.
    /// </para>
    /// <para>
    /// Subclasses of this type define all of the kinds of templates that
    /// MbUnit uses as part of its reflective infrastructure.
    /// </para>
    /// </summary>
    public abstract class MbUnitTemplate : BaseTemplate
    {
        private ActionChain<MbUnitTemplateBinding> processBindingChain;
        private ActionChain<MbUnitTest> processTestChain;

        /// <summary>
        /// Initializes a template initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="codeReference"/> is null</exception>
        public MbUnitTemplate(string name, CodeReference codeReference)
            : base(name, codeReference)
        {
        }

        /// <summary>
        /// This chain of actions is invoked when a template binding is derived from
        /// this template.  MbUnit framework attributes add behavior to this chain
        /// to apply contributions to the binding in the correct sequence.
        /// </summary>
        /// <remarks>
        /// The actions are invoked immediately after the template binding is
        /// constructed.
        /// </remarks>
        public ActionChain<MbUnitTemplateBinding> ProcessBindingChain
        {
            get
            {
                if (processBindingChain == null)
                    processBindingChain = new ActionChain<MbUnitTemplateBinding>();
                return processBindingChain;
            }
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
        public override ITemplateBinding Bind(TemplateBindingScope scope, IDictionary<ITemplateParameter, IDataFactory> arguments)
        {
            MbUnitTemplateBinding binding = new MbUnitTemplateBinding(this, scope, arguments);

            if (processTestChain != null)
                binding.ProcessTestChain.Action = processTestChain.Action;

            if (processBindingChain != null)
                processBindingChain.Action(binding);

            return binding;
        }

        /// <summary>
        /// Creates an MbUnit test instance.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <param name="templateBinding">The template binding that produced this test</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>,
        /// <paramref name="codeReference"/> or <paramref name="templateBinding"/> is null</exception>
        /// <returns>The new test</returns>
        public virtual MbUnitTest CreateMbUnitTest(string name, CodeReference codeReference, MbUnitTemplateBinding templateBinding)
        {
            return new MbUnitTest(name, codeReference, templateBinding);
        }
    }
}