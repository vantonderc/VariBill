namespace VariBillWebAPI.Exceptions;

public class ConcurrencyException : BaseException
{
    public ConcurrencyException(string message)
        : base(message, "CONCURRENCY", 409)
    {
    }
}