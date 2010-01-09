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

        private DeviceContext(Graphics graphics, ScriptMetricsCache scriptMetricsCache)
        {
            this.graphics = graphics;
            this.scriptMetricsCache = scriptMetricsCache;

            HDC = graphics != null ? graphics.GetHdc() : NativeMethods.GetDC(IntPtr.Zero);
        }

        public static DeviceContext CreateFromGraphics(Graphics graphics, ScriptMetricsCache scriptMetricsCache)
        {
            return new DeviceContext(graphics, scriptMetricsCache);
        }

        public static DeviceContext CreateFromScreen(ScriptMetricsCache scriptMetricsCache)
        {
            return new DeviceContext(null, scriptMetricsCache);
        }

        public void Dispose()
        {
            if (graphics != null)
                graphics.ReleaseHdc(HDC);
            else
                NativeMethods.ReleaseDC(IntPtr.Zero, HDC);
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

        public static IntPtr GetStockObject(int fnObject)
        {
            return NativeMethods.GetStockObject(fnObject);
        }

        public void SetDCPenColor(Color color)
        {
            NativeMethods.SetDCPenColor(HDC, ToCOLORREF(color));
        }

        public void SetDCBrushColor(Color color)
        {
            NativeMethods.SetDCBrushColor(HDC, ToCOLORREF(color));
        }

        public void SetTextColor(Color color)
        {
            NativeMethods.SetTextColor(HDC, ToCOLORREF(color));
        }

        public static void DeleteObject(IntPtr hObject)
        {
            NativeMethods.DeleteObject(hObject);
        }

        public static IntPtr CreateRectRegion(Rectangle rect)
        {
            return NativeMethods.CreateRectRgn(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public bool GetClipRegion(IntPtr hRgn)
        {
            return NativeMethods.GetClipRgn(HDC, hRgn) > 0;
        }

        public void SelectClipRegion(IntPtr hRgn)
        {
            NativeMethods.SelectClipRgn(HDC, hRgn);
        }

        public void ExcludeClipRect(Rectangle rect)
        {
            NativeMethods.ExcludeClipRect(HDC, rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public void XorClipRegion(IntPtr hRgn)
        {
            NativeMethods.ExtSelectClipRgn(HDC, hRgn, NativeConstants.RGN_XOR);
        }

        public void FillRect(Rectangle rectangle, IntPtr hBrush)
        {
            RECT rect = new RECT()
            {
                left = rectangle.Left,
                top = rectangle.Top,
                right = rectangle.Right,
                bottom = rectangle.Bottom
            };
            NativeMethods.FillRect(HDC, ref rect, hBrush);
        }

        private static int ToCOLORREF(Color color)
        {
            return (color.B << 16) | (color.G << 8) | color.R;
        }
    }
}
