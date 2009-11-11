using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Conversions
{
    internal struct ConversionKey
    {
        private readonly Type sourceType;
        private readonly Type targetType;

        public ConversionKey(Type sourceType, Type targetType)
        {
            this.sourceType = sourceType;
            this.targetType = targetType;
        }
    }
}
