using System.ComponentModel.DataAnnotations;

namespace API.ResourceModels
{
    /// <summary>
    /// Card
    /// </summary>
    public class Card
    {
        /// <summary>
        /// The card number. Numeric string, 13-19 digits
        /// </summary>
        /// <example>
        /// 4444111111111111
        /// </example>
        [Required]
        [RegularExpression("^[0-9]{13,19}$", ErrorMessage = "The card number length must be 13 to 19 digits")]
        public string Number { get; set; }

        /// <summary>
        /// The name of the cardholder. According the ISO IEC 7813 the cardholder name length must be 2 to 26 characters including first name, last name and spaces
        /// </summary>
        /// <example>
        /// John Smith
        /// </example>
        [Required]
        [StringLength(26, MinimumLength = 2, ErrorMessage = "The card holder name must be 2 to 26 characters")]
        public string Name { get; set; }

        /// <summary>
        /// The card expiration month. Numeric string, 2 digits
        /// </summary>
        /// <example>
        /// 09
        /// </example>
        [Required]
        [RegularExpression("^0[1-9]|1[0-2]$")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "The expiry month must be 2 digits")]
        public string ExpiryMonth { get; set; }

        /// <summary>
        /// The card expiration year. Numeric string, 4 digits
        /// </summary>
        /// <example>
        /// 2021
        /// </example>
        [Required]
        [RegularExpression(@"^[2-9]\d{3}$", ErrorMessage = "The expiry year must be 4 digits")]
        public string ExpiryYear { get; set; }

        /// <summary>
        /// The card verification value/code. Numeric string, 3-4 digits
        /// </summary>
        /// <example>
        /// 123
        /// </example>
        [Required]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV/CVC must be 3 to 4 digits")]
        public string CVV { get; set; }
    }
}
