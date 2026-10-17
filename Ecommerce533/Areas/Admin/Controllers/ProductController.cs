using Ecommerce533.Data;
using Ecommerce533.Models;
using Ecommerce533.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace Ecommerce533.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext(); 
        private readonly IRepository<Product> _productRepository ;//= new Repository<Product>();
        private readonly IRepository<Category> _categoryRepository;//= new Repository<Category>();
        private readonly IRepository<Brand> _brandRepository;//= new Repository<Brand>();
        private readonly IProductSubImageRepository _productSubImageRepository;//= new ProductSubImageRepository();
        private readonly IProductColorRepository _productColorRepository;//= new ProductColorRepository();

        public ProductController(IRepository<Product> productRepository, IRepository<Category> categoryRepository, IRepository<Brand> brandRepository, IProductSubImageRepository productSubImageRepository, IProductColorRepository productColorRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _productSubImageRepository = productSubImageRepository;
            _productColorRepository = productColorRepository;
        }

        public async Task<IActionResult> Index(FilterProductVM filter)
        {
            //var products = _context.Products.AsQueryable();
            var products = await _productRepository.GetListAsync(includes:[p => p.Category, p => p.Brand]);
            //products = products.Include(p => p.Category).Include(p => p.Brand);
            // filter
            if (filter.ProductName != null)
            {
                products = products.Where(p => p.Name.Contains(filter.ProductName));
                ViewBag.ProductName = filter.ProductName;
            }
            if (filter.MinPrice > 0)
            {
                products = products.Where(p => p.Price - (p.Price * p.Discount / 100) >= filter.MinPrice);
                ViewBag.MinPrice = filter.MinPrice;

            }
            if (filter.MaxPrice > 0)
            {
                products = products.Where(p => p.Price - (p.Price * p.Discount / 100) >= filter.MaxPrice);
                ViewBag.MaxPrice = filter.MaxPrice;

            }
            if (filter.CategoryId > 0)
            {
                products = products.Where(p => p.CategoryId == filter.CategoryId);
                ViewBag.CategoryId = filter.CategoryId;

            }
            if (filter.BrandId > 0)
            {
                products = products.Where(p => p.BrandId == filter.BrandId);
                ViewBag.BrandId = filter.BrandId;
            }
            if (filter.IsLowQuentity)
            {
                products = products.OrderBy(p => p.Quantity); 
                ViewBag.IsLowQuentity = filter.IsLowQuentity;
            }
            //ViewData["Categories"] = _context.Categories.ToList();
            //ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Categories = await _categoryRepository.GetListAsync();
            //ViewBag.Brands = _context.Brands.ToList();
            //ViewData["Brands"] = _context.Brands.ToList();
            ViewData["Brands"] = await _brandRepository.GetListAsync();
            ViewBag.TotalPages = (int)Math.Ceiling(products.Count() / 8.0);
            ViewBag.CurrentPage = filter.Page;

            products = products.Skip((filter.Page - 1) * 8).Take(8);
            return View(products.AsEnumerable());
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetListAsync();
            //var brands = _context.Brands.ToList();
            var brands =  await _brandRepository.GetListAsync();
            
            return View( new ProductVM()
            {
                Categories = categories.ToList(),
                Brands = brands.ToList()
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product , IFormFile ImageFile , List<IFormFile> SubImages , List<string> Colors)
        {
            if(ImageFile != null && ImageFile.Length>0)
            {
                //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName) ;
                var fileName = Guid.NewGuid().ToString() + "-" +ImageFile.FileName;
                //var filePath = "D:\\EraaSoft\\533\\project\\Ecommerce533\\Ecommerce533\\wwwroot\\images"; 
                var filePath = Path.Combine(Directory.GetCurrentDirectory() , "wwwroot\\images" , fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream); 
                }
                product.MainImg = fileName; 
            }
            //var savedProduct = _context.Products.Add(product);
            var savedProduct = await _productRepository.AddAsync(product);
            //_context.SaveChanges();
            await _productRepository.CommitAsync();

            // subimages 
            if (SubImages != null && SubImages.Count > 0)
            {
                foreach (var image in SubImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        // insert in wwwroot
                        var fileName = Guid.NewGuid().ToString() + "-" + image.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_subImages", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            image.CopyTo(stream);
                        }
                        // insert in DB
                        //_context.ProductSubImages.Add(new ProductSubImage()
                        await _productSubImageRepository.AddAsync(new ProductSubImage()
                        {
                            Image = fileName,
                            ProductId = savedProduct.Entity.Id
                        });   
                    }
                }
                //_context.SaveChanges(); 
                await _productSubImageRepository.CommitAsync(); 
            }
            if (Colors != null && Colors.Count > 0)
            {
                foreach(var color in Colors)
                {
                    //_context.ProductColors.Add(new ProductColor()
                    await _productColorRepository.AddAsync(new ProductColor()
                    {
                        Color = color,
                        ProductId = savedProduct.Entity.Id
                    }); 
                }
                //_context.SaveChanges(); 
                await _productColorRepository.CommitAsync(); 
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var product = _context.Products.FirstOrDefault(c=> c.Id == id ); 
            var product = await _productRepository.GetOneAsync(c => c.Id == id); 
            if (product == null)
            {
                return RedirectToAction("NotFoundPage" , "Home");
            }
            //var categories = _context.Categories.ToList();
            var categories = await _categoryRepository.GetListAsync();
            //var brands = _context.Brands.ToList();
            var brands = await _brandRepository.GetListAsync();
            return View(new ProductVM()
            {
                Product = product,
                Categories = categories.ToList(),
                Brands = brands.ToList() , 
                //ProductSubImages = _context.ProductSubImages.Where(p=>p.ProductId == id).ToList()  , 
                ProductSubImages =(await _productSubImageRepository.GetListAsync(p => p.ProductId == id)).ToList()  , 
                //ProductColors = _context.ProductColors.Where(p => p.ProductId == id).ToList()  , 
                ProductColors = (await _productColorRepository.GetListAsync(p => p.ProductId == id)).ToList()  , 
            });
        }
        [HttpPost]
        public async Task<IActionResult> Update(Product product ,  IFormFile ImageFile , List<IFormFile> SubImages, List<string> Colors)
        {
            //var brndInDb = _context.Products.AsNoTracking().FirstOrDefault(b => b.Id == product.Id);
            var brndInDb = await _productRepository.GetOneAsync(b => b.Id == product.Id , isTracked : false);

            if (ImageFile != null && ImageFile.Length > 0)
            {
                //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName) ;
                var fileName = Guid.NewGuid().ToString() + "-" + ImageFile.FileName;
                //var filePath = "D:\\EraaSoft\\533\\project\\Ecommerce533\\Ecommerce533\\wwwroot\\images"; 
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }
                product.MainImg = fileName;

                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", brndInDb.MainImg);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath); 
                }

            }
            else
            {
                product.MainImg = brndInDb.MainImg; 
            }
            //_context.Products.Update(product);
            //_context.SaveChanges();
            _productRepository.Update(product);
            await _productRepository.CommitAsync(); 
            // subimages 
            if (SubImages != null && SubImages.Count > 0)
            {
                //var oldSubImages = _context.ProductSubImages.Where(p=>p.ProductId == product.Id); 
                var oldSubImages = await _productSubImageRepository.GetListAsync(p => p.ProductId == product.Id);
                 

                // delete from wwwroot 
                foreach (var item in oldSubImages)
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_subImages", item.Image);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                // delete from DB
                // ****** 
                _productSubImageRepository.RemoveRange(oldSubImages);
                //_context.SaveChanges();
                await _productSubImageRepository.CommitAsync();

                foreach (var image in SubImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        // insert in wwwroot
                        var fileName = Guid.NewGuid().ToString() + "-" + image.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_subImages", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            image.CopyTo(stream);
                        }
                        // insert in DB
                        //_context.ProductSubImages.Add(new ProductSubImage()
                        await _productSubImageRepository.AddAsync(new ProductSubImage()
                        {
                            Image = fileName,
                            ProductId = product.Id
                        });
                    }
                }
                //_context.SaveChanges();
                await _productSubImageRepository.CommitAsync(); 
            }
            if (Colors != null && Colors.Count > 0)
            {
                //var oldColors = _context.ProductColors.Where(p => p.ProductId == product.Id);
                var oldColors = await _productColorRepository.GetListAsync(p => p.ProductId == product.Id);
                // delete from DB 
                // ****** 
                //_context.ProductColors.RemoveRange(oldColors);
                _productColorRepository.RemoveRange(oldColors);
                //_context.SaveChanges();
                await _productColorRepository.CommitAsync();

                foreach (var color in Colors)
                {
                    //_context.ProductColors.Add(new ProductColor()
                    await _productColorRepository.AddAsync(new ProductColor()
                    {
                        Color = color,
                        ProductId = product.Id
                    });
                }
                //_context.SaveChanges();
                await _productColorRepository.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            //var product = _context.Products.FirstOrDefault(c=>c.Id == id);
            var product = await _productRepository.GetOneAsync(c => c.Id == id); 
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", product.MainImg);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            //var oldSubImages = _context.ProductSubImages.Where(p => p.ProductId == product.Id);
            var oldSubImages = await _productSubImageRepository.GetListAsync(p => p.ProductId == product.Id);


            // delete from wwwroot 
            foreach (var item in oldSubImages)
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_subImages", item.Image);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            //_context.Products.Remove(product);
            //_context.SaveChanges();
            _productRepository.Remove(product);
            await _productRepository.CommitAsync();  
            return RedirectToAction(nameof(Index)); 
        }


    }
}
