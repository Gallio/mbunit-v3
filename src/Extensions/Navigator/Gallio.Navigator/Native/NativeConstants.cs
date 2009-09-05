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

namespace Gallio.Navigator.Native
{
    internal static class NativeConstants
    {
        public static readonly Guid IID_IDispatch = new Guid("{00020400-0000-0000-C000-000000000046}");
        public static readonly Guid IID_IDispatchEx = new Guid("{a6ef9860-c720-11d0-9337-00a0c90dcaa9}");
        public static readonly Guid IID_IPersistStorage = new Guid("{0000010A-0000-0000-C000-000000000046}");
        public static readonly Guid IID_IPersistStream = new Guid("{00000109-0000-0000-C000-000000000046}");
        public static readonly Guid IID_IPersistPropertyBag = new Guid("{37D84F60-42CB-11CE-8135-00AA004BB851}");

        public const int INTERFACESAFE_FOR_UNTRUSTED_CALLER = 0x00000001;
        public const int INTERFACESAFE_FOR_UNTRUSTED_DATA = 0x00000002;

        public const int INET_E_DEFAULT_ACTION = unchecked((int) 0x800C0011);
        public const int INET_E_INVALID_URL = unchecked((int)0x800C0002);
        public const int INET_E_DATA_NOT_AVAILABLE = unchecked((int)0x800C0007);

        public const int STG_E_FILENOTFOUND = unchecked((int)0x80030002);

        public const int S_OK = 0;
        public const int S_FALSE = 1;

        public const int E_PENDING = unchecked((int)0x8000000A);
        public const int E_FAIL = unchecked((int)0x80004005);
        public const int E_NOTIMPL = unchecked((int)0x80004001);
        public const int E_NOINTERFACE = unchecked((int)0x80004002);

        public const int MAX_PATH = 260;

        public const int NORMAL_CACHE_ENTRY = 1;
    }
}
