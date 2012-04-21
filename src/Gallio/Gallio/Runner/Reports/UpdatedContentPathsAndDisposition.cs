// Copyright 2012 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Markup;
using Gallio.Runner.Reports.Schema;

namespace Gallio.Runner.Reports
{
    internal class UpdatedContentPathsAndDisposition : IDisposable
    {
        private readonly IAttachmentPathResolver attachmentPathResolver;
        private readonly AttachmentContentDisposition attachmentContentDisposition;
        private readonly TestPackageRun testPackageRun;
        private readonly Dictionary<AttachmentData, KeyValuePair<AttachmentContentDisposition, string>> originalAttachmentData = new Dictionary<AttachmentData, KeyValuePair<AttachmentContentDisposition, string>>();

        public UpdatedContentPathsAndDisposition(IAttachmentPathResolver attachmentPathResolver, AttachmentContentDisposition attachmentContentDisposition, TestPackageRun testPackageRun)
        {
            this.attachmentPathResolver = attachmentPathResolver;
            this.attachmentContentDisposition = attachmentContentDisposition;
            this.testPackageRun = testPackageRun;

            UpdateContentPathsAndDispositions();
        }

        private void UpdateContentPathsAndDispositions()
        {
            if (testPackageRun == null)
                return;

            foreach (var testStepRun in testPackageRun.AllTestStepRuns)
            {
                foreach (var attachment in testStepRun.TestLog.Attachments)
                {
                    originalAttachmentData.Add(attachment, new KeyValuePair<AttachmentContentDisposition, string>(attachment.ContentDisposition, attachment.ContentPath));

                    var attachmentPath = attachmentPathResolver.GetAttachmentPath(testStepRun.Step.Id, attachment.Name, attachment.ContentType);

                    attachment.ContentDisposition = attachmentContentDisposition;
                    attachment.ContentPath = attachmentPath;
                }
            }
        }

        public void Dispose()
        {
            foreach (var pair in originalAttachmentData)
            {
                pair.Key.ContentDisposition = pair.Value.Key;
                pair.Key.ContentPath = pair.Value.Value;
            }
        }
    }
}
