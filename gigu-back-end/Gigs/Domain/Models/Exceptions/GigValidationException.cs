using System;

namespace Gigs.Domain.Models.Exceptions
{
    public class GigValidationException : Exception
    {
        public GigValidationException() : base("Gig validation failed")
        {
        }

        public GigValidationException(string message) : base(message)
        {
        }

        public GigValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}