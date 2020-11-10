using System;
using System.ComponentModel.DataAnnotations;

namespace API.ResourceModels
{
    /// <summary>
    /// Payment
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// The payment's unique identifier. It is readonly and set by the system
        /// </summary>
        public Guid? PaymentId { get; set; }

        /// <summary>
        /// The payment's status. It is readonly and set by the systemw
        /// </summary>
        public PaymentStatus? Status { get; set; }

        /// <summary>
        /// The shopper card
        /// </summary>
        [Required]
        public Card Card { get; set; }

        /// <summary>
        /// The amount
        /// </summary>
        [Required]
        public Amount Amount { get; set; }
    }
}
