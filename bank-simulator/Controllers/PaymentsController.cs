using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BankSimulator.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Process(PaymentRequest request)
        {
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
        public string Number { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ExpiryMonth { get; set; }

        [Required]
        public string ExpiryYear { get; set; }

        [Required]
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
