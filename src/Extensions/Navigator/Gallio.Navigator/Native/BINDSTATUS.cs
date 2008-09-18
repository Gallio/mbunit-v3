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
    internal enum BINDSTATUS
    {
        BINDSTATUS_FINDINGRESOURCE,
        BINDSTATUS_CONNECTING,
        BINDSTATUS_REDIRECTING,
        BINDSTATUS_BEGINDOWNLOADDATA,
        BINDSTATUS_DOWNLOADINGDATA,
        BINDSTATUS_ENDDOWNLOADDATA,
        BINDSTATUS_BEGINDOWNLOADCOMPONENTS,
        BINDSTATUS_INSTALLINGCOMPONENTS,
        BINDSTATUS_ENDDOWNLOADCOMPONENTS,
        BINDSTATUS_USINGCACHEDCOPY,
        BINDSTATUS_SENDINGREQUEST,
        BINDSTATUS_CLASSIDAVAILABLE,
        BINDSTATUS_MIMETYPEAVAILABLE,
        BINDSTATUS_CACHEFILENAMEAVAILABLE,
        BINDSTATUS_BEGINSYNCOPERATION,
        BINDSTATUS_ENDSYNCOPERATION,
        BINDSTATUS_BEGINUPLOADDATA,
        BINDSTATUS_UPLOADINGDATA,
        BINDSTATUS_ENDUPLOADINGDATA,
        BINDSTATUS_PROTOCOLCLASSID,
        BINDSTATUS_ENCODING,
        BINDSTATUS_VERFIEDMIMETYPEAVAILABLE,
        BINDSTATUS_CLASSINSTALLLOCATION,
        BINDSTATUS_DECODING,
        BINDSTATUS_LOADINGMIMEHANDLER,
        BINDSTATUS_CONTENTDISPOSITIONATTACH,
        BINDSTATUS_FILTERREPORTMIMETYPE,
        BINDSTATUS_CLSIDCANINSTANTIATE,
        BINDSTATUS_IUNKNOWNAVAILABLE,
        BINDSTATUS_DIRECTBIND,
        BINDSTATUS_RAWMIMETYPE,
        BINDSTATUS_PROXYDETECTING,
        BINDSTATUS_ACCEPTRANGES
    }
}
