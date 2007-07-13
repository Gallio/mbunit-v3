using System;

namespace MbUnit.Framework.Core.Attributes
{
    /// <summary>
    /// The data pattern attribute applies a data source to a fixture or test
    /// parameter declaratively.  It can be attached to a fixture class, a public property
    /// or field of a fixture, a test method or a test method parameter.  When attached
    /// to a property or field of a fixture, implies that the property or field is
    /// a fixture parameter (so the <see cref="ParameterPatternAttribute" />
    /// may be omitted).
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class
        | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field
        | AttributeTargets.Parameter, AllowMultiple=true, Inherited=true)]
    public abstract class DataPatternAttribute : PatternAttribute
    {
        private string condition;

        /// <summary>
        /// Gets or sets the name of the condition token associated with a condition to
        /// evaluate to decide whether to use the data specified by this attribute.
        /// <seealso cref="ConditionPatternAttribute" />.
        /// </summary>
        public string Condition
        {
            get { return condition; }
            set { condition = value; }
        }
    }
}
