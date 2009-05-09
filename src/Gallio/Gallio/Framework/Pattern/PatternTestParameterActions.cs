// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Common;
using Gallio.Common.Diagnostics;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// An implementation of <see cref="IPatternTestParameterHandler" /> based on
    /// actions that can be sequenced and composed as chains.
    /// </para>
    /// </summary>
    /// <seealso cref="IPatternTestParameterHandler" /> for documentation about the behaviors themselves.
    public class PatternTestParameterActions : IPatternTestParameterHandler
    {
        private readonly ActionChain<PatternTestInstanceState, object> bindTestParameterChain;
        private readonly ActionChain<PatternTestInstanceState, object> unbindTestParameterChain;

        /// <summary>
        /// Creates a test parameter actions object initially configured with empty action chains
        /// that do nothing.
        /// </summary>
        public PatternTestParameterActions()
        {
            bindTestParameterChain = new ActionChain<PatternTestInstanceState, object>();
            unbindTestParameterChain = new ActionChain<PatternTestInstanceState, object>();
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestParameterHandler.BindTestParameter" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestParameterHandler.BindTestParameter"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState, object> BindTestParameterChain
        {
            get { return bindTestParameterChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestParameterHandler.UnbindTestParameter" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestParameterHandler.UnbindTestParameter"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState, object> UnbindTestParameterChain
        {
            get { return unbindTestParameterChain; }
        }

        /// <inheritdoc />
        [UserCodeEntryPoint]
        public void BindTestParameter(PatternTestInstanceState testInstanceState, object value)
        {
            bindTestParameterChain.Action(testInstanceState, value);
        }

        /// <inheritdoc />
        [UserCodeEntryPoint]
        public void UnbindTestParameter(PatternTestInstanceState testInstanceState, object value)
        {
            unbindTestParameterChain.Action(testInstanceState, value);
        }
    }
}
