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
using System.Collections.Generic;
using System.Text;

namespace Gallio.Navigator.Native
{
    internal enum BINDF
    {
        BINDF_ASYNCHRONOUS = 0x00000001,
        BINDF_ASYNCSTORAGE = 0x00000002,
        BINDF_NOPROGRESSIVERENDERING = 0x00000004,
        BINDF_OFFLINEOPERATION = 0x00000008,
        BINDF_GETNEWESTVERSION = 0x00000010,
        BINDF_NOWRITECACHE = 0x00000020,
        BINDF_NEEDFILE = 0x00000040,
        BINDF_PULLDATA = 0x00000080,
        BINDF_IGNORESECURITYPROBLEM = 0x00000100,
        BINDF_RESYNCHRONIZE = 0x00000200,
        BINDF_HYPERLINK = 0x00000400,
        BINDF_NO_UI = 0x00000800,
        BINDF_SILENTOPERATION = 0x00001000,
        BINDF_PRAGMA_NO_CACHE = 0x00002000,
        BINDF_GETCLASSOBJECT = 0x00004000,
        BINDF_RESERVED_1 = 0x00008000,
        BINDF_FREE_THREADED = 0x00010000,
        BINDF_DIRECT_READ = 0x00020000,
        BINDF_FORMS_SUBMIT = 0x00040000,
        BINDF_GETFROMCACHE_IF_NET_FAIL = 0x00080000,
        BINDF_FROMURLMON = 0x00100000,
        BINDF_FWD_BACK = 0x00200000,
        BINDF_PREFERDEFAULTHANDLER = 0x00400000,
        BINDF_RESERVED_3 = 0x00800000
    }
}
