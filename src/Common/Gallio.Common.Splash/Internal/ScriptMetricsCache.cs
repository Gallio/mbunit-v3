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
using System.Drawing;
using System.Text;
using Gallio.Common.Splash.Native;

namespace Gallio.Common.Splash.Internal
{
    internal sealed class ScriptMetricsCache
    {
        private readonly Dictionary<Font, ScriptMetrics> dict;

        public ScriptMetricsCache()
        {
            dict = new Dictionary<Font, ScriptMetrics>();
        }

        ~ScriptMetricsCache()
        {
            Clear();
        }

        public void Clear()
        {
            foreach (ScriptMetrics scriptCache in dict.Values)
            {
                if (scriptCache.ScriptCache != IntPtr.Zero)
                    NativeMethods.ScriptFreeCache(ref scriptCache.ScriptCache);
            }
        }

        public ScriptMetrics this[Font font]
        {
            get
            {
                ScriptMetrics scriptMetrics;
                if (!dict.TryGetValue(font, out scriptMetrics))
                {
                    scriptMetrics = new ScriptMetrics();
                    dict.Add(font, scriptMetrics);
                }
                return scriptMetrics;
            }
        }
    }
}
