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
#include <stdio.h>
#include <stdlib.h>
#include <stdarg.h>
#include <wchar.h>
#include <math.h>
#include "mbunit.h"

#pragma warning (disable: 4996 4355) // Hide some warnings.

namespace mbunit
{
	// =========
	// Internals
	// =========

	String::String() : buffer(0), length(-1)
	{
		Clear();
	}

	String::~String()
	{
		if (buffer != 0)
			delete[] buffer;
	}

	String::String(const String& rhs) : buffer(0), length(-1)
	{
		AppendImpl(rhs.buffer, rhs.length);
	}

	String::String(const char* str) : buffer(0), length(-1)
	{
		AppendImpl(str);
	}

	String::String(const wchar_t* wstr) : buffer(0), length(-1)
	{
		AppendImpl(wstr, wcslen(wstr));
	}

	void String::Clear()
	{
		if (length != 0)
		{
			if (buffer != 0)
				delete[] buffer;

			buffer = new wchar_t[1];
			buffer[0] = L'\0';
			length = 0;
		}
	}

	void String::AppendImpl(const wchar_t* wstr, int n)
	{
		if ((n > 0) || (length < 0))
		{
            if (length < 0)
                length = 0;

			wchar_t* newBuffer = new wchar_t[length + n + 1];

			if (length > 0)
				wcsncpy(newBuffer, buffer, length);

			if (n > 0)
				wcsncpy(newBuffer + length, wstr, n);

			length += n;
			newBuffer[length] = L'\0';

			if (buffer != 0)
				delete[] buffer;

			buffer = newBuffer;
		}
	}

	void String::AppendImpl(const char* str)
	{
		int n = mbstowcs(0, str, -1);
		wchar_t* tmp = new wchar_t[n + 1];
		mbstowcs(tmp, str, n);
		AppendImpl(tmp, n);
		delete[] tmp;
	}

	void String::Append(const String& rhs)
	{
		AppendImpl(rhs.buffer, rhs.length);
	}

	void String::Append(const char* str)
	{
		AppendImpl(str);
	}

	void String::Append(const wchar_t* wstr)
	{
		AppendImpl(wstr, wcslen(wstr));
	}

	void String::AppendFormat(const char* format, ...)
	{
		va_list args;
		va_start(args, format);
		AppendFormat(format, args);
		va_end(args);
	}

	void String::AppendFormat(const wchar_t* format, ...)
	{
		va_list args;
		va_start(args, format);
		AppendFormat(format, args);
		va_end(args);
	}

	void String::AppendFormat(const wchar_t* format, va_list args)
	{
		int n = _vscwprintf(format, args);
		wchar_t* tmp = new wchar_t[n + 1];
		vswprintf(tmp, format, args);
		AppendImpl(tmp, n);
		delete[] tmp;
	}

	void String::AppendFormat(const char* format, va_list args)
	{
		int n = _vscprintf(format, args);
		char* tmp = new char[n + 1];
		vsprintf(tmp, format, args);
		AppendImpl(tmp);
		delete[] tmp;
	}

	void String::Format(const char* format, ...)
	{
		va_list args;
		va_start(args, format);
		Format(format, args);
		va_end(args);
	}

	void String::Format(const wchar_t* format, ...)
	{
		va_list args;
		va_start(args, format);
		Format(format, args);
		va_end(args);
	}

	void String::Format(const char* format, va_list args)
	{
		Clear();
		AppendFormat(format, args);
	}

	void String::Format(const wchar_t* format, va_list args)
	{
		Clear();
		AppendFormat(format, args);
	}

	String* String::New(const char* str)
	{
		return (str == 0) ? 0 :  new String(str);
	}

	String* String::New(const wchar_t* wstr)
	{
		return (wstr == 0) ? 0 : new String(wstr);
	}

	String* String::NewFormatted(const char* format, ...)
	{
		va_list args;
		va_start(args, format);
		String* str = new String;
		str->Format(format, args);
		va_end(args);
		return str;
	}

	String* String::NewFormatted(const wchar_t* format, ...)
	{
		va_list args;
		va_start(args, format);
		String* str = new String;
		str->Format(format, args);
		va_end(args);
		return str;
	}

