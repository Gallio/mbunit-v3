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

#include "stdafx.h"
#include "mbunit.h"

TESTFIXTURE(DataDriven)
{
	DATA(First, int x)
	{
		ROW(123)
		ROW(456, "Optional description of the data row")
		ROW(789)
	}

    TEST(BoundToFirst, BIND(First, Row))
    {
		TestLog.WriteLineFormat(L"x = %d, dataRowDescription = '%s'", Row.x, Row.dataRowDescription.GetBuffer());
    }

	DATA(Second, int i, const wchar_t* text, double d)
	{
		ROW(0, L"Red", 3.14159)
		ROW(1, L"Green", 1.41421)
		ROW(2, L"Blue", 2.71828, "This data row is blue")
	}

    TEST(BoundToSecond, BIND(Second, Row))
    {
		TestLog.WriteLineFormat(L"i = %d, text = %s, d = %.5lf, dataRowDescription = '%s'", Row.i, Row.text, Row.d, Row.dataRowDescription.GetBuffer());
    }
}
