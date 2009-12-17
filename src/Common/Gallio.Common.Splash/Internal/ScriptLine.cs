// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Common.Splash.Internal
{
    /// <summary>
    /// Internal structure that maps a visual line onto a particular range of runs
    /// within a paragraph and includes the character position of required line breaks.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ScriptLine
    {
        /// <summary>
        /// The absolute vertical offset of the line in the document when rendered.
        /// </summary>
        public int Y;

        /// <summary>
        /// The absolute horizontal offset of the line in the document when rendered.
        /// </summary>
        public int X;

        /// <summary>
        /// The height of the line.
        /// </summary>
        public int Height;

        /// <summary>
        /// The descent height of the line (below text baseline).
        /// </summary>
        public int Descent;

        /// <summary>
        /// The ascent height of the line (above text baseline).
        /// </summary>
        public int Ascent
        {
            get { return Height - Descent; }
        }

        /// <summary>
        /// The index of the paragraph that appears on the line.
        /// </summary>
        public int ParagraphIndex;

        /// <summary>
        /// The index of the paragraph's first script run that appears on the line.
        /// </summary>
        public int ScriptRunIndex;

        /// <summary>
        /// The number of script runs that appear on the line.
        /// </summary>
        public int ScriptRunCount;

        /// <summary>
        /// The number of leading chars in the first script run to truncate due to word wrap.
        /// </summary>
        public int TruncatedLeadingCharsCount;

        /// <summary>
        /// The number of trailing chars in the last script run to truncate due to word wrap.
        /// </summary>
        public int TruncatedTrailingCharsCount;

        public void Initialize(int paragraphIndex, int scriptRunIndex, int y)
        {
            ParagraphIndex = paragraphIndex;
            ScriptRunIndex = scriptRunIndex;
            Y = y;
            X = 0;
            Height = 0;
            Descent = 0;
            ScriptRunCount = 0;
            TruncatedLeadingCharsCount = 0;
            TruncatedTrailingCharsCount = 0;
        }
    }
}
