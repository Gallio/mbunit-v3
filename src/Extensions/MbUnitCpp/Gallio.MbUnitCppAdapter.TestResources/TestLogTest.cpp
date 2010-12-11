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

TESTFIXTURE(TestLog)
{
    TEST(Write_once)
    {
		TestLog.Write("Wolf zombies quickly spot the jinxed grave.");
    }

    TEST(Write_multiple_times)
    {
		TestLog.Write("Wolf zombies ");
		TestLog.Write("quickly spot ");
		TestLog.Write("the jinxed grave.");
    }

    TEST(WriteFormat)
    {
		TestLog.WriteFormat("Wolf zombies %s spot the %s grave.", "quickly", "jinxed");
    }

    TEST(WriteLine_once)
    {
		TestLog.WriteLine("Wolf zombies quickly spot the jinxed grave.");
    }

    TEST(WriteLine_multiple_times)
    {
		TestLog.WriteLine("Wolf zombies ");
		TestLog.WriteLine("quickly spot ");
		TestLog.WriteLine("the jinxed grave.");
    }

    TEST(WriteLineFormat)
    {
		TestLog.WriteLineFormat("Wolf zombies %s spot the %s grave.", "quickly", "jinxed");
    }

    TEST(Write_once_wide)
    {
		TestLog.Write(L"Wolf zombies quickly spot the jinxed grave.");
    }

    TEST(Write_multiple_times_wide)
    {
		TestLog.Write(L"Wolf zombies ");
		TestLog.Write(L"quickly spot ");
		TestLog.Write(L"the jinxed grave.");
    }

    TEST(WriteFormat_wide)
    {
		TestLog.WriteFormat(L"Wolf zombies %s spot the %s grave.", L"quickly", L"jinxed");
    }

    TEST(WriteLine_once_wide)
    {
		TestLog.WriteLine(L"Wolf zombies quickly spot the jinxed grave.");
    }

    TEST(WriteLine_multiple_times_wide)
    {
		TestLog.WriteLine(L"Wolf zombies ");
		TestLog.WriteLine(L"quickly spot ");
		TestLog.WriteLine(L"the jinxed grave.");
    }

    TEST(WriteLineFormat_wide)
    {
		TestLog.WriteLineFormat(L"Wolf zombies %s spot the %s grave.", L"quickly", L"jinxed");
    }

    TEST(Write_once_String)
    {
		TestLog.Write(mbunit::String("Wolf zombies quickly spot the jinxed grave."));
    }

    TEST(Write_multiple_times_String)
    {
		TestLog.Write(mbunit::String("Wolf zombies "));
		TestLog.Write(mbunit::String("quickly spot "));
		TestLog.Write(mbunit::String("the jinxed grave."));
    }

    TEST(WriteLine_once_String)
    {
		TestLog.WriteLine(mbunit::String("Wolf zombies quickly spot the jinxed grave."));
    }

    TEST(WriteLine_multiple_times_String)
    {
		TestLog.WriteLine(mbunit::String("Wolf zombies "));
		TestLog.WriteLine(mbunit::String("quickly spot "));
		TestLog.WriteLine(mbunit::String("the jinxed grave."));
    }
}
