using PayPal.Api;
using RadioTaxi.Models;
using RadioTaxi.Models.CheckoutVM;

namespace RadioTaxi.Services
{
    public interface IPayPal
    {
        Task<Payment> CreateOrder(CheckoutCRUD order, string returnUrl, string cancelUrl);
        Task<Payment> CapturePayment(string paymentId, string payerId);

    }
}
