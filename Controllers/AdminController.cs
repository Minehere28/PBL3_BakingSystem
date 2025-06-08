using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Entities;
using PBL3.Models;
using PBL3.Services;

namespace PBL3.Controllers
{
    public class AdminController : Controller
    {
        private readonly IBankAccountService _bankAccountService;
        private readonly IUserService _userService;
        public AdminController(IBankAccountService bankAccountService, IUserService userService)
        {
            _bankAccountService = bankAccountService;
            _userService = userService;
        }
        [HttpGet]
        public IActionResult Dashboard()
        {
            // Kiểm tra quyền Admin
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            // Lấy thông tin Admin từ session
            string loggedInUserSdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(loggedInUserSdt) == true)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [HttpGet]
        public IActionResult AdminInfo()
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            string loggedInUserSdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(loggedInUserSdt) == true)
            {
                return RedirectToAction("Login", "Account");
            }
            User user = _userService.GetUserBySdt(loggedInUserSdt);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.HoTen = user.Hoten;
            ViewBag.Username = user.Sdt;
            ViewBag.NgaySinh = user.NS;

            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("UserInfo", model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp.");
                return View("UserInfo", model);
            }
            string sdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(sdt))
            {
                return RedirectToAction("Login", "Account");
            }
            bool result = _userService.ChangePassword(sdt, model.NewPassword);
            if (!result)
            {
                ModelState.AddModelError("", "Không tìm thấy người dùng.");
                return View("AdminInfo", model);
            }
            ViewBag.Status = "✅ Mật khẩu đã được cập nhật thành công!";
            model.NewPassword = model.ConfirmPassword = string.Empty;

            User user = _userService.GetUserBySdt(sdt);
            ViewBag.Hoten = user?.Hoten;
            ViewBag.Username = user?.Sdt;
            ViewBag.NS = user?.NS;

            return View("AdminInfo", model);
        }
        public IActionResult Freeze()
        {
            // Kiểm tra quyền Admin
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            // Lấy thông tin Admin từ session
            string loggedInUserSdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(loggedInUserSdt) == true)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Freeze(FreezeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var account = _bankAccountService.GetByID(model.AccountId);
            if (account == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản.";
                return View(model);
            }
            if (!account.IsActive())
            {
                ViewBag.Error = "Tài khoản đã bị đóng băng trước đó.";
                return View(model);
            }
            try
            {
                bool result = _bankAccountService.FreezeAccount(model.AccountId);
                if (!result)
                {
                    ViewBag.Error = "Không thể đóng băng tài khoản. Vui lòng thử lại sau.";
                    return View(model);
                }
                else
                {
                    ViewBag.Status = "Đã đóng băng tài khoản thành công!";
                }  
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Cập nhật thất bại: " + ex.Message;
            }
            return View();
        }
        [HttpGet]
        public IActionResult History(DateTime? fromDate, DateTime? toDate)
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            string loggedInUserSdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(loggedInUserSdt) == true)
            {
                return RedirectToAction("Login", "Account");
            }

            DateTime to = toDate ?? DateTime.Now;
            DateTime from = fromDate ?? to.AddDays(-30);

            var transactions = _bankAccountService.GetTransactionByDateRange(from, to);
            
            if (transactions.Count == 0)
            {
                ViewBag.Message = "Không có giao dịch nào trong khoảng thời gian này.";
            }
            var model = new AdminHistoryViewModel
            {
                Transactions = transactions,
                FromDate = from,
                ToDate = to
            };
            return View(model);
        }
        [HttpGet]
        public IActionResult ListSTK()
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            
            string loggedInUserSdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(loggedInUserSdt) == true)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        public IActionResult Unlock()
        {
            // Kiểm tra quyền Admin
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            // Lấy thông tin Admin từ session
            string loggedInUserSdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(loggedInUserSdt) == true)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Unlock(UnlockViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var account = _bankAccountService.GetByID(model.AccountId);
            if (account == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản.";
                return View(model);
            }
            if (account.IsActive())
            {
                ViewBag.Error = "Tài khoản không bị đóng băng.";
                return View(model);
            }
            try
            {
                bool result = _bankAccountService.UnlockAccount(model.AccountId);
                if (result)
                {
                    ViewBag.Status = "Đã mở khóa tài khoản thành công!";
                }
                else
                {
                    ViewBag.Error = "Không thể mở khóa tài khoản. Vui lòng thử lại sau.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Cập nhật thất bại: " + ex.Message;
            }
            return View();


        }
    }
}
