namespace VariBillWebAPI.Exceptions;

public class BusinessRuleException : BaseException
{
    public BusinessRuleException(string message)
        : base(message, "BUSINESS_RULE", 409)
    {
    }
}