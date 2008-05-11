// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Microsoft.VisualStudio.TestTools.Common;

namespace Gallio.MSTestRunner
{
    internal static class Guids
    {
        public const string MSTestRunnerPkgGuidString = "9e600ffc-344d-4e6f-89c0-ded6afb42459";
        public const string MSTestRunnerCmdSetGuidString = "8433bd03-19c1-4919-b7ba-9d13bb423b41";

        public const string GallioTestTypeGuidString = "F3589083-259C-4054-87F7-75CDAD4B08E5";
        public static readonly TestType GallioTestType = new TestType(new Guid(GallioTestTypeGuidString));
    };
}
