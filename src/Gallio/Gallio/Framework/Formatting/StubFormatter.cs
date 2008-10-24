using System;
using System.Collections.Generic;
using Gallio.Collections;

namespace Gallio.Framework.Formatting
{
    /// <summary>
    /// <para>
    /// A formatter that is used when the runtime is not initialized.
    /// </para>
    /// </summary>
    public class StubFormatter : RuleBasedFormatter
    {
        /// <summary>
        /// Creates a stub formatter using only a few built-in formatting rules.
        /// </summary>
        public StubFormatter()
            : base(GetBuiltInRules())
        {
        }

        private static IFormattingRule[] GetBuiltInRules()
        {
            List<IFormattingRule> rules = new List<IFormattingRule>();

            foreach (Type type in typeof(StubFormatter).Assembly.GetExportedTypes())
            {
                if (typeof(IFormattingRule).IsAssignableFrom(type)
                    && type.GetConstructor(EmptyArray<Type>.Instance) != null)
                {
                    rules.Add((IFormattingRule)Activator.CreateInstance(type));
                }
            }

            return rules.ToArray();
        }
    }
}
