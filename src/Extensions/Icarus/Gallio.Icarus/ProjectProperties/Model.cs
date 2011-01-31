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

using Gallio.UI.DataBinding;

namespace Gallio.Icarus.ProjectProperties
{
    /// <summary>
    /// View model for Project Properties tab.
    /// </summary>
    public class Model : IModel
    {
        public Observable<string> ApplicationBaseDirectory { get; private set; }
        public ObservableList<string> HintDirectories { get; private set; }
        public Observable<string> ReportDirectory { get; private set; }
        public Observable<string> ReportNameFormat { get; private set; }
        public Observable<bool> ShadowCopy { get; private set; }
        public ObservableList<string> TestRunnerExtensionSpecifications { get; private set; }
        public Observable<string> WorkingDirectory { get; private set; }

        public Model()
        {
            ApplicationBaseDirectory = new Observable<string>();
            HintDirectories = new ObservableList<string>();
            ReportDirectory = new Observable<string>();
            ReportNameFormat = new Observable<string>();
            ShadowCopy = new Observable<bool>();
            TestRunnerExtensionSpecifications = new ObservableList<string>();
            WorkingDirectory = new Observable<string>();
        }
    }
}