	StringMap::StringMap()
		: head(0), nextId(1)
	{
	}

	StringMap::~StringMap()
	{
		RemoveAll();
	}

	void StringMap::RemoveAll()
	{
		StringMapNode* current = head;

		while (current != 0)
		{
			StringMapNode* next = current->Next;
			delete current->Str;
			delete current;
			current = next;
		}

		head = 0;
	}

	String* StringMap::Get(StringId key)
	{
		StringMapNode* current = head;

		while (current != 0)
		{
			if (current->Key == key)
				return current->Str;
            
			current = current->Next;
		}

		throw "Key not found.";
	}

	StringId StringMap::Add(String* str)
	{
		if (str == 0)
			return 0;

		StringMapNode* node = new StringMapNode;
		node->Key = nextId;
		node->Str = str;
		node->Next = head;
		head = node;
		return nextId++;
	}

	void StringMap::Remove(StringId key)
	{
		StringMapNode* previous = 0;
		StringMapNode* current = head;

		while (current != 0)
		{
			if (current->Key == key)
			{
				if (previous != 0)
					previous->Next = current->Next;
				else
					head = current->Next;

				delete current->Str;
				delete current;
				return;
			}
            
			previous = current;
			current = current->Next;
		}
	}

    // Construct a base test or test fixture instance.
	DecoratorTarget::DecoratorTarget(int metadataPrototypeId)
		: metadataId(0)
	{
		if (metadataPrototypeId > 0)
		{
			StringMap& map = TestFixture::GetStringMap();
			String* s = map.Get(metadataPrototypeId);
			metadataId = map.Add(new String(*s));
		}
	}
		
	// Attaches a key/value metadata to the current test or test fixture.
	void DecoratorTarget::SetMetadata(const wchar_t* key, const wchar_t* value)
	{
		String str;
		str.Format(L"%s={%s},", key, value);
		AppendTo(metadataId, str);
	}

	// Create a new string ID or append the specified text if it already exists.
	void DecoratorTarget::AppendTo(int& id, const String& s)
	{
		StringMap& map = TestFixture::GetStringMap();

		if (id == 0)
		{
			id = map.Add(new String(s));
		}
		else
		{
			map.Get(id)->Append(s);
		}
	}

    // Construct an executable test case.
    Test::Test(TestFixture* testFixture, const wchar_t* name, const wchar_t* fileName, int lineNumber)
        : index(testFixture->GetTestList().GetNextIndex())
		, DecoratorTarget(testFixture->GetMetadataId())
		, name(name)
		, fileName(fileName)
		, lineNumber(lineNumber)
		, Assert(this)
		, TestLog(this)
		, testLogId(0)
		, dataSource(0)
    {
	}

	// Desctructor.
    Test::~Test()
    {
		if (dataSource != 0)
			delete dataSource;
    }

    // Specifies the next test of the chained list.
    void Test::SetNext(Test* test)
    {
        next = test;
    }

    // Runs the current test and captures the failure(s).
    void Test::Run(TestResultData* testResultData, void* dataRow)
    {
        try
        {
            Clear();
			BindDataRow(dataRow);
            RunImpl();
            testResultData->NativeOutcome = Passed;
		}
        catch (AssertionFailure failure)
        {
            testResultData->NativeOutcome = Failed;
            testResultData->Failure = failure;
        }

		testResultData->TestLogId = testLogId;
        testResultData->AssertCount = assertCount;
    }

	// Clears internal variables for new run.
    void Test::Clear()
    {
        assertCount = 0;
		testLogId = 0;
    }

	// Increment the assertion count by 1.
    void Test::IncrementAssertCount()
    {
        assertCount++;
    }

	// Appends the specified text to the test log.
	void Test::AppendToTestLog(const String& s)
	{
		AppendTo(testLogId, s);
	}

    // Default empty implementation of the test execution.
    void Test::RunImpl()
    {
    }

	// Binds the specified data source to the test instance.
	void Test::Bind(AbstractDataSource* dataSource)
	{
		this->dataSource = dataSource;
	}

	// Binds the specified data row to the test step.
	void Test::BindDataRow(void* dataRow) 
	{
	}

    // Constructs an empty list of tests.
    TestList::TestList()
        : head(0), tail(0), nextIndex(0)
    {
    }

