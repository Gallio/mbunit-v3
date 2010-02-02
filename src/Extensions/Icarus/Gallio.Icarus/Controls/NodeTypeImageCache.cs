using System.Collections.Generic;
using System.Drawing;
using Gallio.Model;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;

namespace Gallio.Icarus.Controls
{
    public static class NodeTypeImageCache
    {
        private static readonly object imageCacheLock = new object();
        private static Dictionary<string, Image> imageCache;

        static NodeTypeImageCache()
        {
            PopulateImageCache();
        }

        private static void PopulateImageCache() 
        {
            if (imageCache != null) 
                return;

            lock (imageCacheLock)
            {
                if (imageCache != null)
                    return;

                imageCache = new Dictionary<string, Image>();

                var testKindManager = RuntimeAccessor.ServiceLocator.Resolve<ITestKindManager>();

                foreach (var handle in testKindManager.TestKindHandles)
                {
                    AddImageToCache(handle);
                }
            }
        }

        private static void AddImageToCache(ComponentHandle<ITestKind, TestKindTraits> handle)
        {
            var traits = handle.GetTraits();
            
            if (traits.Icon == null)
                return;

            var image = new Icon(traits.Icon, 16, 16).ToBitmap();
            imageCache.Add(traits.Name, image);
        }

        public static Image GetNodeTypeImage(string nodeType)
        {
            Image nodeTypeImage;
            imageCache.TryGetValue(nodeType, out nodeTypeImage);
            return nodeTypeImage;
        }
    }
}
