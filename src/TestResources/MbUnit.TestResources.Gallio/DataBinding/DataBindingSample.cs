using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace MbUnit.TestResources.Gallio.DataBinding
{
    /// <summary>
    /// Basic data-binding scenarios.
    /// </summary>
    [TestFixture]
    public class BindingSample
    {
        [RowTest]
        [Row(1, "quux", "2002/01/01")]
        public void AutoBindingByPropertyIndex(int a, string b, DateTime c)
        {
            Assert.AreEqual(1, a);
            Assert.AreEqual("quux", b);
            Assert.AreEqual(new DateTime(2002, 1, 1), c);
        }

#if false
        [RowTest]
        [Header("c", "b", "a")]
        [Row("2002/01/01", "quux", 1)]
        public void AutoBindingByPropertyPath(int a, string b, DateTime c)
        {
            Assert.AreEqual(1, a);
            Assert.AreEqual("quux", b);
            Assert.AreEqual(new DateTime(2002, 1, 1), c);
        }

        [RowTest]
        [Row("2002/01/01", "quux", 1)]
        public void CustomBindingByPropertyIndex(
            [Bind(2)] int a, [Bind(1)] string b, [Bind(0)] DateTime c)
        {
            Assert.AreEqual(1, a);
            Assert.AreEqual("quux", b);
            Assert.AreEqual(new DateTime(2002, 1, 1), c);
        }

        [RowTest]
        [Header("c", "b", "a")]
        [Row(1, "quux", "2002/01/01")]
        public void CustomBindingByPropertyPath(
            [Bind("c")] int a, [Bind("b")] string b, [Bind("a")] DateTime c)
        {
            Assert.AreEqual(1, a);
            Assert.AreEqual("quux", b);
            Assert.AreEqual(new DateTime(2002, 1, 1), c);
        }

        [RowTest]
        [Header("c", "b", "a")]
        [Row("2002/01/01", "quux", 1)]
        public void TypeBindingWithMatchingNames(
            [BindType] Data s)
        {
            Assert.AreEqual(1, a);
            Assert.AreEqual("quux", b);
            Assert.AreEqual(new DateTime(2002, 1, 1), c);
        }

        [RowTest]
        [Header("a", "b", "c")]
        [Row("2002/01/01", "quux", 1)]
        public void TypeBindingWithCustomNamesAndType(
            [BindType(typeof(Data)), BindProperty("a", "C"), BindProperty("b", "B"), BindProperty("c", "A")] object s)
        {
            Data data = (Data)s;
            Assert.AreEqual(1, data.A);
            Assert.AreEqual("quux", data.B);
            Assert.AreEqual(new DateTime(2002, 1, 1), data.C);
        }

        [RowTest]
        [Header("a", "b", "c")]
        [Row("2002/01/01", "quux", 1)]
        public void FactoryBinding(
            [BindFactory("DataFactory", "a", "b", "c")] Data s)
        {
            Assert.AreEqual(1, s.A);
            Assert.AreEqual("quux", s.B);
            Assert.AreEqual(new DateTime(2002, 1, 1), s.C);
        }

        [RowTest]
        [Header("c", "b", Source="First")]
        [Row("2002/01/01", "quux", Source="First")]
        [Header("a", Source="Second")]
        [Row(1, Source="Second")]
        public void JoinsAcrossSources(
            [Bind(Source = "Second")] int a, [Bind(Source = "First")] string b, [Bind(Source = "First")] DateTime c)
        {
            Assert.AreEqual(1, a);
            Assert.AreEqual("quux", b);
            Assert.AreEqual(new DateTime(2002, 1, 1), c);
        }

        public static Data DataFactory(int a, string b, DateTime c)
        {
            Data data = new Data();
            data.A = a;
            data.B = b;
            data.C = c;
            return data;
        }

        public class Data
        {
            public int A;
            public string B;
            public DateTime C;
        }
#endif
    }
}
