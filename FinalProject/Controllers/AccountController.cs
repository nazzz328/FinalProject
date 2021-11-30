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
using System.Dynamic;

namespace FinalProject.Controllers
{
    public class AccountController : Controller
    {
        dynamic IndexModels = new ExpandoObject();
        dynamic GynecView = new ExpandoObject();
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
            var doctors = await db.Doctors.Where(p => p. Id != 1).Include(p => p.User).ToListAsync();
            var viewDocs = new List<DocViewModel>();
            foreach (var doc in doctors)
            {
                viewDocs.Add(new DocViewModel
                {
                    Id = doc.Id,
                    FirstName = doc.FirstName,
                    LastName = doc.LastName,
                    RusName = await db.Roles.Where(p => p.Id == doc.User.RoleId).Select(p => p.RusName).FirstOrDefaultAsync()
                }); ;
            }
            var initPatients = await db.Patients.Where(p => p.ProcessingStatus == 0).ToListAsync();
            var ProcPatients = await db.Patients.Where(p => p.ProcessingStatus == 1).ToListAsync();
            var viewInitPatients = new List<ViewPatient>();
            var viewProcPatients = new List<ViewPatient>();
            foreach (var item in initPatients)
            {
                viewInitPatients.Add(new ViewPatient
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    ReceiptDate = item.ReceiptDate
                });
            }
            foreach (var item in ProcPatients)
            {
                viewProcPatients.Add(new ViewPatient
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    ReceiptDate = item.ReceiptDate
                });
            }
            IndexModels.viewDocs = viewDocs;
            IndexModels.viewInitPatients = viewInitPatients;
            IndexModels.viewProcPatients = viewProcPatients;
            return View(IndexModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login (LoginModel model)
        {
            bool isVerified;
            var hashedPassword = db.Users.Where(p => p.PhoneNumber == model.PhoneNumber).Select(p => p.HashedPassword).FirstOrDefault();
            try
            {
                isVerified = Crypto.VerifyHashedPassword(hashedPassword, model.Password);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Данного пользователя не существует");
                return View(model);
            }
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
                Roles =await db.Roles.Where(p => p.Id != 1).Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.RusName }).ToListAsync()
        };
            return View(result);
        }

        [HttpGet]
        [Authorize (Roles = "Head")]
        public async Task<IActionResult> View(int id)
        {
            var doctor = await db.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return RedirectToAction("Index", "Account");
            }
            var user = await db.Users.Where(p => p.Id == doctor.UserId).Include(p => p.Role).FirstOrDefaultAsync();
            if (user == null)
            {
                return RedirectToAction("Index", "Account");
            }
            var fullDoc = new DocFullViewModel
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                MiddleName = doctor.MiddleName,
                DateOfBirth = doctor.DateOfBirth,
                Address = doctor.Address,
                PassportNumber = doctor.PassportNumber,
                PhoneNumber = user.PhoneNumber,
                RusName = user.Role.RusName
            };
            return View(fullDoc);
        }

        [HttpGet]
        [Authorize(Roles = "Obstet")]
        public async Task<IActionResult> ViewInitPatient(int id)
        {
            var patient = await db.Patients.FindAsync(id);
            if (patient == null)
            {
                return RedirectToAction("Index", "Account");
            }
            
            return View(patient);
        }

        [HttpGet]
        [Authorize(Roles = "Gynec")]
        public async Task<IActionResult> ViewProcPatient(int id)
        {
            var history = await db.Histories.FindAsync(id);
            if (history == null)
            {
                return RedirectToAction("Index", "Account");
            }
            var patient = await db.Patients.FirstOrDefaultAsync(p => p.Id == history.PatientId);
            GynecView.Patient = patient;
            GynecView.History = history;

            return View(GynecView);
        }

        [HttpGet]
        [Authorize(Roles = "Obstet")]
        public async Task<IActionResult> SubmitInitPatient(int id)
        {
            var patient = await db.Patients.FindAsync(id);
            if (patient == null)
            {
                return RedirectToAction("Index", "Account");
            }
            patient.ProcessingStatus = 1;
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Account");
        }

        [HttpGet]
        [Authorize(Roles = "Gynec")]
        public async Task<IActionResult> SubmitProcPatient(int id)
        {
            var history = await db.Histories.FindAsync(id);
            if (history == null)
            {
                return RedirectToAction("Index", "Account");
            }
            var patient = await db.Patients.FirstOrDefaultAsync(p => p.Id == history.PatientId);
            if (patient == null)
            {
                return RedirectToAction("Index", "Account");
            }
            patient.ProcessingStatus = 2;
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Account");
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
                model.Roles = await db.Roles.Where(p => p.Id != 1).Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.RusName }).ToListAsync();
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
            var user = await db.Users.Where(p => p.Id == id).FirstOrDefaultAsync();
            var doctor = new Doctor { FirstName = doctorModel.FirstName, LastName = doctorModel.LastName, MiddleName = doctorModel.MiddleName, Address = doctorModel.Address, CreatedDate = DateTime.Now, DateOfBirth = doctorModel.DateOfBirth, PassportNumber = doctorModel.PassportNumber, UserId = id, User = user};
            if (ModelState.IsValid)
            {
                await db.Doctors.AddAsync(doctor);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Account");
            }
            ModelState.AddModelError("", "Данные были введены неправильно");
            return View(doctor);
        }

        [HttpGet]
        [Authorize(Roles = "Obstet")]
        public IActionResult AddPatient ()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Obstet")]
        public async Task <IActionResult> AddPatient (Patient model)
        {
            int age;
            age = DateTime.Now.Subtract(model.DateOfBirth).Days;
            age = age / 365;
            var patient = new Patient { FirstName = model.FirstName, LastName = model.LastName, MiddleName = model.MiddleName, Address = model.Address, DateOfBirth = model.DateOfBirth, Age = age, Gender = model.Gender, Height = model.Height, PassportNumber = model.PassportNumber, ReceiptDate = DateTime.Now, Weight = model.Weight, BloodPressure = model.BloodPressure, Temperature = model.Temperature };
            if (ModelState.IsValid)
            {
                await db.Patients.AddAsync(patient);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Account");
            }
            ModelState.AddModelError("", "Данные были введены неправильно");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Obstet")]
        public async Task<IActionResult> EditInitPatient(int id)
        {
            var patient = await db.Patients.FindAsync(id);
            if (patient == null)
            {
                return RedirectToAction("Index", "Account");
            }
            return View(patient);
        }

        [HttpGet]
        [Authorize(Roles = "Gynec")]
        public async Task<IActionResult> EditProcPatient(int id)
        {
            var history = await db.Histories.FindAsync(id);
            if (history == null)
            {
                return RedirectToAction("Index", "Account");
            }
            return View(history);
        }

        [HttpGet]
        [Authorize(Roles = "Gynec")]
        public async Task<IActionResult> AddProcPatient(int id)
        {
            var patient = await db.Patients.FindAsync(id);
            if (patient == null)
            {
                return RedirectToAction("Index", "Account");
            }
            var history = new History { Patient = patient, PatientId = id };
            ViewBag.FirstName = patient.FirstName;
            ViewBag.LastName = patient.LastName;
            return View(history);
        }

        [HttpPost]
        [Authorize(Roles = "Gynec")]
        public async Task<IActionResult> AddProcPatient(History model)
        {
            var history = new History { Anamnesis = model.Anamnesis, Inspection = model.Inspection, Conclusion = model.Conclusion, Complaints = model.Complaints, PatientId = model.PatientId, Patient = model.Patient, CreatedAt = DateTime.Now, Treatment = model.Treatment };
            if (ModelState.IsValid)
            {
                await db.Histories.AddAsync(history);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Account");
            }
            ModelState.AddModelError("", "Данные были введены неправильно");
            return View(history);
        }

        [HttpPost]
        [Authorize(Roles = "Obstet")]
        public async Task<IActionResult> EditInitPatient(Patient model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Данные введены неправильно");
                return View(model);
            }
            var patient = await db.Patients.FindAsync(model.Id);
            if (patient == null)
            {
                return RedirectToAction("Index", "Account");
            }
            patient.FirstName = model.FirstName;
            patient.LastName = model.LastName;
            patient.MiddleName = model.MiddleName;
            patient.PassportNumber = model.PassportNumber;
            patient.DateOfBirth = model.DateOfBirth;
            patient.Address = model.Address;
            patient.Gender = model.Gender;
            patient.Temperature = model.Temperature;
            patient.BloodPressure = model.BloodPressure;
            patient.Weight = model.Weight;
            patient.Height = model.Height;
            patient.Age = model.Age;
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        [Authorize(Roles = "Gynec")]
        public async Task<IActionResult> EditProcPatient(History model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Данные введены неправильно");
                return View(model);
            }
            var history = await db.Histories.FindAsync(model.Id);
            if (history == null)
            {
                return RedirectToAction("Index", "Account");
            }
            history.Anamnesis = model.Anamnesis;
            history.Complaints = model.Complaints;
            history.Treatment = model.Treatment;
            history.Conclusion = model.Conclusion;
            history.Inspection = model.Inspection;
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Account");
        }


        [HttpGet]
        [Authorize(Roles = "Obstet")]
        public async Task<IActionResult> DeleteInitPatient(int id)
        {
            var patient = await db.Patients.FindAsync(id);
            if (patient == null)
            {
                return RedirectToAction("Index", "Account");
            }
            db.Patients.Remove(patient);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Account");
        }

        [HttpGet]
        [Authorize(Roles = "Gynec")]
        public async Task<IActionResult> DeleteProcPatient(int id)
        { 
            var history = await db.Histories.FindAsync(id);
            if (history == null)
            {
                return RedirectToAction("Index", "Account");
            }
            var patient = await db.Patients.FirstOrDefaultAsync(x => x.Id == history.PatientId);
            db.Histories.Remove(history);
            await db.SaveChangesAsync();
            if (patient == null)
            {
                return RedirectToAction("Index", "Account");
            }
            db.Patients.Remove(patient);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Account");
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

