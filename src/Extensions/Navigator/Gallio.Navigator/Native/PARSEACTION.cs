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
using System.Collections.Generic;
using System.Text;

namespace Gallio.Navigator.Native
{
    internal enum PARSEACTION
    {
        PARSE_CANONICALIZE = 1,
        PARSE_FRIENDLY,
        PARSE_SECURITY_URL,
        PARSE_ROOTDOCUMENT,
        PARSE_DOCUMENT,
        PARSE_ANCHOR,
        PARSE_ENCODE,
        PARSE_DECODE,
        PARSE_PATH_FROM_URL,
        PARSE_URL_FROM_PATH,
        PARSE_MIME,
        PARSE_SERVER,
        PARSE_SCHEMA,
        PARSE_SITE,
        PARSE_DOMAIN,
        PARSE_LOCATION,
        PARSE_SECURITY_DOMAIN,
        PARSE_ESCAPE,
        PARSE_UNESCAPE
    }
}
