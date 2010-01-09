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

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// Specifies the verbosity of the output.
    /// </summary>
    public enum Verbosity
    {
        ///<summary>
        /// Will only display Error and Warning messages.
        ///</summary>
        Quiet = 0,
        /// <summary>
        /// Will display Important, Warning and Error messages.
        /// </summary>
        Normal,
        /// <summary>
        /// Will display all messages, except Debug.
        /// </summary>
        Verbose,
        /// <summary>
        /// Will display all messages.
        /// </summary>
        Debug
    }
}