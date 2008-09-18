using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Navigator.Native
{
    internal class Constants
    {
        public static readonly Guid IID_IDispatch = new Guid("{00020400-0000-0000-C000-000000000046}");
        public static readonly Guid IID_IDispatchEx = new Guid("{a6ef9860-c720-11d0-9337-00a0c90dcaa9}");
        public static readonly Guid IID_IPersistStorage = new Guid("{0000010A-0000-0000-C000-000000000046}");
        public static readonly Guid IID_IPersistStream = new Guid("{00000109-0000-0000-C000-000000000046}");
        public static readonly Guid IID_IPersistPropertyBag = new Guid("{37D84F60-42CB-11CE-8135-00AA004BB851}");

        public const int INTERFACESAFE_FOR_UNTRUSTED_CALLER = 0x00000001;
        public const int INTERFACESAFE_FOR_UNTRUSTED_DATA = 0x00000002;

        public const int INET_E_DEFAULT_ACTION = unchecked((int) 0x800C0011L);
        public const int INET_E_INVALID_URL = unchecked((int)0x800C0002L);
        public const int INET_E_DATA_NOT_AVAILABLE = unchecked((int)0x800C0007L);

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
