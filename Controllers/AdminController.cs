using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Entities;
using PBL3.Models;

namespace PBL3.Controllers
{
    public class AdminController : Controller
    {
        private readonly BMContext _bMContext;

        public AdminController(BMContext bmcontext)
        {
            _bMContext = bmcontext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Dashboard()
        {
            // Kiểm tra quyền Admin
            String role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            // Lấy thông tin Admin từ session
            String loggedInUserSdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(loggedInUserSdt) == true)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [HttpGet]
        public IActionResult AdminInfo()
        {
            // Kiểm tra quyền Admin
            String role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            // Lấy thông tin Admin từ session
            String loggedInUserSdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(loggedInUserSdt) == true)
            {
                return RedirectToAction("Login", "Account");
            }
            //var user = _bMContext.Users.FirstOrDefault(u => u.Sdt == loggedInUserSdt);
            User user = _bMContext.Users.FirstOrDefault(u => u.Sdt == loggedInUserSdt);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            AdminInfoViewModel model = new AdminInfoViewModel
            {
                HoTen = user.Hoten,
                Username = user.Sdt,
                NgaySinh = user.NS,
            };

            return View(model);
        }
        public IActionResult Freeze()
        {
            // Kiểm tra quyền Admin
            String role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            // Lấy thông tin Admin từ session
            String loggedInUserSdt = HttpContext.Session.GetString("Sdt");
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

            var account = _bMContext.BankAccounts.FirstOrDefault(a => a.AccountId == model.AccountId);
            if (account == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản.";
                return View(model);
            }
            if (account.IsFrozen)
            {
                ViewBag.Error = "Tài khoản đã bị đóng băng trước đó.";
                return View(model);
            }
            try
            {
                account.IsFrozen = true;
                _bMContext.SaveChanges();
                ViewBag.Status = "Đã đóng băng tài khoản thành công!";
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
            // Kiểm tra quyền Admin
            String role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            // Lấy thông tin Admin từ session
            String loggedInUserSdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(loggedInUserSdt) == true)
            {
                return RedirectToAction("Login", "Account");
            }
            DateTime to = toDate ?? DateTime.Now;
            DateTime from = fromDate ?? to.AddDays(-30); 
            //Tìm kiếm các giao dịch trong khoảng thời gian từ 'from' đến 'to'
            var transactions = _bMContext.Transactions.Include(t=>t.FromAccount).ThenInclude(a=>a.user)
                .Where(t => t.TransactionDate >= from && t.TransactionDate <= to)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();
            // Nếu không có giao dịch nào, trả về thông báo
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
        public IActionResult ListSTK()
        {
            // Kiểm tra quyền Admin
            String role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            // Lấy thông tin Admin từ session
            String loggedInUserSdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(loggedInUserSdt) == true)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        public IActionResult Unlock()
        {
            // Kiểm tra quyền Admin
            String role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            // Lấy thông tin Admin từ session
            String loggedInUserSdt = HttpContext.Session.GetString("Sdt");
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
            var account = _bMContext.BankAccounts.FirstOrDefault(a => a.AccountId == model.AccountId);
            if (account == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản.";
                return View(model);
            }
            if (!account.IsFrozen)
            {
                ViewBag.Error = "Tài khoản không bị đóng băng.";
                return View(model);
            }
            try
            {
                account.IsFrozen = false;
                _bMContext.SaveChanges();
                ViewBag.Status = "Đã mở khóa tài khoản thành công!";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Cập nhật thất bại: " + ex.Message;
            }
            return View();


        }
    }
}
