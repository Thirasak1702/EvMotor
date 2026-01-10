namespace EbikeRental.Shared;

public class Result
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static Result Ok(string message = "Operation successful")
    {
        return new Result { Success = true, Message = message };
    }

    public static Result Fail(string message, params string[] errors)
    {
        return new Result
        {
            Success = false,
            Message = message,
            Errors = errors.ToList()
        };
    }
}

public class Result<T> : Result
{
    public T? Data { get; set; }

    public static Result<T> Ok(T data, string message = "Operation successful")
    {
        return new Result<T> { Success = true, Data = data, Message = message };
    }

    public new static Result<T> Fail(string message, params string[] errors)
    {
        return new Result<T>
        {
            Success = false,
            Message = message,
            Errors = errors.ToList()
        };
    }
}
