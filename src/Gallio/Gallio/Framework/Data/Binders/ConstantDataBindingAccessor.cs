namespace Gallio.Framework.Data.Binders
{
    /// <summary>
    /// An implementation of <see cref="IDataBindingAccessor" /> that
    /// returns a constant value.
    /// </summary>
    public sealed class ConstantDataBindingAccessor : BaseDataBindingAccessor
    {
        private readonly object value;

        /// <summary>
        /// Creates a data binding accessor for a constant value.
        /// </summary>
        /// <param name="value">The constant value to be returned by <see cref="IDataBindingAccessor.GetValue" /></param>
        public ConstantDataBindingAccessor(object value)
        {
            this.value = value;
        }

        /// <inheritdoc />
        protected override object GetValueInternal(DataBindingItem item)
        {
            return value;
        }
    }
}
