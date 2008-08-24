using System.Drawing;
using System.Windows.Forms;
using stdole;

namespace Gallio.VisualStudio.Toolkit.Interop
{
    /// <summary>
    /// <para>
    /// Converts images to native formats.
    /// </para>
    /// <para>
    /// From Roy Osherove's blog.
    /// http://weblogs.asp.net/rosherove/archive/2008/08/02/declarative-visual-studio-addin-buttons-with-icons.aspx
    /// </para>
    /// </summary>
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