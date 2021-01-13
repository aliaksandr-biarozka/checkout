using System;
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
                    Number = "4539889990915926",
                    Name = "John Smith",
                    ExpiryMonth = DateTime.UtcNow.Month.ToString("00"),
                    ExpiryYear = DateTime.UtcNow.Year.ToString(),
                    CVV = "123"
                }
            };
        }
    }
}
