using API.ResourceModels;
using Swashbuckle.AspNetCore.Filters;

namespace API.Infrastructure.Swagger
{
    public class PaymentRequestExample : IExamplesProvider<Payment>
    {
        public Payment GetExamples()
        {
            return new Payment
            {
                Amount = new Amount { Currency = "EUR", Value = 1020 },
                Card = new Card
                {
                    Number = "4444111111111111",
                    Name = "John Smith",
                    ExpiryMonth = "07",
                    ExpiryYear = "2021",
                    CVV = "123"
                }
            };
        }
    }
}
