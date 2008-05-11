// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Runtime.ProgressMonitoring
{
    /// <summary>
    /// A progress monitor provider runs a task with progress monitoring
    /// and provides clear notification of cancelation in the form of
    /// an <see cref="OperationCanceledException" />.
    /// </summary>
    public interface IProgressMonitorProvider
    {
        /// <summary>
        /// Runs a task with a progress monitor.
        /// Throws <see cref="OperationCanceledException" /> if the task is canceled.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The provider should automatically dispose of the progress monitor
        /// in case the task forgets to do so.  It is also responsible for calling
        /// <see cref="IProgressMonitor.ThrowIfCanceled" /> before and after running
        /// the task to ensure that the <see cref="OperationCanceledException" /> is
        /// thrown as required.
        /// </para>
        /// <para>
        /// The provider can choose to execute the task in a different thread
        /// but the method should still complete synchronously.
        /// </para>
        /// </remarks>
        /// <param name="task">The task to run, never null</param>
        /// <exception cref="OperationCanceledException">Thrown if the task is canceled</exception>
        void Run(TaskWithProgress task);
    }
}