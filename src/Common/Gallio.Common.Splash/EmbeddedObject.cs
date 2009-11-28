using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// An embedded object that can be drawn into a <see cref="SplashView" />.
    /// </summary>
    public abstract class EmbeddedObject
    {
        /// <summary>
        /// Creates an instance of an embedded object client attached to the specified site.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns>The client.</returns>
        public abstract IEmbeddedObjectClient CreateClient(IEmbeddedObjectSite site);
    }
}
