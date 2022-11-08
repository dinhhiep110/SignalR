using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Specialized;
using System.Text.Json;
using SignalR.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SignalR.Helper;

namespace SignalR.Pages.Cart
{
    public class IndexModel : PageModel
    {
        public Dictionary<Models.Product, int> Cart { get; set; } = new Dictionary<Models.Product, int>();

        public decimal Sum { get; set; } = 0;

        private readonly PRN221DBContext dbContext;

        [BindProperty, DataType(DataType.DateTime)]
        public DateTime RequiredDate { get; set; }

        [BindProperty]
        public Customer Customer { get; set; }

        public IndexModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> OnGet()
        {
            var cart = HttpContext.Session.GetString("cart");

            Dictionary<int, int> list;
            if (cart != null)
            {
                list = JsonSerializer.Deserialize<Dictionary<int, int>>(cart);
            }
            else
            {
                list = new Dictionary<int, int>();
            }

            foreach (var item in list)
            {
                Models.Product product = (await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == item.Key));

                Cart.Add(product, item.Value);

                Sum += (decimal)product.UnitPrice * item.Value;
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {

            var Auth = (HttpContext.Session.GetString("CustSession") != null)
                ? JsonSerializer.Deserialize<Models.Account>(HttpContext.Session.GetString("CustSession")) : null;

            var cart = HttpContext.Session.GetString("cart");

            Dictionary<int, int> list;
            if (cart != null && cart.Length != 0)
            {
                list = JsonSerializer.Deserialize<Dictionary<int, int>>(cart);
            }
            else
            {
                list = new Dictionary<int, int>();
                ViewData["fail"] = "Your Cart Is Empty";
                return Page();
            }

            try
            {
                var custId = RandomCustID(5);
                if (Auth == null)
                {
                    Customer newCustomer = new Customer()
                    {
                        CustomerId = custId,
                        CompanyName = Customer.CompanyName,
                        ContactTitle = Customer.ContactTitle,
                        Address = Customer.Address,
                        ContactName = Customer.ContactName,
                        IsActive = false,
                        CreatedDate = DateTime.Now
                    };
                    await dbContext.Customers.AddAsync(newCustomer);

                }
                else
                {
                    custId = Auth.CustomerId;
                }
                Models.Order order = new Models.Order();
                order.CustomerId = custId;
                order.OrderDate = DateTime.Now;
                if (RequiredDate <= order.OrderDate)
                {
                    ViewData["fail"] = "Required Date is invalid";
                    return Page();
                }
                order.RequiredDate = RequiredDate;

                await dbContext.Orders.AddAsync(order);
                await dbContext.SaveChangesAsync();
                order = await dbContext.Orders.OrderBy(o => o.OrderDate).LastOrDefaultAsync();
                Dictionary<Models.Product, OrderDetail> listProducts = new Dictionary<Models.Product, OrderDetail>();

                foreach (var item in list)
                {
                    Models.Product product = (await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == item.Key));
                    OrderDetail od = new OrderDetail();
                    od.OrderId = order.OrderId;
                    od.ProductId = product.ProductId;
                    od.Quantity = (short)item.Value;
                    od.UnitPrice = (decimal)product.UnitPrice;
                    od.Discount = 0;
                    listProducts.Add(product, od);
                    await dbContext.OrderDetails.AddAsync(od);

                }
                await dbContext.SaveChangesAsync();

                ViewData["success"] = "Order successfull";


                if (Auth != null)
                {
                    Dictionary<Stream, string> attachments = new Dictionary<Stream, string>();
                    var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == Auth.CustomerId);
                    attachments.Add(new MemoryStream(PDFHelper.GenPDFInvoice(customer.CompanyName,customer.ContactName, customer.Address, listProducts).Save()), "Invoice_" + order.OrderId + ".pdf");
                    var subject = $"WE HAVE RECEIVED YOUR ORDER {order.OrderId}";
                    var body = "<h2>Thank you, we have received your order!</h2>" +
                    "Hope you can enjoy our product<br/>";
                    body += $"<p><b> Here Is Your Invoice </b> </p>";
                    MailHelper.SendMail(Auth.Email, body, subject, attachments);
                }
                HttpContext.Session.Remove("cart");
                HttpContext.Session.Remove("CartCount");
            }
            catch (Exception e)
            {
                ViewData["fail"] = e.Message;
            }

            return Page();
        }

        private string RandomCustID(int length)
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
