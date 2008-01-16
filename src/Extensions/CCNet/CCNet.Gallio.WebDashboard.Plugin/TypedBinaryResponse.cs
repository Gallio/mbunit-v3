using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using System.Web;

namespace CCNet.Gallio.WebDashboard.Plugin
{
    public class TypedBinaryResponse : IResponse
    {
        private readonly byte[] content;
        private readonly string contentType;

        private ConditionalGetFingerprint serverFingerprint;

        public TypedBinaryResponse(byte[] content, string contentType)
        {
            this.content = content;
            this.contentType = contentType;
        }

        public void Process(HttpResponse response)
        {
            response.Clear();

            response.AppendHeader("Last-Modified", serverFingerprint.LastModifiedTime.ToString("r"));
            response.AppendHeader("ETag", serverFingerprint.ETag);
            response.AppendHeader("Cache-Control", "private, max-age=0");

            response.ContentType = contentType;

            response.BinaryWrite(content);
            response.End();
        }

        public ConditionalGetFingerprint ServerFingerprint
        {
            get { return serverFingerprint; }
            set { serverFingerprint = value; }
        }
    }
}
