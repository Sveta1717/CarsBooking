using CarBooking.Data;
using CarBooking.Models.User;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Filters;
using CarBooking.Services.Hash;

namespace CarBooking.Controllers
{
    public class UserController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IHashService _hashService;

        public UserController(DataContext dataContext, IHashService hashService)
        {
            _dataContext = dataContext;
            _hashService = hashService;
        }

        public IActionResult Register(UserRegisterModel? model)
        {
            if (HttpContext.Request.Method == "POST")
            {
                ViewData["form"] = _ValidateModel(model);
            }           
            return View(model);
        }

        public IActionResult Exit()   
        {
            HttpContext.Session.Remove("AuthUserId");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public JsonResult LogIn([FromForm] String login, String password)
        {
            //Перевірка логіну та паролю для адміністратора
            string adminLogin = "Admin";
            string adminPassword = "Qq123456";            


            var user = _dataContext.Users.FirstOrDefault(u => u.Login == login);
            if (login == adminLogin && password == adminPassword)
            {
                return Json(new { status = "Admin" });
            }
            if (user != null)
            {
                if (user.PasswordHash == _hashService.HashString(password))
                {                    
                    HttpContext.Session.SetString("AuthUserId", user.Id.ToString());
                    return Json(new { status = "OK" });
                }
            }
            bool userYes = _dataContext.Users.Any(user =>
            user.Login == login && user.PasswordHash == _hashService.HashString(password));

            if (userYes)
            {
                return Json(new { status = "OK" });               
            }
            else
            {
                return Json(new { status = "NO" });
            }           
        }

        private bool _LoginUnique(string login)
        {
            return _dataContext.Users.All(user => user.Login != login);
        }

        private String _ValidateModel(UserRegisterModel? model)
        {
            if (model == null) { return "Дані не передані"; }
            if (String.IsNullOrEmpty(model.Login)) { return "Логін не може бути порожним"; }
            if (String.IsNullOrEmpty(model.Password)) { return "Пароль не може бути порожним"; }           
            if (String.IsNullOrEmpty(model.Email)) { return "Email не може бути порожним"; }
            if (!model.Agree) { return "Необхідно дотримуватись правил сайту"; }
          
            String? newName = null;
            if (model.AvatarFile != null)  
            {                
                String ext = Path.GetExtension(model.AvatarFile.FileName);
               
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" }; 
                if (!allowedExtensions.Contains(ext))
                {
                    return "Недопустимий тип файлу. Дозволені лише файли з розширеннями .jpg, .jpeg, .png";
                }
                
                newName = Guid.NewGuid().ToString() + ext;
                
                model.AvatarFile.CopyTo(
                    new FileStream(
                        $"wwwroot/uploads/{newName}",
                        FileMode.Create));
            }
           
            if (!String.Equals(model.Password, model.RepeatPassword)) { return "Паролі не однакові"; }
           
            if (model.Password.Length < 8 ||
                !model.Password.Any(char.IsDigit) ||
                !model.Password.Any(char.IsUpper) ||
                !model.Password.Any(char.IsLower))
            {
                return "Пароль повинен містити щонайменше 8 символів, цифри, великі та маленькі літери";
            }
            
            string emailUnique = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            if (!Regex.IsMatch(model.Email, emailUnique))
            {
                return "Невірний формат Email";
            }

            if (!_LoginUnique(model.Login))
            {
                return "Цей логін вже використовується";
            }
            
            _dataContext.Users.Add(new Data.Entity.User
            {
                Id = Guid.NewGuid(),
                Login = model.Login,
                PasswordHash = _hashService.HashString(model.Password),
                Email = model.Email,
                Avatar = newName,
                RealName = model.RealName,
                RegisteredDt = DateTime.Now,
            });
            
            _dataContext.SaveChanges();  

            return String.Empty;
        }
    }
}
