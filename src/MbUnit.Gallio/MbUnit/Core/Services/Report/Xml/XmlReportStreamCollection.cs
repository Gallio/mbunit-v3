using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using MbUnit.Core.Collections;

namespace MbUnit.Core.Services.Report.Xml
{
    /// <summary>
    /// A collection of report streams.
    /// </summary>
    /// <remarks>
    /// The operations on this class are thread-safe, including enumeration.
    /// </remarks>
    [Serializable]
    public sealed class XmlReportStreamCollection : SyncUnorderedKeyedCollection<string, IReportStream>, IReportStreamCollection
    {
        private XmlReport report;

        /// <summary>
        /// Creates an empty stream collection associated with the specified report.
        /// </summary>
        /// <param name="report">The associated report</param>
        public XmlReportStreamCollection(XmlReport report)
        {
            this.report = report;
        }

        protected override string GetKeyForItem(IReportStream item)
        {
            return item.Name;
        }

        public override IReportStream this[string streamName]
        {
            get
            {
                lock (SyncRoot)
                {
                    IReportStream stream;
                    if (!Collection.TryGetValue(streamName, out stream))
                    {
                        XmlReportStream xmlStream = new XmlReportStream();
                        xmlStream.Name = streamName;
                        xmlStream.Report = report;

                        Collection.Add(streamName, xmlStream);
                        ClearCachedArray();

                        stream = xmlStream;
                    }

                    return stream;
                }
            }
        }

        public override bool TryGetValue(string key, out IReportStream value)
        {
            value = this[key];
            return true;
        }

        public override void Add(IReportStream item)
        {
            if (! (item is XmlReportStream))
                throw new ArgumentException("Stream must be an XmlReportStream.", "item");

            base.Add(item);
        }
    }
}
