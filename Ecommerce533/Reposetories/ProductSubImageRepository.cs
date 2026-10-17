using Ecommerce533.Data;

namespace Ecommerce533.Reposetories
{
    public class ProductSubImageRepository :Repository<ProductSubImage> , IProductSubImageRepository
    {
        public ProductSubImageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void RemoveRange(IEnumerable<ProductSubImage> productSubImages)
        {
            _context.RemoveRange(productSubImages); 
        }
    }
}
