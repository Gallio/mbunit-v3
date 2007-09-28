using System;
using System.Collections.Generic;

namespace MbUnit.Framework.Kernel.Model.Reflection
{
    /// <summary>
    /// Wraps a list of <see cref="ITemplate"/> for reflection.
    /// </summary>
    public sealed class TemplateInfoList : BaseInfoList<ITemplate, TemplateInfo>
    {
        /// <summary>
        /// Creates a wrapper for the specified input list of model objects.
        /// </summary>
        /// <param name="inputList">The input list</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inputList"/> is null</exception>
        public TemplateInfoList(IList<ITemplate> inputList)
            : base(inputList)
        {
        }

        /// <inheritdoc />
        protected override TemplateInfo Wrap(ITemplate inputItem)
        {
            return new TemplateInfo(inputItem);
        }
    }
}
