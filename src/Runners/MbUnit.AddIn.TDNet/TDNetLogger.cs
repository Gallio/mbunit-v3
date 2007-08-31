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
using Castle.Core.Logging;
using TestDriven.Framework;
using TDF = TestDriven.Framework;

namespace MbUnit.AddIn.TDNet
{
    internal class TDNetLogger : ConsoleLogger
    {
        private readonly ITestListener tddLogger = null;
        public TDNetLogger(ITestListener testListener)
            : this(testListener, "TDNetLogger")
        {
        }

        public TDNetLogger(ITestListener testListener, string name)
            : base(name)
        {
            tddLogger = testListener;
            Level = LoggerLevel.Info;
        }

        protected override void Log(LoggerLevel level, string name, string message, Exception exception)
        {
            switch (level)
            {
                case LoggerLevel.Fatal:
                case LoggerLevel.Error:
                    tddLogger.WriteLine(message, Category.Error);
                    break;

                case LoggerLevel.Warn:
                    tddLogger.WriteLine(message, Category.Warning);
                    break;

                case LoggerLevel.Info:
                    tddLogger.WriteLine(message, Category.Info);
                    break;

                case LoggerLevel.Debug:
                    tddLogger.WriteLine(message, Category.Debug);
                    break;
            }

            if (exception != null)
                tddLogger.WriteLine(exception.ToString(), Category.Error);
        }

        public override ILogger CreateChildLogger(string newName)
        {
            //TODO: Check if this is OK
            return new TDNetLogger(tddLogger, newName);
        }
    }
}
