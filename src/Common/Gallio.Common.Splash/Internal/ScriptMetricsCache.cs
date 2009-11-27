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
