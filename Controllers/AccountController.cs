using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Entities;
using PBL3.Models;
using PBL3.Services;

namespace PBL3.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IUserService _userService;
        private readonly IBankAccountService _bankAccountService;
        public AccountController(ILogger<AccountController> logger, IUserService userService, IBankAccountService bankAccountService)
        {
            _logger = logger;
            _userService = userService;
            _bankAccountService = bankAccountService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegistrationViewModel model)
        {
            if (ModelState.IsValid == true)
            {
                if (_userService.IsUserExists(model.Sdt))
                {
                    ModelState.AddModelError("Sdt", "Số điện thoại đã tồn tại.");
                    return View(model);
                }
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp.");
                    return View(model);
                }
                var user = new User
                {
                    Sdt = model.Sdt,
                    Hoten = model.Hoten,
                    NS = model.NS,
                    Role = "Customer"
                };
                user.SetPassword(model.Password);
                try
                {
                    //Thêm người dùng
                    _userService.RegisterUser(user);
                    //Thêm tài khoản ngân hàng
                    _bankAccountService.CreateBankAccount(user);

                    ModelState.Clear(); 
                    ViewBag.Status = "Đăng ký thành công!.";
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Cập nhật dữ liệu không thành công.");
                    return View(model);

                }
                return View(model);
            }
            return View(model);
        }
        //Action tr? v? view ??ng nh?p(Login)
        [HttpGet]
        public IActionResult Login()
        {
            return View();  // Tr? v? Login.cshtml
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = _userService.Authenticate(model.Sdt, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Số điện thoại hoặc mật khẩu không chính xác.");
                return View(model);
            }

            //RegularAccount bankAccount = _bankAccountService.GetRegularAccountBySdt(user.Sdt);
            HttpContext.Session.SetString("Sdt", user.Sdt);
            HttpContext.Session.SetString("Name", user.Hoten);
            HttpContext.Session.SetString("Role", user.Role);
            if (user.Role == "Admin")
            {
                if(user.PasswordHash.Trim() == model.Password.Trim())
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Sai mật khẩu cho tài khoản Admin.");
                    return View(model);
                }
            }
            else if (user.Role == "Customer")
            {
                if (user.VerifyPassword(model.Password) == true)
                {
                    if (!_bankAccountService.IsAccountActive(user.Sdt))//Cần đóng gói lại
                    {
                        ModelState.AddModelError("", "Tài khoản của bạn đã bị đóng băng. Vui lòng liên hệ với quản trị viên để biết thêm chi tiết.");
                        return View(model);
                    }
                    else
                    {
                        return RedirectToAction("user", "User");
                    }     
                }
            }
            return View(model);
        }
        //public IActionResult SecurePage()
        //{

        //}
        public IActionResult LogOut()
        {
            return RedirectToAction("Login");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
