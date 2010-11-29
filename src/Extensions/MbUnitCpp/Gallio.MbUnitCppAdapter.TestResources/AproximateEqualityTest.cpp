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

TESTFIXTURE(AproximateEquality)
{
	TEST(Assert_AreApproximatelyEqual_higher_char_should_pass)
    {
		char expected = 'A';
		char actual = 'D';
        Assert.AreApproximatelyEqual(expected, actual, (char)5);
    }

    TEST(Assert_AreApproximatelyEqual_higher_char_should_fail)
    {
		char expected = 'A';
		char actual = 'D';
        Assert.AreApproximatelyEqual(expected, actual, (char)2);
    }

	TEST(Assert_AreApproximatelyEqual_higher_wchar_should_pass)
    {
		wchar_t expected = L'A';
		wchar_t actual = L'D';
        Assert.AreApproximatelyEqual(expected, actual, (wchar_t)5);
    }

    TEST(Assert_AreApproximatelyEqual_higher_wchar_should_fail)
    {
		wchar_t expected = L'A';
		wchar_t actual = L'D';
        Assert.AreApproximatelyEqual(expected, actual, (wchar_t)2);
    }

	TEST(Assert_AreApproximatelyEqual_higher_uchar_should_pass)
    {
		unsigned char expected = 50;
		unsigned char actual = 54;
        Assert.AreApproximatelyEqual(expected, actual, (unsigned char)5);
    }

    TEST(Assert_AreApproximatelyEqual_higher_uchar_should_fail)
    {
		unsigned char expected = 50;
		unsigned char actual = 54;
        Assert.AreApproximatelyEqual(expected, actual, (unsigned char)2);
    }

	TEST(Assert_AreApproximatelyEqual_higher_short_should_pass)
    {
		short expected = 1234;
		short actual = 1239;
        Assert.AreApproximatelyEqual(expected, actual, (short)6);
    }

    TEST(Assert_AreApproximatelyEqual_higher_short_should_fail)
    {
		short expected = 1234;
		short actual = 1239;
        Assert.AreApproximatelyEqual(expected, actual, (short)3);
    }

	TEST(Assert_AreApproximatelyEqual_higher_ushort_should_pass)
    {
		unsigned short expected = 1234;
		unsigned short actual = 1239;
        Assert.AreApproximatelyEqual(expected, actual, (unsigned short)6);
    }

    TEST(Assert_AreApproximatelyEqual_higher_ushort_should_fail)
    {
		unsigned short expected = 1234;
		unsigned short actual = 1239;
        Assert.AreApproximatelyEqual(expected, actual, (unsigned short)3);
    }

	TEST(Assert_AreApproximatelyEqual_higher_int_should_pass)
    {
		int expected = 123456;
		int actual = 123459;
        Assert.AreApproximatelyEqual(expected, actual, 5);
    }

    TEST(Assert_AreApproximatelyEqual_higher_int_should_fail)
    {
		int expected = 123456;
		int actual = 123459;
        Assert.AreApproximatelyEqual(expected, actual, 2);
    }

	TEST(Assert_AreApproximatelyEqual_higher_uint_should_pass)
    {
		unsigned int expected = 123456;
		unsigned int actual = 123459;
        Assert.AreApproximatelyEqual(expected, actual, 5u);
    }

    TEST(Assert_AreApproximatelyEqual_higher_uint_should_fail)
    {
		unsigned int expected = 123456;
		unsigned int actual = 123459;
        Assert.AreApproximatelyEqual(expected, actual, 2u);
    }

	TEST(Assert_AreApproximatelyEqual_higher_long_should_pass)
    {
		long expected = 123456;
		long actual = 123459;
        Assert.AreApproximatelyEqual(expected, actual, (long)5);
    }

    TEST(Assert_AreApproximatelyEqual_higher_long_should_fail)
    {
		long expected = 123456;
		long actual = 123459;
        Assert.AreApproximatelyEqual(expected, actual, (long)2);
    }

	TEST(Assert_AreApproximatelyEqual_higher_ulong_should_pass)
    {
		unsigned long expected = 123456;
		unsigned long actual = 123459;
        Assert.AreApproximatelyEqual(expected, actual, (unsigned long)5);
    }

    TEST(Assert_AreApproximatelyEqual_higher_ulong_should_fail)
    {
		unsigned long expected = 123456;
		unsigned long actual = 123459;
        Assert.AreApproximatelyEqual(expected, actual, (unsigned long)2);
    }

	TEST(Assert_AreApproximatelyEqual_higher_longlong_should_pass)
    {
		long long expected = 123456786;
		long long actual = 123456789;
        Assert.AreApproximatelyEqual(expected, actual, (long long)5);
    }

    TEST(Assert_AreApproximatelyEqual_higher_longlong_should_fail)
    {
		long long expected = 123456786;
		long long actual = 123456789;
        Assert.AreApproximatelyEqual(expected, actual, (long long)2);
    }

	TEST(Assert_AreApproximatelyEqual_higher_ulonglong_should_pass)
    {
		unsigned long long expected = 123456786;
		unsigned long long actual = 123456789;
        Assert.AreApproximatelyEqual(expected, actual, (unsigned long long)5);
    }

    TEST(Assert_AreApproximatelyEqual_higher_ulonglong_should_fail)
    {
		unsigned long long expected = 123456786;
		unsigned long long actual = 123456788;
        Assert.AreApproximatelyEqual(expected, actual, (unsigned long long)2);
    }

	TEST(Assert_AreApproximatelyEqual_higher_float_should_pass)
    {
		float expected = 123.456f;
		float actual = 123.457f;
        Assert.AreApproximatelyEqual(expected, actual, 0.001f);
    }

    TEST(Assert_AreApproximatelyEqual_higher_float_should_fail)
    {
		float expected = 123.456f;
		float actual = 123.457f;
        Assert.AreApproximatelyEqual(expected, actual, 0.0005f);
    }

	TEST(Assert_AreApproximatelyEqual_higher_double_should_pass)
    {
		double expected = 123.456;
		double actual = 123.457;
        Assert.AreApproximatelyEqual(expected, actual, 0.001);
    }

    TEST(Assert_AreApproximatelyEqual_higher_double_should_fail)
    {
		double expected = 123.456;
		double actual = 123.457;
        Assert.AreApproximatelyEqual(expected, actual, 0.0005);
    }

	TEST(Assert_AreApproximatelyEqual_lower_char_should_pass)
    {
		char actual = 'A';
		char expected = 'D';
        Assert.AreApproximatelyEqual(expected, actual, (char)5);
    }

    TEST(Assert_AreApproximatelyEqual_lower_char_should_fail)
    {
		char actual = 'A';
		char expected = 'D';
        Assert.AreApproximatelyEqual(expected, actual, (char)2);
    }

	TEST(Assert_AreApproximatelyEqual_lower_wchar_should_pass)
    {
		wchar_t actual = L'A';
		wchar_t expected = L'D';
        Assert.AreApproximatelyEqual(expected, actual, (wchar_t)5);
    }

    TEST(Assert_AreApproximatelyEqual_lower_wchar_should_fail)
    {
		wchar_t actual = L'A';
		wchar_t expected = L'D';
        Assert.AreApproximatelyEqual(expected, actual, (wchar_t)2);
    }

	TEST(Assert_AreApproximatelyEqual_lower_uchar_should_pass)
    {
		unsigned char actual = 50;
		unsigned char expected = 54;
        Assert.AreApproximatelyEqual(expected, actual, (unsigned char)5);
    }

    TEST(Assert_AreApproximatelyEqual_lower_uchar_should_fail)
    {
		unsigned char actual = 50;
		unsigned char expected = 54;
        Assert.AreApproximatelyEqual(expected, actual, (unsigned char)2);
    }

	TEST(Assert_AreApproximatelyEqual_lower_short_should_pass)
    {
		short actual = 1234;
		short expected = 1239;
        Assert.AreApproximatelyEqual(expected, actual, (short)6);
    }

    TEST(Assert_AreApproximatelyEqual_lower_short_should_fail)
    {
		short actual = 1234;
		short expected = 1239;
        Assert.AreApproximatelyEqual(expected, actual, (short)3);
    }

	TEST(Assert_AreApproximatelyEqual_lower_ushort_should_pass)
    {
		unsigned short actual = 1234;
		unsigned short expected = 1239;
        Assert.AreApproximatelyEqual(expected, actual, (unsigned short)6);
    }

    TEST(Assert_AreApproximatelyEqual_lower_ushort_should_fail)
    {
		unsigned short actual = 1234;
		unsigned short expected = 1239;
        Assert.AreApproximatelyEqual(expected, actual, (unsigned short)3);
    }

	TEST(Assert_AreApproximatelyEqual_lower_int_should_pass)
    {
		int actual = 123456;
		int expected = 123459;
        Assert.AreApproximatelyEqual(expected, actual, 5);
    }

    TEST(Assert_AreApproximatelyEqual_lower_int_should_fail)
    {
		int actual = 123456;
		int expected = 123459;
        Assert.AreApproximatelyEqual(expected, actual, 2);
    }

	TEST(Assert_AreApproximatelyEqual_lower_uint_should_pass)
    {
		unsigned int actual = 123456;
		unsigned int expected = 123459;
        Assert.AreApproximatelyEqual(expected, actual, 5u);
    }

    TEST(Assert_AreApproximatelyEqual_lower_uint_should_fail)
    {
		unsigned int actual = 123456;
		unsigned int expected = 123459;
        Assert.AreApproximatelyEqual(expected, actual, 2u);
    }

	TEST(Assert_AreApproximatelyEqual_lower_long_should_pass)
    {
		long actual = 123456;
		long expected = 123459;
        Assert.AreApproximatelyEqual(expected, actual, (long)5);
    }

    TEST(Assert_AreApproximatelyEqual_lower_long_should_fail)
    {
		long actual = 123456;
		long expected = 123459;
        Assert.AreApproximatelyEqual(expected, actual, (long)2);
    }

	TEST(Assert_AreApproximatelyEqual_lower_ulong_should_pass)
    {
		unsigned long actual = 123456;
		unsigned long expected = 123459;
        Assert.AreApproximatelyEqual(expected, actual, (unsigned long)5);
    }

    TEST(Assert_AreApproximatelyEqual_lower_ulong_should_fail)
    {
		unsigned long actual = 123456;
		unsigned long expected = 123459;
        Assert.AreApproximatelyEqual(expected, actual, (unsigned long)2);
    }

	TEST(Assert_AreApproximatelyEqual_lower_longlong_should_pass)
    {
		long long actual = 123456786;
		long long expected = 123456789;
        Assert.AreApproximatelyEqual(expected, actual, (long long)5);
    }

    TEST(Assert_AreApproximatelyEqual_lower_longlong_should_fail)
    {
		long long actual = 123456786;
		long long expected = 123456789;
        Assert.AreApproximatelyEqual(expected, actual, (long long)2);
    }

	TEST(Assert_AreApproximatelyEqual_lower_ulonglong_should_pass)
    {
		unsigned long long actual = 123456786;
		unsigned long long expected = 123456789;
        Assert.AreApproximatelyEqual(expected, actual, (unsigned long long)5);
    }

    TEST(Assert_AreApproximatelyEqual_lower_ulonglong_should_fail)
    {
		unsigned long long actual = 123456786;
		unsigned long long expected = 123456788;
        Assert.AreApproximatelyEqual(expected, actual, (unsigned long long)2);
    }

	TEST(Assert_AreApproximatelyEqual_lower_float_should_pass)
    {
		float actual = 123.456f;
		float expected = 123.457f;
        Assert.AreApproximatelyEqual(expected, actual, 0.001f);
    }

    TEST(Assert_AreApproximatelyEqual_lower_float_should_fail)
    {
		float actual = 123.456f;
		float expected = 123.457f;
        Assert.AreApproximatelyEqual(expected, actual, 0.0005f);
    }

	TEST(Assert_AreApproximatelyEqual_lower_double_should_pass)
    {
		double actual = 123.456;
		double expected = 123.457;
        Assert.AreApproximatelyEqual(expected, actual, 0.001);
    }

    TEST(Assert_AreApproximatelyEqual_lower_double_should_fail)
    {
		double actual = 123.456;
		double expected = 123.457;
        Assert.AreApproximatelyEqual(expected, actual, 0.0005);
    }
}