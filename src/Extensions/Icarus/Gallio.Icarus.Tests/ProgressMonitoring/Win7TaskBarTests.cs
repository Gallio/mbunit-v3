using System;
using System.ComponentModel;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.ProgressMonitoring;
using Gallio.Icarus.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.ProgressMonitoring
{
    internal class Win7TaskBarTests
    {
        private IntPtr windowHandle;
        private ITaskbarList4 taskBarList;
        private ITestController testController;

        private void EstablishContext()
        {
            windowHandle = new IntPtr();
            taskBarList = MockRepository.GenerateStub<ITaskbarList4>();
            testController = MockRepository.GenerateStub<ITestController>();
            new Win7TaskBar(windowHandle, taskBarList, testController);
        }

        [Test]
        public void Progress_bar_should_be_red_if_tests_have_failed()
        {
            EstablishContext();
            testController.Stub(ttm => ttm.Failed).Return(1);
            testController.Stub(ttm => ttm.Skipped).Return(1);
            testController.Stub(ttm => ttm.Passed).Return(1);
            
            testController.Raise(ttm => ttm.PropertyChanged += null, testController, 
                new PropertyChangedEventArgs("Passed"));

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressState(Arg<IntPtr>.Is.Anything, 
                Arg<TBPFLAG>.Is.Equal(TBPFLAG.TBPF_ERROR)));
        }

        [Test]
        public void Progress_bar_should_be_yellow_if_tests_have_been_skipped()
        {
            EstablishContext();
            testController.Stub(ttm => ttm.Skipped).Return(1);
            testController.Stub(ttm => ttm.Passed).Return(1);

            testController.Raise(ttm => ttm.PropertyChanged += null, testController,
                new PropertyChangedEventArgs("Passed"));

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressState(Arg<IntPtr>.Is.Anything,
                Arg<TBPFLAG>.Is.Equal(TBPFLAG.TBPF_PAUSED)));
        }

        [Test]
        public void Progress_bar_should_be_green_otherwise()
        {
            EstablishContext();
            testController.Stub(ttm => ttm.Passed).Return(1);

            testController.Raise(ttm => ttm.PropertyChanged += null, testController,
                new PropertyChangedEventArgs("Passed"));

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressState(Arg<IntPtr>.Is.Anything,
                Arg<TBPFLAG>.Is.Equal(TBPFLAG.TBPF_NORMAL)));
        }

        [Test]
        public void Progress_state_should_not_be_set_if_it_has_not_changed()
        {
            EstablishContext();
            testController.Stub(ttm => ttm.Passed).Return(1);

            testController.Raise(ttm => ttm.PropertyChanged += null, testController,
                new PropertyChangedEventArgs("Passed"));
            testController.Raise(ttm => ttm.PropertyChanged += null, testController,
                new PropertyChangedEventArgs("Passed"));

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressState(Arg<IntPtr>.Is.Anything,
                Arg.Is(TBPFLAG.TBPF_NORMAL)));
        }

        [Test]
        public void Progress_state_should_be_set_using_window_handle()
        {
            EstablishContext();
            testController.Stub(ttm => ttm.Passed).Return(1);

            testController.Raise(ttm => ttm.PropertyChanged += null, testController,
                new PropertyChangedEventArgs("Passed"));

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressState(Arg.Is(windowHandle),
                Arg<TBPFLAG>.Is.Anything));
        }

        [Test]
        public void Progress_value_completed_should_be_sum_of_test_statuses()
        {
            EstablishContext();
            testController.Stub(ttm => ttm.Passed).Return(3);
            testController.Stub(ttm => ttm.Failed).Return(1);
            testController.Stub(ttm => ttm.Skipped).Return(2);

            testController.Raise(ttm => ttm.PropertyChanged += null, testController,
                new PropertyChangedEventArgs("Passed"));

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressValue(Arg<IntPtr>.Is.Anything,
                Arg.Is(6UL), Arg<ulong>.Is.Anything));
        }

        [Test]
        public void Progress_value_total_should_be_test_count_from_model()
        {
            EstablishContext();
            testController.Stub(ttm => ttm.TestCount).Return(12);

            testController.Raise(ttm => ttm.PropertyChanged += null, testController,
                new PropertyChangedEventArgs("Passed"));

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressValue(Arg<IntPtr>.Is.Anything,
                Arg<ulong>.Is.Anything, Arg.Is(12UL)));
        }

        [Test]
        public void Progress_value_should_be_set_using_window_handle()
        {
            EstablishContext();
            testController.Stub(ttm => ttm.TestCount).Return(12);

            testController.Raise(ttm => ttm.PropertyChanged += null, testController,
                new PropertyChangedEventArgs("Passed"));

            taskBarList.AssertWasCalled(tbl => tbl.SetProgressValue(Arg.Is(windowHandle),
                Arg<ulong>.Is.Anything, Arg<ulong>.Is.Anything));
        }
    }
}
