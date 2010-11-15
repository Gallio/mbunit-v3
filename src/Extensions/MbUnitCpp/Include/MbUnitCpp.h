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

namespace MbUnitCpp
{
	// A simple general purpose string container with formatting capabilities.
	class String
	{
		char* m_data;
		void Initialize(const char* format, va_list argList);

		public:
		String(const char* format, ...);
		String(const char* format, va_list args);
		~String();
		char* GetData() const { return m_data; };
	};

	// A node that reference a mapped string value.
	struct StringMapNode
	{
		int Key;
		String* Data;
		StringMapNode* Next;
	};

	// Key type for the string map.
	#define StringId int

	// A simple list of strings (chained list)
	// TODO: Use a hash table for more efficiency.
	class StringMap
	{
		StringMapNode* m_head;
		StringId m_nextId;

		public:
		StringMap();
		~StringMap();
		void RemoveAll();
		String* Get(StringId key);
		StringId Add(String* data);
		StringId Add(const char* format, ...);
		void Remove(StringId key);
	};

    class Test;
    class TestList;
    class TestFixtureList;

    // Standard outcome of a test.
    enum Outcome
    {
        Inconclusive,
        Passed,
        Failed,
    };

	// The inner type of an actual/expected value.
	// Will be used by the Gallio test adapter to parse and represent the values properly.
	enum ValueType
	{
		// A raw string that represents a custom/user type.
		// Not parsed and displayed as it is.
		TypeRaw,

		// A string type copied later in a System.String.
		// Displayed with diffing if both the actual and expected values are available.
		TypeString,

		// A boolean type (should be "true" or "false")
		// Parsed by the test adapater with System.Boolean.Parse.
		TypeBoolean,

		// A simple character. Parsed with System.Char.Parse.
		TypeChar,

		// Primitive values parsed with the corresponding parsing method (System.Byte.Parse, System.Int16.Parse, etc.)
		TypeByte,
		TypeInt16,
		TypeUInt16,
		TypeInt32,
		TypeUInt32,
		TypeInt64,
		TypeSingle,
		TypeDouble,
	};

    // Represents a single assertion failure.
    struct AssertionFailure
    {
        StringId DescriptionId;
        StringId MessageId;
        StringId ActualValueId;
		ValueType ActualValueType;
        StringId ExpectedValueId;
		ValueType ExpectedValueType;
		AssertionFailure();
    };

    // Describes the result of a test.
    struct TestResultData
    {
        Outcome NativeOutcome;
        int AssertCount;
		AssertionFailure Failure;
	};

    // The MbUnitCpp Assertion Framework.
    class AssertionFramework
    {
        Test *m_pTest;
        void IncrementAssertCount();
		StringMap& Map() const;

        public:
        AssertionFramework(Test* pTest);

		// Outcome assertions.
        void Fail(const char* message = 0);

		// Logic assertions.
		void IsTrue(bool actualValue, const char* message = 0);
		void IsFalse(bool actualValue, const char* message = 0);
		void IsTrue(int actualValue, const char* message = 0); // Sometimes, boolean values are just int's (e.g. BOOL)
		void IsFalse(int actualValue, const char* message = 0);

		// Equality assertions.
		void AreEqual(bool expectedValue, bool actualValue, const char* message = 0);
		void AreEqual(char expectedValue, char actualValue, const char* message = 0);
		void AreEqual(__wchar_t expectedValue, __wchar_t actualValue, const char* message = 0);
		void AreEqual(unsigned char expectedValue, unsigned char actualValue, const char* message = 0);
		void AreEqual(short expectedValue, short actualValue, const char* message = 0);
		void AreEqual(unsigned short expectedValue, unsigned short actualValue, const char* message = 0);
		void AreEqual(int expectedValue, int actualValue, const char* message = 0);
		void AreEqual(unsigned int expectedValue, unsigned int actualValue, const char* message = 0);
		void AreEqual(long long expectedValue, long long actualValue, const char* message = 0);
		void AreEqual(float expectedValue, float actualValue, const char* message = 0);
		void AreEqual(double expectedValue, double actualValue, const char* message = 0);
		void AreEqual(char* expectedValue, char* actualValue, const char* message = 0);
		void AreEqual(const char* expectedValue, const char* actualValue, const char* message = 0);
    };

    // Base class for executable tests.
    class Test
    {
        int m_index;
        char const* m_name;
        char const* m_fileName;
        int m_lineNumber;
        Test* m_next;
        int m_assertCount;

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
        virtual void RunImpl();
        void IncrementAssertCount();

        private:
        void Clear();

        protected:
        AssertionFramework Assert;
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
		static StringMap& GetStringMap();
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
}

// Macro to create a new test fixture.
#define TESTFIXTURE(Name) \
    using namespace MbUnitCpp; \
    namespace NamespaceTestFixture##Name \
    { \
        class TestFixture##Name : public TestFixture \
        { \
            public: \
            TestFixture##Name() : TestFixture(MbUnitCpp::TestFixture::GetTestFixtureList().GetNextIndex(), #Name) {} \
        } testFixtureInstance; \
        \
        MbUnitCpp::TestFixtureRecorder fixtureRecorder(MbUnitCpp::TestFixture::GetTestFixtureList(), &testFixtureInstance); \
    } \
    namespace NamespaceTestFixture##Name

// Macro to create a new test.
#define TEST(Name) \
    class Test##Name : public MbUnitCpp::Test \
    { \
        public: \
		Test##Name() : Test(testFixtureInstance.GetTestList().GetNextIndex(), #Name, __FILE__, __LINE__) {} \
        private: \
        virtual void RunImpl(); \
    } test##Name##Instance; \
    \
    MbUnitCpp::TestRecorder recorder##Name (testFixtureInstance.GetTestList(), &test##Name##Instance); \
    void Test##Name::RunImpl()
