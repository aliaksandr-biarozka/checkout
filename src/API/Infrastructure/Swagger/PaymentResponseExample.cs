using System;
using API.ResourceModels;
using Swashbuckle.AspNetCore.Filters;

namespace API.Infrastructure.Swagger
{
    public class PaymentResponseExample : IExamplesProvider<Payment>
    {
        public Payment GetExamples()
        {
            return new Payment
            {
                PaymentId = Guid.NewGuid(),
                Status = PaymentStatus.Approved,
                Amount = new Amount { Currency = "EUR", Value = 1020 },
                Card = new Card
                {
                    Number = "************1111",
                    Name = "John Smith",
                    ExpiryMonth = DateTime.UtcNow.Month.ToString("00"),
                    ExpiryYear = DateTime.UtcNow.Year.ToString(),
                }
            };
        }
    }
}
