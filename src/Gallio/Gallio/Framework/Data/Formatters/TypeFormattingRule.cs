using System;

namespace Gallio.Framework.Data.Formatters
{
    /// <summary>
    /// <para>
    /// A formatting rule for <see cref="Type" />.
    /// </para>
    /// <para>
    /// Formats values like: System.String, MyType+Nested, System.Int32[]
    /// </para>
    /// </summary>
    public sealed class TypeFormattingRule : IFormattingRule
    {
        /// <inheritdoc />
        public int? GetPriority(Type type)
        {
            if (typeof(Type).IsAssignableFrom(type))
                return FormattingRulePriority.Best;
            return null;
        }

        /// <inheritdoc />
        public string Format(object obj, IFormatter formatter)
        {
            Type value = (Type)obj;
            return value.ToString();
        }
    }
}