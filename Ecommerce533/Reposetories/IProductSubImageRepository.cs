namespace Ecommerce533.Reposetories
{
    public interface IProductSubImageRepository : IRepository<ProductSubImage>
    {
        void RemoveRange(IEnumerable<ProductSubImage> productSubImages); 
    }
}
