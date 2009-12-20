namespace Gallio.Icarus.Specifications
{
    public class AnySpecification<T> : ISpecification<T>
    {
        public bool Matches(T item)
        {
            return true;
        }
    }
}