    // Adds a new test at the end of the list.
    void TestList::Add(Test* test)
    {
        if (tail == 0)
            head = test;
        else
            tail->SetNext(test);
        
        tail = test;
    }

    // Returns the next unused test ID.
    int TestList::GetNextIndex()
    {
        return nextIndex ++;
    }

    // Constructs a test fixture.
    TestFixture::TestFixture(int index, const wchar_t* name)
        : index(index), name(name)
    {
    }

    TestFixture::~TestFixture()
    {
    }

    // Specifies the next test fixture of the chained list.
    void TestFixture::SetNext(TestFixture* testFixture)
    {
        next = testFixture;
    }

    // Returns the list of tests defined in the current test fixture.
    TestList& TestFixture::GetTestList()
    {
        return children;
    }

    // Gets the singleton list of test fixtures.
    TestFixtureList& TestFixture::GetTestFixtureList()
    {
        static TestFixtureList list;
        return list;
    }

	// Gets the singleton map of strings.
	StringMap& TestFixture::GetStringMap()
	{
	    static StringMap map;
        return map;
	}

    // Constructs an empty list of test fixtures.
    TestFixtureList::TestFixtureList()
        : head(0), tail(0), nextIndex(0)
    {
    }

    // Adds a new test fixture at the end of the list.
    void TestFixtureList::Add(TestFixture* testFixture)
    {
        if (tail == 0)
            head = testFixture;
        else
            tail->SetNext(testFixture);
        
        tail = testFixture;
    }

    // Gets the next unused test fixture ID.
    int TestFixtureList::GetNextIndex()
    {
        return nextIndex ++;
    }

    // Registers the specified test in the list.
    TestRecorder::TestRecorder(TestList& list, Test* test)
    {
        list.Add(test);
    }

    // Registers the specified test fixture in the list.
    TestFixtureRecorder::TestFixtureRecorder(TestFixtureList& list, TestFixture* testFixture)
    {
        list.Add(testFixture);
    }

    // A structure describing the currently enumerated test or test fixture.
    struct Position
    {
        TestFixture* TestFixture;
        Test* Test;
		void* DataRow;
    };

	// Type of the curent test.
	enum TestKind
	{
		KindFixture,
        KindTest,
        KindGroup,
		KindRowTest,
	};

    // A portable structure to describe the current test or test fixture.
    struct TestInfoData
    {
        const wchar_t* Name;
        int Index;
        TestKind Kind;
        const wchar_t* FileName;
        int LineNumber;
        Position Position;
		int MetadataId;
    };

    // Constructs an assertion framework instance for the specified test.
    AssertionFramework::AssertionFramework(Test* test)
        : test(test)
    {
    }

    // Internal assert count increment.
    void AssertionFramework::IncrementAssertCount()
    {
        test->IncrementAssertCount();
    }

	// Constructs an empty assertion failure.
	AssertionFailure::AssertionFailure()
		: DescriptionId(0),  MessageId(0)
	{
	}

	// Constructs an empty labeled value.
	LabeledValue::LabeledValue()
		: LabelId(0), ValueId(0), ValueType(TypeRaw)
	{
	}

	// Initialize a labeled value.
	void LabeledValue::Set(StringId valueId, mbunit::ValueType valueType, StringId labelId)
	{
		ValueId = valueId;
		ValueType = valueType;
		LabelId = labelId;
	}

	AbstractDataSource::AbstractDataSource()
		: head(0)
	{
	}

	void AbstractDataSource::SetHead(void* dataRow)
	{
		head = dataRow;
	}

	// =============
	// Log Recording
	// =============

	TestLogRecorder::TestLogRecorder(Test* test)
		: test(test)
	{
	}
	
	void TestLogRecorder::Write(const wchar_t* format, ...)
	{
		va_list args;
		va_start(args, format);
		String str;
		str.Format(format, args);
		test->AppendToTestLog(str);
		va_end(args);
	}

	void TestLogRecorder::WriteLine(const wchar_t* format, ...)
	{
		va_list args;
		va_start(args, format);
		String raw;
		raw.Append(format);
		raw.Append(L"\r\n");
		String str;
		str.Format(raw.GetBuffer(), args);
		test->AppendToTestLog(str);
		va_end(args);
	}

