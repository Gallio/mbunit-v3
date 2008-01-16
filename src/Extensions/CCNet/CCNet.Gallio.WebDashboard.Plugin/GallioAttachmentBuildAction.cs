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
            string stepId = cruiseRequest.Request.GetText(@"testStepId");
            if (stepId.Length == 0)
                throw new InvalidOperationException("Missing test step id.");

            string attachmentName = cruiseRequest.Request.GetText(@"attachmentName");
            if (attachmentName.Length == 0)
                throw new InvalidOperationException("Missing attachment name.");

            Build build = buildRetriever.GetBuild(cruiseRequest.BuildSpecifier);

            XPathDocument document = new XPathDocument(new StringReader(build.Log));
            XPathNavigator rootNavigator = document.CreateNavigator();

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(rootNavigator.NameTable);
            nsmgr.AddNamespace(@"g", NamespaceUri);

            XPathNavigator testStepNavigator = FindTestStepNode(rootNavigator, nsmgr, stepId);
            XPathNavigator attachmentNavigator = FindAttachmentNode(testStepNavigator, nsmgr, attachmentName);
            return CreateResponseFromAttachment(attachmentNavigator);
        }

        public ConditionalGetFingerprint GetFingerprint(IRequest request)
        {
            return fingerprintFactory.BuildFromRequest(request);
        }

        private static XPathNavigator FindTestStepNode(XPathNavigator rootNavigator, IXmlNamespaceResolver resolver, string stepId)
        {
            foreach (XPathNavigator testStepNavigator in rootNavigator.Select(@"//g:report/g:packageRun/descendant::g:testStepRun/g:testStep", resolver))
            {
                if (testStepNavigator.GetAttribute(@"id", "") == stepId)
                    return testStepNavigator;
            }

            throw new InvalidOperationException("The step id is not valid.");
        }

        private static XPathNavigator FindAttachmentNode(XPathNavigator testStepNavigator, IXmlNamespaceResolver resolver, string attachmentName)
        {
            foreach (XPathNavigator attachmentNavigator in testStepNavigator.Select(@"../g:executionLog/g:attachments/g:attachment", resolver))
            {
                if (attachmentNavigator.GetAttribute(@"name", "") == attachmentName)
                    return attachmentNavigator;
            }

            throw new InvalidOperationException("The attachment name is not valid.");
        }

        private static IResponse CreateResponseFromAttachment(XPathNavigator attachmentNavigator)
        {
            string contentDisposition = attachmentNavigator.GetAttribute(@"contentDisposition", "");
            if (contentDisposition != @"inline")
                throw new InvalidOperationException("The attachment was not inlined into the XML report.");

            string contentType = attachmentNavigator.GetAttribute(@"contentType", "");
            string encoding = attachmentNavigator.GetAttribute(@"encoding", "");
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
}