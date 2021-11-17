using FinalProject.Context;
using FinalProject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FinalProject.Controllers
{
    public class AccountController : Controller
    {
        private UsersContext db;
        public AccountController (UsersContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login (LoginModel model)
        {
            User user = await db.Users
                .Include(p => p.Role)
                .FirstOrDefaultAsync(p => p.PhoneNumber == model.PhoneNumber && p.Password == model.Password);
            var wrongPassUser = await db.Users.FirstOrDefaultAsync(p => p.PhoneNumber == model.PhoneNumber && p.Password != model.Password);
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    await Authenticate(user);
                    //var listId = db.Users.Where(p => p.PhoneNumber == model.PhoneNumber).Select(p => p.id).ToList();
                    //int id = listId[0];
                    return RedirectToAction("Index", "Account");
                }

                else if (wrongPassUser != null)
                {
                    ModelState.AddModelError("", "Неправильный пароль");
                }

                else if (user == null)
                {
                    ModelState.AddModelError("", "Данный пользователь не найден");
                }

                else
                {
                    ModelState.AddModelError("", "Некорректный логин и(или) пароль");
                }
            }
            else 
            {
                ModelState.AddModelError("", "Некорректный логин и(или) пароль");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Head")]
        public IActionResult Register ()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Head")]
        public async Task<IActionResult> Register (RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await db.Users.FirstOrDefaultAsync(p => p.PhoneNumber == model.PhoneNumber);
                if (user == null)
                {
                    user = new User { PhoneNumber = model.PhoneNumber, Password = model.Password };
                    Role obstetRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "Obstet");
                    if (obstetRole != null)
                        user.Role = obstetRole;
                    await db.Users.AddAsync(user);
                    await db.SaveChangesAsync();
                    await Authenticate(user);
                    return RedirectToAction("Index", "Account");
                }
                else if (user != null)
                {
                    ModelState.AddModelError("", "Данный пользователь уже существует");
                }

                else
                {
                    ModelState.AddModelError("", "Некорректный логин и(или) пароль");
                }
            }
            else
            {
                ModelState.AddModelError("", "Некорректный логин и(или) пароль");
            }
            return View(model);
        }

            private async Task Authenticate (User user)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.PhoneNumber),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
                };

                ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
            }

            public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        }


    }

