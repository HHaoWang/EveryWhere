namespace EveryWhere.MainServer.Entity.Exception;

public class NoNecessaryParameterException : System.Exception
{
    public NoNecessaryParameterException(string requiredParameter, string message = "No Necessary Parameter") : base(message)
    {
        this.RequiredParameter = requiredParameter;
    }

    public string RequiredParameter { get; }
}