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

using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Util;
using Gallio.Loader.Isolation;

namespace Gallio.ReSharperRunner.Provider.Facade
{
    /// <summary>
    /// A facade and remote proxy for the ReSharper logger utilities.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type is part of a facade that decouples the Gallio test runner from the ReSharper interfaces.
    /// </para>
    /// </remarks>
    public class AdapterFacadeLogger : MarshalByRefObject, IFacadeLogger
    {
        /// <inheritdoc />
        public void LogVerbose(string message)
        {
            try
            {
                Logger.LogMessage(LoggingLevel.VERBOSE, message);
            }
            catch (Exception ex)
            {
                throw ServerExceptionUtils.Wrap(ex);
            }
        }

        /// <inheritdoc />
        public void LogMessage(string message)
        {
            try
            {
                Logger.LogMessage(LoggingLevel.NORMAL, message);
            }
            catch (Exception ex)
            {
                throw ServerExceptionUtils.Wrap(ex);
            }
        }

        /// <inheritdoc />
        public void LogError(string message)
        {
            try
            {
                Logger.LogError(message);
            }
            catch (Exception ex)
            {
                throw ServerExceptionUtils.Wrap(ex);
            }
        }

        /// <inheritdoc />
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
