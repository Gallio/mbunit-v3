using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Navigator.Native
{
    public enum QUERYOPTION
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
