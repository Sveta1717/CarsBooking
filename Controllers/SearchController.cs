using CarBooking.Data;
using CarBooking.Data.Entity;
using CarBooking.Models.Car;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
namespace CarBooking.Controllers
{
    public class SearchController : Controller
    {      
        private readonly DataContext? _context;

        public SearchController(DataContext? context)
        {
            _context = context;
        }

        //public IActionResult Index(string search)
        //{
        //    if (string.IsNullOrEmpty(search))
        //    {
        //        return RedirectToAction("Index", "Home"); // Повертаємо на головну, якщо не вказано марку авто
        //    }

        //    var matchingBrands = _context.Brands.Where(b => b.Title.Contains(search)).ToList();
        //    if (matchingBrands.Count == 0)
        //    {
        //        return View("~/Views/Search/Index.cshtml", new BookingIndexViewModel());
        //    }

        //    var brandIds = matchingBrands.Select(b => b.Id).ToList();

        //    var viewModel = new BookingIndexViewModel
        //    {
        //        Brands = matchingBrands,
        //        Cars = _context.Cars.Where(c => brandIds.Contains(c.Brand_id)).ToList()
        //    };

        //    return View("~/Views/Search/Index.cshtml", viewModel);
        //}
        public IActionResult Index(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return RedirectToAction("Index", "Home");
            }

            var matchingBrands = _context.Brands.Where(b => b.Title.Contains(search)).ToList();
            if (matchingBrands.Count == 0)
            {
                return View("~/Views/Search/Index.cshtml", new BookingIndexViewModel());
            }

            var brandIds = matchingBrands.Select(b => b.Id).ToList();

            var viewModel = new BookingIndexViewModel
            {
                Brands = matchingBrands,
                Cars = _context.Cars
                    .Where(c => brandIds.Contains(c.Brand_id))
                    .ToList()
            };

            return View("~/Views/Search/Index.cshtml", viewModel);
        }
    }
}