	// ===================
	// Assertion Framework
	// ===================

	StringMap& AssertionFramework::Map() const
	{ 
		return TestFixture::GetStringMap(); 
	}

    // Assertion that makes inconditionally the test fail.
    void AssertionFramework::Fail(const wchar_t* message)
    {
        IncrementAssertCount();
		AssertionFailure failure;
		failure.DescriptionId = Map().Add(String::New(L"An assertion failed."));
		failure.MessageId = Map().Add(String::New(message));
		throw failure;
    }

	// Asserts that the specified boolean value is true.
	void AssertionFramework::IsTrue(bool actualValue, const wchar_t* message)
	{
        IncrementAssertCount();

		if (!actualValue)
		{
			AssertionFailure failure;
			failure.DescriptionId = Map().Add(String::New(L"Expected value to be true."));
			failure.Actual.Set(Map().Add(String::New(L"false")), TypeBoolean);
			failure.MessageId = Map().Add(String::New(message));
			throw failure;
		}
	}

	void AssertionFramework::IsTrue(int actualValue, const wchar_t* message)
	{
		IsTrue(actualValue != 0, message);
	}

	// Asserts that the specified boolean value is false.
	void AssertionFramework::IsFalse(bool actualValue, const wchar_t* message)
	{
        IncrementAssertCount();

		if (actualValue)
		{
			AssertionFailure failure;
			failure.DescriptionId = Map().Add(String::New(L"Expected value to be false."));
			failure.Actual.Set(Map().Add(String::New(L"true")), TypeBoolean);
			failure.MessageId = Map().Add(String::New(message));
			throw failure;
		}
	}

	void AssertionFramework::IsFalse(int actualValue, const wchar_t* message)
	{
		IsFalse(actualValue != 0, message);
	}

	#define _AssertionFramework_AreEqual(TYPE, INEQUALITY, FORMATEXPECTED, FORMATACTUAL, MANAGEDTYPE) \
	void AssertionFramework::AreEqual(TYPE expectedValue, TYPE actualValue, const wchar_t* message) \
	{ \
        IncrementAssertCount(); \
		\
		if (INEQUALITY) \
		{ \
			AssertionFailure failure; \
			failure.DescriptionId = Map().Add(String::New(L"Expected values to be equal.")); \
			failure.Expected.Set(FORMATEXPECTED, MANAGEDTYPE); \
			failure.Actual.Set(FORMATACTUAL, MANAGEDTYPE); \
			failure.MessageId = Map().Add(String::New(message)); \
			throw failure; \
		} \
	}

	_AssertionFramework_AreEqual(bool, 
		expectedValue != actualValue, 
		Map().Add(String::New(expectedValue ? L"true" : L"false")), 
		Map().Add(String::New(actualValue ? L"true" : L"false")), 
		TypeBoolean)

	_AssertionFramework_AreEqual(char, 
		expectedValue != actualValue, 
		Map().Add(String::NewFormatted(L"%c", expectedValue)), 
		Map().Add(String::NewFormatted(L"%c", actualValue)), 
		TypeChar)

	_AssertionFramework_AreEqual(wchar_t, 
		expectedValue != actualValue, 
		Map().Add(String::NewFormatted(L"%lc", expectedValue)), 
		Map().Add(String::NewFormatted(L"%lc", actualValue)), 
		TypeChar)

	_AssertionFramework_AreEqual(unsigned char, 
		expectedValue != actualValue, 
		Map().Add(String::NewFormatted(L"%u", expectedValue)), 
		Map().Add(String::NewFormatted(L"%u", actualValue)), 
		TypeByte)

	_AssertionFramework_AreEqual(short, 
		expectedValue != actualValue, 
		Map().Add(String::NewFormatted(L"%d", expectedValue)), 
		Map().Add(String::NewFormatted(L"%d", actualValue)), 
		TypeInt16)

	_AssertionFramework_AreEqual(unsigned short, 
		expectedValue != actualValue, 
		Map().Add(String::NewFormatted(L"%u", expectedValue)), 
		Map().Add(String::NewFormatted(L"%u", actualValue)), 
		TypeUInt16)

