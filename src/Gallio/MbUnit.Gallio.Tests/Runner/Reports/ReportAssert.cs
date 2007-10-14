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
using System.Text;
using MbUnit.Runner;
using MbUnit.Runner.Reports;
using MbUnit.Model;
using MbUnit.Tests.Framework;
using MbUnit.Tests.Model;
using MbUnit.Framework;

namespace MbUnit.Tests.Runner.Reports
{
    /// <summary>
    /// Provides assertions for common Gallio core types.
    /// </summary>
    public static class ReportAssert
    {
        public static void AreEqual(Report expected, Report actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            AreEqual(expected.Package, actual.Package);
            ModelAssert.AreEqual(expected.TemplateModel, actual.TemplateModel);
            ModelAssert.AreEqual(expected.TestModel, actual.TestModel);
            AreEqual(expected.PackageRun, actual.PackageRun);
        }

        public static void AreEqual(TestPackage expected, TestPackage actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.ApplicationBase, actual.ApplicationBase);
            CollectionAssert.AreElementsEqual(expected.AssemblyFiles, actual.AssemblyFiles);
            Assert.AreEqual(expected.EnableShadowCopy, actual.EnableShadowCopy);
            CollectionAssert.AreElementsEqual(expected.HintDirectories, actual.HintDirectories);
        }

        public static void AreEqual(PackageRun expected, PackageRun actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.StartTime, actual.StartTime);
            Assert.AreEqual(expected.EndTime, actual.EndTime);
            AreEqual(expected.Statistics, actual.Statistics);

            MbUnit.Framework.InterimAssert.WithPairs(expected.TestRuns, actual.TestRuns, AreEqual);
        }

        public static void AreEqual(PackageRunStatistics expected, PackageRunStatistics actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.AssertCount, actual.AssertCount);
            Assert.AreEqual(expected.Duration, actual.Duration);
            Assert.AreEqual(expected.FailureCount, actual.FailureCount);
            Assert.AreEqual(expected.IgnoreCount, actual.IgnoreCount);
            Assert.AreEqual(expected.PassCount, actual.PassCount);
            Assert.AreEqual(expected.RunCount, actual.RunCount);
            Assert.AreEqual(expected.SkipCount, actual.SkipCount);
            Assert.AreEqual(expected.TestCount, actual.TestCount);
        }

        public static void AreEqual(TestRun expected, TestRun actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.TestId, actual.TestId);
            AreEqual(expected.RootStepRun, actual.RootStepRun);
        }

        public static void AreEqual(StepRun expected, StepRun actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            ModelAssert.AreEqual(expected.Step, actual.Step);
            Assert.AreEqual(expected.StartTime, actual.StartTime);
            Assert.AreEqual(expected.EndTime, actual.EndTime);
            ModelAssert.AreEqual(expected.Result, actual.Result);
            AreEqual(expected.ExecutionLog, actual.ExecutionLog);

            MbUnit.Framework.InterimAssert.WithPairs(expected.Children, actual.Children, AreEqual);
        }

        public static void AreEqual(ExecutionLog expected, ExecutionLog actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            InterimAssert.WithPairs(expected.Attachments, actual.Attachments, AreEqual);
            InterimAssert.WithPairs(expected.Streams, actual.Streams, AreEqual);
        }

        public static void AreEqual(ExecutionLogStream expected, ExecutionLogStream actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.ToString(), actual.ToString());

            // FIXME: not precise
        }

        public static void AreEqual(ExecutionLogAttachment expected, ExecutionLogAttachment actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.ContentType, actual.ContentType);
            Assert.AreEqual(expected.Encoding, actual.Encoding);
            Assert.AreEqual(expected.ContentPath, actual.ContentPath);
            Assert.AreEqual(expected.InnerText, actual.InnerText);
            Assert.AreEqual(expected.InnerXml, actual.InnerXml);
        }
    }
}