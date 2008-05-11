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


namespace Gallio.Runtime.ProgressMonitoring
{
    /// <summary>
    /// A null progress monitor presenter simply does nothing.
    /// </summary>
    public sealed class NullProgressMonitorPresenter : IProgressMonitorPresenter
    {
        /// <summary>
        /// Gets the singleton instance of the presenter.
        /// </summary>
        public static readonly NullProgressMonitorPresenter Instance = new NullProgressMonitorPresenter();

        private NullProgressMonitorPresenter()
        {
        }

        /// <inheritdoc />
        public void Present(ObservableProgressMonitor progressMonitor)
        {
        }
    }
}
