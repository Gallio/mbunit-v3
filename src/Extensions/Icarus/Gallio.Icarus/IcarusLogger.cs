// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using Castle.Core.Logging;

using Gallio.Icarus.Interfaces;

namespace Gallio.Icarus
{
    public class IcarusLogger : LevelFilteredLogger
    {
        private IProjectAdapterView projectAdapterView;

        public IcarusLogger(IProjectAdapterView projectAdapterView)
        {
            this.projectAdapterView = projectAdapterView;
        }

        /// <inheritdoc />
        public override ILogger CreateChildLogger(string name)
        {
            return new IcarusLogger(projectAdapterView);
        }

        /// <inheritdoc />
        protected override void Log(LoggerLevel level, string name, string message, Exception exception)
        {
            projectAdapterView.WriteToLog(level, name, message, exception);
        }
    }
}