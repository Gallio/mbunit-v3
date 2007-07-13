// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;

namespace TestFu.Operations
{
    public sealed class Domains
    {
        private Domains() { }

        public static EmptyDomain Empty
        {
            get
            {
                return new EmptyDomain();
            }
        }

        public static bool IsUniform(IDomainCollection domains)
        {
            if (domains == null)
                throw new ArgumentNullException("domains");

            int maxCount = int.MinValue;
            int minCount = int.MaxValue;
            foreach (IDomain domain in domains)
            {
                maxCount = Math.Max(maxCount, domain.Count);
                minCount = Math.Max(minCount, domain.Count);
                if (maxCount != minCount)
                    return false;
            }
            return true;
        }

        public static IDomainCollection Uniformize(params Object[] domains)
        {
            return Uniformize(ToDomains(domains));
        }

        public static IDomainCollection Uniformize(params ICollection[] domains)
        {
            return Uniformize(ToDomains(domains));
        }

        public static IDomainCollection Uniformize(params IEnumerable[] domains)
        {
            return Uniformize(ToDomains(domains));
        }

        public static IDomainCollection Uniformize(IDomainCollection domains)
        {
            if (domains == null)
                throw new ArgumentNullException("domains");

            Random rnd = new Random((int)DateTime.Now.Ticks);
            // find max
            int maxCount = int.MinValue;
            int minCount = int.MaxValue;
            foreach (IDomain domain in domains)
            {
                maxCount = Math.Max(maxCount, domain.Count);
                minCount = Math.Max(minCount, domain.Count);
            }

            if (minCount == maxCount)
                return domains;

            DomainCollection udomains = new DomainCollection();
            foreach (IDomain domain in domains)
            {
                if (domain.Count == maxCount)
                {
                    udomains.Add(domain);
                    continue;
                }

                Object[] udomain = new Object[maxCount];
                int i;
                for(i = 0;i<domain.Count;++i)
                    udomain[i] = domain[i];
                for (; i<maxCount;++i)
                {
                    udomain[i] = domain[ rnd.Next(domain.Count) ];
                }
                udomains.Add(Domains.ToDomain(udomain));
            }
            return udomains;
        }

        public static IDomainCollection ToDomains(Array array)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (array.Length == 0)
                throw new ArgumentException("Length is zero", "array");
            IDomain[] domains = new IDomain[array.Length];
            for (int i = 0; i < array.Length; ++i)
                domains[i] = ToDomain(array.GetValue(i));
            return new DomainCollection(domains);
        }

        public static IDomainCollection ToDomains(params Object[] items)
        {
            if (items.Length == 0)
                throw new ArgumentException("Length is zero", "items");
            DomainCollection ds = new DomainCollection();
            foreach (Object domain in items)
                ds.Add(ToDomain(domain));
            return ds;
        }

        public static IDomain ToDomain(Object item)
        {
            IDomain domain = item as IDomain;
            if (domain != null)
                return domain;

            String s = item as String;
            if (s != null)
                return ToDomain(s);

            Array array = item as Array;
            if (array != null)
                return ToDomain(array);

            ICollection collection = item as ICollection;
            if (collection != null)
                return ToDomain(collection);

            IEnumerable enumerable = item as IEnumerable;
            if (enumerable != null)
                return ToDomain(enumerable);

            return new CollectionDomain(new object[]{item});
        }

        public static IDomain ToDomain(string s)
        {
            return new StringDomain(s);
        }

        public static CollectionDomain ToDomain(ICollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (collection.Count == 0)
                throw new ArgumentNullException("Collection is emtpy");
            return new CollectionDomain(collection);
        }

        public static CollectionDomain ToDomain(IEnumerable enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");
            return new CollectionDomain(enumerable);
        }

        public static ArrayDomain ToDomain(params object[] items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            if (items.Length == 0)
                throw new ArgumentException("No arguments");
            return new ArrayDomain(items);
        }

        public static ArrayDomain ToDomain(Array items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            if (items.Length == 0)
                throw new ArgumentException("No arguments");
            return new ArrayDomain(items);
        }
    }
}
