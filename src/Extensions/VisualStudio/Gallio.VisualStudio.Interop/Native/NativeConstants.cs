// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

namespace Gallio.VisualStudio.Interop.Native
{
    internal class NativeConstants
    {
        public const int SW_RESTORE = 9;

        public const int S_OK = 0;
        public const int S_FALSE = 1;

        public const int E_FAIL = unchecked((int)0x80004005);
        public const int E_NOTIMPL = unchecked((int)0x80004001);
        public const int E_NOINTERFACE = unchecked((int)0x80004002);

        public const int SERVERCALL_ISHANDLED = 0;
        public const int SERVERCALL_RETRYLATER = 2;
        public const int PENDINGMSG_WAITDEFPROCESS = 2;
    }
}
