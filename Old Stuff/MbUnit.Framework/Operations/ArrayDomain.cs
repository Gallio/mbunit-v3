using System;
using System.Collections;

namespace TestFu.Operations
{
    public class ArrayDomain : DomainBase
    {
        private Array array;

        public ArrayDomain(params Object[] array)
        {
            this.array = array;
        }
        public ArrayDomain(Array array)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            this.array = array;
        }
        public override int Count
        {
            get { return array.Length; }
        }

        public override IEnumerator GetEnumerator()
        {
            return array.GetEnumerator();
        }

        public override Object this[int index]
        {
            get { return this.array.GetValue(index); }
        }
    }
}
