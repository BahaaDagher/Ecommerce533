using Ecommerce533.Data;
using Ecommerce533.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce533.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext(); 
        public IActionResult Index()
        {
            var brands = _context.Brands.AsQueryable();
            // filter
            return View(brands.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Brand brand , IFormFile ImageFile)
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
                brand.Logo = fileName; 
            }
            _context.Brands.Add(brand);
            _context.SaveChanges(); 
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            var brand = _context.Brands.FirstOrDefault(c=> c.Id == id ); 
            if(brand == null)
            {
                return RedirectToAction("NotFoundPage" , "Home");
            }
            return View(brand);
        }
        [HttpPost]
        public IActionResult Update(Brand brand ,  IFormFile ImageFile)
        {
            var brndInDb = _context.Brands.AsNoTracking().FirstOrDefault(b => b.Id == brand.Id);

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
                brand.Logo = fileName;

                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", brndInDb.Logo);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath); 
                }

            }
            else
            {
                brand.Logo = brndInDb.Logo; 
            }
            _context.Brands.Update(brand);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var brand = _context.Brands.FirstOrDefault(c=>c.Id == id);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", brand.Logo);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            _context.Brands.Remove(brand);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index)); 
        }


    }
}
