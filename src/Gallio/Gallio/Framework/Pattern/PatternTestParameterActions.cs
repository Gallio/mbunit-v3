using System;
using Gallio.Model.Diagnostics;

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
        [TestEntryPoint]
        public void BindTestParameter(PatternTestInstanceState testInstanceState, object value)
        {
            bindTestParameterChain.Action(testInstanceState, value);
        }

        /// <inheritdoc />
        [TestEntryPoint]
        public void UnbindTestParameter(PatternTestInstanceState testInstanceState, object value)
        {
            unbindTestParameterChain.Action(testInstanceState, value);
        }
    }
}