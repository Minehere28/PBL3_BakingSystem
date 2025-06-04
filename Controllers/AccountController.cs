using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Entities;
using PBL3.Models;

namespace PBL3.Controllers
{
    public class AccountController : Controller
    {
        private readonly BMContext _bMContext;
        private readonly ILogger<AccountController> _logger;

        public AccountController(BMContext bmcontext, ILogger<AccountController> logger)
        {
            _bMContext = bmcontext;
            _logger = logger;
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
                if (_bMContext.Users.Any(u => u.Sdt == model.Sdt))
                {
                    ModelState.AddModelError("Sdt", "Số điện thoại đã tồn tại.");
                    return View(model);
                }
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp.");
                    return View(model);
                }
                User user = new User();
                user.Sdt = model.Sdt;
                user.Hoten = model.Hoten;
                user.NS = model.NS;
                user.SetPassword(model.Password);
                user.Role = "Customer";
                try
                {
                    //Thêm người dùng
                    _bMContext.Users.Add(user);
                    _bMContext.SaveChanges();
                    //Thêm tài khoản ngân hàng
                    _bMContext.BankAccounts.Add(new RegularAccount { Sdt = user.Sdt, user = user, AccountId = BankAccount.GenerateAccountId(_bMContext) });
                    _bMContext.SaveChanges();

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
            User account = _bMContext.Users.FirstOrDefault(a => a.Sdt == model.Sdt);

            if (account == null)
            {
                ModelState.AddModelError("", "Số điện thoại hoặc mật khẩu không chính xác.");
                return View(model);
            }

            BankAccount bankAccount = _bMContext.BankAccounts
                .Include(a => a.user)
                .FirstOrDefault(a => a.Sdt == account.Sdt);
            HttpContext.Session.SetString("Sdt", account.Sdt);
            HttpContext.Session.SetString("Name", account.Hoten);
            HttpContext.Session.SetString("Role", account.Role);

            

            if (account.Role == "Admin")
            {
                if(account.PasswordHash.Trim() == model.Password.Trim())
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Sai mật khẩu cho tài khoản Admin.");
                    return View(model);
                }
            }
            else if (account.Role == "Customer")
            {
                if (account.VerifyPassword(model.Password) == true)
                {
                    if (bankAccount.IsFrozen == true)
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
