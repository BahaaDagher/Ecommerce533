namespace Ecommerce533.Reposetories
{
    public interface IProductColorRepository : IRepository<ProductColor>
    {
        void RemoveRange(IEnumerable<ProductColor> productColors);
    }
}
