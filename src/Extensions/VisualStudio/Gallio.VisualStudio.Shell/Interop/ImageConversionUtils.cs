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

using System.Drawing;
using System.Windows.Forms;
using stdole;

namespace Gallio.VisualStudio.Shell.Interop
{
    /// <summary>
    /// Converts images to native formats.
    /// </summary>
    /// <remarks>
    /// <para>
    /// From <a href="http://weblogs.asp.net/rosherove/archive/2008/08/02/declarative-visual-studio-addin-buttons-with-icons.aspx">Roy Osherove's blog</a>.
    /// </para>
    /// </remarks>
    internal abstract class ImageConversionUtils : AxHost
    {
        private ImageConversionUtils() : base("") { }

        public static IPictureDisp GetIPictureDispFromImage(Image image)
        {
            if (image == null)
                return null;
            return (IPictureDisp) GetIPictureDispFromPicture(image);
        }
    }
}
