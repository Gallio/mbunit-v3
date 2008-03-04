// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
