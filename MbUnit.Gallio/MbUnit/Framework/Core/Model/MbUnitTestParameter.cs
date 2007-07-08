using System;
using System.Reflection;
using MbUnit.Core.Model;

namespace MbUnit.Framework.Core.Model
{
    /// <summary>
    /// Represents a template parameter.
    /// </summary>
    public class MbUnitTestParameter : BaseTestParameter
    {
        private Slot slot;

        /// <summary>
        /// Initializes an MbUnit test parameter model object.
        /// </summary>
        /// <param name="parameterSet">The parameter set</param>
        /// <param name="slot">The slot, non-null</param>
        public MbUnitTestParameter(MbUnitTestParameterSet parameterSet, Slot slot)
            : base(slot.Name, slot.CodeReference, parameterSet, slot.ValueType)
        {
            this.slot = slot;

            Index = slot.Position;
        }

        /// <inheritdoc />
        public new MbUnitTestParameterSet ParameterSet
        {
            get { return (MbUnitTestParameterSet) base.ParameterSet; }
            set { base.ParameterSet = value; }
        }

        /// <summary>
        /// Gets the associated slot.
        /// </summary>
        public Slot Slot
        {
            get { return slot; }
        }
    }
}
