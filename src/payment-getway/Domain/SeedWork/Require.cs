using System;
namespace Domain.SeedWork
{
    public static class Require
    {
        public static void That(bool expectedCondition, string violationMessage)
        {
            if (!expectedCondition)
                throw new DomainRuleViolationException(violationMessage);
        }
    }
}