	_AssertionFramework_AreEqual(int, 
		expectedValue != actualValue, 
		Map().Add(String::NewFormatted(L"%d", expectedValue)), 
		Map().Add(String::NewFormatted(L"%d", actualValue)), 
		TypeInt32)

	_AssertionFramework_AreEqual(unsigned int, 
		expectedValue != actualValue, 
		Map().Add(String::NewFormatted(L"%u", expectedValue)), 
		Map().Add(String::NewFormatted(L"%u", actualValue)), 
		TypeUInt32)

	_AssertionFramework_AreEqual(long, 
		expectedValue != actualValue, 
		Map().Add(String::NewFormatted(L"%ld", expectedValue)),
		Map().Add(String::NewFormatted(L"%ld", actualValue)), 
		TypeUInt64)

	_AssertionFramework_AreEqual(unsigned long, 
		expectedValue != actualValue, 
		Map().Add(String::NewFormatted(L"%uld", expectedValue)),
		Map().Add(String::NewFormatted(L"%uld", actualValue)), 
		TypeUInt64)

	_AssertionFramework_AreEqual(long long, 
		expectedValue != actualValue, 
		Map().Add(String::NewFormatted(L"%ld", expectedValue)),
		Map().Add(String::NewFormatted(L"%ld", actualValue)), 
		TypeUInt64)

	_AssertionFramework_AreEqual(unsigned long long, 
		expectedValue != actualValue, 
		Map().Add(String::NewFormatted(L"%uld", expectedValue)),
		Map().Add(String::NewFormatted(L"%uld", actualValue)), 
		TypeUInt64)

	_AssertionFramework_AreEqual(float, 
		expectedValue != actualValue, 
		Map().Add(String::NewFormatted(L"%f", expectedValue)), 
		Map().Add(String::NewFormatted(L"%f", actualValue)), 
		TypeSingle)

	_AssertionFramework_AreEqual(double, 
		expectedValue != actualValue, 
		Map().Add(String::NewFormatted(L"%Lf", expectedValue)), 
		Map().Add(String::NewFormatted(L"%Lf", actualValue)), 
		TypeDouble)

	_AssertionFramework_AreEqual(char*, 
		strcmp(expectedValue, actualValue) != 0, 
		Map().Add(String::New(expectedValue)), 
		Map().Add(String::New(actualValue)), 
		TypeString)

	_AssertionFramework_AreEqual(const char*, 
		strcmp(expectedValue, actualValue) != 0, 
		Map().Add(String::New(expectedValue)), 
		Map().Add(String::New(actualValue)), 
		TypeString)

	_AssertionFramework_AreEqual(wchar_t*, 
		wcscmp(expectedValue, actualValue) != 0, 
		Map().Add(String::New(expectedValue)), 
		Map().Add(String::New(actualValue)), 
		TypeString)

	_AssertionFramework_AreEqual(const wchar_t*, 
		wcscmp(expectedValue, actualValue) != 0, 
		Map().Add(String::New(expectedValue)), 
		Map().Add(String::New(actualValue)), 
		TypeString)

	#define _AssertionFramework_AreApproximatelyEqual(TYPE, INEQUALITY, FORMATEXPECTED, FORMATACTUAL, FORMATDELTA, MANAGEDTYPE) \
	void AssertionFramework::AreApproximatelyEqual(TYPE expectedValue, TYPE actualValue, TYPE delta, const wchar_t* message) \
	{ \
        IncrementAssertCount(); \
		\
		if (INEQUALITY) \
		{ \
			AssertionFailure failure; \
			failure.DescriptionId = Map().Add(String::New(L"Expected values to be approximately equal to within a delta.")); \
			failure.Expected.Set(FORMATEXPECTED, MANAGEDTYPE); \
			failure.Actual.Set(FORMATACTUAL, MANAGEDTYPE); \
			failure.Extra_0.Set(FORMATDELTA, MANAGEDTYPE, Map().Add(String::New(L"Delta"))); \
			failure.MessageId = Map().Add(String::New(message)); \
			throw failure; \
		} \
	}

	_AssertionFramework_AreApproximatelyEqual(char, 
		abs(expectedValue - actualValue) > delta, 
		Map().Add(String::NewFormatted(L"%c", expectedValue)), 
		Map().Add(String::NewFormatted(L"%c", actualValue)), 
		Map().Add(String::NewFormatted(L"%c", delta)), 
		TypeChar)

