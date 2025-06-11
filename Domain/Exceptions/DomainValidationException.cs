namespace Domain.Entities;

public class DomainValidationException : Exception
{
    public DomainValidationException(string message) : base(message)
    {
    }
}