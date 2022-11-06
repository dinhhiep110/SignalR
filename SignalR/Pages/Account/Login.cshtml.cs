using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalR.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace SignalR.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public LoginModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [BindProperty]
        public SignalR.Models.Account Account { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var acc = await dbContext.Accounts
                .SingleOrDefaultAsync(a => a.Email.Equals(Account.Email) && a.Password.Equals(Account.Password));
            var customer = await dbContext.Customers.SingleOrDefaultAsync(c => c.CustomerId == acc.CustomerId);

            if (acc == null)
            {
                ViewData["msg"] = "Email/ Password is wrong";
                return Page();
            }

            if (customer != null && !customer.IsActive.Value)
            {
                ViewData["msg"] = "Your Account Cannot Login Into System";
                return Page();
            }

            HttpContext.Session.SetString("CustSession", JsonSerializer.Serialize(acc));
            if (acc.Role == 1)
            {
                HttpContext.Session.SetString("IsAdmin", "Admin");
                return Redirect("/DashBoard");
            }
            return RedirectToPage("/index");
        }

        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Remove("CustSession");

            return RedirectToPage("/index");
        }
    }
}
