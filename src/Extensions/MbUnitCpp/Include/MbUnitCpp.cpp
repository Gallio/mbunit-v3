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
#include <stdarg.h>
#include "MbUnitCpp.h"

#pragma warning (disable: 4996) // Hide deprecation warnings.

namespace MbUnitCpp
{
	// =========
	// Internals
	// =========

	String::String(const char* format, ...)
	{
		va_list argList;
		va_start(argList, format);
		Initialize(format, argList);
		va_end(argList);
	}

	String::String(const char* format, va_list argList)
	{
		Initialize(format, argList);
	}

	void String::Initialize(const char* format, va_list argList)
	{
		int n = _vscprintf(format, argList) + 1;
		m_data = new char[n];
		vsprintf(m_data, format, argList);
	}

	String::~String()
	{
		delete[] m_data;
	}

	StringMap::StringMap()
		: m_head(0), m_nextId(1)
	{
	}

	StringMap::~StringMap()
	{
		RemoveAll();
	}

	void StringMap::RemoveAll()
	{
		StringMapNode* current = m_head;

		while (current != 0)
		{
			StringMapNode* next = current->Next;
			delete current->Data;
			delete current;
			current = next;
		}

		m_head = 0;
	}

	String* StringMap::Get(StringId key)
	{
		StringMapNode* current = m_head;

		while (current != 0)
		{
			if (current->Key == key)
				return current->Data;
            
			current = current->Next;
		}

		throw "Key not found.";
	}

	StringId StringMap::Add(const char* format, ...)
	{
		if (format == 0)
			return 0;

		va_list argList;
		va_start(argList, format);
		String* data = new String(format, argList);
		va_end(argList);
		return Add(data);
	}

	StringId StringMap::Add(String* data)
	{
		StringMapNode* node = new StringMapNode;
		node->Key = m_nextId;
		node->Data = data;
		node->Next = m_head;
		m_head = node;
		return m_nextId++;
	}

	void StringMap::Remove(StringId key)
	{
		StringMapNode* previous = 0;
		StringMapNode* current = m_head;

		while (current != 0)
		{
			if (current->Key == key)
			{
				if (previous != 0)
					previous->Next = current->Next;
				else
					m_head = current->Next;

				delete current->Data;
				delete current;
				return;
			}
            
			previous = current;
			current = current->Next;
		}
	}

    // Construct an executable test case.
    Test::Test(int index, char const* name, char const* fileName, int lineNumber)
        : m_index(index), m_name(name), m_fileName(fileName), m_lineNumber(lineNumber), Assert(this)
    {
    }

    Test::~Test()
    {
    }

    // Specifies the next test of the chained list.
    void Test::SetNext(Test* test)
    {
        m_next = test;
    }

    // Runs the current test and captures the failure(s).
    void Test::Run(TestResultData* pTestResultData)
    {
        try
        {
            Clear();
            RunImpl();
            pTestResultData->AssertCount = m_assertCount;
            pTestResultData->NativeOutcome = Passed;
		}
        catch (AssertionFailure failure)
        {
            pTestResultData->AssertCount = m_assertCount;
            pTestResultData->NativeOutcome = Failed;
            pTestResultData->Failure = failure;
        }
    }

    void Test::Clear()
    {
        m_assertCount = 0;
    }

    void Test::IncrementAssertCount()
    {
        m_assertCount++;
    }

    // Default empty implementation of the test execution.
    void Test::RunImpl()
    {
    }

    // Constructs an empty list of tests.
    TestList::TestList()
        : m_head(0), m_tail(0), m_nextIndex(0)
    {
    }

    // Adds a new test at the end of the list.
    void TestList::Add(Test* test)
    {
        if (m_tail == 0)
            m_head = test;
        else
            m_tail->SetNext(test);
        
        m_tail = test;
    }

    // Returns the next unused test ID.
    int TestList::GetNextIndex()
    {
        return m_nextIndex ++;
    }

    // Constructs a test fixture.
    TestFixture::TestFixture(int index, char const* name)
        : m_index(index), m_name(name)
    {
    }

    TestFixture::~TestFixture()
    {
    }

    // Specifies the next test fixture of the chained list.
    void TestFixture::SetNext(TestFixture* pTestFixture)
    {
        m_next = pTestFixture;
    }

