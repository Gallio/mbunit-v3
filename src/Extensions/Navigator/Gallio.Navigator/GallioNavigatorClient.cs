using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Navigator
{
    /// <summary>
    /// Abstract class for a service that uses <see cref="IGallioNavigator"/>.
    /// </summary>
    public abstract class GallioNavigatorClient
    {
        private IGallioNavigator navigator;

        /// <summary>
        /// Gets the navigator service.
        /// </summary>
        protected IGallioNavigator Navigator
        {
            get
            {
                lock (this)
                {
                    if (navigator == null)
                        navigator = new GallioNavigatorImpl();
                    return navigator;
                }
            }
        }
    }
}
