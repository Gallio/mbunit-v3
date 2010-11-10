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

namespace MbUnitCpp
{
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
            pTestResultData->NativeOutcome = PASSED;
		}
        catch (AssertionFailure failure)
        {
            pTestResultData->AssertCount = m_assertCount;
            pTestResultData->NativeOutcome = FAILED;
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

	AssertionFailure::AssertionFailure(char const* description)
		: Description(description), Message(0), ActualValue(0), ExpectedValue(0)
	{
	}

    // Assertion that makes inconditionally the test fail.
    void AssertionFramework::Fail(const char* message)
    {
        IncrementAssertCount();
        AssertionFailure failure("An assertion failed.");
		failure.Message = message;
		throw failure;
    }

	void AssertionFramework::IsTrue(bool actualValue, const char* message)
	{
        IncrementAssertCount();

		if (!actualValue)
		{
			AssertionFailure failure("Expected value to be true.");
			failure.ActualValue = "false";
			failure.Message = message;
			throw failure;
		}
	}

	void AssertionFramework::IsFalse(bool actualValue, const char* message)
	{
        IncrementAssertCount();

		if (actualValue)
		{
			AssertionFailure failure("Expected value to be false.");
			failure.ActualValue = "true";
			failure.Message = message;
			throw failure;
		}
	}

    extern "C" 
    {
        int __cdecl MbUnitCpp_GetVersion()
        {
            return MBUNITCPP_VERSION;
        }

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
    }
}

#if defined(_WIN64) 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetVersion") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetHeadTest") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetNextTest") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_RunTest") 
#else 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetVersion=_MbUnitCpp_GetVersion") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetHeadTest=_MbUnitCpp_GetHeadTest") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetNextTest=_MbUnitCpp_GetNextTest") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_RunTest=_MbUnitCpp_RunTest") 
#endif 



