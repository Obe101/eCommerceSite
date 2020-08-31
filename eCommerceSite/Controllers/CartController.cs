using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerceSite.Data;
using eCommerceSite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace eCommerceSite.Controllers
{
    public class CartController : Controller
    {
        private readonly ProductContext _context;
        private readonly IHttpContextAccessor _httpContext;
        public CartController(ProductContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }
        /// <summary>
        /// /Adds product to shopping cart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Add(int id) // Id of the produuct to add
        {
            //Get product from the database
            Product p = await ProductDb.GetProductAsync(_context, id);
            const string CartCookie = "CartCookie";
            
            //Get existing cart items
            string exisingCart =_httpContext.HttpContext.Request.Cookies[CartCookie];
            List<Product> cartProducts = new List<Product>();
            if(exisingCart != null)
            {
                 cartProducts = JsonConvert.DeserializeObject<List <Product>>(exisingCart);
            }

            //Add current products to existing cart
            cartProducts.Add(p);

            //Add product to cart cookies
            string data = JsonConvert.SerializeObject(p);
            CookieOptions options = new CookieOptions()
            {
                Expires = DateTime.Now.AddYears(1),
                Secure = true,
                IsEssential = true,
            };
            _httpContext.HttpContext.Response.Cookies.Append(CartCookie, data, options);
            // Redirectback to previous page
            return RedirectToAction("Index", "Product");
        }

        public IActionResult Summary()
        {
            //Display all products in shopping cart cookie
            string cookieData = _httpContext.HttpContext.Request.Cookies["CartCookie"];

            List<Product> cartProducts = JsonConvert.DeserializeObject<List<Product>>(cookieData);
            return View();
        }
    }
}
