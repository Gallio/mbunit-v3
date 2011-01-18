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
#include "Model.h"

// Extends Assert.AreEqual to support Foo equality tests.
template<> void mbunit::AssertionFramework::AreEqual<Foo>(Foo expectedValue, Foo actualValue, const mbunit::String& message)
{
	controller->IncrementAssertCount();

    if (expectedValue.GetValue() != actualValue.GetValue())
	{
        throw AssertionFailureBuilder("Expected values to be equal.")
            .Expected(String::Format("Foo: value = %d", expectedValue.GetValue()))
            .Actual(String::Format("Foo: value = %d", actualValue.GetValue()))
            .Message(message)
            .ToAssertionFailure(controller);
	}
}

// Extends Assert.AreNotEqual to support Foo inequality tests.
template<> void mbunit::AssertionFramework::AreNotEqual<Foo>(Foo unexpectedValue, Foo actualValue, const mbunit::String& message)
{
	controller->IncrementAssertCount();

    if (unexpectedValue.GetValue() == actualValue.GetValue())
	{
        throw AssertionFailureBuilder("Expected values to be non-equal.")
            .Unexpected(String::Format("Foo: value = %d", unexpectedValue.GetValue()))
            .Actual(String::Format("Foo: value = %d", actualValue.GetValue()))
            .Message(message)
            .ToAssertionFailure(controller);
	}
}

TESTFIXTURE(Extensibility)
{
    TEST(Foo_equality_should_pass)
    {
        const Foo foo1(123);
        const Foo foo2(123);
        Assert.AreEqual(foo1, foo2);
    }

    TEST(Foo_equality_should_fail)
    {
        const Foo foo1(123);
        const Foo foo2(456);
        Assert.AreEqual(foo1, foo2);
    }

    TEST(Foo_inequality_should_pass)
    {
        Foo foo1(123);
        Foo foo2(456);
        Assert.AreNotEqual(foo1, foo2);
    }

    TEST(Foo_inequality_should_fail)
    {
        Foo foo1(123);
        Foo foo2(123);
        Assert.AreNotEqual(foo1, foo2);
    }
}
