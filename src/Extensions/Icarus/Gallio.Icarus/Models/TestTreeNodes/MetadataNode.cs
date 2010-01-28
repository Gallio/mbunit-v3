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

namespace Gallio.Icarus.Models.TestTreeNodes
{
    public sealed class MetadataNode : TestTreeNode
    {
        private readonly string metadataType;

        public MetadataNode(string metadata, string metadataType)
            : base(metadata, metadata)
        {
            this.metadataType = metadataType;
            CheckState = System.Windows.Forms.CheckState.Checked;
        }

        public override string TestKind
        {
            get
            {
                return metadataType;
            }
        }
    }
}
