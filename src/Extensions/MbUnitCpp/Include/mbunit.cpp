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
#include <time.h>
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
		AppendImpl(wstr, (int)wcslen(wstr));
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
		int n = (int)mbstowcs(0, str, -1);
		wchar_t* tmp = new wchar_t[n + 1];
		mbstowcs(tmp, str, n);
		AppendImpl(tmp, n);
		delete[] tmp;
	}

	#define _Impl_StringAppend(TYPE, IMPL) \
		template<> String& String::Append<TYPE>(TYPE arg) \
		{ \
			IMPL ; \
			return *this; \
		}

	_Impl_StringAppend(const String&, AppendImpl(arg.buffer, arg.length))
	_Impl_StringAppend(String, AppendImpl(arg.buffer, arg.length))
	_Impl_StringAppend(const char*, AppendImpl(arg))
	_Impl_StringAppend(const wchar_t*, AppendImpl(arg, (int)wcslen(arg)))
	_Impl_StringAppend(int, AppendFormat(L"%d", arg))
	_Impl_StringAppend(bool, AppendImpl(arg ? "true" : "false"))
	_Impl_StringAppend(char, AppendFormat("%c", arg))
	_Impl_StringAppend(wchar_t, AppendFormat(L"%lc", arg))
	_Impl_StringAppend(unsigned char, AppendFormat("%u", arg))
	_Impl_StringAppend(short, AppendFormat("%d", arg))
	_Impl_StringAppend(unsigned short, AppendFormat("%u", arg))
	_Impl_StringAppend(unsigned int,AppendFormat("%u", arg))
	_Impl_StringAppend(long, AppendFormat("%ld", arg))
	_Impl_StringAppend(unsigned long, AppendFormat("%lu", arg))
	_Impl_StringAppend(long long, AppendFormat("%ld", arg))
	_Impl_StringAppend(unsigned long long, AppendFormat("%lu", arg))
	_Impl_StringAppend(float, AppendFormat("%f", arg))
	_Impl_StringAppend(double, AppendFormat("%Lf", arg))
	_Impl_StringAppend(char*, AppendImpl(arg))
	_Impl_StringAppend(wchar_t*, AppendImpl(arg, (int)wcslen(arg)))

	String& String::AppendFormat(const char* format, ...)
	{
		va_list args;
		va_start(args, format);
		AppendFormat(format, args);
		va_end(args);
		return *this;
	}

	String& String::AppendFormat(const wchar_t* format, ...)
	{
		va_list args;
		va_start(args, format);
		AppendFormat(format, args);
		va_end(args);
		return *this;
	}

	String& String::AppendFormat(const wchar_t* format, va_list args)
	{
		int n = _vscwprintf(format, args);
		wchar_t* tmp = new wchar_t[n + 1];
		vswprintf(tmp, format, args);
		AppendImpl(tmp, n);
		delete[] tmp;
		return *this;
	}

	String& String::AppendFormat(const char* format, va_list args)
	{
		int n = _vscprintf(format, args);
		char* tmp = new char[n + 1];
		vsprintf(tmp, format, args);
		AppendImpl(tmp);
		delete[] tmp;
		return *this;
	}

	String String::Format(const char* format, ...)
	{
		va_list args;
		va_start(args, format);
		String str;
		str.AppendFormat(format, args);
		va_end(args);
		return str;
	}

	String String::Format(const wchar_t* format, ...)
	{
		va_list args;
		va_start(args, format);
		String str;
		str.AppendFormat(format, args);
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
		AppendTo(metadataId, String::Format(L"%s={%s},", key, value));
	}

	void DecoratorTarget::SetMetadata(const wchar_t* key, const char* value)
	{
		String strValue(value);
		SetMetadata(key, strValue.GetBuffer());
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
		clock_t started = clock();

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
		catch (char* exceptionMessage)
		{
            testResultData->NativeOutcome = Failed;
            testResultData->Failure = AssertionFailure::FromException(exceptionMessage);
		}
		catch (...)
		{
            testResultData->NativeOutcome = Failed;
            testResultData->Failure = AssertionFailure::FromException();
		}

		testResultData->DurationMilliseconds = 1000 * (clock() - started) / CLOCKS_PER_SEC;
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
		KindFixture = 0,
        KindTest = 1,
        KindGroup = 2,
		KindRowTest = 3,
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

	// Creates an assertion failure for an unhandled exception.
	AssertionFailure AssertionFailure::FromException(char* exceptionMessage)
	{
		StringMap& map = TestFixture::GetStringMap();
		AssertionFailure failure;
		failure.DescriptionId = map.Add(new String(L"An unhandled exception was thrown"));
		failure.MessageId = map.Add(exceptionMessage == 0 ? 0 : new String(exceptionMessage));
		return failure;
	}

	// Initialize a labeled value.
	void LabeledValue::Set(StringId valueId, mbunit::ValueType valueType, StringId labelId)
	{
		ValueId = valueId;
		ValueType = valueType;
		LabelId = labelId;
	}

	// Default constructor for abstract data sources.
	AbstractDataSource::AbstractDataSource()
		: head(0)
	{
	}

	// Stores the head data row.
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
	
    void TestLogRecorder::Write(const String& str)
    {
		test->AppendToTestLog(str);
    }

    void TestLogRecorder::WriteLine(const String& str)
    {
		test->AppendToTestLog(str);
		test->AppendToTestLog(String(L"\r\n"));
    }

    void TestLogRecorder::WriteFormat(const char* format, ...)
    {
		va_list args;
		va_start(args, format);
        String str;
        str.AppendFormat(format, args);
		va_end(args);
		test->AppendToTestLog(str);
    }

    void TestLogRecorder::WriteFormat(const wchar_t* format,...)
    {
		va_list args;
		va_start(args, format);
        String str;
        str.AppendFormat(format, args);
		va_end(args);
		test->AppendToTestLog(str);
    }

    void TestLogRecorder::WriteLineFormat(const char* format, ...)
    {
		va_list args;
		va_start(args, format);
        String str;
        str.AppendFormat(format, args);
		va_end(args);
		test->AppendToTestLog(str);
		test->AppendToTestLog(String(L"\r\n"));
    }

    void TestLogRecorder::WriteLineFormat(const wchar_t* format, ...)
    {
		va_list args;
		va_start(args, format);
        String str;
        str.AppendFormat(format, args);
		va_end(args);
		test->AppendToTestLog(str);
		test->AppendToTestLog(String(L"\r\n"));
    }

	// ===================
	// Assertion Framework
	// ===================

	StringId AssertionFramework::AddNewString(const char* str)
	{
		return Map().Add((str == 0) ? 0 :  new String(str));
	}

	StringId AssertionFramework::AddNewString(const wchar_t* wstr)
	{
		return Map().Add((wstr == 0) ? 0 : new String(wstr));
	}

	StringId AssertionFramework::AddNewString(const String& str)
    {
		return Map().Add(new String(str));
    }

	template<typename T> StringId AssertionFramework::AddNewStringFrom(T arg)
	{
		String str;
		str.Append(arg);
		return Map().Add(new String(str));
	}

	StringMap& AssertionFramework::Map() const
	{ 
		return TestFixture::GetStringMap(); 
	}

    // Assertion that makes inconditionally the test fail.
    #define _AssertionFramework_Fail(MESSAGETYPE) \
        _Impl_AssertionFramework_Fail(MESSAGETYPE)

    #define _Impl_AssertionFramework_Fail(MESSAGETYPE) \
        void AssertionFramework::Fail(MESSAGETYPE message) \
        { \
            IncrementAssertCount(); \
		    AssertionFailure failure; \
		    failure.DescriptionId = AddNewString(L"An assertion failed."); \
		    failure.MessageId = AddNewString(message); \
		    throw failure; \
        }

    _AssertionFramework_Fail(const String&)

	// Asserts that the specified boolean value is true.
	#define _AssertionFramework_IsTrue(MESSAGETYPE) \
    	_Impl_AssertionFramework_IsTrue(bool, !actualValue, MESSAGETYPE)

    #define _Impl_AssertionFramework_IsTrue(TYPE, CONDITION, MESSAGETYPE) \
		void AssertionFramework::IsTrue(TYPE actualValue, MESSAGETYPE message) \
		{ \
			IncrementAssertCount(); \
			if (CONDITION) \
			{ \
				AssertionFailure failure; \
				failure.DescriptionId = AddNewString(L"Expected value to be true."); \
				failure.Actual.Set(AddNewString(L"false"), TypeBoolean); \
				failure.MessageId = AddNewString(message); \
				throw failure; \
			} \
		}

    _AssertionFramework_IsTrue(const String&)

	// Asserts that the specified boolean value is false.
	#define _AssertionFramework_IsFalse(MESSAGETYPE) \
    	_Impl_AssertionFramework_IsFalse(bool, actualValue, MESSAGETYPE)

	#define _Impl_AssertionFramework_IsFalse(TYPE, CONDITION, MESSAGETYPE) \
		void AssertionFramework::IsFalse(TYPE actualValue, MESSAGETYPE message) \
		{ \
			IncrementAssertCount(); \
			if (CONDITION) \
			{ \
				AssertionFailure failure; \
				failure.DescriptionId = AddNewString(L"Expected value to be false."); \
				failure.Actual.Set(AddNewString(L"true"), TypeBoolean); \
				failure.MessageId = AddNewString(message); \
				throw failure; \
			} \
		}

    _AssertionFramework_IsFalse(const String&)

	// Asserts that the expected value and the actual value are equivalent.
	#define _AssertionFramework_AreEqual(MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(bool, expectedValue != actualValue, TypeBoolean, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(char, expectedValue != actualValue, TypeChar, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(wchar_t, expectedValue != actualValue, TypeChar, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(unsigned char, expectedValue != actualValue, TypeByte, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(short, expectedValue != actualValue, TypeInt16, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(unsigned short, expectedValue != actualValue, TypeUInt16, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(int, expectedValue != actualValue, TypeInt32, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(unsigned int, expectedValue != actualValue, TypeUInt32, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(long, expectedValue != actualValue, TypeUInt64, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(unsigned long, expectedValue != actualValue, TypeUInt64, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(long long, expectedValue != actualValue, TypeUInt64, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(unsigned long long, expectedValue != actualValue, TypeUInt64, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(float, expectedValue != actualValue, TypeSingle, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(double, expectedValue != actualValue, TypeDouble, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(char*, strcmp(expectedValue, actualValue) != 0, TypeString, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(const char*, strcmp(expectedValue, actualValue) != 0, TypeString, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(wchar_t*, wcscmp(expectedValue, actualValue) != 0, TypeString, MESSAGETYPE) \
        _Impl_AssertionFramework_AreEqual(const wchar_t*, wcscmp(expectedValue, actualValue) != 0, TypeString, MESSAGETYPE)

	#define _AssertionFramework_AreEqual_AFX(MESSAGETYPE) \
    	_Impl_AssertionFramework_AreEqual(CString, (expectedValue).Compare(actualValue) != 0, TypeString, MESSAGETYPE) \
	
    #define _Impl_AssertionFramework_AreEqual(TYPE, CONDITION, MANAGEDTYPE, MESSAGETYPE) \
		template<> void AssertionFramework::AreEqual<TYPE>(TYPE expectedValue, TYPE actualValue, MESSAGETYPE message) \
		{ \
			IncrementAssertCount(); \
			if (CONDITION) \
			{ \
				AssertionFailure failure; \
				failure.DescriptionId = AddNewString(L"Expected values to be equal."); \
				failure.Expected.Set(AddNewStringFrom(expectedValue), MANAGEDTYPE); \
				failure.Actual.Set(AddNewStringFrom(actualValue), MANAGEDTYPE); \
				failure.MessageId = AddNewString(message); \
				throw failure; \
			} \
		}

    _AssertionFramework_AreEqual(const String&)

    #ifdef _AFX
    _AssertionFramework_AreEqual_AFX(const wchar_t*)
    _AssertionFramework_AreEqual_AFX(const char*)
    #endif

    // Asserts that the expected value and the actual value are approximately equal.
	#define _AssertionFramework_AreApproximatelyEqual(MESSAGETYPE) \
        _Impl_AssertionFramework_AreApproximatelyEqual(char, abs(expectedValue - actualValue) > delta, TypeChar, MESSAGETYPE) \
        _Impl_AssertionFramework_AreApproximatelyEqual(wchar_t, abs(expectedValue - actualValue) > delta, TypeChar, MESSAGETYPE) \
        _Impl_AssertionFramework_AreApproximatelyEqual(unsigned char, abs((short)expectedValue - (short)actualValue) > (short)delta, TypeByte, MESSAGETYPE) \
        _Impl_AssertionFramework_AreApproximatelyEqual(short, abs(expectedValue - actualValue) > delta, TypeInt16, MESSAGETYPE) \
        _Impl_AssertionFramework_AreApproximatelyEqual(unsigned short, abs((int)expectedValue - (int)actualValue) > (int)delta, TypeUInt16, MESSAGETYPE) \
        _Impl_AssertionFramework_AreApproximatelyEqual(int, abs(expectedValue - actualValue) > delta, TypeInt32, MESSAGETYPE) \
        _Impl_AssertionFramework_AreApproximatelyEqual(unsigned int, _abs64((long long)expectedValue - (long long)actualValue) > (long long)delta, TypeUInt32, MESSAGETYPE) \
        _Impl_AssertionFramework_AreApproximatelyEqual(long, abs(expectedValue - actualValue) > delta, TypeInt32, MESSAGETYPE) \
        _Impl_AssertionFramework_AreApproximatelyEqual(unsigned long, _abs64((long long)expectedValue - (long long)actualValue) > (long long)delta, TypeUInt32, MESSAGETYPE) \
        _Impl_AssertionFramework_AreApproximatelyEqual(long long, _abs64(expectedValue - actualValue) > delta, TypeInt64, MESSAGETYPE) \
        _Impl_AssertionFramework_AreApproximatelyEqual(unsigned long long, fabs((double)expectedValue - (double)actualValue) > (double)delta, TypeInt64, MESSAGETYPE) \
        _Impl_AssertionFramework_AreApproximatelyEqual(float, fabs(expectedValue - actualValue) > delta, TypeSingle, MESSAGETYPE) \
        _Impl_AssertionFramework_AreApproximatelyEqual(double, fabs(expectedValue - actualValue) > delta, TypeDouble, MESSAGETYPE)

    #define _Impl_AssertionFramework_AreApproximatelyEqual(TYPE, CONDITION, MANAGEDTYPE, MESSAGETYPE) \
		template<> void AssertionFramework::AreApproximatelyEqual<TYPE>(TYPE expectedValue, TYPE actualValue, TYPE delta, MESSAGETYPE message) \
		{ \
			IncrementAssertCount(); \
			if (CONDITION) \
			{ \
				AssertionFailure failure; \
				failure.DescriptionId = AddNewString(L"Expected values to be approximately equal to within a delta."); \
				failure.Expected.Set(AddNewStringFrom(expectedValue), MANAGEDTYPE); \
				failure.Actual.Set(AddNewStringFrom(actualValue), MANAGEDTYPE); \
				failure.Extra_0.Set(AddNewStringFrom(delta), MANAGEDTYPE, AddNewString(L"Delta")); \
				failure.MessageId = AddNewString(message); \
				throw failure; \
			} \
		}

    _AssertionFramework_AreApproximatelyEqual(const String&)

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
