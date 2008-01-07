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

namespace Gallio.Runner.Domains
{
    /// <summary>
    /// Wraps a logger so that it can be accessed remotely.
    /// </summary>
    [Serializable]
    public class RemoteLogger : LevelFilteredLogger // note: this is a subclass of MarshalByRefObject
    {
        private readonly ILogger logger;

        private RemoteLogger(ILogger logger)
        {
            this.logger = logger;
            Level = LoggerLevel.Debug;
        }

        /// <summary>
        /// Obtains a logger that can be used remotely.
        /// </summary>
        /// <param name="logger">The logger to wrap</param>
        /// <returns>The wrapped logger</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null</exception>
        public static ILogger Wrap(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            if (logger is MarshalByRefObject)
                return logger;

            return new RemoteLogger(logger);
        }

        /// <inheritdoc />
        public override ILogger CreateChildLogger(string name)
        {
            return Wrap(logger.CreateChildLogger(name));
        }

        /// <inheritdoc />
        protected override void Log(LoggerLevel level, string name, string message, Exception exception)
        {
            switch (level)
            {
                case LoggerLevel.Fatal:
                    logger.Fatal(message, exception);
                    break;

                case LoggerLevel.Error:
                    logger.Error(message, exception);
                    break;

                case LoggerLevel.Warn:
                    logger.Warn(message, exception);
                    break;

                case LoggerLevel.Info:
                    logger.Info(message, exception);
                    break;

                case LoggerLevel.Debug:
                    logger.Debug(message, exception);
                    break;
            }
        }
    }
}
