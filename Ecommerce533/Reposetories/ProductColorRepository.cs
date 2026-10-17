using Ecommerce533.Data;

namespace Ecommerce533.Reposetories
{
    public class ProductColorRepository : Repository<ProductColor>, IProductColorRepository
    {
        public ProductColorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void RemoveRange(IEnumerable<ProductColor> productColors)
        {
            _context.RemoveRange(productColors);
        }
    }
}
