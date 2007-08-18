using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Core.Tests
{
    /// <summary>
    /// Constructs mock test data model instances.
    /// </summary>
    public static class MockTestDataFactory
    {
        public static TemplateModel CreateEmptyTemplateModel()
        {
            TemplateInfo root = new TemplateInfo("root", "root");
            return new TemplateModel(root);
        }

        public static TestModel CreateEmptyTestModel()
        {
            TestInfo root = new TestInfo("root", "root");
            return new TestModel(root);
        }
    }
}
