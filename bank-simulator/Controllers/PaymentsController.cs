using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BankSimulator.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentsController : ControllerBase
    {
        //it cointains simple responses for testing
        [HttpPost]
        public IActionResult Process(PaymentRequest request)
        {
            if(Int32.Parse(request.Card.ExpiryYear) < DateTime.UtcNow.Year ||
               Int32.Parse(request.Card.ExpiryYear) == DateTime.UtcNow.Year && Int32.Parse(request.Card.ExpiryMonth) < DateTime.UtcNow.Month)
            {
                return StatusCode(403, new ProblemDetails
                {
                    Title = "Card is expired"
                });
            }

            if(request.Amount.Value > 1_000_000_000)
            {
                return Ok(new PaymentResult { Status = PaymentStatus.Rejected, TransactionId = Guid.NewGuid() });
            }
            else if (request.Amount.Value > 10_000_000)
            {
                var n = new Random(128).Next(0, 10);
                if (n == 1)
                    return StatusCode(500);
                else if (n == 0)
                    return StatusCode(429);
            }
            else if (request.Amount.Value >  1_000_000)
            {
                return Ok(new PaymentResult { Status = (PaymentStatus)new Random(128).Next(0, 2), TransactionId = Guid.NewGuid() });
            }
            

            return Ok(new PaymentResult { Status = PaymentStatus.Approved, TransactionId = Guid.NewGuid() });
        }
    }

    public class PaymentRequest
    {
        [Required]
        public Guid MerchantId { get; set; }

        [Required]
        public Card Card { get; set; }

        [Required]
        public Amount Amount { get; set; }
    }

    public class Card
    {
        [Required]
        [RegularExpression("^[0-9]{13,19}$", ErrorMessage = "The card number length must be 13 to 19 digits")]
        public string Number { get; set; }

        [Required]
        [StringLength(26, MinimumLength = 2, ErrorMessage = "The card holder name must be 2 to 26 characters")]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^0[1-9]|1[0-2]$", ErrorMessage = "The expiry month must be 2 digits")]
        public string ExpiryMonth { get; set; }

        [Required]
        [RegularExpression(@"^[2-9]\d{3}$", ErrorMessage = "The expiry year must be 4 digits")]
        public string ExpiryYear { get; set; }

        [Required]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV/CVC must be 3 to 4 digits")]
        public string CVV { get; set; }
    }

    public class Amount
    {
        public string Currency { get; set; }

        public long Value { get; set; }
    }

    public class PaymentResult
    {
        public Guid TransactionId { get; set; }

        public PaymentStatus Status { get; set; }
    }

    public enum PaymentStatus
    {
        Approved,
        Rejected
    }
}
