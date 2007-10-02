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
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.ExecutionLogs;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Core.RuntimeSupport
{
    /// <summary>
    /// A core context service provider grants access to resources that a context needs
    /// to have in order to operate.
    /// </summary>
    public interface ICoreContextServiceProvider
    {
        /// <summary>
        /// Gets the step.
        /// </summary>
        IStep Step { get; }

        /// <summary>
        /// Gets the step's log writer.
        /// </summary>
        LogWriter LogWriter { get; }

        /// <summary>
        /// Gets or sets the step's lifecycle phase.
        /// </summary>
        /// <seealso cref="LifecyclePhases"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        string LifecyclePhase { get; set; }

        /// <summary>
        /// Runs a block of code as a new step.
        /// </summary>
        /// <remarks>
        /// This method may be called recursively to create nested steps or concurrently
        /// to create parallel steps.
        /// </remarks>
        /// <param name="name">The name of the step</param>
        /// <param name="block">The block of code to run</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or
        /// <paramref name="block"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is the empty string</exception>
        /// <exception cref="Exception">Any exception thrown by the block</exception>
        void RunStep(string name, Block block);
    }
}
