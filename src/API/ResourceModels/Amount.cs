using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.ResourceModels
{
    /// <summary>
    /// Amount
    /// </summary>
    public class Amount
    {
        /// <summary>
        ///  The amount represented in smallest unit of the currency passed.
        ///  Most currencies have 2 decimals, some currencies, such as JPY, do not have decimals and some have 3 decimals.
        ///  For example:
        ///  - 10 EUR is submitted as 1000
        ///  - 10.20 EUR is submitted as 1020
        ///  - 10 JPY is submitted as 10
        /// </summary>
        /// <example>
        /// 1000
        /// </example>
        [Required]
        [Range(typeof(long), "1", "999999999999999999")]
        public long Value { get; set; }

        /// <summary>
        /// Currency of the payment
        /// Use the <a href="https://www.currency-iso.org/en/home/tables/table-a1.html">ISO 4217</a> three-letter alphabetic code for the currency
        /// </summary>
        /// <example>
        /// EUR
        /// </example>
        [Required]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Invalid currency format")]
        public string Currency { get; set; }
    }
}
