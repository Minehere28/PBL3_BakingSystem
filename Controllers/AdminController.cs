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
        public IActionResult ListSTK(string? searchBy, string? searchValue)
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            ViewBag.SearchBy = searchBy;
            ViewBag.SearchValue = searchValue;

            var accounts = _bankAccountService.GetAllBankAccounts();

            // Lọc theo tiêu chí tìm kiếm
            if (!string.IsNullOrEmpty(searchBy) && !string.IsNullOrEmpty(searchValue))
            {
                switch (searchBy)
                {
                    case "AccountNumber":
                        accounts = accounts
                            .Where(a => a.AccountId.ToString().Contains(searchValue))
                            .ToList();
                        break;

                    case "AccountName":
                        accounts = accounts
                            .Where(a => a.user != null && a.user.Hoten.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;

                    case "Status":
                        if (searchValue == "Active")
                        {
                            accounts = accounts
                                .Where(a => a.IsActive()) // chỉ tài khoản đang hoạt động
                                .ToList();
                        }
                        else if (searchValue == "Blocked")
                        {
                            accounts = accounts
                                .Where(a => !a.IsActive()) // chỉ tài khoản đã bị khóa
                                .ToList();
                        }
                        break;

                    case "AccountType":
                        accounts = accounts
                            .Where(a =>
                                (searchValue == "Thông thường" && a is RegularAccount) ||
                                (searchValue == "Tiết kiệm" && a is SavingAccount) ||
                                (searchValue == "Vay" && a is LoanAccount))
                            .ToList();
                        break;

                    case "Balance":
                        if (double.TryParse(searchValue, out double minBalance))
                        {
                            accounts = accounts
                                .Where(a => a.Balance >= minBalance)
                                .ToList();
                        }
                        break;
                }
            }

            var viewModel = accounts.Select(acc => new ListSTKViewModel
            {
                AccountId = acc.AccountId,
                Sdt = acc.Sdt,
                FullName = acc.user?.Hoten ?? "",
                Balance = acc.Balance,
                AccountType = acc switch
                {
                    RegularAccount => "Thông thường",
                    SavingAccount => "Tiết kiệm",
                    LoanAccount => "Vay",
                    _ => "Không xác định"
                },
                IsActive = acc.IsActive(),
                CreatedDate = acc.CreatedDate
            }).ToList();

            return View(viewModel);
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
        [HttpGet]
        public IActionResult Deposit()
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
        public IActionResult Deposit(DepositViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var (success, message) = _bankAccountService.Deposit(model);
            if (success)
            {
                ModelState.Clear();
                var newModel = new DepositViewModel
                {
                    AccountId = model.AccountId,
                    Amount = model.Amount,
                };
                ViewBag.Message = message;
                return View(newModel);
            }
            ModelState.AddModelError("", message);
            return View(model);
        }
        [HttpPost]
        public IActionResult Withdraw(WithdrawViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var (success, message) = _bankAccountService.Withdraw(model);
            if (success)
            {
                ModelState.Clear();
                var newModel = new WithdrawViewModel
                {
                    AccountId = model.AccountId,
                    Amount = model.Amount,
                };
                ViewBag.Message = message;
                return View(newModel);
            }
            ModelState.AddModelError("", message);
            return View(model);
        }

        [HttpGet]
        public IActionResult Withdraw()
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

    }
}
