using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Gallio.Common.Splash.Native;

namespace Gallio.Common.Splash.Internal
{
    /// <summary>
    /// A low-level wrapper for a graphics context.
    /// </summary>
    internal sealed class DeviceContext : IDisposable
    {
        public readonly IntPtr HDC;

        private readonly Graphics graphics;
        private readonly ScriptMetricsCache scriptMetricsCache;
        private Font currentFont;
        private ScriptMetrics currentScriptMetrics;

        public DeviceContext(Graphics graphics, ScriptMetricsCache scriptMetricsCache)
        {
            this.graphics = graphics;
            HDC = graphics.GetHdc();
            this.scriptMetricsCache = scriptMetricsCache;
        }

        public void Dispose()
        {
            graphics.ReleaseHdc(HDC);
        }

        public ScriptMetrics SelectFont(Font font)
        {
            if (currentFont != font)
            {
                NativeMethods.SelectObject(HDC, font.ToHfont());
                currentFont = font;
                currentScriptMetrics = scriptMetricsCache[currentFont];

                if (!currentScriptMetrics.HaveMetrics)
                {
                    TEXTMETRIC textMetric;
                    NativeMethods.GetTextMetrics(HDC, out textMetric);

                    currentScriptMetrics.Height = textMetric.tmHeight;
                    currentScriptMetrics.Descent = textMetric.tmDescent;
                    currentScriptMetrics.HaveMetrics = true;
                }
            }
 
            return currentScriptMetrics;
        }

        public void SetBkMode(int bkMode)
        {
            NativeMethods.SetBkMode(HDC, bkMode);
        }

        public void SetPenColor(Color color)
        {
            NativeMethods.SelectObject(HDC, NativeMethods.GetStockObject(NativeConstants.DC_PEN));
            NativeMethods.SetDCPenColor(HDC, ToCOLORREF(color));
        }

        public void SetBrushColor(Color color)
        {
            NativeMethods.SelectObject(HDC, NativeMethods.GetStockObject(NativeConstants.DC_BRUSH));
            NativeMethods.SetDCBrushColor(HDC, ToCOLORREF(color));
        }

        public void SetTextColor(Color color)
        {
            NativeMethods.SetTextColor(HDC, ToCOLORREF(color));
        }

        private static int ToCOLORREF(Color color)
        {
            return (color.B << 16) | (color.G << 8) | color.R;
        }
    }
}
