using System;
using System.Collections;

namespace TestFu.Operations
{
    public abstract class DomainBase : IDomain
    {
        private string name = null;

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public abstract int Count { get;}
        public virtual IDomain Boundary 
        { 
            get
            {
                if (this.Count <= 2)
                    return this;
                return new ArrayDomain(this[0], this[this.Count - 1]);
            }
        }
        public abstract IEnumerator GetEnumerator();
        public abstract Object this[int index] { get;}
    }
}
