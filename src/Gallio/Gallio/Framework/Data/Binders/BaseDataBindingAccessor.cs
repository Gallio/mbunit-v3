using System;

namespace Gallio.Framework.Data.Binders
{
    /// <summary>
    /// A base implementation of <see cref="IDataBindingAccessor" /> that
    /// performs argument validation.
    /// </summary>
    public abstract class BaseDataBindingAccessor : IDataBindingAccessor
    {
        /// <inheritdoc />
        public object GetValue(DataBindingItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return GetValueInternal(item);
        }

        /// <summary>
        /// Internal implementation of <see cref="GetValue" /> after argument
        /// validation has been performed.
        /// </summary>
        /// <param name="item">The data binding item, not null</param>
        /// <returns>The value</returns>
        protected abstract object GetValueInternal(DataBindingItem item);
    }
}
