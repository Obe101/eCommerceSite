using System.Collections.Generic;
using System.Linq;
using eCommerceSite.Data;
using eCommerceSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceSite.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductContext _context;
        public ProductController(ProductContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Display a view that lists all products
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            //get all products from database
            List<Product> products = (
                from p in _context.Products
                select p).ToList();

            //send list of products to view to be displayed
            return View(products);
        }
    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Add(Product p)
    {
        if(ModelState.IsValid)
        {
                // Add to DB
                _context.Products.Add(p);
                _context.SaveChanges();

                //Add success message
                TempData["Message"] = $"{p.Title} was added successfully"; 

                // redirect back  to catalog page
                return RedirectToAction("Index");
        }

        return View();
    }
    }
}
