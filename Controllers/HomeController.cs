﻿using CarBooking.Data;
using CarBooking.Models;
using CarBooking.Models.Car;
using CarBooking.Services.Hash;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Diagnostics;



namespace CarBooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHashService _hashService;
        private readonly DataContext _dataContext;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(ILogger<HomeController> logger, IHashService hashService, DataContext dataContext, IStringLocalizer<HomeController> localizer)
        {
            _logger = logger;
            _hashService = hashService;
            _dataContext = dataContext;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = _localizer["WelcomeTitle"];
            return View();
        }
        public IActionResult RentTerms()
        {
            return View();
        }
       
        public ViewResult Services()
        {           
            ViewData["obj"] = _hashService.GetHashCode();
            ViewData["ctr"] = this.GetHashCode();
            return View();
        }

        public ViewResult Sessions(String? userstring)
        {
            if (userstring != null)      // є дані від форми
            {
                HttpContext.Session.SetString("StoredString", userstring);
            }
            if (HttpContext.Session.Keys.Contains("StoredString"))
            {

                ViewData["StoredString"] = HttpContext.Session.GetString("StoredString");
            }
            else
            {
                ViewData["StoredString"] = "У сесії немає збережених даних";
            }

            if (HttpContext.Session.Keys.Contains("Form2String"))
            {

                ViewData["Form2String"] = HttpContext.Session.GetString("Form2String");
            }
            else
            {
                ViewData["Form2String"] = "У сесії немає збережених даних";
            }
            return View();
        }

        public RedirectToActionResult SessionsForm(String? userstring)
        {            
            HttpContext.Session.SetString("Form2String", userstring!);
            return RedirectToAction("Sessions");           
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult PersonalArea()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult SetCulture(string culture, string returnUrl)
        {
            _logger.LogInformation($"Changing culture to {culture}, ReturnUrl: {returnUrl}");
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        //public IActionResult SetCulture(string culture, string returnUrl)
        //{
        //    Response.Cookies.Append(
        //        CookieRequestCultureProvider.DefaultCookieName,
        //        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        //        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        //    );

        //    return LocalRedirect(returnUrl);
        //}

        [HttpGet]
        public IActionResult AdminSignUp()
        {
            BookingIndexViewModel model = new()
            {
                Brands = _dataContext.Brands
                      .Where(b => b.DeleteDt == null)
                      .ToList(),
                Cars = _dataContext.Cars
                      .Where (c => c.DeleteDt == null)
                      .ToList()
            };
            if (HttpContext.Session.Keys.Contains("AddMessage"))
            {
                model.AddMessage = HttpContext.Session.GetString("AddMessage");
                HttpContext.Session.Remove("AddMessage");
            }           

            return View(model);
            //return RedirectToAction("Index", "Car");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}