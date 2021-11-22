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
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Web.Helpers;

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
        public async Task <IActionResult> Index()
        {
            var Doctors = await db.Doctors.Where(p => p. Id != 1).Include(p => p.User).ToListAsync();
            var viewDocs = new List<DocViewModel>();
            foreach (var doc in Doctors)
            {
                viewDocs.Add(new DocViewModel
                {
                    Id = doc.Id,
                    FirstName = doc.FirstName,
                    LastName = doc.LastName,
                    RusName = await db.Roles.Where(p => p.Id == doc.User.RoleId).Select(p => p.RusName).FirstOrDefaultAsync()
                }); ;
            }
            return View(viewDocs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login (LoginModel model)
        {
            var hashedPassword = db.Users.Where(p => p.PhoneNumber == model.PhoneNumber).Select(p => p.HashedPassword).FirstOrDefault();
            var isVerified = Crypto.VerifyHashedPassword(hashedPassword, model.Password);
            User user = await db.Users
                .Include(p => p.Role)
                .FirstOrDefaultAsync(p => p.PhoneNumber == model.PhoneNumber && isVerified);
            var wrongPassUser = await db.Users.FirstOrDefaultAsync(p => p.PhoneNumber == model.PhoneNumber && p.HashedPassword != hashedPassword);
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    await Authenticate(user);
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
        public async Task <IActionResult> Register ()
        {
            var model = new RegisterModel
            {
                Roles = await db.Roles.Where(p => p.Id != 1).Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.RusName }).ToListAsync()
            };
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Head")]
        public async Task <IActionResult> Edit (int id)
        {
            var doctor = await db.Doctors.FindAsync(id); 
            if (doctor == null)
            {
                return RedirectToAction("Index", "Account");
            }
            var user = new User();
            user = await db.Users.Include(p => p.Role).FirstOrDefaultAsync(p => p.Id == doctor.UserId);
            var result = new DocEditModel
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                MiddleName = doctor.MiddleName,
                Address = doctor.Address,
                DateOfBirth = doctor.DateOfBirth,
                PassportNumber = doctor.PassportNumber,
                User = user,
                UserId = user.Id,
                Role = user.Role,
                RoleId = user.RoleId,
                Roles =await db.Roles.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.RusName }).ToListAsync()
        };
            return View(result);
        }

        [HttpGet]
        [Authorize (Roles = "Head")]
        public async Task<IActionResult> Delete (int id)
        {
            var doctor = await db.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return RedirectToAction("Index", "Account");
            }
            var user = await db.Users.FirstOrDefaultAsync(p => p.Id == doctor.UserId);
            if (user == null)
            {
                return RedirectToAction("Index", "Account");
            }
            db.Users.Remove(user);
            db.Doctors.Remove(doctor);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        [Authorize(Roles = "Head")]
        public async Task <IActionResult> Edit(DocEditModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = await db.Roles.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.RusName }).ToListAsync();
                return View(model);
            }
            var doctor = await db.Doctors.FindAsync(model.Id);
            if (doctor == null)
            {
                return RedirectToAction("Index", "Account");
            }
            doctor.FirstName = model.FirstName;
            doctor.LastName = model.LastName;
            doctor.MiddleName = model.MiddleName;
            doctor.DateOfBirth = model.DateOfBirth;
            doctor.Address = model.Address;
            doctor.PassportNumber = model.PassportNumber;
            var user = await db.Users.FindAsync(doctor.UserId);
            if (user == null)
            {
                return RedirectToAction("Index", "Account");
            }
            user.RoleId = model.RoleId;
            Role userRole = await db.Roles.FirstOrDefaultAsync(p => p.Id == user.RoleId);
            if (userRole != null)
            {
                user.Role = userRole;
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Account");
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
                    model.HashedPassword = Crypto.HashPassword(model.Password);
                    user = new User { PhoneNumber = model.PhoneNumber, HashedPassword = model.HashedPassword, RoleId = model.RoleId};
                    Role userRole = await db.Roles.FirstOrDefaultAsync(r => r.Id == model.RoleId);
                    if (userRole != null)
                        user.Role = userRole;
                    await db.Users.AddAsync(user);
                    await db.SaveChangesAsync();
                    ViewBag.Id = user.Id;
                    return RedirectToAction("FillDoctorInfo", new { id = user.Id });
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
            model.Roles = await db.Roles.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.RusName}).ToListAsync();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Head")]
        public IActionResult FillDoctorInfo()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Head")]
        public async Task <IActionResult> FillDoctorInfo(Doctor doctorModel, int id)
        {
            var doctor = new Doctor { FirstName = doctorModel.FirstName, LastName = doctorModel.LastName, MiddleName = doctorModel.MiddleName, Address = doctorModel.Address, CreatedDate = DateTime.Now, DateOfBirth = doctorModel.DateOfBirth, PassportNumber = doctorModel.PassportNumber, UserId = id};
            if (ModelState.IsValid)
            {
                await db.Doctors.AddAsync(doctor);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Account");
            }

            return View(doctor);
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
            return RedirectToAction("Index", "Home");
        }

        }


    }

