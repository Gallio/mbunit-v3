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

using System;
using System.Collections.Generic;
using System.Linq;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.Framework.Data;
using Gallio.Framework.Pattern;
using MbUnit.Framework;
using Gallio.Framework;
using Gallio.Tests;
using Gallio.Runner.Reports.Schema;
using Gallio.Model;
using Gallio.Common.Markup;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [RunSample(typeof(SharedParamFixtureTest))]
    public class SharedDataSetTest : BaseTestWithSampleRunner
    {
        public class SharedDataAttribute : DataAttribute
        {
            protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, Gallio.Common.Reflection.ICodeElementInfo codeElement)
            {
                dataSource.AddDataSet(new SharedDataSet());
                base.PopulateDataSource(scope, dataSource, codeElement);
            }
        }

        public class SharedDataSet : IDataSet
        {
            public IEnumerable<IDataItem> GetItems(ICollection<DataBinding> bindings, bool includeDynamicItems)
            {
                yield return new SharedDataItem();
            }

            public bool CanBind(DataBinding binding)
            {
                return true;
            }

            public int ColumnCount
            {
                get { return 0; }
            }
        }

        public class SharedDataItem : IDataItem
        {
            public object GetValue(DataBinding binding)
            {
                var parameterBinding = binding as TestParameterDataBinding;
                if (parameterBinding == null)
                    throw new InvalidOperationException();

                var slotInfo = parameterBinding.TestParameter.CodeElement as ISlotInfo;
                if (slotInfo == null)
                    throw new InvalidOperationException();

                if (typeof(IService) == slotInfo.ValueType.Resolve(true))
                    return new ConcreteService();
                if (typeof(IService1) == slotInfo.ValueType.Resolve(true))
                    return new ConcreteService1();

                throw new InvalidOperationException();
            }

            public void PopulateMetadata(PropertyBag map)
            {
            }

            public IEnumerable<DataBinding> GetBindingsForInformalDescription()
            {
                return Enumerable.Empty<DataBinding>();
            }

            public bool IsDynamic
            {
                get { return true; }
            }
        }


        public interface IService
        {
            void DoSomething();
        }
        public class DummyServiceImplementationBase : IService
        {
            public void DoSomething()
            {
                TestLog.WriteLine("Service is {0}", GetType().Name);
            }
        }

        public class ConcreteService : DummyServiceImplementationBase, IService
        {
        }

        public interface IService1 : IService
        {
        }

        public class ConcreteService1 : DummyServiceImplementationBase, IService1
        {
        }

        [Test]
        public void UseInstanceParameters()
        {
            TestStepRun[] runs = Runner.GetTestStepRuns(CodeReference.CreateFromMember(typeof(SharedParamFixtureTest).GetMethod("UseInstanceParameters"))).ToArray();
            Assert.Count(2, runs);
            Assert.ForAll(runs, run => run.Result.Outcome == TestOutcome.Passed);
            AssertLogLike(runs[0], "Parameter is 123|456", MarkupStreamNames.Default);
            AssertLogContains(runs[0], "Service is ConcreteService");
            AssertLogContains(runs[0], "Service is ConcreteService1");
            AssertLogLike(runs[1], "Parameter is 123|456", MarkupStreamNames.Default);
            AssertLogContains(runs[1], "Service is ConcreteService");
            AssertLogContains(runs[1], "Service is ConcreteService1");
        }

        [SharedData, Explicit("Sample")]
        public class SharedParamFixtureTest
        {
            private readonly IService service;
            private readonly IService1 service1;
            private readonly int parameter = 3;

            public SharedParamFixtureTest(IService service, IService1 service1, [Column(123, 456)] int parameter)
            {
                this.service = service;
                this.service1 = service1;
                this.parameter = parameter;
            }

            [Test]
            public void UseInstanceParameters()
            {
                TestLog.WriteLine("Parameter is {0}", parameter);
                service.DoSomething();
                service1.DoSomething();
            }
        }
    }
}
