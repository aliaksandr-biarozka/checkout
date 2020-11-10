using System;
namespace Application
{
    public class ApplicationServiceException : ApplicationException
    {
        public ApplicationServiceException()
        {
        }

        public ApplicationServiceException(string message)
            : base(message)
        {
        }

        public ApplicationServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
