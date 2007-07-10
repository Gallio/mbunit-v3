using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using System.Reflection;


namespace MbUnit.Framework
{
    public sealed class DataAssert
    {
        /// <summary>
        /// A private constructor disallows any instances of this object. 
        /// </summary>
        private DataAssert()
        { }

        /// <summary>
        /// Asserts that two <see cref="DataColumn"/> are equal.
        /// </summary> 
        /// <param name="expected">Expected <see cref="DataColumn"/> instance.</param>
        /// <param name="actual">Actual <see cref="DataColumn"/> instance.</param>
        /// <remarks>		
        public static void AreEqual(DataColumn expected, DataColumn actual)
        {
            // null case, trivial comparaison
            if (expected == null)
                Assert.AreEqual(expected, actual);

            // check same type
            Assert.AreEqual(expected.GetType(), actual.GetType());

            foreach (PropertyInfo pi in expected.GetType().GetProperties())
            {
                switch (pi.Name)
                {
                    case "Container": continue;
                    case "Site": continue;
                }

                Assert.AreValueEqual(pi, expected, actual);
            }
        }



        /// <summary>
        /// Asserts that two <see cref="DataRow"/> are equal.
        /// </summary> 
        /// <param name="expected">Expected <see cref="DataRow"/> instance.</param>
        /// <param name="actual">Actual <see cref="DataRow"/> instance.</param>
        /// <remarks>
        /// <para>
        /// Insipired from this 
        /// <a href="http://dotnetjunkies.com/WebLog/darrell.norton/archive/2003/06/05/213.aspx">
        /// blog entry.</a>.
        /// </para>
        /// </remarks>
        public static void AreEqual(DataRow expected, DataRow actual)
        {
            // if expected is null, trivial check
            if (expected == null)
                Assert.AreEqual(expected, actual);

            // check number of columns
            Assert.AreEqual(expected.Table.Columns.Count,
                            actual.Table.Columns.Count,
                            "DataRow column count not the same"
                            );

            for (int currentIndex = 0; currentIndex < expected.ItemArray.Length; currentIndex++)
            {
                // Check all columns except autoincrement columns
                if (expected.Table.Columns[currentIndex].AutoIncrement == false)
                {
                    Object ep = expected[currentIndex];
                    Object ac = actual[currentIndex];

                    if (ep is DateTime)
                    {
                        AreEqual((DateTime)ep, (DateTime)ac);
                    }
                    else
                    {
                        // check values are equal
                        Assert.AreEqual(
                            ep,
                            ac,
                            "DataRow comparison failed on column {0}",
                            expected.Table.Columns[currentIndex].ToString()
                            );
                    }
                }
            }
        }


        static public void AreEqual(DateTime expected, DateTime actual)
        {
            Assert.AreEqual(expected.Year, actual.Year);
            Assert.AreEqual(expected.Month, actual.Month);
            Assert.AreEqual(expected.Day, actual.Day);
            Assert.AreEqual(expected.Hour, actual.Hour);
            Assert.AreEqual(expected.Minute, actual.Minute);
            Assert.AreEqual(expected.Second, actual.Second);
            //  Assert.AreEqual(expected.Millisecond, actual.Millisecond);
        }

        /// <summary>
        /// Assert that <see cref="DataSet"/> schemas are equal.
        /// </summary>
        public static void AreSchemasEqual(DataSet expected, DataSet actual)
        {
            if (expected == null)
            {
                Assert.AreEqual(expected, actual);
                return;
            }

            string sex = getXmlSchema(expected);
            string sact = getXmlSchema(actual);

            XmlAssert.XmlEquals(sex, sact);
        }

        /// <summary>
        /// Assert that <see cref="DataSet"/> schemas and data are equal.
        /// </summary>
        public static void AreEqual(DataSet expected, DataSet actual)
        {
            if (expected == null)
            {
                Assert.AreEqual(expected, actual);
                return;
            }

            string sex = getXml(expected, true);
            string sact = getXml(actual, true);

            XmlAssert.XmlEquals(sex, sact);
        }

        /// <summary>
        /// Assert that <see cref="DataSet"/> data are equal.
        /// </summary>		
        public static void AreDataEqual(DataSet expected, DataSet actual)
        {
            if (expected == null)
            {
                Assert.AreEqual(expected, actual);
                return;
            }

            string sex = getXml(expected, false);
            string sact = getXml(actual, false);

            XmlAssert.XmlEquals(sex, sact);
        }

        #region Private methods
        private static string getXmlSchema(DataSet ds)
        {
            if (ds == null)
                throw new ArgumentNullException("ds");

            StringWriter sw = new StringWriter();
            XmlTextWriter xm = new XmlTextWriter(sw);
            xm.Formatting = Formatting.Indented;

            ds.WriteXmlSchema(xm);
            xm.Close();

            return sw.ToString();
        }

        private static string getXml(DataSet ds, bool writeSchema)
        {
            if (ds == null)
                throw new ArgumentNullException("ds");

            StringWriter sw = new StringWriter();
            XmlTextWriter xm = new XmlTextWriter(sw);
            xm.Formatting = Formatting.Indented;

            if (writeSchema)
                ds.WriteXml(xm, XmlWriteMode.WriteSchema);
            else
                ds.WriteXml(xm, XmlWriteMode.IgnoreSchema);
            xm.Close();

            return sw.ToString();
        }
        #endregion
    }
}
