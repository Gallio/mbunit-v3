namespace Gallio.Icarus.Utilities
{
    public interface IXmlSerialization
    {
        void SaveToXml<T>(T root, string filename);
        T LoadFromXml<T>(string filename);
    }
}
