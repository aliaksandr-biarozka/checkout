using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using API.Infrastructure;
using API.Infrastructure.Swagger;
using API.ResourceModels;
using Application;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace API.Controllers
{
    /// <summary>
    /// Payment endpoints
    /// </summary>
    [ApiVersion("1.0")]
    [Route("payments")]
    public class PaymentsController : ApiController
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Process payment
        /// </summary>
        /// <param name="payment">payment to process</param>
        /// <param name="merchant_id">merchant id</param>
        /// <param name="request_id">Allows you to send a request that is "idempotent safe". The key must be unique and should only be used in one request</param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [SwaggerRequestExample(typeof(Payment), typeof(PaymentRequestExample))]
        [SwaggerResponseExample((int)HttpStatusCode.Created, typeof(PaymentResponseExample))]
        [ProducesResponseType(typeof(Payment), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> ProcessPayment([FromBody]Payment payment, [FromHeader, Required] Guid merchant_id, [FromHeader, Required] Guid request_id)
        {
            var result = await _paymentService.ProcessPaymentRequest(merchant_id, request_id, payment.ToDto());

            return CreatedAtRoute(new { paymentId = result.PaymentId }, result.ToResource());
        }

        /// <summary>
        /// Payment details retrival
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        [HttpGet("{paymentId:guid}")]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(PaymentResponseExample))]
        [ProducesResponseType(typeof(Payment), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetPayment(Guid paymentId, [FromHeader, Required]Guid merchant_id)
        {
            var payment = await _paymentService.GetPayment(merchant_id, paymentId);

            if (payment == null)
                return NotFound();

            return Ok(payment.ToResource());
        }
    }
}
