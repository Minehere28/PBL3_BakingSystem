using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Entities;
using PBL3.Models;
using PBL3.Services;

namespace PBL3.Controllers
{
    public class UserController : Controller
    {
        private readonly IBankAccountService _bankAccountService;
        private readonly IUserService _userService; 
        public UserController(IBankAccountService bankAccountService, IUserService userService)
        {
            _bankAccountService = bankAccountService;
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult User()
        {
            string role = HttpContext.Session.GetString("Role");
            if(role != "Customer")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            string loggedInUserSdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(loggedInUserSdt))
            {
                return RedirectToAction("Login", "Account");
            }

            var bankAccount = _bankAccountService.GetRegularAccountBySdt(loggedInUserSdt);

            if (bankAccount == null)
            {
                ViewBag.Message = "Không tìm thấy tài khoản ngân hàng.";
                return View();
            }
            HttpContext.Session.SetString("AccountId", bankAccount.AccountId.ToString());
            HttpContext.Session.SetString("AccountBalance", bankAccount.Balance.ToString());
            var model = new UserViewModel
            {
                AccountId = bankAccount.AccountId,
                Balance = bankAccount.Balance
            };
            return View(model);
        }
        [HttpGet]
        public IActionResult UserInfo()
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "Customer")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            string loggedInUserSdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(loggedInUserSdt))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _userService.GetUserBySdt(loggedInUserSdt);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Hoten = user.Hoten;
            ViewBag.Username = user.Sdt;
            ViewBag.NS = user.NS;
            //var accountType = _bankAccountService.GetAccountType(loggedInUserSdt, accountID); 

            //ViewBag.AccountType = accountType;

            return View();
        }
        [HttpGet]
        public IActionResult Transfer()
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "Customer")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            string myaccountName = HttpContext.Session.GetString("Name");

            TransferViewModel model = new TransferViewModel
            {
                FromAccountId = Convert.ToInt32(HttpContext.Session.GetString("AccountId")),
                AccountUserName = myaccountName,
                balance = Convert.ToDouble(HttpContext.Session.GetString("AccountBalance")),
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Transfer(TransferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var (success, message) = _bankAccountService.Transfer(model);
            if (success)
            {
                ModelState.Clear();
                var newModel = new TransferViewModel
                {
                    FromAccountId = model.FromAccountId,
                    AccountUserName = model.AccountUserName,
                    balance = _bankAccountService.GetBalance(model.FromAccountId)
                };
                ViewBag.Message = message;
                return View(model);
            }
            ModelState.AddModelError("", message);
            return View(model);
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
                return View("UserInfo", model);
            }
            ViewBag.Status = "✅ Mật khẩu đã được cập nhật thành công!";
            model.NewPassword = model.ConfirmPassword = string.Empty;

            User user = _userService.GetUserBySdt(sdt);
            ViewBag.Hoten = user?.Hoten;
            ViewBag.Username = user?.Sdt;
            ViewBag.NS = user?.NS;

            return View("UserInfo", model);
        }
        [HttpGet]
        public IActionResult History()
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "Customer")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            string sdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(sdt))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [HttpPost]
        public IActionResult History(DateTime FromDate, DateTime ToDate, string TransactionType)
        {
            ViewBag.HasFiltered = false;

            if (FromDate == default || ToDate == default)
            {
                ViewBag.FromDate = "";
                ViewBag.ToDate = "";
                ViewBag.TransactionType = "All";
                return View(new List<Trans>());
            }

            int accountId = Convert.ToInt32(HttpContext.Session.GetString("AccountId"));
            if (accountId == 0) return NotFound();

            var allTransactions = _bankAccountService.GetTransactionByAccountAndDate(accountId, FromDate, ToDate);

            List<Trans> filteredTransactions = new List<Trans>();
            switch (TransactionType)
            {
                case "In":
                    filteredTransactions = allTransactions
                        .Where(t => t.ToAccountId == accountId)
                        .ToList();
                    break;
                case "Out":
                    filteredTransactions = allTransactions
                        .Where(t => t.FromAccountId == accountId)
                        .ToList();
                    break;
                case "All":
                default:
                    filteredTransactions = allTransactions;
                    break;
            }

            // Gửi lại dữ liệu lọc về view
            ViewBag.HasFiltered = true;
            ViewBag.FromDate = FromDate.ToString("yyyy-MM-dd");
            ViewBag.ToDate = ToDate.ToString("yyyy-MM-dd");
            ViewBag.TransactionType = TransactionType;

            return View(filteredTransactions);
        }


        [HttpGet]
        public IActionResult Saving()
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "Customer")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            string sdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(sdt))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [HttpGet]
        public IActionResult Loan()
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "Customer")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            string sdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(sdt))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }


    }
}
