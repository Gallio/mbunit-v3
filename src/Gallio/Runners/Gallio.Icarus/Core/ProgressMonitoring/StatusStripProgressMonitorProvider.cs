// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Icarus.Core.Interfaces;
using Gallio.Core.ProgressMonitoring;

namespace Gallio.Icarus.Core.ProgressMonitoring
{
    /// <summary>
    /// Runs tasks with a <see cref="StatusStripProgressMonitorPresenter" />.
    /// </summary>
    public class StatusStripProgressMonitorProvider : BaseProgressMonitorProvider
    {
        private readonly IProjectPresenter presenter;

        /// <summary>
        /// Creates a status strip progress monitor provider.
        /// </summary>
        /// <param name="presenter">The presenter to which messages should be written</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="presenter"/> is null</exception>
        public StatusStripProgressMonitorProvider(IProjectPresenter presenter)
        {
            if (presenter == null)
                throw new ArgumentNullException(@"presenter");

            this.presenter = presenter;
        }

        /// <inheritdoc />
        protected override IProgressMonitorPresenter GetPresenter()
        {
            return new StatusStripProgressMonitorPresenter(presenter);
        }
    }
}