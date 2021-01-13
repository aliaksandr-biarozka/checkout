using System;
namespace Domain.SeedWork
{
    public class DomainRuleViolationException : ApplicationException
    {
        public DomainRuleViolationException()
        {
        }

        public DomainRuleViolationException(string message)
            : base(message)
        {
        }

        public DomainRuleViolationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