    // Returns the list of tests defined in the current test fixture.
    TestList& TestFixture::GetTestList()
    {
        return m_children;
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
        : m_head(0), m_tail(0), m_nextIndex(0)
    {
    }

    // Adds a new test fixture at the end of the list.
    void TestFixtureList::Add(TestFixture* pTestFixture)
    {
        if (m_tail == 0)
            m_head = pTestFixture;
        else
            m_tail->SetNext(pTestFixture);
        
        m_tail = pTestFixture;
    }

    // Gets the next unused test fixture ID.
    int TestFixtureList::GetNextIndex()
    {
        return m_nextIndex ++;
    }

    // Registers the specified test in the list.
    TestRecorder::TestRecorder(TestList& list, Test* pTest)
    {
        list.Add(pTest);
    }

    // Registers the specified test fixture in the list.
    TestFixtureRecorder::TestFixtureRecorder(TestFixtureList& list, TestFixture* pTestFixture)
    {
        list.Add(pTestFixture);
    }

    // A structure describing the currently enumerated test or test fixture.
    struct Position
    {
        TestFixture* pTestFixture;
        Test* pTest;
    };

    // A portable structure to describe the current test or test fixture.
    struct TestInfoData
    {
        const char* Name;
        int Index;
        bool IsTestFixture;
        const char* FileName;
        int LineNumber;
        Position Position;
    };

    // Constructs an assertion framework instance for the specified test.
    AssertionFramework::AssertionFramework(Test* pTest)
        : m_pTest(pTest)
    {
    }

    // Internal assert count increment.
    void AssertionFramework::IncrementAssertCount()
    {
        m_pTest->IncrementAssertCount();
    }

	// Constructs an empty assertion failure.
	AssertionFailure::AssertionFailure()
		: DescriptionId(0),  MessageId(0), ActualValueId(0), ActualValueType(TypeRaw), ExpectedValueId(0), ExpectedValueType(TypeRaw)
	{
	}

	// ===================
	// Assertion Framework
	// ===================

	StringMap& AssertionFramework::Map() const
	{ 
		return TestFixture::GetStringMap(); 
	}

    // Assertion that makes inconditionally the test fail.
    void AssertionFramework::Fail(const char* message)
    {
        IncrementAssertCount();
		AssertionFailure failure;
		failure.DescriptionId = Map().Add("An assertion failed.");
		failure.MessageId = Map().Add(message);
		throw failure;
    }

	// Asserts that the specified boolean value is true.
	void AssertionFramework::IsTrue(bool actualValue, const char* message)
	{
        IncrementAssertCount();

		if (!actualValue)
		{
			AssertionFailure failure;
			failure.DescriptionId = Map().Add("Expected value to be true.");
			failure.ActualValueId = Map().Add("false");
			failure.ActualValueType = TypeBoolean;
			failure.MessageId = Map().Add(message);
			throw failure;
		}
	}

	void AssertionFramework::IsTrue(int actualValue, const char* message)
	{
		IsTrue(actualValue != 0, message);
	}

	// Asserts that the specified boolean value is false.
	void AssertionFramework::IsFalse(bool actualValue, const char* message)
	{
        IncrementAssertCount();

		if (actualValue)
		{
			AssertionFailure failure;
			failure.DescriptionId = Map().Add("Expected value to be false.");
			failure.ActualValueId = Map().Add("true");
			failure.ActualValueType = TypeBoolean;
			failure.MessageId = Map().Add(message);
			throw failure;
		}
	}

	void AssertionFramework::IsFalse(int actualValue, const char* message)
	{
		IsFalse(actualValue != 0, message);
	}

	#define _AssertionFramework_AreEqual(TYPE, INEQUALITY, FORMATEXPECTED, FORMATACTUAL, MANAGEDTYPE) \
	void AssertionFramework::AreEqual(TYPE expectedValue, TYPE actualValue, const char* message) \
	{ \
        IncrementAssertCount(); \
		\
		if (INEQUALITY) \
		{ \
			AssertionFailure failure; \
			failure.DescriptionId = Map().Add("Expected values to be equal."); \
			failure.ExpectedValueId = FORMATEXPECTED; \
			failure.ExpectedValueType = MANAGEDTYPE; \
			failure.ActualValueId = FORMATACTUAL; \
			failure.ActualValueType = MANAGEDTYPE; \
			failure.MessageId = Map().Add(message); \
			throw failure; \
		} \
	}

	_AssertionFramework_AreEqual(bool, 
		expectedValue != actualValue, 
		Map().Add(expectedValue ? "true" : "false"), 
		Map().Add(actualValue ? "true" : "false"), 
		TypeBoolean)

	_AssertionFramework_AreEqual(char, 
		expectedValue != actualValue, 
		Map().Add("%c", expectedValue), 
		Map().Add("%c", actualValue), 
		TypeChar)

	_AssertionFramework_AreEqual(__wchar_t, 
		expectedValue != actualValue, 
		Map().Add("%c", expectedValue), 
		Map().Add("%c", actualValue), 
		TypeChar)

	_AssertionFramework_AreEqual(unsigned char, 
		expectedValue != actualValue, 
		Map().Add("%u", expectedValue), 
		Map().Add("%u", actualValue), 
		TypeByte)

	_AssertionFramework_AreEqual(short, 
		expectedValue != actualValue, 
		Map().Add("%d", expectedValue), 
		Map().Add("%d", actualValue), 
		TypeInt16)

	_AssertionFramework_AreEqual(unsigned short, 
		expectedValue != actualValue, 
		Map().Add("%u", expectedValue), 
		Map().Add("%u", actualValue), 
		TypeUInt16)

	_AssertionFramework_AreEqual(int, 
		expectedValue != actualValue, 
		Map().Add("%d", expectedValue), 
		Map().Add("%d", actualValue), 
		TypeInt32)

	_AssertionFramework_AreEqual(unsigned int, 
		expectedValue != actualValue, 
		Map().Add("%u", expectedValue), 
		Map().Add("%u", actualValue), 
		TypeUInt32)

	_AssertionFramework_AreEqual(long long, 
		expectedValue != actualValue, 
		Map().Add("%u", expectedValue), 
		Map().Add("%u", actualValue), 
		TypeInt64)

	_AssertionFramework_AreEqual(float, 
		expectedValue != actualValue, 
		Map().Add("%f", expectedValue), 
		Map().Add("%f", actualValue), 
		TypeSingle)

	_AssertionFramework_AreEqual(double, 
		expectedValue != actualValue, 
		Map().Add("%Lf", expectedValue), 
		Map().Add("%Lf", actualValue), 
		TypeDouble)

	_AssertionFramework_AreEqual(char*, 
		strcmp(expectedValue, actualValue) != 0, 
		Map().Add(expectedValue), 
		Map().Add(actualValue), 
		TypeString)

	_AssertionFramework_AreEqual(const char*, 
		strcmp(expectedValue, actualValue) != 0, 
		Map().Add(expectedValue), 
		Map().Add(actualValue), 
		TypeString)

	// ======================================
	// Interface functions for Gallio adapter
	// ======================================

    extern "C" 
    {
        void __cdecl MbUnitCpp_GetHeadTest(Position* pPosition)
        {
            TestFixtureList& list = TestFixture::GetTestFixtureList();
            TestFixture* pFirstTestFixture = list.GetHead();
            pPosition->pTestFixture = pFirstTestFixture;
            pPosition->pTest = 0;
        }

        int __cdecl MbUnitCpp_GetNextTest(Position* pPosition, TestInfoData* pTestInfoData)
        {
            TestFixture* pTestFixture = pPosition->pTestFixture;
            Test* pTest = pPosition->pTest;

            if (pTestFixture == 0)
                return 0;
            
            if (pTest == 0)
            {
                pTestInfoData->IsTestFixture = true;
                pTestInfoData->FileName = 0;
                pTestInfoData->LineNumber = 0;
                pTestInfoData->Name = pTestFixture->GetName();
                pTestInfoData->Index = pTestFixture->GetIndex();
                pTestInfoData->Position.pTestFixture = pTestFixture;
                pTestInfoData->Position.pTest = 0;
                pPosition->pTest = pTestFixture->GetTestList().GetHead();
                return 1;            
            }

            pTestInfoData->IsTestFixture = false;
            pTestInfoData->FileName = pTest->GetFileName();
            pTestInfoData->LineNumber = pTest->GetLineNumber();
            pTestInfoData->Name = pTest->GetName();
            pTestInfoData->Index = pTest->GetIndex();
            pTestInfoData->Position.pTestFixture = pTestFixture;
            pTestInfoData->Position.pTest = pTest;
            pPosition->pTest = pTest->GetNext();

            if (pPosition->pTest == 0)
                pPosition->pTestFixture = pTestFixture->GetNext();
            
            return 1;
        }

        void __cdecl MbUnitCpp_RunTest(Position* pPosition, TestResultData* pTestResultData)
        {
            Test* pTest = pPosition->pTest;
            pTest->Run(pTestResultData);
        }

		char* __cdecl MbUnitCpp_GetString(StringId stringId)
		{
			StringMap& map = TestFixture::GetStringMap();
			return map.Get(stringId)->GetData();
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



