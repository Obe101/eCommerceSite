using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerceSite.Data;
using eCommerceSite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eCommerceSite.Controllers
{
    public class UserController : Controller
    {
        private readonly ProductContext _context;

        public UserController(ProductContext context)
        {
            _context = context;
        }
        public IActionResult Register()
        {
            return View();
        }
        public  async Task<IActionResult> Register(RegisterViewModel reg)
        {
            if (ModelState.IsValid)
            {
                //Check if username/email is in use
                bool isEmailTaken = await (from account in _context.UserAccounts
                                     where account.Email == reg.Email
                                     select account).AnyAsync();
                
                //if so, add custom error and send back to view
                if (isEmailTaken)
                {
                    ModelState.AddModelError(nameof(RegisterViewModel.Email), "That email is already in use");
                }

                bool isUsernameTaken = await (from account in _context.UserAccounts
                                              where account.Username == reg.UserName
                                              select account).AnyAsync();
                if (isUsernameTaken)
                {
                    ModelState.AddModelError(nameof(RegisterViewModel.UserName), "That username is already in use");
                }
                if (isEmailTaken || isUsernameTaken)
                {
                    return View(reg);
                }

                //Map data to user accout instanc
                UserAccount acc = new UserAccount()
                { 
                    DateOfBirth = reg.DateOfBirth,
                    Email = reg.Email,
                    Password = reg.Password,
                    Username = reg.UserName

                };
                // add to database
                _context.UserAccounts.Add(acc);
                await _context.SaveChangesAsync();

                LogUserIn(acc.UserId);

                //redirect to home page
                return RedirectToAction("Index", "Home");
            }
            return View(reg);
        }
        public IActionResult Login()
        {
            // check if user already logged in
            if (HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            UserAccount account =
                              await (from u in _context.UserAccounts
                                     where (u.Username == model.UsernameOrEmail ||
                                          u.Email == model.UsernameOrEmail) &&
                                          u.Password == model.Password
                                     select u).SingleOrDefaultAsync();
            //Method syntax 
            //UserAccount account = 
            //        _context.UserAccounts
            //                .Where(userAcc => userAcc.UserName == model.UsernameOrEmail ||
            //                                   userAcc.email == model.UsernameOrEmail) &&
            //                                   UserAcc.Password == model.Password
            //                                   ).SingleOrDefaultAsync();
            if (account == null)
            {
                // Credentials did not match

                // Custom error message
                ModelState.AddModelError(string.Empty, "Credentials were not found");
                return View(model);
            }
            LogUserIn(account.UserId);

            return RedirectToAction("Index", "Home");
        }

        private void LogUserIn(int accountId)
        {
            //log user into website
            HttpContext.Session.SetInt32("UserId", accountId);
        }

        public IActionResult Logout()
        {
            //Removes all current session data
            HttpContext.Session.Clear();

            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }
    }
}
