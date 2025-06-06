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
            int accountID = Convert.ToInt32(HttpContext.Session.GetString("AccountId"));
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
            if (ModelState.IsValid)
            {
                var toAccount = _bMContext.BankAccounts.FirstOrDefault(b => b.AccountId == model.ToAccountId);
                var fromAccount = _bMContext.BankAccounts.FirstOrDefault(b => b.AccountId == model.FromAccountId);
                if (toAccount == null)
                {
                    ModelState.AddModelError("ToAccountId", "Tài khoản người nhận không tồn tại.");
                    return View(model);
                }
                if(fromAccount == null)
                {
                    ModelState.AddModelError("FromAccountId", "Tài khoản người gửi không tồn tại.");
                    return View(model);
                }
                double Amount = model.Amount;
                if (Amount <= 0)
                {
                    ModelState.AddModelError("Amount", "Số tiền phải lớn hơn 0.");
                    return View(model);
                }
                if (Amount > fromAccount.Balance)
                {
                    ModelState.AddModelError("Amount", "Số tiền chuyển lớn hơn số dư tài khoản.");
                    return View(model);
                }
                try
                {
                    fromAccount.Balance -= model.Amount;
                    toAccount.Balance += model.Amount;

                    Trans trans1 = new Trans(fromAccount.AccountId, toAccount.AccountId,fromAccount,toAccount, model.Amount, TransactionType.Transfer,model.description);
                    _bMContext.Transactions.Add(trans1);
                    _bMContext.SaveChanges();

                    // Reset ModelState và tạo model mới để tránh submit lại
                    ModelState.Clear();
                    TransferViewModel newModel = new TransferViewModel
                    {
                        FromAccountId = model.FromAccountId,
                        AccountUserName = model.AccountUserName,
                        balance = fromAccount.Balance
                    };
                    ViewBag.Message = "Chuyển tiền thành công.";
                    return View(newModel);
                }
                catch (DbUpdateException dbEx)
                {
                    // Lấy thông tin lỗi chi tiết nhất
                    var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                    ModelState.AddModelError("", "Có lỗi xảy ra khi chuyển tiền: " + inner);
                    return View(model);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi chuyển tiền: " + ex.Message);
                    return View(model);
                }
            }
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

            // Lấy thông tin user hiện tại từ session
            var sdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(sdt))
            {
                return RedirectToAction("Login", "Account");
            }
            var result = _userService.ChangePassword(sdt, model.NewPassword);
            if (!result)
            {
                ModelState.AddModelError("", "Không tìm thấy người dùng.");
                return View("UserInfo", model);
            }
            ViewBag.Status = "✅ Mật khẩu đã được cập nhật thành công!";
            model.NewPassword = model.ConfirmPassword = string.Empty;

            var user = _userService.GetUserBySdt(sdt);
            ViewBag.Hoten = user?.Hoten;
            ViewBag.Username = user?.Sdt;
            ViewBag.NS = user?.NS;
             
            ViewBag.AccountType = _bankAccountService.GetAccountType(sdt,);

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
    }
}
