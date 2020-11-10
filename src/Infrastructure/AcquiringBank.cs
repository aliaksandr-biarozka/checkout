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

            HttpResponseMessage response = null;
            var requestModel = JsonConvert.SerializeObject(ToBankRequestModel(paymentRequest));

            try
            {
                // provided client has cofigured retries and circuit breaker and logging
                response = await _client.PostAsync(config.Endpoint, new StringContent(requestModel, Encoding.UTF8, MediaTypeNames.Application.Json));
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException($"Attempt to access third-party API returns {response.StatusCode} status code. POST: {_client.BaseAddress + config.Endpoint}. Request Body: {requestModel}.", e);
            }

            if (!response.IsSuccessStatusCode)
            {
                //convert error data from acquiring bank to our domain langunage data and return it to a client. Code below is just for an example.
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
            return new TransactionRequest
            {
                MerchantId = paymentRequest.MerchantId,
                RequestId = paymentRequest.RequestId,
                Amount = new Amount
                {
                    Currency = paymentRequest.Amount.Currency,
                    Value = paymentRequest.Amount.Value
                },
                Card = new Card
                {
                    Number = paymentRequest.Card.Number,
                    Name = paymentRequest.Card.Name,
                    ExpiryMonth = paymentRequest.Card.ExpiryMonth,
                    ExpiryYear = paymentRequest.Card.ExpiryYear,
                    Cvv = paymentRequest.Card.Cvv
                }
            };
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

        class TransactionRequest
        {
            public Guid MerchantId { get; set; }

            public Guid RequestId { get; set; }

            public Card Card { get; set; }

            public Amount Amount { get; set; }
        }

        class Card
        {
            public string Number { get; set; }

            public string Name { get; set; }

            public string ExpiryMonth { get; set; }

            public string ExpiryYear { get; set; }

            public string Cvv { get; set; }
        }

        class Amount
        {
            public string Currency { get; set; }

            public long Value { get; set; }
        }

        class TransactionResult
        {
            public Guid TransactionId { get; set; }

            public TransactionStatus Status { get; set; }
        }

        enum TransactionStatus
        {
            Approved,
            Rejected
        }
    }
}
