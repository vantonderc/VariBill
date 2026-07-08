namespace VariBillWebAPI.Exceptions;

public abstract class BaseException : Exception
{
    public string ErrorCode { get; }
    public int HttpStatusCode { get; }

    protected BaseException(string message, string errorCode, int httpStatusCode)
        : base(message)
    {
        ErrorCode = errorCode;
        HttpStatusCode = httpStatusCode;
    }
}







//using System;

//namespace VariBillWebAPI.Exceptions;

//public class NotFoundException : Exception
//{
//    public NotFoundException() { }
//    public NotFoundException(string message) : base(message) { }
//    public NotFoundException(string message, Exception inner) : base(message, inner) { }
//}