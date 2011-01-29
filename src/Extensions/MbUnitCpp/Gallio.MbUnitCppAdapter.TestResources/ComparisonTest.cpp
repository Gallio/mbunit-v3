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

TESTFIXTURE(Comparison)
{
    TEST(Assert_GreaterThan_with_greater_char_should_pass)
    {
		char left = 'F';
		char right = 'E';
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_equal_char_should_fail)
    {
		char left = 'E';
		char right = 'E';
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_less_char_should_fail)
    {
		char left = 'D';
		char right = 'E';
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_greater_wide_char_should_pass)
    {
		wchar_t left = L'F';
		wchar_t right = L'E';
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_equal_wide_char_should_fail)
    {
		wchar_t left = L'E';
		wchar_t right = L'E';
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_less_wide_char_should_fail)
    {
		wchar_t left = L'D';
		wchar_t right = L'E';
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_greater_unsigned_char_should_pass)
    {
		unsigned char left = 56;
		unsigned char right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_equal_unsigned_char_should_fail)
    {
		unsigned char left = 55;
		unsigned char right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_less_unsigned_char_should_fail)
    {
		unsigned char left = 54;
		unsigned char right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_greater_short_should_pass)
    {
		short left = 56;
		short right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_equal_short_should_fail)
    {
		short left = 55;
		short right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_less_short_should_fail)
    {
		short left = 54;
		short right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_greater_unsigned_short_should_pass)
    {
		unsigned short left = 56;
		unsigned short right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_equal_unsigned_short_should_fail)
    {
		unsigned short left = 55;
		unsigned short right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_less_unsigned_short_should_fail)
    {
		unsigned short left = 54;
		unsigned short right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_greater_int_should_pass)
    {
		int left = 56;
		int right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_equal_int_should_fail)
    {
		int left = 55;
		int right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_less_int_should_fail)
    {
		int left = 54;
		int right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_greater_unsigned_int_should_pass)
    {
		unsigned int left = 56;
		unsigned int right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_equal_unsigned_int_should_fail)
    {
		unsigned int left = 55;
		unsigned int right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_less_unsigned_int_should_fail)
    {
		unsigned int left = 54;
		unsigned int right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_greater_long_should_pass)
    {
		long left = 56;
		long right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_equal_long_should_fail)
    {
		long left = 55;
		long right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_less_long_should_fail)
    {
		long left = 54;
		long right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_greater_unsigned_long_should_pass)
    {
		unsigned long left = 56;
		unsigned long right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_equal_unsigned_long_should_fail)
    {
		unsigned long left = 55;
		unsigned long right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_less_unsigned_long_should_fail)
    {
		unsigned long left = 54;
		unsigned long right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_greater_long_long_should_pass)
    {
		long long left = 56;
		long long right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_equal_long_long_should_fail)
    {
		long long left = 55;
		long long right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_less_long_long_should_fail)
    {
		long long left = 54;
		long long right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_greater_unsigned_long_long_should_pass)
    {
		unsigned long long left = 56;
		unsigned long long right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_equal_unsigned_long_long_should_fail)
    {
		unsigned long long left = 55;
		unsigned long long right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_less_unsigned_long_long_should_fail)
    {
		unsigned long long left = 54;
		unsigned long long right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_greater_float_should_pass)
    {
		float left = 56;
		float right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_equal_float_should_fail)
    {
		float left = 55;
		float right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_less_float_should_fail)
    {
		float left = 54;
		float right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_greater_double_should_pass)
    {
		double left = 56;
		double right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_equal_double_should_fail)
    {
		double left = 55;
		double right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThan_with_less_double_should_fail)
    {
		double left = 54;
		double right = 55;
        Assert.GreaterThan(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_greater_char_should_pass)
    {
		char left = 'F';
		char right = 'E';
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_equal_char_should_pass)
    {
		char left = 'E';
		char right = 'E';
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_less_char_should_fail)
    {
		char left = 'D';
		char right = 'E';
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_greater_wide_char_should_pass)
    {
		wchar_t left = L'F';
		wchar_t right = L'E';
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_equal_wide_char_should_pass)
    {
		wchar_t left = L'E';
		wchar_t right = L'E';
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_less_wide_char_should_fail)
    {
		wchar_t left = L'D';
		wchar_t right = L'E';
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_greater_unsigned_char_should_pass)
    {
		unsigned char left = 56;
		unsigned char right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_equal_unsigned_char_should_pass)
    {
		unsigned char left = 55;
		unsigned char right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_less_unsigned_char_should_fail)
    {
		unsigned char left = 54;
		unsigned char right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_greater_short_should_pass)
    {
		short left = 56;
		short right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_equal_short_should_pass)
    {
		short left = 55;
		short right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_less_short_should_fail)
    {
		short left = 54;
		short right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_greater_unsigned_short_should_pass)
    {
		unsigned short left = 56;
		unsigned short right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_equal_unsigned_short_should_pass)
    {
		unsigned short left = 55;
		unsigned short right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_less_unsigned_short_should_fail)
    {
		unsigned short left = 54;
		unsigned short right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_greater_int_should_pass)
    {
		int left = 56;
		int right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_equal_int_should_pass)
    {
		int left = 55;
		int right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_less_int_should_fail)
    {
		int left = 54;
		int right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_greater_unsigned_int_should_pass)
    {
		unsigned int left = 56;
		unsigned int right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_equal_unsigned_int_should_pass)
    {
		unsigned int left = 55;
		unsigned int right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_less_unsigned_int_should_fail)
    {
		unsigned int left = 54;
		unsigned int right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_greater_long_should_pass)
    {
		long left = 56;
		long right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_equal_long_should_pass)
    {
		long left = 55;
		long right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_less_long_should_fail)
    {
		long left = 54;
		long right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_greater_unsigned_long_should_pass)
    {
		unsigned long left = 56;
		unsigned long right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_equal_unsigned_long_should_pass)
    {
		unsigned long left = 55;
		unsigned long right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_less_unsigned_long_should_fail)
    {
		unsigned long left = 54;
		unsigned long right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_greater_long_long_should_pass)
    {
		long long left = 56;
		long long right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_equal_long_long_should_pass)
    {
		long long left = 55;
		long long right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_less_long_long_should_fail)
    {
		long long left = 54;
		long long right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_greater_unsigned_long_long_should_pass)
    {
		unsigned long long left = 56;
		unsigned long long right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_equal_unsigned_long_long_should_pass)
    {
		unsigned long long left = 55;
		unsigned long long right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_less_unsigned_long_long_should_fail)
    {
		unsigned long long left = 54;
		unsigned long long right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_greater_float_should_pass)
    {
		float left = 56;
		float right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_equal_float_should_pass)
    {
		float left = 55;
		float right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_less_float_should_fail)
    {
		float left = 54;
		float right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_greater_double_should_pass)
    {
		double left = 56;
		double right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_equal_double_should_pass)
    {
		double left = 55;
		double right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
    }

    TEST(Assert_GreaterThanOrEqualTo_with_less_double_should_fail)
    {
		double left = 54;
		double right = 55;
        Assert.GreaterThanOrEqualTo(left, right);
	}

    TEST(Assert_LessThan_with_greater_char_should_fail)
    {
		char left = 'F';
		char right = 'E';
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_equal_char_should_fail)
    {
		char left = 'E';
		char right = 'E';
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_less_char_should_pass)
    {
		char left = 'D';
		char right = 'E';
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_greater_wide_char_should_fail)
    {
		wchar_t left = L'F';
		wchar_t right = L'E';
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_equal_wide_char_should_fail)
    {
		wchar_t left = L'E';
		wchar_t right = L'E';
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_less_wide_char_should_pass)
    {
		wchar_t left = L'D';
		wchar_t right = L'E';
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_greater_unsigned_char_should_fail)
    {
		unsigned char left = 56;
		unsigned char right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_equal_unsigned_char_should_fail)
    {
		unsigned char left = 55;
		unsigned char right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_less_unsigned_char_should_pass)
    {
		unsigned char left = 54;
		unsigned char right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_greater_short_should_fail)
    {
		short left = 56;
		short right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_equal_short_should_fail)
    {
		short left = 55;
		short right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_less_short_should_pass)
    {
		short left = 54;
		short right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_greater_unsigned_short_should_fail)
    {
		unsigned short left = 56;
		unsigned short right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_equal_unsigned_short_should_fail)
    {
		unsigned short left = 55;
		unsigned short right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_less_unsigned_short_should_pass)
    {
		unsigned short left = 54;
		unsigned short right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_greater_int_should_fail)
    {
		int left = 56;
		int right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_equal_int_should_fail)
    {
		int left = 55;
		int right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_less_int_should_pass)
    {
		int left = 54;
		int right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_greater_unsigned_int_should_fail)
    {
		unsigned int left = 56;
		unsigned int right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_equal_unsigned_int_should_fail)
    {
		unsigned int left = 55;
		unsigned int right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_less_unsigned_int_should_pass)
    {
		unsigned int left = 54;
		unsigned int right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_greater_long_should_fail)
    {
		long left = 56;
		long right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_equal_long_should_fail)
    {
		long left = 55;
		long right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_less_long_should_pass)
    {
		long left = 54;
		long right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_greater_unsigned_long_should_fail)
    {
		unsigned long left = 56;
		unsigned long right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_equal_unsigned_long_should_fail)
    {
		unsigned long left = 55;
		unsigned long right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_less_unsigned_long_should_pass)
    {
		unsigned long left = 54;
		unsigned long right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_greater_long_long_should_fail)
    {
		long long left = 56;
		long long right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_equal_long_long_should_fail)
    {
		long long left = 55;
		long long right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_less_long_long_should_pass)
    {
		long long left = 54;
		long long right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_greater_unsigned_long_long_should_fail)
    {
		unsigned long long left = 56;
		unsigned long long right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_equal_unsigned_long_long_should_fail)
    {
		unsigned long long left = 55;
		unsigned long long right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_less_unsigned_long_long_should_pass)
    {
		unsigned long long left = 54;
		unsigned long long right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_greater_float_should_fail)
    {
		float left = 56;
		float right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_equal_float_should_fail)
    {
		float left = 55;
		float right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_less_float_should_pass)
    {
		float left = 54;
		float right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_greater_double_should_fail)
    {
		double left = 56;
		double right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_equal_double_should_fail)
    {
		double left = 55;
		double right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThan_with_less_double_should_pass)
    {
		double left = 54;
		double right = 55;
        Assert.LessThan(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_greater_char_should_fail)
    {
		char left = 'F';
		char right = 'E';
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_equal_char_should_pass)
    {
		char left = 'E';
		char right = 'E';
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_less_char_should_pass)
    {
		char left = 'D';
		char right = 'E';
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_greater_wide_char_should_fail)
    {
		wchar_t left = L'F';
		wchar_t right = L'E';
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_equal_wide_char_should_pass)
    {
		wchar_t left = L'E';
		wchar_t right = L'E';
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_less_wide_char_should_pass)
    {
		wchar_t left = L'D';
		wchar_t right = L'E';
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_greater_unsigned_char_should_fail)
    {
		unsigned char left = 56;
		unsigned char right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_equal_unsigned_char_should_pass)
    {
		unsigned char left = 55;
		unsigned char right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_less_unsigned_char_should_pass)
    {
		unsigned char left = 54;
		unsigned char right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_greater_short_should_fail)
    {
		short left = 56;
		short right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_equal_short_should_pass)
    {
		short left = 55;
		short right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_less_short_should_pass)
    {
		short left = 54;
		short right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_greater_unsigned_short_should_fail)
    {
		unsigned short left = 56;
		unsigned short right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_equal_unsigned_short_should_pass)
    {
		unsigned short left = 55;
		unsigned short right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_less_unsigned_short_should_pass)
    {
		unsigned short left = 54;
		unsigned short right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_greater_int_should_fail)
    {
		int left = 56;
		int right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_equal_int_should_pass)
    {
		int left = 55;
		int right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_less_int_should_pass)
    {
		int left = 54;
		int right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_greater_unsigned_int_should_fail)
    {
		unsigned int left = 56;
		unsigned int right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_equal_unsigned_int_should_pass)
    {
		unsigned int left = 55;
		unsigned int right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_less_unsigned_int_should_pass)
    {
		unsigned int left = 54;
		unsigned int right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_greater_long_should_fail)
    {
		long left = 56;
		long right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_equal_long_should_pass)
    {
		long left = 55;
		long right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_less_long_should_pass)
    {
		long left = 54;
		long right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_greater_unsigned_long_should_fail)
    {
		unsigned long left = 56;
		unsigned long right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_equal_unsigned_long_should_pass)
    {
		unsigned long left = 55;
		unsigned long right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_less_unsigned_long_should_pass)
    {
		unsigned long left = 54;
		unsigned long right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_greater_long_long_should_fail)
    {
		long long left = 56;
		long long right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_equal_long_long_should_pass)
    {
		long long left = 55;
		long long right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_less_long_long_should_pass)
    {
		long long left = 54;
		long long right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_greater_unsigned_long_long_should_fail)
    {
		unsigned long long left = 56;
		unsigned long long right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_equal_unsigned_long_long_should_pass)
    {
		unsigned long long left = 55;
		unsigned long long right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_less_unsigned_long_long_should_pass)
    {
		unsigned long long left = 54;
		unsigned long long right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_greater_float_should_fail)
    {
		float left = 56;
		float right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_equal_float_should_pass)
    {
		float left = 55;
		float right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }
	
    TEST(Assert_LessThanOrEqualTo_with_less_float_should_pass)
    {
		float left = 54;
		float right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_greater_double_should_fail)
    {
		double left = 56;
		double right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_equal_double_should_pass)
    {
		double left = 55;
		double right = 55;
        Assert.LessThanOrEqualTo(left, right);
    }

    TEST(Assert_LessThanOrEqualTo_with_less_double_should_pass)
    {
		double left = 54;
		double right = 55;
        Assert.LessThanOrEqualTo(left, right);
	}
}