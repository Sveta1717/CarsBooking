using CarBooking.Data;
using CarBooking.Data.Entity;
using CarBooking.Models;
using CarBooking.Models.Car;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Diagnostics;

namespace CarBooking.Controllers
{
    public class CarController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<CarController> _logger;

        public CarController(DataContext dataContext, ILogger<CarController> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
            BookingIndexViewModel model = new()
            {
                Brands = _dataContext.Brands
                         .Where(b => b.DeleteDt == null)
                         .ToList(),
                Cars = _dataContext.Cars
                       .ToList()
            };
            if (HttpContext.Session.Keys.Contains("AddMessage"))
            {
                model.AddMessage = HttpContext.Session.GetString("AddMessage");
                HttpContext.Session.Remove("AddMessage");
            }

            return View(model);
        }

        [HttpPost]
        public RedirectToActionResult AddCar(BookingIndexFormModel model)
        {
            String errorMessage = _ValidateModel(model);
            if (errorMessage != String.Empty)
            {                
                HttpContext.Session.SetString("AddMessage", errorMessage);
            }
            else
            {                
                HttpContext.Session.SetString("AddMessage", "Додано успішно!");
            }

            return RedirectToAction("AdminSignUp", "Home");
           
        }

        private String _ValidateModel(BookingIndexFormModel? model)
        {
            if (model == null) { return "Дані не передані"; }
            if (String.IsNullOrEmpty(model.Title)) { return "Назва не може бути порожньою"; }
            if (string.IsNullOrEmpty(model.Description)) { return "Опис не може бути порожнім"; }

            if (model.Price_per_day == 0)
            {
                model.Price_per_day = Convert.ToSingle(
                    Request.Form["carPrice"].First()?.Replace(',', '.'),
                    CultureInfo.InvariantCulture);
                if (model.Price_per_day <= 0) { return "Ціна повинна бути більшою за 0"; }
            }

            // завантажуємо файл
            String? newName = null;
            if (model.Image != null)
            {

                String ext = Path.GetExtension(model.Image.FileName);

                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" }; // Допустимі розширення файлів
                if (!allowedExtensions.Contains(ext))
                {
                    return "Недопустимий тип файлу. Дозволені лише файли з розширеннями .jpg, .jpeg, .png";
                }

                newName = Guid.NewGuid().ToString() + ext;
                model.Image.CopyTo(
                    new FileStream(
                        $"wwwroot/uploads/{newName}",
                        FileMode.Create));
            }
            else { return "Файл-картинка необхідний"; }

            _dataContext.Cars.Add(new Data.Entity.Car
            {
                Id = Guid.NewGuid(),
                Brand_id = model.BrandId,
                Title = model.Title,
                Description = model.Description,
                CreateDt = DateTime.Now,              
                Price_per_day = model.Price_per_day,                
                ImageUri = newName                
            });

            _dataContext.SaveChanges();

            return String.Empty;
        }
    }
}
