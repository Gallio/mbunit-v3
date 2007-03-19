using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Collections;

namespace MbUnit.Core.Services.Report
{
    /// <summary>
    /// A lazily-populated collection of report streams for the report.
    /// Streams are automatically created on demand if no stream with the specified name
    /// exists at the time of the request.
    /// </summary>
    public interface IReportStreamCollection : IKeyedCollection<string, IReportStream>
    {
    }
}
