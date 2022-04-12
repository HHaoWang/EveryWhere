namespace EveryWhere.MainServer.Entity.Exception;

public class RequestWechatOpenIdException : System.Exception
{
    public enum ErrorCode
    {
        WechatBusy = -1,
        Succes = 0,
        InvalidCode = 40029,
        TooManyUserRequest = 45011
    }

    private ErrorCode? _errorCode;

    public RequestWechatOpenIdException(string? message, ErrorCode? errorCode) : base(message)
    {
        this._errorCode = errorCode;
    }
}