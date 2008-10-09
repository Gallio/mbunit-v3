namespace Gallio.Ambience.Tests
{
    public class Item
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public override bool Equals(object obj)
        {
            Item other = (Item) obj;
            return Name == other.Name && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Value.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", Name, Value);
        }
    }
}