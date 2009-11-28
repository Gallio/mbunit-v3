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
using Gallio.Icarus.Models;
using Gallio.Icarus.ProgressMonitoring;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Icarus.Utilities;
using Gallio.UI.DataBinding;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.ProgressMonitoring
{
    [Category("ProgressMonitoring"), TestsOn(typeof(Win7TaskBar)), Author("Graham Hay")]
    internal class Win7TaskBarTests
    {
        private IntPtr windowHandle;
        private ITaskbarList4 taskBarList;
        private ITestTreeModel testTreeModel;
        private Observable<int> passed;
        private Observable<int> failed;
        private Observable<int> skipped;
        private Observable<int> inconclusive;
        private Observable<int> testCount;

        private void EstablishContext()
        {
            windowHandle = new IntPtr();
            taskBarList = MockRepository.GenerateStub<ITaskbarList4>();
            testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testCount = new Observable<int>();
            testTreeModel.Stub(ttm => ttm.TestCount).Return(testCount);
            var testStatistics = MockRepository.GenerateStub<ITestStatistics>();
            passed = new Observable<int>();
            failed = new Observable<int>();
            skipped = new Observable<int>();
            inconclusive = new Observable<int>();
            testStatistics.Stub(ts => ts.Passed).Return(passed);
            testStatistics.Stub(ts => ts.Failed).Return(failed);
            testStatistics.Stub(ts => ts.Skipped).Return(skipped);
            testStatistics.Stub(ts => ts.Inconclusive).Return(inconclusive);
            new Win7TaskBar(windowHandle, taskBarList, testTreeModel, testStatistics);
        }

        [SyncTest]
        public void Progress_bar_should_be_red_if_tests_have_failed()
        {
            EstablishContext();

            failed.Value = 1;

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressState(Arg<IntPtr>.Is.Anything, 
                Arg<TBPFLAG>.Is.Equal(TBPFLAG.TBPF_ERROR)));
        }

        [SyncTest]
        public void Progress_bar_should_be_yellow_if_tests_have_been_skipped()
        {
            EstablishContext();

            skipped.Value = 1;

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressState(Arg<IntPtr>.Is.Anything,
                Arg<TBPFLAG>.Is.Equal(TBPFLAG.TBPF_PAUSED)));
        }

        [SyncTest]
        public void Progress_bar_should_be_green_if_tests_have_passed()
        {
            EstablishContext();

            passed.Value = 1;

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressState(Arg<IntPtr>.Is.Anything,
                Arg<TBPFLAG>.Is.Equal(TBPFLAG.TBPF_NORMAL)));
        }

        [SyncTest]
        public void Progress_bar_state_should_be_no_progress_if_no_tests_have_run()
        {
            EstablishContext();

            passed.Value = 1;
            passed.Value = 0;

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressState(Arg<IntPtr>.Is.Anything,
                Arg<TBPFLAG>.Is.Equal(TBPFLAG.TBPF_NOPROGRESS)));
        }

        [SyncTest]
        public void Progress_state_should_not_be_set_if_it_has_not_changed()
        {
            EstablishContext();

            passed.Value = 1;
            passed.Value = 2;

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressState(Arg<IntPtr>.Is.Anything,
                Arg.Is(TBPFLAG.TBPF_NORMAL)));
        }

        [SyncTest]
        public void Progress_state_should_be_set_using_window_handle()
        {
            EstablishContext();

            passed.Value = 1;

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressState(Arg.Is(windowHandle),
                Arg<TBPFLAG>.Is.Anything));
        }

        [SyncTest]
        public void Progress_value_completed_should_be_sum_of_test_statuses()
        {
            EstablishContext();

            passed.Value = 1;
            failed.Value = 2;
            skipped.Value = 3;

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressValue(Arg<IntPtr>.Is.Anything,
                Arg.Is(6UL), Arg<ulong>.Is.Anything));
        }

        [SyncTest]
        public void Progress_value_total_should_be_test_count_from_model()
        {
            EstablishContext();

            testCount.Value = 12;
            passed.Value = 1;

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressValue(Arg<IntPtr>.Is.Anything,
                Arg<ulong>.Is.Anything, Arg.Is(12UL)));
        }

        [SyncTest]
        public void Progress_value_should_be_set_using_window_handle()
        {
            EstablishContext();
            testTreeModel.Stub(ttm => ttm.TestCount).Return(new Observable<int>(12));

            passed.Value = 1;

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressValue(Arg.Is(windowHandle),
                Arg<ulong>.Is.Anything, Arg<ulong>.Is.Anything));
        }
    }
}
