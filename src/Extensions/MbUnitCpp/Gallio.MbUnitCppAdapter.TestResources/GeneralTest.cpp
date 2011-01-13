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

TESTFIXTURE(General)
{
    TEST(Empty_should_pass)
    {
    }

	TEST(Framework_should_catch_and_report_unhandled_text_exception)
	{
		throw "It happens sometimes!";
	}

	class FooException
	{
	};

	TEST(Framework_should_catch_and_report_unhandled_custom_exception)
	{
		throw FooException();
	}

	void MyFunc(int value)
	{
		Assert.AreEqual(123, value);
		TestLog.WriteFormat("value = %d", value);
	}

	TEST(Assert_and_TestLog_are_accessible_from_outside_function)
	{
		MyFunc(123);
	}
}
