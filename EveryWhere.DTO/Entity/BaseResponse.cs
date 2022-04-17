namespace EveryWhere.DTO.Entity;

public class BaseResponse<T>
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public virtual T? Data { get; set; }
}