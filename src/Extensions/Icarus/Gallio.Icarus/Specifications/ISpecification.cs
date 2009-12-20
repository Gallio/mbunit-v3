namespace Gallio.Icarus.Specifications
{
    public interface ISpecification<T>
    {
        bool Matches(T item);
    }
}