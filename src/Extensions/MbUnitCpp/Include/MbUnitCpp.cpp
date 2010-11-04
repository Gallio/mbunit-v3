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
        : m_index(index), m_name(name), m_fileName(fileName), m_lineNumber(lineNumber)
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
            RunImpl();
            pTestResultData->Outcome = PASSED;
        }
        catch (AssertionFailure failure)
        {
            pTestResultData->Outcome = FAILED;
            pTestResultData->Message = failure.GetMessage();
        }
    }

    // Default empty implementation of the test execution.
    void Test::RunImpl() const
    {
    }

    // Constructs an empty list of tests.
    TestList::TestList()
        : m_head(NULL), m_tail(NULL), m_nextIndex(0)
    {
    }

    // Adds a new test at the end of the list.
    void TestList::Add(Test* test)
    {
        if (m_tail == NULL)
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
        : m_head(NULL), m_tail(NULL), m_nextIndex(0)
    {
    }

    // Adds a new test fixture at the end of the list.
    void TestFixtureList::Add(TestFixture* pTestFixture)
    {
        if (m_tail == NULL)
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

    // Constructs an assertion failure with the specified message.
    AssertionFailure::AssertionFailure(char const* message)
        : m_message(message)
    {
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
        char const* Name;
        int Index;
        bool IsTestFixture;
        char const* FileName;
        int LineNumber;
        Position Position;
    };

    void Test::Assert::Fail(const char* reason)
    {
        throw MbUnitCpp::AssertionFailure(reason);
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
            pPosition->pTest = NULL;
        }

        int __cdecl MbUnitCpp_GetNextTest(Position* pPosition, TestInfoData* pTestInfoData)
        {
            TestFixture* pTestFixture = pPosition->pTestFixture;
            Test* pTest = pPosition->pTest;

            if (pTestFixture == NULL)
                return 0;
            
            if (pTest == NULL)
            {
                pTestInfoData->IsTestFixture = true;
                pTestInfoData->FileName = NULL;
                pTestInfoData->LineNumber = 0;
                pTestInfoData->Name = pTestFixture->GetName();
                pTestInfoData->Index = pTestFixture->GetIndex();
                pTestInfoData->Position.pTestFixture = pTestFixture;
                pTestInfoData->Position.pTest = NULL;
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

            if (pPosition->pTest == NULL)
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



