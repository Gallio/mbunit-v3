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

TESTFIXTURE(ExpectedException)
{
    TEST(ExpectedException_should_pass_when_exception_is_actually_thrown, EXPECTED_EXCEPTION(char*))
    {
		throw "This expection is expected.";
    }

	TEST(ExpectedException_should_fail_when_exception_is_not_thrown, EXPECTED_EXCEPTION(char*))
	{
	}

	TEST(ExpectedException_should_fail_when_exception_of_a_wrong_type_is_thrown, EXPECTED_EXCEPTION(char*))
	{
		throw 123;
	}
}
