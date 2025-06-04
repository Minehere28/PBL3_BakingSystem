using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Entities;
using PBL3.Models;

namespace PBL3.Controllers
{
    public class UserController : Controller
    {
        private readonly BMContext _bMContext;
        public UserController(BMContext bmcontext)
        {
            _bMContext = bmcontext;
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

            var bankAccount = _bMContext.BankAccounts
                .Where(b => b.Sdt == loggedInUserSdt && EF.Property<string>(b, "AccountType") == "Regular").FirstOrDefault();

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
            string loggedInUserSdt = HttpContext.Session.GetString("Sdt");
            if (string.IsNullOrEmpty(loggedInUserSdt))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _bMContext.Users.FirstOrDefault(u => u.Sdt == loggedInUserSdt);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Hoten = user.Hoten;
            ViewBag.Username = user.Sdt;
            ViewBag.NS = user.NS;
            int accountID = Convert.ToInt32(HttpContext.Session.GetString("AccountId"));
            var account = _bMContext.BankAccounts
                .Where(b => b.Sdt == loggedInUserSdt && b.AccountId == accountID)
                .Select(b => new
                {
                    AccountType = EF.Property<string>(b, "AccountType") // Lấy giá trị cột Discriminator
                })
                .FirstOrDefault();//LINQ TO ENTITY  

            if (account == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.AccountType = account.AccountType;

            return View();
        }
        [HttpGet]
        public IActionResult Transfer()
        {
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
            // Kiểm tra hợp lệ dữ liệu đầu vào
            if (!ModelState.IsValid)
            {
                // Hiển thị lỗi validation
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

            var user = _bMContext.Users.FirstOrDefault(u => u.Sdt == sdt);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Cập nhật mật khẩu mới (nên dùng hàm băm nếu có)
            user.SetPassword(model.NewPassword);
            _bMContext.SaveChanges();

            // Hiển thị thông báo thành công
            ViewBag.Status = "✅ Mật khẩu đã được cập nhật thành công!";
            // Xóa trường mật khẩu khỏi model để tránh hiển thị lại
            model.NewPassword = model.ConfirmPassword = string.Empty;

            // Cập nhật lại các thông tin khác nếu cần
            ViewBag.Hoten = user.Hoten;
            ViewBag.Username = user.Sdt;
            ViewBag.NS = user.NS;
            var account = _bMContext.BankAccounts
                .Where(b => b.Sdt == sdt)
                .Select(b => new
                {
                    AccountType = EF.Property<string>(b, "AccountType") // Lấy giá trị cột Discriminator
                })
                .FirstOrDefault();//LINQ TO ENTITY  
            ViewBag.AccountType = account.AccountType;

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
