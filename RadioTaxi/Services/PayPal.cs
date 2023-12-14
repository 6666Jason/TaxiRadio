using Microsoft.Extensions.Configuration;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RadioTaxi.Models;
using RadioTaxi.Models.CheckoutVM;

namespace RadioTaxi.Services
{
    public class PayPal : IPayPal
    {
        private readonly IConfiguration _configuration;
        private readonly APIContext _apiContext;

        public PayPal(IConfiguration configuration)
        {
            _configuration = configuration;
            var clientId = _configuration["PaypalSettings:ClientId"];
            var secret = _configuration["PaypalSettings:Secret"];

            var config = new Dictionary<string, string>
            {
                { "mode", "sandbox" },
                { "clientId", clientId },
                { "clientSecret", secret },
            };

            var accessToken = new OAuthTokenCredential(clientId, secret, config).GetAccessToken();
            _apiContext = new APIContext(accessToken);
        }

        public async Task<Payment> CreateOrder(CheckoutCRUD order, string returnUrl, string cancelUrl)
        {
            decimal taxRate = 0.1m;
			var itemList = new ItemList()
			{
				items = new List<Item>() // Khởi tạo danh sách Item
			};
			var item = new Item()
			{
				name = order.NamePackage,
				currency = "USD",
				price = order.Price.ToString(),
				quantity = "1",
				sku = order.IDkey.ToString()
			};
			itemList.items.Add(item);
			decimal subtotal = (decimal)order.Price * 1;


			decimal tax = subtotal * taxRate;
            decimal total = subtotal + tax;
            var transaction = new Transaction()
            {
                amount = new Amount()
                {
                    currency = "USD",
                    details = new Details()
                    {
                        subtotal = subtotal.ToString(),
                        tax = tax.ToString()
                    },
                    total = total.ToString()
                },
                item_list = itemList,
                description = order.NamePackage + order.DateSet + "/Month" ,
            };

            var payment = new Payment()
            {
                intent = "sale",
                payer = new Payer() { payment_method = "paypal" },
                redirect_urls = new RedirectUrls()
                {
                    return_url = returnUrl,
                    cancel_url = cancelUrl,
                },
                transactions = new List<Transaction> { transaction }
            };

            var createPayment = payment.Create(_apiContext);
            return createPayment;
        }
        public async Task<Payment> CapturePayment(string paymentId, string payerId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            var payment = new Payment() { id = paymentId };

            var executedPayment = payment.Execute(_apiContext, paymentExecution);

            return executedPayment;
        }

    }
}
