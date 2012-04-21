// Copyright 2012 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.IO;
using Gallio.Common.Markup;

namespace Gallio.Runner.Reports
{
    internal class AttachmentPathResolver : IAttachmentPathResolver
    {
        private readonly IReportContainer reportContainer;

        public AttachmentPathResolver(IReportContainer reportContainer)
        {
            this.reportContainer = reportContainer;
        }

        public string GetAttachmentPath(string testStepId, string attachmentName, string mimeType)
        {
            var path = BuildAttachmentPath(testStepId, attachmentName);
            return AddExtension(mimeType, path);
        }

        private static string AddExtension(string mimeType, string path)
        {
            var extension = MimeTypes.GetExtensionByMimeType(mimeType);
            if (extension != null)
                path += extension;
            return path;
        }

        private string BuildAttachmentPath(string testStepId, string attachmentName)
        {
            var attachmentFolder = Path.Combine(reportContainer.ReportName, reportContainer.EncodeFileName(testStepId));
            return Path.Combine(attachmentFolder, reportContainer.EncodeFileName(attachmentName));
        }
    }
}
