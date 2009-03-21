using System.Xml.Serialization;

namespace Gallio.Utilities
{
    ///<summary>
    /// Wrapper for XmlSerializationUtils static methods (allows testing).
    ///</summary>
    public interface IXmlSerializer
    {
        /// <summary>
        /// Saves an object graph to a pretty-printed Xml file using <see cref="XmlSerializer" />.
        /// </summary>
        /// <param name="root">The root object</param>
        /// <param name="filename">The filename</param>
        /// <typeparam name="T">The root object type</typeparam>
        void SaveToXml<T>(T root, string filename);

        /// <summary>
        /// Loads an object graph from an Xml file using <see cref="XmlSerializer" />.
        /// </summary>
        /// <param name="filename">The filename</param>
        /// <returns>The root object</returns>
        /// <typeparam name="T">The root object type</typeparam>
        T LoadFromXml<T>(string filename);
    }
}
