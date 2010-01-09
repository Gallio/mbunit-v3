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
    internal enum Uri_PROPERTY
    {
        Uri_PROPERTY_ABSOLUTE_URI = 0,
        Uri_PROPERTY_STRING_START = Uri_PROPERTY_ABSOLUTE_URI,
        Uri_PROPERTY_AUTHORITY = 1,
        Uri_PROPERTY_DISPLAY_URI = 2,
        Uri_PROPERTY_DOMAIN = 3,
        Uri_PROPERTY_EXTENSION = 4,
        Uri_PROPERTY_FRAGMENT = 5,
        Uri_PROPERTY_HOST = 6,
        Uri_PROPERTY_PASSWORD = 7,
        Uri_PROPERTY_PATH = 8,
        Uri_PROPERTY_PATH_AND_QUERY = 9,
        Uri_PROPERTY_QUERY = 10,
        Uri_PROPERTY_RAW_URI = 11,
        Uri_PROPERTY_SCHEME_NAME = 12,
        Uri_PROPERTY_USER_INFO = 13,
        Uri_PROPERTY_USER_NAME = 14,
        Uri_PROPERTY_STRING_LAST = Uri_PROPERTY_USER_NAME,
        Uri_PROPERTY_HOST_TYPE = 15,
        Uri_PROPERTY_DWORD_START = Uri_PROPERTY_HOST_TYPE,
        Uri_PROPERTY_PORT = 16,
        Uri_PROPERTY_SCHEME = 17,
        Uri_PROPERTY_ZONE = 18,
        Uri_PROPERTY_DWORD_LAST = Uri_PROPERTY_ZONE
    }
}
