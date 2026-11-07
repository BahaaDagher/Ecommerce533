using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace Ecommerce533.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IRepository<Cart> cartRepository, UserManager<ApplicationUser> userManager, IRepository<Product> productRepository)
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var carts = await _cartRepository.GetListAsync(c=>c.ApplicationUserId == user.Id , includes: [c=>c.Product]);
            return View(carts);
        }
        public async Task<IActionResult> AddToCart(int productId  , int count)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var product = await _productRepository.GetOneAsync(p=>p.Id == productId);
            if (product is null) return NotFound(); 
            if(product.Quantity< count)
            {
                return BadRequest();
            }
            var ExcitProduct = await _cartRepository.GetOneAsync( c=>
                c.ApplicationUserId == user.Id && 
                c.ProductId == productId
            );  
            if(ExcitProduct != null)
            {
                var x = ExcitProduct.Count + count;
                if (product.Quantity < x)
                {
                    return BadRequest();
                }
                ExcitProduct.Count += count;
                await _cartRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }

            var cart = new Cart()
            {
                ApplicationUserId = user.Id,
                ProductId = productId,
                Count = count
            };
            await _cartRepository.AddAsync(cart); 
            await _cartRepository.CommitAsync(); 

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> IncrementProduct(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var product = await _productRepository.GetOneAsync(p => p.Id == productId);
            if (product is null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(c =>
               c.ApplicationUserId == user.Id &&
               c.ProductId == productId
            );
            if (cart == null) return NotFound();
            
            if(cart.Count == product.Quantity)
            {
                TempData["Error_Notification"] = "Stop"; 
                return RedirectToAction(nameof(Index));
            }
            cart.Count++;
            await _cartRepository.CommitAsync(); 
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DecrementProduct(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var product = await _productRepository.GetOneAsync(p => p.Id == productId);
            if (product is null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(c =>
               c.ApplicationUserId == user.Id &&
               c.ProductId == productId
            );
            if (cart == null) return NotFound();

            if (cart.Count ==1)
            {
                TempData["Error_Notification"] = "Stop";
                return RedirectToAction(nameof(Index));
            }
            cart.Count--;
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var product = await _productRepository.GetOneAsync(p => p.Id == productId);
            if (product is null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(c =>
               c.ApplicationUserId == user.Id &&
               c.ProductId == productId
            );
            if (cart == null) return NotFound();

            _cartRepository.Remove(cart); 
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Pay()
        {
            var options = new SessionCreateOptions
            {
                AllowedPaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/cancel",
            };
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(); 
            var carts = await _cartRepository.GetListAsync(c=>c.ApplicationUserId == user.Id , includes: [c=>c.Product]);
            foreach(var item in carts)
            {
                options.LineItems.Add(new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            Description = item.Product.Description,
                        },
                        UnitAmount =  (long)(item.Product.Price * 100) ,
                    },
                    Quantity = item.Count,
                });
            }
            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url); 
        }

    }
}
