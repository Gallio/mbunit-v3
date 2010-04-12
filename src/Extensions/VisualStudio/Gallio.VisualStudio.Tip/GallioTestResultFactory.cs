// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Gallio.Model;
using Gallio.Common.Markup;
using Gallio.Runner.Reports.Schema;
using Microsoft.VisualStudio.TestTools.Common;
using GallioTestOutcome = Gallio.Model.TestOutcome;
using VSTestOutcome = Microsoft.VisualStudio.TestTools.Common.TestOutcome;

namespace Gallio.VisualStudio.Tip
{
    internal static class GallioTestResultFactory
    {
        /// <summary>
        /// Creates an aggregate root test result.
        /// </summary>
        /// <param name="runId">The test run id.</param>
        /// <param name="test">The test element.</param>
        /// <returns>The test result.</returns>
        public static GallioTestResult CreateAggregateRootTestResult(Guid runId, ITestElement test)
        {
            var result = new GallioTestResult(runId, test);
            result.Outcome = VSTestOutcome.Pending;
            return result;
        }

        /// <summary>
        /// Creates a test result from a test step run.
        /// </summary>
        /// <param name="run">The test step run.</param>
        /// <param name="runId">The test run id.</param>
        /// <param name="test">The test element.</param>
        /// <returns>The test result.</returns>
        public static GallioTestResult CreateTestResult(TestStepRun run, Guid runId, ITestElement test)
        {
            return CreateTestResult(run, runId, test, true);
        }

        private static GallioTestResult CreateTestResult(TestStepRun run, Guid runId, ITestElement test, bool includeTestStepRunXml)
        {
            var result = new GallioTestResult(runId, test);
            result.TestName = run.Step.FullName;
            result.Outcome = GetOutcome(run.Result.Outcome);

            foreach (StructuredStream stream in run.TestLog.Streams)
            {
                string contents = stream.ToString();

                if (stream.Name == MarkupStreamNames.DebugTrace)
                    result.DebugTrace += contents;
                else if (stream.Name == MarkupStreamNames.ConsoleOutput)
                    result.StdOut += contents;
                else if (stream.Name == MarkupStreamNames.ConsoleError)
                    result.StdErr += contents;
                else if (stream.Name == MarkupStreamNames.Failures || stream.Name == MarkupStreamNames.Warnings)
                    result.ErrorMessage += contents;
                else
                    result.DebugTrace += contents;
            }

            result.SetTimings(run.StartTime, run.EndTime, run.Result.Duration);

            if (includeTestStepRunXml)
                result.TestStepRunXml = TestStepRunToXml(run);

            foreach (TestStepRun childRun in run.Children)
                result.AddInnerResult(CreateTestResult(childRun, runId, test, false));

            return result;
        }

        /// <summary>
        /// Gets the test step runs associated with a test result.
        /// </summary>
        public static IList<TestStepRun> GetTestStepRuns(GallioTestResult result)
        {
            List<TestStepRun> runs = new List<TestStepRun>();

            if (result.IsAggregateRoot)
            {
                foreach (GallioTestResult innerResult in result.InnerResults)
                {
                    TestStepRun run = GetTestStepRun(innerResult);
                    if (run != null)
                        runs.Add(run);
                }
            }
            else
            {
                TestStepRun run = GetTestStepRun(result);
                if (run != null)
                    runs.Add(run);
            }

            return runs;
        }

        /// <summary>
        /// Gets a test step run from a given test result.
        /// </summary>
        /// <param name="result">A test result.</param>
        /// <returns>A test step run instance.</returns>
        public static TestStepRun GetTestStepRun(GallioTestResult result)
        {
            TestStepRun run = TestStepRunFromXml(result.TestStepRunXml);
#if VS100_OR_NEWER
            if (run != null)
            {
                foreach (CollectorDataEntry collectorEntry in result.CollectorDataEntries)
                {
                    if (collectorEntry != null)
                    {
                        var attachmentData = CreateAttachmentData(collectorEntry);
                        if(attachmentData != null)
                        {
                            run.TestLog.Attachments.Add(attachmentData.ToAttachmentData());
                        }
                    }
                }                
            }
#endif
            return run;
        }

#if VS100_OR_NEWER
        /// <summary>
        /// Creates a Gallio attachement from any VSTS collector data.
        /// </summary>
        /// <param name="collectorEntry">A VSTS data collector.</param>
        /// <returns>Gallio attachment instance.</returns>
        private static Attachment CreateAttachmentData(CollectorDataEntry collectorEntry)
        {
            Attachment attachment = null;
            
            var vstsAttachment = (UriDataAttachment)collectorEntry.Attachments[0];

            if (vstsAttachment != null)
            {
                //var file = vstsAttachment.Uri.Segments.Last();
                //var fileEx = file.Substring(file.Length - 7, 7);

                var file = new FileInfo(vstsAttachment.Uri.Segments.Last());

                var fileEx = file.Extension;

                switch(fileEx)
                {
                    case ".png":
                        attachment = GetImageAttachment(vstsAttachment);
                        break;
                    case ".jpg":
                        attachment = GetImageAttachment(vstsAttachment);
                        break;
                    case ".gif":
                        attachment = GetImageAttachment(vstsAttachment);
                        break;
                    case ".txt":
                        attachment = GetTextAttachment(vstsAttachment);
                        break;
                    case ".xml":
                        attachment = GetXmlAttachment(vstsAttachment);
                        break;
                    case ".htm":
                        attachment = GetHtmlAttachment(vstsAttachment);
                        break;
                    default :
                        attachment = GetBinaryAttachment(vstsAttachment, fileEx);
                        break;

                }
               
            }
            return attachment;
        }

