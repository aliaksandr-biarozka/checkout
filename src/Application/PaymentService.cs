using System;
using System.Threading.Tasks;
using Domain;
using Domain.SeedWork;

namespace Application
{
    public class PaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        private readonly IPaymentRequestDuplicateChecker _paymentRequestDuplicateChecker;

        private readonly IAcquiringBank _acquiringBank;

        public PaymentService(IPaymentRepository paymentRepository, IAcquiringBank acquiringBank, IPaymentRequestDuplicateChecker paymentRequestDuplicateChecker)
        {
            _paymentRepository = paymentRepository;
            _paymentRequestDuplicateChecker = paymentRequestDuplicateChecker;
            _acquiringBank = acquiringBank;
        }

        public async Task<PaymentDto> ProcessPaymentRequest(Guid merchantId, Guid requestId, PaymentReqestDto request)
        {

            try
            {
                // there should be preliminary fraud checking.
                var card = new Card(request.CardNumber, request.CardHolderName, request.ExpiryMonth, request.ExpiryYear, request.CVV);
                var amount = new Money(request.Currency, request.Amount);
                var paymentRequest = new PaymentRequest(merchantId, requestId, card, amount);

                await _paymentRequestDuplicateChecker.Check(paymentRequest);
                
                var paymentResult = await _acquiringBank.Process(paymentRequest);

                var payment = new Payment(CardDetails.From(card), amount, paymentResult.Status, paymentResult.AcquiringBankTransactionId, merchantId);

                await _paymentRepository.Save(payment);

                return ToDto(payment);
            }
            catch(DomainRuleViolationException e)
            {
                throw new ApplicationServiceException(e.Message, e);
            }
        }

        public async Task<PaymentDto> GetPayment(Guid merchantId, Guid paymentId)
        {
            var payment = await _paymentRepository.Get(paymentId, merchantId);

            return payment != null ? ToDto(payment) : null;
        }

        private PaymentDto ToDto(Payment payment)
        {
            return new PaymentDto
            {
                PaymentId = payment.Id,
                CardNumber = payment.CardDetails.Number,
                CardHolderName = payment.CardDetails.Name,
                ExpiryMonth = payment.CardDetails.ExpiryMonth,
                ExpiryYear = payment.CardDetails.ExpiryYear,
                Status = payment.Status == Domain.PaymentStatus.Approved ? PaymentStatus.Approved : PaymentStatus.Rejected,
                Amount = payment.Amount.Value,
                Currency = payment.Amount.Currency
            };
        }
    }
}
