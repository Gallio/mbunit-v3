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
#include "MbUnitCpp.h"

TESTFIXTURE(Equality)
{
    TEST(Assert_AreEqual_bool_should_pass)
    {
		bool expected = true;
		bool actual = true;
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_bool_should_fail)
    {
		bool expected = true;
		bool actual = false;
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_char_should_pass)
    {
		char expected = 'A';
		char actual = 'A';
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_char_should_fail)
    {
		char expected = 'A';
		char actual = 'B';
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_wchar_should_pass)
    {
		__wchar_t expected = L'A';
		__wchar_t actual = L'A';
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_wchar_should_fail)
    {
		__wchar_t expected = L'A';
		__wchar_t actual = L'B';
        Assert.AreEqual(expected, actual);
    }

	TEST(Assert_AreEqual_uchar_should_pass)
    {
		unsigned char expected = 55;
		unsigned char actual = 55;
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_uchar_should_fail)
    {
		unsigned char expected = 55;
		unsigned char actual = 66;
        Assert.AreEqual(expected, actual);
    }

	TEST(Assert_AreEqual_short_should_pass)
    {
		short expected = 123;
		short actual = 123;
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_short_should_fail)
    {
		short expected = 123;
		short actual = 456;
        Assert.AreEqual(expected, actual);
    }

	TEST(Assert_AreEqual_ushort_should_pass)
    {
		short expected = 123;
		short actual = 123;
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_ushort_should_fail)
    {
		short expected = 123;
		short actual = 456;
        Assert.AreEqual(expected, actual);
    }

	TEST(Assert_AreEqual_int_should_pass)
    {
		int expected = 123;
		int actual = 123;
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_int_should_fail)
    {
		int expected = 123;
		int actual = 456;
        Assert.AreEqual(expected, actual);
    }

	TEST(Assert_AreEqual_uint_should_pass)
    {
		int expected = 123;
		int actual = 123;
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_uint_should_fail)
    {
		int expected = 123;
		int actual = 456;
        Assert.AreEqual(expected, actual);
    }

	TEST(Assert_AreEqual_longlong_should_pass)
    {
		int expected = 123;
		int actual = 123;
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_longlong_should_fail)
    {
		int expected = 123;
		int actual = 456;
        Assert.AreEqual(expected, actual);
    }

	TEST(Assert_AreEqual_float_should_pass)
    {
		float expected = 123.456f;
		float actual = 123.456f;
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_float_should_fail)
    {
		float expected = 123.456f;
		float actual = 456.789f;
        Assert.AreEqual(expected, actual);
    }

	TEST(Assert_AreEqual_double_should_pass)
    {
		double expected = 123.456;
		double actual = 123.456;
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_double_should_fail)
    {
		double expected = 123.456;
		double actual = 456.789;
        Assert.AreEqual(expected, actual);
    }

	TEST(Assert_AreEqual_char_pointer_should_pass)
    {
		char* expected = "Hello World";
		char* actual = "Hello World";
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_char_pointer_should_fail)
    {
		char* expected = "Hello World";
		char* actual = "Crazy World";
        Assert.AreEqual(expected, actual);
    }

	TEST(Assert_AreEqual_constant_char_pointer_should_pass)
    {
		const char* expected = "Hello World";
		const char* actual = "Hello World";
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_constant_char_pointer_should_fail)
    {
		const char* expected = "Hello World";
		const char* actual = "Crazy World";
        Assert.AreEqual(expected, actual);
    }

	TEST(Assert_AreEqual_wide_char_pointer_should_pass)
    {
		wchar_t* expected = L"Hello World";
		wchar_t* actual = L"Hello World";
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_wide_char_pointer_should_fail)
    {
		wchar_t* expected = L"Hello World";
		wchar_t* actual = L"Crazy World";
        Assert.AreEqual(expected, actual);
    }

	TEST(Assert_AreEqual_constant_wide_char_pointer_should_pass)
    {
		const wchar_t* expected = L"Hello World";
		const wchar_t* actual = L"Hello World";
        Assert.AreEqual(expected, actual);
    }

    TEST(Assert_AreEqual_constant_wide_char_pointer_should_fail)
    {
		const wchar_t* expected = L"Hello World";
		const wchar_t* actual = L"Crazy World";
        Assert.AreEqual(expected, actual);
    }
}

