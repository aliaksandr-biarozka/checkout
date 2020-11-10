using System;

namespace Infrastructure
{
    public interface IAcquiringBankConfigurationProvider
    {
        AcquiringBankConfiguration Get(Guid merchantId);
    }

    // provde all configuration such as base adress, endpoint and etc., that are needed to do a call to an acquiring bank
    // for simplicity there are only two params
    public class AcquiringBankConfiguration
    {
        public string BaseAddress { get; set; }

        public string Endpoint { get; set; }
    }

    // mock impl for implicity. we can get configuration from a store taking into consideration closer location and etc. 
    public class AcquiringBankConfigurationProvider : IAcquiringBankConfigurationProvider
    {
        private AcquiringBankConfiguration _acquiringBankConfiguration;

        public AcquiringBankConfigurationProvider(AcquiringBankConfiguration acquiringBankConfiguration)
            => _acquiringBankConfiguration = acquiringBankConfiguration;

        public AcquiringBankConfiguration Get(Guid merchantId)
        {
            return _acquiringBankConfiguration;
        }
    }
}
