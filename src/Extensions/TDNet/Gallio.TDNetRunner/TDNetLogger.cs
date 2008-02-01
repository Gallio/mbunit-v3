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
using Castle.Core.Logging;
using Gallio.Utilities;
using TestDriven.Framework;
using TDF = TestDriven.Framework;

namespace Gallio.TDNetRunner
{
    /// <summary>
    /// An <see cref="ILogger" /> implementation that writes messages to a
    /// <see cref="ITestListener" /> object.
    /// </summary>
    internal class TDNetLogger : ConsoleLogger
    {
        private readonly ITestListener tdNetLogger = null;

        /// <summary>
        /// Initializes a new instance of the TDNetLogger class.
        /// </summary>
        /// <param name="testListener">An ITestListener object where the
        /// messages will be written to.</param>
        public TDNetLogger(ITestListener testListener)
            : this(testListener, "TDNetLogger")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TDNetLogger" /> class.
        /// </summary>
        /// <param name="testListener">An <see cref="ITestListener" /> object where the
        /// messages will be written to.</param>
        /// <param name="name">The name of the logger.</param>
        public TDNetLogger(ITestListener testListener, string name)
            : base(name)
        {
            tdNetLogger = testListener;
            Level = LoggerLevel.Info;
        }

        /// <inheritdoc />
        protected override void Log(LoggerLevel level, string name, string message, Exception exception)
        {
            switch (level)
            {
                case LoggerLevel.Fatal:
                case LoggerLevel.Error:
                    tdNetLogger.WriteLine(message, Category.Error);
                    break;

                case LoggerLevel.Warn:
                    tdNetLogger.WriteLine(message, Category.Warning);
                    break;

                case LoggerLevel.Info:
                    tdNetLogger.WriteLine(message, Category.Info);
                    break;

                case LoggerLevel.Debug:
                    tdNetLogger.WriteLine(message, Category.Debug);
                    break;
            }

            if (exception != null)
                tdNetLogger.WriteLine(ExceptionUtils.SafeToString(exception), Category.Error);
        }

        /// <inheritdoc />
        public override ILogger CreateChildLogger(string newName)
        {
            //TODO: Check if this is OK
            return new TDNetLogger(tdNetLogger, newName);
        }
    }
}
