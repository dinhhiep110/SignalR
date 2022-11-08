using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalR.Models;
using System.Net.Mail;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Text;
using SignalR.Helper;

namespace SignalR.Pages.Account
{
    public class ForgotModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public ForgotModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [Required(ErrorMessage = "Email is required")]
        [BindProperty, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var account = dbContext.Accounts.FirstOrDefault(a => a.Email == Email);
                if (account == null)
                {
                    ViewData["errMsg"] = "Email is not existed";
                    return Page();
                }
                account.Password = RandomPassword(6);
                await dbContext.SaveChangesAsync();
                var subject = "FORGOT PASSWORD";
                var body = $@"<h2>Thank you, we have received your request for your password!</h2>
                    Please don't leak this email to everyone.  Thanks!<br/>";
                body += $"<p> Your New Password is: <b>{account.Password}</b> </p>";
                MailHelper.SendMail(Email, body, subject);
            }
            return Redirect("/Account/Login");
        }

        private string RandomPassword(int length)
        {
            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();
            char letter;
            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }
    }
}