	_AssertionFramework_AreApproximatelyEqual(wchar_t, 
		abs(expectedValue - actualValue) > delta, 
		Map().Add(String::NewFormatted(L"%lc", expectedValue)), 
		Map().Add(String::NewFormatted(L"%lc", actualValue)), 
		Map().Add(String::NewFormatted(L"%lc", delta)), 
		TypeChar)

	_AssertionFramework_AreApproximatelyEqual(unsigned char, 
		abs((short)expectedValue - (short)actualValue) > (short)delta, 
		Map().Add(String::NewFormatted(L"%u", expectedValue)), 
		Map().Add(String::NewFormatted(L"%u", actualValue)), 
		Map().Add(String::NewFormatted(L"%u", delta)), 
		TypeByte)

	_AssertionFramework_AreApproximatelyEqual(short, 
		abs(expectedValue - actualValue) > delta, 
		Map().Add(String::NewFormatted(L"%d", expectedValue)), 
		Map().Add(String::NewFormatted(L"%d", actualValue)), 
		Map().Add(String::NewFormatted(L"%d", delta)), 
		TypeInt16)

	_AssertionFramework_AreApproximatelyEqual(unsigned short, 
		abs((int)expectedValue - (int)actualValue) > (int)delta, 
		Map().Add(String::NewFormatted(L"%u", expectedValue)), 
		Map().Add(String::NewFormatted(L"%u", actualValue)), 
		Map().Add(String::NewFormatted(L"%u", delta)), 
		TypeUInt16)

	_AssertionFramework_AreApproximatelyEqual(int, 
		abs(expectedValue - actualValue) > delta, 
		Map().Add(String::NewFormatted(L"%d", expectedValue)), 
		Map().Add(String::NewFormatted(L"%d", actualValue)), 
		Map().Add(String::NewFormatted(L"%d", delta)), 
		TypeInt32)

	_AssertionFramework_AreApproximatelyEqual(unsigned int, 
		_abs64((long long)expectedValue - (long long)actualValue) > (long long)delta, 
		Map().Add(String::NewFormatted(L"%u", expectedValue)), 
		Map().Add(String::NewFormatted(L"%u", actualValue)), 
		Map().Add(String::NewFormatted(L"%u", delta)), 
		TypeUInt32)

	_AssertionFramework_AreApproximatelyEqual(long, 
		abs(expectedValue - actualValue) > delta, 
		Map().Add(String::NewFormatted(L"%d", expectedValue)), 
		Map().Add(String::NewFormatted(L"%d", actualValue)), 
		Map().Add(String::NewFormatted(L"%d", delta)), 
		TypeInt32)

	_AssertionFramework_AreApproximatelyEqual(unsigned long, 
		_abs64((long long)expectedValue - (long long)actualValue) > (long long)delta, 
		Map().Add(String::NewFormatted(L"%u", expectedValue)), 
		Map().Add(String::NewFormatted(L"%u", actualValue)), 
		Map().Add(String::NewFormatted(L"%u", delta)), 
		TypeUInt32)

	_AssertionFramework_AreApproximatelyEqual(long long, 
		_abs64(expectedValue - actualValue) > delta, 
		Map().Add(String::NewFormatted(L"%ld", expectedValue)), 
		Map().Add(String::NewFormatted(L"%ld", actualValue)), 
		Map().Add(String::NewFormatted(L"%ld", delta)), 
		TypeInt64)

	_AssertionFramework_AreApproximatelyEqual(unsigned long long, 
		fabs((double)expectedValue - (double)actualValue) > (double)delta, 
		Map().Add(String::NewFormatted(L"%ld", expectedValue)), 
		Map().Add(String::NewFormatted(L"%ld", actualValue)), 
		Map().Add(String::NewFormatted(L"%ld", delta)), 
		TypeInt64)

	_AssertionFramework_AreApproximatelyEqual(float, 
		fabs(expectedValue - actualValue) > delta, 
		Map().Add(String::NewFormatted(L"%f", expectedValue)), 
		Map().Add(String::NewFormatted(L"%f", actualValue)), 
		Map().Add(String::NewFormatted(L"%f", delta)), 
		TypeSingle)