        private static Attachment GetBinaryAttachment(UriDataAttachment vstsAttachment, string fileEx)
        {
            var mem = new MemoryStream();
            var f = new FileStream(vstsAttachment.Uri.LocalPath, FileMode.Open, FileAccess.Read);
            mem.SetLength(f.Length);

            f.Read(mem.GetBuffer(), 0, (int)f.Length -1);

            try
            {
                var binaryAttachment = new BinaryAttachment(vstsAttachment.Description, MimeTypes.GetMimeTypeByExtension(fileEx), mem.ToArray());

                f.Close();
                mem.Close();

                return binaryAttachment;
            }
            catch (Exception)
            {
                //bury the exception and return null back so no attachment is returned
                return null;
            }
        }

        private static BinaryAttachment GetImageAttachment(UriDataAttachment vstsAttachment)
        {
            var image = Image.FromFile(vstsAttachment.Uri.LocalPath);
                
            return Attachment.CreateImageAttachment(vstsAttachment.Description, image);
        }

        private static TextAttachment GetXmlAttachment(UriDataAttachment vstsAttachment)
        {
            var sb = GetFileAsString(vstsAttachment);

            return Attachment.CreateXmlAttachment(vstsAttachment.Description, sb.ToString());
          
        }

        private static TextAttachment GetTextAttachment(UriDataAttachment vstsAttachment)
        {
            var sb = GetFileAsString(vstsAttachment);

            return Attachment.CreatePlainTextAttachment(vstsAttachment.Description, sb.ToString());
        }

        private static TextAttachment GetHtmlAttachment(UriDataAttachment vstsAttachment)
        {
            var sb = GetFileAsString(vstsAttachment);

            return Attachment.CreateHtmlAttachment(vstsAttachment.Description, sb.ToString());
        }

        private static StringBuilder GetFileAsString(UriDataAttachment vstsAttachment)
        {
            var sb = new StringBuilder();
            using (var sr = new StreamReader(vstsAttachment.Uri.LocalPath))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
            }
            return sb;
        }

#endif

        /// <summary>
        /// Merges Gallio test results.
        /// </summary>
        /// <param name="inMemory">The currently available test result, or null if none.</param>
        /// <param name="fromTheWire">The test result to add, or null if none.</param>
        /// <returns>The merged test result.</returns>
        public static GallioTestResult Merge(GallioTestResult inMemory, GallioTestResult fromTheWire)
        {
            if (fromTheWire == null)
                return inMemory;
            if (inMemory == null)
                return fromTheWire;

            List<GallioTestResult> combinedResults = new List<GallioTestResult>();
            VSTestOutcome combinedOutcome = VSTestOutcome.Pending;
            CombineResults(combinedResults, ref combinedOutcome, inMemory);
            CombineResults(combinedResults, ref combinedOutcome, fromTheWire);

            GallioTestResult result;
            if (combinedResults.Count == 1)
            {
                result = combinedResults[0];
            }
            else
            {
                result = CreateAggregateRootTestResult(inMemory.Id.RunId, inMemory.Test);

                if (combinedResults.Count != 0)
                {
                    result.SetInnerResults(combinedResults);

                    DateTime startTime = DateTime.MaxValue;
                    DateTime endTime = DateTime.MinValue;
                    TimeSpan duration = TimeSpan.Zero;
                    foreach (GallioTestResult innerResult in combinedResults)
                    {
                        if (innerResult.StartTime < startTime)
                            startTime = innerResult.StartTime;
                        if (innerResult.EndTime > endTime)
                            endTime = innerResult.EndTime;
                        duration += innerResult.Duration;
                    }

                    result.SetTimings(startTime, endTime, duration);
                }
            }

            result.Outcome = combinedOutcome;
            return result;
        }

        private static void CombineResults(List<GallioTestResult> combinedResults, ref VSTestOutcome combinedOutcome, GallioTestResult source)
        {
            CombineOutcome(ref combinedOutcome, source.Outcome);

            if (source.IsAggregateRoot)
            {
                foreach (GallioTestResult innerResult in source.InnerResults)
                {
                    combinedResults.Add(innerResult);
                    CombineOutcome(ref combinedOutcome, innerResult.Outcome);
                }
            }
            else
            {
                combinedResults.Add(source);
            }
        }

        private static void CombineOutcome(ref VSTestOutcome combinedOutcome, VSTestOutcome sourceOutcome)
        {
            if (sourceOutcome < combinedOutcome)
                combinedOutcome = sourceOutcome;
        }

        private static VSTestOutcome GetOutcome(GallioTestOutcome outcome)
        {
            if (outcome == GallioTestOutcome.Canceled)
                return VSTestOutcome.Aborted;
            if (outcome == GallioTestOutcome.Error)
                return VSTestOutcome.Error;
            if (outcome == GallioTestOutcome.Timeout)
                return VSTestOutcome.Timeout;
            if (outcome == GallioTestOutcome.Ignored || outcome == GallioTestOutcome.Pending || outcome == GallioTestOutcome.Explicit)
                return VSTestOutcome.NotRunnable;

            switch (outcome.Status)
            {
                case TestStatus.Passed:
                    return VSTestOutcome.Passed;

                case TestStatus.Inconclusive:
                    return VSTestOutcome.Inconclusive;

                case TestStatus.Skipped:
                    return VSTestOutcome.NotExecuted;

                case TestStatus.Failed:
                default:
                    return VSTestOutcome.Failed;
            }
        }

        private static string TestStepRunToXml(TestStepRun run)
        {
            StringWriter stringWriter = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(TestStepRun));
            serializer.Serialize(stringWriter, run);
            return stringWriter.ToString();
        }

        private static TestStepRun TestStepRunFromXml(string xml)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TestStepRun));
                return (TestStepRun)serializer.Deserialize(new StringReader(xml));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
