using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerceSite.Data;
using eCommerceSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eCommerceSite.Controllers
{
    public class CartController : Controller
    {
        private readonly ProductContext _context;
        public CartController(ProductContext context)
        {
            _context = context;
        }
        /// <summary>
        /// /Adds product to shopping cart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Add(int id) // Id of the produuct to add
        {
            //Get product from the database
            Product p = await (from products in _context.Products
                               where products.ProductId == id
                               select products).SingleAsync();
            //Add product to cart cookies

            // Redirectback to previous page
            return View();
        }

        public IActionResult Summary()
        {
            //Display all products in shopping cart cookie
            return View();
        }
    }
}
