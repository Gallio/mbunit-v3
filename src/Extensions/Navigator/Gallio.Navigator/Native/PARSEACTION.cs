using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Navigator.Native
{
    public enum PARSEACTION
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
