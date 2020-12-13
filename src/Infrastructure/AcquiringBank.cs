using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.SeedWork;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Infrastructure
{
    public class AcquiringBank : IAcquiringBank
    {
        private readonly IAcquiringBankConfigurationProvider _configurationProvider;

        private readonly HttpClient _client;

        public AcquiringBank(IAcquiringBankConfigurationProvider configurationProvider, HttpClient client)
        {
            _configurationProvider = configurationProvider;
            _client = client;
        }

        public async Task<PaymentResult> Process(PaymentRequest paymentRequest)
        {
            var config = _configurationProvider.Get(paymentRequest.MerchantId);

            _client.BaseAddress = new Uri(config.BaseAddress);

            var requestModel = JsonConvert.SerializeObject(ToBankRequestModel(paymentRequest));

            var response = await _client.PostAsync(config.Endpoint, new StringContent(requestModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new DomainRuleViolationException(JsonConvert.DeserializeObject<ProblemDetails >(await response.Content.ReadAsStringAsync()).Title);
                }
                else
                    throw new HttpRequestException($"Attempt to access third-party API returns {response.StatusCode} status code. POST: {_client.BaseAddress + config.Endpoint}. Request Body: {requestModel}. Response: {await response.Content.ReadAsStringAsync()}");
            }

            var transactionResult = JsonConvert.DeserializeObject<TransactionResult>(await response.Content.ReadAsStringAsync());

            return ToDomainModel(transactionResult);
        }

        private static TransactionRequest ToBankRequestModel(PaymentRequest paymentRequest)
        {
            return new TransactionRequest(paymentRequest.MerchantId, paymentRequest.RequestId,
                new Amount(paymentRequest.Amount.Currency, paymentRequest.Amount.Value),
                new Card(paymentRequest.Card.Number, paymentRequest.Card.Name, paymentRequest.Card.ExpiryMonth, paymentRequest.Card.ExpiryYear, paymentRequest.Card.Cvv));
        }

        private static PaymentResult ToDomainModel(TransactionResult transactionResult)
        {
            return new PaymentResult(transactionResult.TransactionId, transactionResult.Status switch
            {
                TransactionStatus.Approved => PaymentStatus.Approved,
                TransactionStatus.Rejected => PaymentStatus.Rejected,
                _ => throw new DomainRuleViolationException($"Unknown status: {transactionResult.Status}")
            });
        }

        record TransactionRequest (Guid MerchantId, Guid RequestId, Amount Amount, Card Card);

        record Card (string Number, string Name, string ExpiryMonth, string ExpiryYear, string Cvv);

        record Amount (string Currency, long Value);

        record TransactionResult (Guid TransactionId, TransactionStatus Status);

        enum TransactionStatus
        {
            Approved,
            Rejected
        }
    }
}
