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

#pragma once
#define MBUNITCPP_VERSION 1

namespace MbUnitCpp
{
    class TestList;
    class TestFixtureList;

    // Standard outcome of a test.
    enum Outcome
    {
        INCONCLUSIVE = 0,
        PASSED = 1,
        FAILED = 2,
    };

    // Describes the result of test.
    struct TestResultData
    {
        Outcome Outcome;
        char const* Message;
    };

    // An executable test.
    class Test
    {
        private:
        int m_index;
        char const* m_name;
        char const* m_fileName;
        int m_lineNumber;
        Test* m_next;

        public:
        Test(int index, char const* name, char const* fileName, int lineNumber);
        ~Test();
        int GetIndex() const { return m_index; }
        char const* GetName() const { return m_name; }
        char const* GetFileName() const { return m_fileName; }
        int GetLineNumber() const { return m_lineNumber; }
        Test* GetNext() const { return m_next; }
        void SetNext(Test* test);
        void Run(TestResultData* pTestResultData);
        virtual void RunImpl() const;

        protected:

        // Assertion framework.
        class Assert
        {
            public:
            static void Fail(const char* reason = "Test failed");
        };
    };

    // A chained list of tests.
    class TestList
    {
        private:
        Test* m_head;
        Test* m_tail;
        int m_nextIndex;
    
        public:
        TestList();
        void Add(Test* test);
        Test* GetHead() const { return m_head; }
        int GetNextIndex();
    };

    // A test fixture that defines a sequence of related child tests.
    class TestFixture
    {
        private:
        int m_index;
        char const* m_name;
        TestList m_children;
        TestFixture* m_next;

        public:
        TestFixture(int index, char const* name);
        ~TestFixture();
        int GetIndex() const { return m_index; }
        TestFixture* GetNext() const { return m_next; }
        void SetNext(TestFixture* pTestFixture);
        char const* GetName() const { return m_name; }
        TestList& GetTestList();
        static TestFixtureList& GetTestFixtureList();
    };

    // A chained list of test fixtures
    class TestFixtureList
    {
        private:
        TestFixture* m_head;
        TestFixture* m_tail;
        int m_nextIndex;
    
        public:
        TestFixtureList();
        void Add(TestFixture* pTestFixture);
        TestFixture* GetHead() const { return m_head; }
        int GetNextIndex();
    };

    // Helper class to register a new test.
    class TestRecorder
    {
        public:
        TestRecorder(TestList& list, Test* pTest);
    };

    // Helper class to register a new test fixture.
    class TestFixtureRecorder
    {
        public:
        TestFixtureRecorder(TestFixtureList& list, TestFixture* pTestFixture);
    };

    // Represents a failing assertion.
    class AssertionFailure
    {
        private:
        char const* m_message;

        public:
        AssertionFailure(char const* message);
        char const* GetMessage() const { return m_message; }
    };
}

#define TESTFIXTURE(Name) \
    namespace NamespaceTestFixture##Name \
    { \
        class TestFixture##Name : public MbUnitCpp::TestFixture \
        { \
            public: \
            TestFixture##Name() : TestFixture(MbUnitCpp::TestFixture::GetTestFixtureList().GetNextIndex(), #Name) {} \
        } testFixtureInstance; \
        \
        MbUnitCpp::TestFixtureRecorder fixtureRecorder(MbUnitCpp::TestFixture::GetTestFixtureList(), &testFixtureInstance); \
    } \
    namespace NamespaceTestFixture##Name

#define TEST(Name) \
    class Test##Name : public MbUnitCpp::Test \
    { \
        public: \
        Test##Name() : Test(testFixtureInstance.GetTestList().GetNextIndex(), #Name, __FILE__, __LINE__) {} \
        private: \
        virtual void RunImpl() const; \
    } test##Name##Instance; \
    \
    MbUnitCpp::TestRecorder recorder##Name (testFixtureInstance.GetTestList(), &test##Name##Instance); \
    void Test##Name::RunImpl() const


