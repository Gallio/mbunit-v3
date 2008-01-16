using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace CCNet.Gallio.WebDashboard.Plugin
{
    [ReflectorType("gallioAttachmentBuildAction")]
    public class GallioAttachmentBuildAction : ICruiseAction, IConditionalGetFingerprintProvider
    {
        private const string NamespaceUri = "http://www.gallio.org/";

        private readonly IBuildRetriever buildRetriever;
        private readonly IFingerprintFactory fingerprintFactory;

        public GallioAttachmentBuildAction(IBuildRetriever buildRetriever, IFingerprintFactory fingerprintFactory)
        {
            this.buildRetriever = buildRetriever;
            this.fingerprintFactory = fingerprintFactory;
        }

        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            string attachmentName = cruiseRequest.Request.GetText(@"name");
            if (attachmentName == null)
                throw new InvalidOperationException("Missing attachment name.");

            Build build = buildRetriever.GetBuild(cruiseRequest.BuildSpecifier);

            XPathDocument document = new XPathDocument(new StringReader(build.Log));
            XPathNavigator navigator = document.CreateNavigator();

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(navigator.NameTable);
            nsmgr.AddNamespace(@"g", NamespaceUri);

            foreach (XPathNavigator attachmentNavigator in navigator.Select(@"//g:report/g:packageRun/descendant::g:attachment", nsmgr))
            {
                if (attachmentNavigator.GetAttribute(@"name", NamespaceUri) == attachmentName)
                {
                    string contentDisposition = attachmentNavigator.GetAttribute(@"contentDisposition", NamespaceUri);
                    if (contentDisposition != @"inline")
                        throw new InvalidOperationException("The attachment is not available.");

                    string contentType = attachmentNavigator.GetAttribute(@"contentType", NamespaceUri);
                    string encoding = attachmentNavigator.GetAttribute(@"encoding", NamespaceUri);
                    string encodedContent = attachmentNavigator.Value;

                    byte[] content;
                    if (encoding == @"base64")
                    {
                        content = Convert.FromBase64String(encodedContent);
                    }
                    else
                    {
                        content = Encoding.UTF8.GetBytes(encodedContent);
                        contentType += @"; charset=utf-8";
                    }

                    return new TypedBinaryResponse(content, contentType);
                }
            }

            throw new InvalidOperationException("Invalid attachment name.");
        }

        public ConditionalGetFingerprint GetFingerprint(IRequest request)
        {
            return fingerprintFactory.BuildFromRequest(request);
        }
    }
}