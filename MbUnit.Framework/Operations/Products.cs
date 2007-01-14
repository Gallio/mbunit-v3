using System;
using System.Collections;

namespace TestFu.Operations
{
    public sealed class Products
    {
        private Products() { }

        #region Helpers
        private static void CheckDomains(IDomainCollection domains)
        {
            if (domains == null)
                throw new ArgumentNullException("domains");
            if (domains.Count == 0)
                throw new ArgumentException("domains is empty");
            for (int i = 0; i < domains.Count; ++i)
            {
                if (domains[i] == null)
                    throw new ArgumentNullException("Domain[" + i.ToString() + "] is null");
                if (domains[i].Count==0)
                    throw new ArgumentNullException("Domain[" + i.ToString() + "] is empty");
            }
        }
        public static IDomainCollection ExtractBoundaries(IDomainCollection domains)
        {
            CheckDomains(domains);

            DomainCollection boundaries = new DomainCollection();
            foreach (IDomain domain in domains)
                boundaries.Add(domain.Boundary);
            return boundaries;
        }
        #endregion

        #region Cartesian
        public static ITupleEnumerable Cartesian(IDomainCollection domains)
        {
            CheckDomains(domains);

            return new CartesianProductDomainTupleEnumerable(domains);
        }

        public static ITupleEnumerable Cartesian(params IDomain[] domains)
        {
            return Cartesian(new DomainCollection(domains));
        }

        public static ITupleEnumerable Cartesian(params Object[] domains)
        {
            return Cartesian(Domains.ToDomains(domains));
        }
        #endregion

        #region PairWize
        public static ITupleEnumerable PairWize(IDomainCollection domains)
        {
            CheckDomains(domains);
            if (domains.Count <= 2)
                return Cartesian(domains);

            if (Domains.IsUniform(domains))
                return new UniformPairWizeProductDomainTupleEnumerable(domains);
            else
            {
                IDomainCollection udomains = Domains.Uniformize(domains);
                return Greedy(new UniformPairWizeProductDomainTupleEnumerable(udomains));
            }
        }
        public static ITupleEnumerable PairWize(params IDomain[] domains)
        {
            return PairWize(new DomainCollection(domains));
        }
        public static ITupleEnumerable PairWize(params Object[] domains)
        {
            return PairWize(Domains.ToDomains(domains));
        }
        #endregion

        #region TWize
        public static ITupleEnumerable TWize(int tupleSize, IDomainCollection domains)
        {
            CheckDomains(domains);

            IDomainCollection udomains = Domains.Uniformize(domains);
            return new UniformTWizeProductDomainTupleEnumerable(udomains, tupleSize);
        }
        public static ITupleEnumerable TWize(int tupleSize, params IDomain[] domains)
        {
            return TWize(tupleSize, new DomainCollection(domains));
        }
        public static ITupleEnumerable TWize(int tupleSize, params Object[] domains)
        {
            return TWize(tupleSize, Domains.ToDomains(domains));
        }
        #endregion

        public static ITupleEnumerable Greedy(ITupleEnumerable tuples)
        {
            if (tuples == null)
                throw new ArgumentNullException("tuples");
            return new GreedyTupleEnumerable(tuples);
        }
    }
}
