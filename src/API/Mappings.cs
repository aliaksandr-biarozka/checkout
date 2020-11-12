using System.Collections.Generic;
using API.ResourceModels;
using Application;

namespace API
{
    public static class Mappings
    {
        private static readonly Dictionary<Application.PaymentStatus, ResourceModels.PaymentStatus> _paymentStatusMap = new Dictionary<Application.PaymentStatus, ResourceModels.PaymentStatus>
        {
            { Application.PaymentStatus.Approved, ResourceModels.PaymentStatus.Approved },
            { Application.PaymentStatus.Rejected, ResourceModels.PaymentStatus.Rejected }
        };

        public static Payment ToResource(this PaymentDto dto)
        {
            return new Payment
            {
                PaymentId = dto.PaymentId,
                Status = _paymentStatusMap[dto.Status],
                Amount = new Amount
                {
                    Currency = dto.Currency,
                    Value = dto.Amount
                },
                Card = new Card
                {
                    ExpiryMonth = dto.ExpiryMonth,
                    ExpiryYear = dto.ExpiryYear,
                    Name = dto.CardHolderName,
                    Number = dto.CardNumber
                }
            };
        }

        public static PaymentReqestDto ToDto(this Payment resource)
        {
            return new PaymentReqestDto(
                resource.Card.Number,
                resource.Card.Name,
                resource.Card.ExpiryMonth,
                resource.Card.ExpiryYear,
                resource.Card.CVV,
                resource.Amount.Value,
                resource.Amount.Currency
                );
        }
    }
}
