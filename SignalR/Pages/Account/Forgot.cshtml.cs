using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalR.Models;
using System.Net.Mail;
using System.ComponentModel.DataAnnotations;
using System.Net;

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
        [BindProperty,DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var account = dbContext.Accounts.FirstOrDefault(a => a.Email == Email);
                if(account == null)
                {
                    ViewData["errMsg"] = "Email is not existed";
                    return Page();
                }
                var body = $@"<p>Thank you, we have received your request for your password!</p>
                    Please don't leak this email to everyone.  Thanks!<br/>";
                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "example@gmail.com",  // replace with valid value
                        Password = "password"  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    var message = new MailMessage();
                    message.To.Add(Email);
                    message.Subject = "Fourth Coffee - New Order";
                    message.Body = body;
                    message.IsBodyHtml = true;
                    message.From = new MailAddress("shop@cart.com");
                    await smtp.SendMailAsync(message);
                }
            }
            return Redirect("/Account/Login");
        }
    }
}
