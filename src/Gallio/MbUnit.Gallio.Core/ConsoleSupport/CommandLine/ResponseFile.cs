// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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

using System.Collections.Generic;
using MbUnit.Core.Utilities;

namespace MbUnit.Core.ConsoleSupport.CommandLine
{
    /// <summary>
    /// Responsible for parsing a respond file.
    /// </summary>
    public class ResponseFile
    {
        public readonly IFileManager FileManager;
        public readonly List<string> Arguments;

        /// <summary>
        /// Creates an instance of ResponseFile
        /// </summary>
        /// <param name="fileName">File name that contents attributes.</param>
        public ResponseFile(string fileName) : this(fileName, new FileManager())
        {}

        /// <summary>
        /// Creates an instance of ResponseFile
        /// </summary>
        /// <param name="fileName">File name that contents attributes.</param>
        /// <param name="fileManager">Object responsible for file processing.</param>
        public ResponseFile(string fileName, IFileManager fileManager)
        {
            Arguments = new List<string>();
            FileManager = fileManager;
        }

        ///<summary>
        /// Returns true if there's no Arguments; otherwise, return false.
        ///</summary>
        public object IsEmpty
        {
            get { return Arguments.Count == 0; }
        }
    }
}