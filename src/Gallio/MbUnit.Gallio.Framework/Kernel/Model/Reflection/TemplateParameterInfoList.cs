using System;
using System.Collections.Generic;

namespace MbUnit.Framework.Kernel.Model.Reflection
{
    /// <summary>
    /// Wraps a list of <see cref="ITemplateParameter" /> for reflection.
    /// </summary>
    public sealed class TemplateParameterInfoList : BaseInfoList<ITemplateParameter, TemplateParameterInfo>
    {
        /// <summary>
        /// Creates a wrapper for the specified input list of model objects.
        /// </summary>
        /// <param name="inputList">The input list</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inputList"/> is null</exception>
        public TemplateParameterInfoList(IList<ITemplateParameter> inputList)
            : base(inputList)
        {
        }

        /// <inheritdoc />
        protected override TemplateParameterInfo Wrap(ITemplateParameter inputItem)
        {
            return new TemplateParameterInfo(inputItem);
        }
    }
}