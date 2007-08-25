using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.Conversions;

namespace MbUnit.Framework.Kernel.DataBinding
{
    /// <summary>
    /// A data factory that applies a converter to the value it obtains.
    /// The implementation assumes that the converter supports the desired
    /// conversion.
    /// </summary>
    public class ConvertedDataFactory : IDataFactory
    {
        private readonly IConverter converter;
        private readonly IDataFactory inner;
        private readonly Type targetType;

        /// <summary>
        /// Creates a converted data factory.
        /// </summary>
        /// <param name="inner">The inner factory</param>
        /// <param name="converter">The converter</param>
        /// <param name="targetType">The target type to which the value produced by
        /// the inner factory should be converted</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inner"/>,
        /// <paramref name="converter"/> or <paramref name="targetType"/> is null</exception>
        public ConvertedDataFactory(IDataFactory inner, IConverter converter, Type targetType)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");
            if (converter == null)
                throw new ArgumentNullException("converter");
            if (targetType == null)
                throw new ArgumentNullException("targetType");

            this.inner = inner;
            this.converter = converter;
            this.targetType = targetType;
        }

        /// <inheritdoc />
        public object GetValue(IDataBindingContext context)
        {
            object sourceValue = inner.GetValue(context);
            return converter.Convert(sourceValue, targetType);
        }
    }
}
