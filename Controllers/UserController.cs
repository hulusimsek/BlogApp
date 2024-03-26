using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlogAppProjesi.Data.Concrete.EfCore;
using BlogAppProjesi.Entity;
using BlogAppProjesi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlogAppProjesi.Controllers
{
    public class UserController : Controller
    {
        private readonly BlogContext _context;
        public UserController(BlogContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Post");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult Register()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Post");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, IFormFile imageFile)
        {
            var extension = "";
            if (imageFile != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                extension = Path.GetExtension(imageFile.FileName); // abc.jpg

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("", "Geçerli bir resim seçiniz.");
                }
            }


            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName || x.Email == model.Email);
                    if (user == null)
                    {

                        var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        };
                        await _context.Users.AddAsync(new User
                        {
                            UserName = model.UserName,
                            Name = model.Name,
                            Email = model.Email,
                            Password = model.Password,
                            Image = randomFileName
                        });
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Username veya Email kullanımda");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Resim seçmediniz");
                }
            }
            else
                {
                    ModelState.AddModelError("", "Bilinmeyen hata");
                }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                var isUser = _context.Users.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);
                if (isUser != null)
                {
                    var userClaims = new List<Claim>();
                    userClaims.Add(new Claim(ClaimTypes.NameIdentifier, isUser.UserId.ToString()));
                    userClaims.Add(new Claim(ClaimTypes.Name, isUser.UserName ?? "")); //null değer gelirse boş string atar
                    userClaims.Add(new Claim(ClaimTypes.GivenName, isUser.Name ?? ""));
                    userClaims.Add(new Claim(ClaimTypes.UserData, isUser.Image ?? ""));

                    if (isUser.Email == "info@sadikturan.com")
                    {
                        userClaims.Add(new Claim(ClaimTypes.Role, "admin"));
                    }

                    var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    //CookieAuthenticationDefaults.AuthenticationScheme cookie türünü belirledik

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true // beni hatırla fonksiyonu
                    };

                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); //önceki cookieleri sildik

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                    new ClaimsPrincipal(claimsIdentity), authProperties);
                    return RedirectToAction("Index", "Post");
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış");
                }
            }
            return View(model);
        }

        public IActionResult Profile(string  username)
        {
            if(string.IsNullOrEmpty(username))
            {
                return NotFound();
            }
            var user = _context
                        .Users
                        .Include(x => x.Posts)
                        .Include(x => x.Comments)
                        .ThenInclude(x => x.Post)
                        .FirstOrDefault(x => x.UserName == username);

            if(user == null)
            {
                return NotFound();
            }
            return View(user);
        }

    }
}