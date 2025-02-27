﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TRPO.Models;

namespace TRPO.Controllers
{
    public class SignInController : Controller
    {
        public async Task<IActionResult> Check()
        {
            if(ModelState.IsValid)
            {
                User user = Models.User.GetFromDBByEmailAndPassword(Request.Form["Email"], Request.Form["Password"]);
                try
                {
                    var claims = new List<Claim> {
                        new Claim("id", Convert.ToString(user.PassangerId)),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Role, user.Role)
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    return Redirect("../Home");
                }
                catch(Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View("Index");
                }
            }
            return View("Index");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}