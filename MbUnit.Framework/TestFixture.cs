namespace MbUnit.Framework
{
    using System;
    using System.Reflection;
    using System.Collections;
    using TestDriven.UnitTesting.Compatibility;
    using TestDriven.UnitTesting;

    sealed class TestFixture : TestFixtureBase
    {
        object fixtureObject;
        bool isExplicit;

        public TestFixture(Type fixtureType)
            : base(fixtureType)
        {
            addFixtureSetUpTearDown(fixtureType);
            addSetUpTearDown(fixtureType);
            addIsExplicit(fixtureType);
            addCategories();
        }

        void addCategories()
        {
            object[] attributes = CompatibleAttributeUtilities.GetCustomAttributes(
                this.FixtureType, typeof(CategoryAttribute), true);
            foreach (CategoryAttribute attribute in attributes)
            {
                this.Categories.Add(attribute.Name);
            }
        }

        private void addFixtureSetUpTearDown(Type fixtureType)
        {
            foreach (MethodInfo method in fixtureType.GetMethods())
            {
                foreach (TestFixtureSetUpAttribute testFixtureSetUpAttribute in CompatibleAttributeUtilities.GetCustomAttributes(method, typeof(TestFixtureSetUpAttribute), false))
                {
                    foreach (ITestCase testFixtureSetUp in testFixtureSetUpAttribute.CreateTests(this, method))
                    {
                        this.SetUp = testFixtureSetUp;
                    }
                }

                foreach (TestFixtureTearDownAttribute testFixtureTearDownAttribute in CompatibleAttributeUtilities.GetCustomAttributes(method, typeof(TestFixtureTearDownAttribute), false))
                {
                    foreach (ITestCase testFixtureTearDown in testFixtureTearDownAttribute.CreateTests(this, method))
                    {
                        this.TearDown = testFixtureTearDown;
                    }
                }
            }
        }

        void addSetUpTearDown(Type fixtureType)
        {
            MethodInfo setUpMethod = null;
            MethodInfo tearDownMethod = null;
            foreach (MethodInfo method in fixtureType.GetMethods())
            {
                foreach (SetUpAttribute setUpAttribute in CompatibleAttributeUtilities.GetCustomAttributes(method, typeof(SetUpAttribute), false))
                {
                    setUpMethod = method;
                }

                foreach (TearDownAttribute setUpAttribute in CompatibleAttributeUtilities.GetCustomAttributes(method, typeof(TearDownAttribute), false))
                {
                    tearDownMethod = method;
                }
            }

            if (setUpMethod != null || tearDownMethod != null)
            {
                this.TestCaseDecorators.Add(new SetUpTearDownTestCaseDecorator(setUpMethod, tearDownMethod));
            }
        }

        private void addIsExplicit(Type fixtureType)
        {
            object[] attributes = CompatibleAttributeUtilities.GetCustomAttributes(fixtureType, typeof(ExplicitAttribute), false);
            this.isExplicit = attributes.Length != 0;
        }

        public override bool IsExplicit
        {
            get { return this.isExplicit; }
        }

        public override object CreateInstance()
        {
            if (this.fixtureObject == null)
            {
                this.fixtureObject = Activator.CreateInstance(this.FixtureType);
            }

            return this.fixtureObject;
        }

        public override ITestCase[] CreateTestCases()
        {
            ArrayList tests = new ArrayList();
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (MethodInfo method in this.FixtureType.GetMethods(bindingFlags))
            {
                object[] decorators = CompatibleAttributeUtilities.GetCustomAttributes(method, typeof(TestDecoratorAttributeBase), true);
                foreach (TestAttributeBase attribute in CompatibleAttributeUtilities.GetCustomAttributes(method, typeof(TestAttributeBase), true))
                {
                    foreach (ITestCase test in attribute.CreateTests(this, method))
                    {
                        ITestCase decoratedTest = test;
                        foreach (ITestCaseDecorator decorator in this.TestCaseDecorators)
                        {
                            decoratedTest = decorator.Decorate(decoratedTest);
                        }

                        foreach (ITestCaseDecorator decorator in decorators)
                        {
                            decoratedTest = decorator.Decorate(decoratedTest);
                        }

                        tests.Add(decoratedTest);
                    }
                }
            }

            return (ITestCase[])tests.ToArray(typeof(ITestCase));
        }
    }
}
