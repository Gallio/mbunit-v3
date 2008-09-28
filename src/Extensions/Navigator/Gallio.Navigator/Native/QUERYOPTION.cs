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
    internal enum QUERYOPTION
    {
        QUERY_EXPIRATION_DATE = 1,
        QUERY_TIME_OF_LAST_CHANGE,
        QUERY_CONTENT_ENCODING,
        QUERY_CONTENT_TYPE,
        QUERY_REFRESH,
        QUERY_RECOMBINE,
        QUERY_CAN_NAVIGATE,
        QUERY_USES_NETWORK,
        QUERY_IS_CACHED,
        QUERY_IS_INSTALLEDENTRY,
        QUERY_IS_CACHED_OR_MAPPED,
        QUERY_USES_CACHE,
        QUERY_IS_SECURE,
        QUERY_IS_SAFE
    }
}
