// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Reflection;
using Xunit;
using Xunit.Sdk;

namespace MbUnit.TestResources.Xunit
{
    /// <summary>
    /// Verifies Xunit RunWith behavior.
    /// </summary>
    [RunWith(typeof(CustomTestClassCommand))]
    public class RunWithSample
    {
        public class CustomTestClassCommand : ITestClassCommand
        {
            public ClassResult Execute(TestResultCallback callback)
            {
                ClassResult classResult = new ClassResult(typeof(RunWithSample));

                Dictionary<string, string> noProperties = new Dictionary<string, string>();
                Dictionary<string, string> sampleProperties = new Dictionary<string, string>();
                sampleProperties.Add("Description", "A test with properties.");
                sampleProperties.Add("NullValue", null);

                AddResultAndInvokeCallback(classResult, new PassedResult("Pass", "RunWithSample", noProperties), callback);
                AddResultAndInvokeCallback(classResult, new FailedResult("Fail", "RunWithSample", noProperties, "Boom!", "The stack trace..."), callback);
                AddResultAndInvokeCallback(classResult, new SkipResult("Skip", "RunWithSample", noProperties, "Skipped!"), callback);
                AddResultAndInvokeCallback(classResult, new PassedResult("PassWithProperties", "RunWithSample", sampleProperties), callback);

                if (callback != null)
                    callback(classResult);

                return classResult;
            }

            public ClassResult Execute(ICollection<MethodInfo> methods, TestResultCallback callback)
            {
                return Execute(callback);
            }

            public Type TypeUnderTest
            {
                set { }
            }

            private void AddResultAndInvokeCallback(ClassResult classResult, MethodResult methodResult, TestResultCallback callback)
            {
                classResult.Add(methodResult);

                if (callback != null)
                    callback(methodResult);
            }
        }
    }
}
