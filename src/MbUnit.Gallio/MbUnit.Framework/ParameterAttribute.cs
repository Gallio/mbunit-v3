using System;
using MbUnit.Framework.Model;
using MbUnit.Framework.Core.Attributes;
using MbUnit.Framework.Core.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares that a property, field or parameter is a test parameter and
    /// specifies its properties.  At most one attribute of this type may appear on
    /// any given test fixture property or field.  If the attribute is omitted from
    /// test method parameters and test fixture constructor parameters the parameter
    /// will be declared with default values (which are usually just fine).
    /// </summary>
    public class ParameterAttribute : ParameterPatternAttribute
    {
        private string name;
        private string set;
        private int? index;

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// If set to null, the parameter is named the same as the property,
        /// field or parameter to which the attribute has been applied.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the name of the parameter set to which the parameter belongs.
        /// If set to null, the parameter belongs to an anonymous parameter set.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        public string Set
        {
            get { return set; }
            set { set = value; }
        }

        /// <summary>
        /// Gets or sets the zero-based index of the parameter.  The index is used
        /// instead of the parameter name in unlabeled table-like data sources
        /// (such as row-tests and headerless CSV files) to select the column to
        /// which the parameter will be bound.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The index does not necessarily correspond to the sequence in which
        /// the parameter appears in its parameter set.
        /// </para>
        /// <para>
        /// Exotic data sources that do not bind by name or by index may use metadata
        /// associated with the parameter to specify how data binding will occur.
        /// For example, metadata containing an XPath expression could be used by XML data sources.
        /// </para>
        /// </remarks>
        /// <value>
        /// The default value is null which causes the parameter's index to be set to 0 for fields
        /// and properties or the parameter's actual positional index for method parameters.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 0</exception>
        public int? Index
        {
            get { return index; }
            set
            {
                if (value.HasValue && value.Value < 0)
                    throw new ArgumentOutOfRangeException("value");

                index = value;
            }
        }

        /// <inheritdoc />
        public override void Apply(TestTemplateTreeBuilder builder, MbUnitTestParameter parameter)
        {
            if (name != null)
                parameter.Name = name;

            if (set != null)
                parameter.ParameterSet.Template.SetParameterSetName(parameter, set);

            if (index.HasValue)
                parameter.Index = index.Value;

            base.Apply(builder, parameter);
        }
    }
}
