using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Resources;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Identifies a stock of predefined string values for the random strings generator.
    /// </summary>
    /// <seealso cref="RandomStringsAttribute"/>
    public enum RandomStringStock
    {
        /// <summary>
        /// A predefined collection of random US male names.
        /// </summary>
        EnUSMaleNames,

        /// <summary>
        /// A predefined collection of random country names.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: <a href="http://unstats.un.org/unsd/methods/m49/m49alpha.htm">Countries or areas, codes and abbreviations</a>.
        /// </para>
        /// </remarks>
        EnCountries,
    }

    internal sealed class RandomStringStockInfo
    {
        private readonly RandomStringStock stock;
        private readonly string databaseResource;
        private string[] items;

        private RandomStringStockInfo(RandomStringStock stock, string databaseResource)
        {
            this.stock = stock;
            this.databaseResource = databaseResource;
        }

        private static readonly Dictionary<RandomStringStock, RandomStringStockInfo> map = new Dictionary<RandomStringStock,RandomStringStockInfo>
        {
            { RandomStringStock.EnUSMaleNames, 
              new RandomStringStockInfo(RandomStringStock.EnUSMaleNames, "Database_EnUSMaleNames") },
            { RandomStringStock.EnCountries, 
              new RandomStringStockInfo(RandomStringStock.EnCountries, "Database_EnCountries") },
        };

        public string[] GetItems()
        {
            if (items == null)
                LoadItems();

            return items;
        }

        private void LoadItems()
        {
            var dataSet = new DataSet();
            dataSet.ReadXmlSchema(new StringReader(Properties.Resources.Database_Schema));
            dataSet.ReadXml(new StringReader(Properties.Resources.ResourceManager.GetString(databaseResource)));
            var rows = dataSet.Tables["Item"].Rows;
            items = new string[rows.Count];

            for (int i = 0; i < rows.Count; i++)
            {
                items[i] = (string)rows[i][0];
            }
        }

        public static RandomStringStockInfo FromStock(RandomStringStock stock)
        {
            try
            {
                return map[stock];
            }
            catch (KeyNotFoundException exception)
            {
                throw new PatternUsageErrorException(String.Format("The stock of predefined string values named '{0}' does not exist.", stock), exception);
            }
        }
    }
}