	_AssertionFramework_AreApproximatelyEqual(double, 
		fabs(expectedValue - actualValue) > delta, 
		Map().Add(String::NewFormatted(L"%Lf", expectedValue)), 
		Map().Add(String::NewFormatted(L"%Lf", actualValue)), 
		Map().Add(String::NewFormatted(L"%Lf", delta)), 
		TypeDouble)

	// ======================================
	// Interface functions for Gallio adapter
	// ======================================

    extern "C" 
    {
        void __cdecl MbUnitCpp_GetHeadTest(Position* position)
        {
            TestFixtureList& list = TestFixture::GetTestFixtureList();
            TestFixture* pFirstTestFixture = list.GetHead();
            position->TestFixture = pFirstTestFixture;
            position->Test = 0;
            position->DataRow = 0;
        }

        int __cdecl MbUnitCpp_GetNextTest(Position* position, TestInfoData* testInfoData)
        {
            TestFixture* testFixture = position->TestFixture;
            Test* test = position->Test;
			void* dataRow = position->DataRow;

            if (testFixture == 0)
                return 0;
            
            if (test == 0)
            {
                testInfoData->Kind = KindFixture;
                testInfoData->FileName = 0;
                testInfoData->LineNumber = 0;
                testInfoData->Name = testFixture->GetName();
                testInfoData->Index = testFixture->GetIndex();
                testInfoData->Position.TestFixture = testFixture;
                testInfoData->Position.Test = 0;
                testInfoData->Position.DataRow = 0;
                testInfoData->MetadataId = 0;
                position->Test = testFixture->GetTestList().GetHead();
                return 1;            
            }

		    testInfoData->FileName = test->GetFileName();
			testInfoData->LineNumber = test->GetLineNumber();
			testInfoData->Name = test->GetName();
			testInfoData->Index = test->GetIndex();
			testInfoData->Position.TestFixture = testFixture;
			testInfoData->Position.Test = test;
			testInfoData->MetadataId = test->GetMetadataId();

			if (dataRow == 0)
			{
				testInfoData->Position.DataRow = 0;
				
				if (test->GetDataSource() != 0)
				{
					testInfoData->Kind = KindGroup;
					position->DataRow = test->GetDataSource()->GetHead();
				}
				else
				{
					testInfoData->Kind = KindTest;
					position->Test = test->GetNext();
					if (position->Test == 0)
						position->TestFixture = testFixture->GetNext();
				}

				return 1;
			}
		
			testInfoData->Kind = KindRowTest;
			testInfoData->Position.DataRow = dataRow;
			position->DataRow = test->GetDataSource()->GetNextRow(dataRow);
			if (position->DataRow == 0)
				position->Test = test->GetNext();
            if (position->Test == 0)
                position->TestFixture = testFixture->GetNext();
            return 1;
        }

        void __cdecl MbUnitCpp_RunTest(Position* position, TestResultData* testResultData)
        {
            Test* test = position->Test;
            test->Run(testResultData, position->DataRow);
        }

		wchar_t* __cdecl MbUnitCpp_GetString(StringId stringId)
		{
			StringMap& map = TestFixture::GetStringMap();
			return map.Get(stringId)->GetBuffer();
		}

		void __cdecl MbUnitCpp_ReleaseString(StringId stringId)
		{
			StringMap& map = TestFixture::GetStringMap();
			map.Remove(stringId);
		}

		void __cdecl MbUnitCpp_ReleaseAllStrings()
		{
			StringMap& map = TestFixture::GetStringMap();
			map.RemoveAll();
		}
    }
}

#if defined(_WIN64) 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetHeadTest") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetNextTest") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_RunTest") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetString") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_ReleaseString") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_ReleaseAllStrings") 
#else 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetHeadTest=_MbUnitCpp_GetHeadTest") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetNextTest=_MbUnitCpp_GetNextTest") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_RunTest=_MbUnitCpp_RunTest") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetString=_MbUnitCpp_GetString") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_ReleaseString=_MbUnitCpp_ReleaseString") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_ReleaseAllStrings=_MbUnitCpp_ReleaseAllStrings") 
#endif
