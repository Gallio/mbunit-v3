// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using TestDriven.Framework;

namespace Gallio.TDNetRunner.Facade
{
    /// <summary>
    /// Utilities for translating facade types.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type is part of a facade that decouples the Gallio test runner from the TestDriven.Net interfaces.
    /// </para>
    /// </remarks>
    public static class FacadeUtils
    {
        public static TestResult ToTestResult(FacadeTestResult result)
        {
            if (result == null)
                return null;

            return new TestResult
            {
                State = ToTestState(result.State),
                Message = result.Message,
                Name = result.Name,
                StackTrace = result.StackTrace,
                TimeSpan = result.TimeSpan,
                TotalTests = result.TotalTests
            };
        }

        public static TestState ToTestState(FacadeTestState state)
        {
            switch (state)
            {
                case FacadeTestState.Passed:
                    return TestState.Passed;

                case FacadeTestState.Failed:
                    return TestState.Failed;

                case FacadeTestState.Ignored:
                    return TestState.Ignored;

                default:
                    throw new ArgumentOutOfRangeException("state");
            }
        }

        public static Category ToCategory(FacadeCategory category)
        {
            switch (category)
            {
                case FacadeCategory.Debug:
                    return Category.Debug;

                case FacadeCategory.Info:
                    return Category.Info;

                case FacadeCategory.Warning:
                    return Category.Warning;

                case FacadeCategory.Error:
                    return Category.Error;

                case FacadeCategory.Output:
                    return Category.Output;

                default:
                    throw new ArgumentOutOfRangeException("category");
            }
        }

        public static TestRunState ToTestRunState(FacadeTestRunState state)
        {
            switch (state)
            {
                case FacadeTestRunState.Success:
                    return TestRunState.Success;

                case FacadeTestRunState.Failure:
                    return TestRunState.Failure;

                case FacadeTestRunState.Error:
                    return TestRunState.Error;

                case FacadeTestRunState.NoTests:
                    return TestRunState.NoTests;

                default:
                    throw new ArgumentOutOfRangeException("state");
            }
        }

        public static string ToCref(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return String.Concat(@"E:", member.DeclaringType.FullName, ".", member.Name);

                case MemberTypes.Field:
                    return String.Concat(@"F:", member.DeclaringType.FullName, ".", member.Name);

                case MemberTypes.Constructor:
                case MemberTypes.Method:
                    return String.Concat(@"M:", member.DeclaringType.FullName, ".", member.Name);

                case MemberTypes.Property:
                    return String.Concat(@"P:", member.DeclaringType.FullName, ".", member.Name);

                case MemberTypes.TypeInfo:
                case MemberTypes.NestedType:
                    return String.Concat(@"T:", ((Type)member).FullName);

                default:
                    return String.Concat("?", member.ToString());
            }
        }
    }
}
