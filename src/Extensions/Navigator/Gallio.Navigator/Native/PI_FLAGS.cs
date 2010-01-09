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

using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Navigator.Native
{
    [Flags]
    internal enum PI_FLAGS : uint
    {
        PI_PARSE_URL = 0x00000001,
        PI_FILTER_MODE = 0x00000002,
        PI_FORCE_ASYNC = 0x00000004,
        PI_USE_WORKERTHREAD = 0x00000008,
        PI_MIMEVERIFICATION = 0x00000010,
        PI_CLSIDLOOKUP = 0x00000020,
        PI_DATAPROGRESS = 0x00000040,
        PI_SYNCHRONOUS = 0x00000080,
        PI_APARTMENTTHREADED = 0x00000100,
        PI_CLASSINSTALL = 0x00000200,
        PI_PASSONBINDCTX = 0x00002000,
        PI_NOMIMEHANDLER = 0x00008000,
        PI_LOADAPPDIRECT = 0x00004000,
        PD_FORCE_SWITCH = 0x00010000
    }
}
