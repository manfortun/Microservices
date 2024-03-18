namespace WebClient.Models;

public enum Result
{
    Success, Failed, ConnectionError
}

public class DeserializationResult<T> where T : class
{
    private Result _result { get; set; }
    public bool Successful => _result == Result.Success;
    public bool Connected => _result != Result.ConnectionError;
    public T Content { get; set; } = default!;

    public static DeserializationResult<T> Success(T output)
    {
        ArgumentNullException.ThrowIfNull(nameof(output));

        return new DeserializationResult<T>
        {
            _result = Result.Success,
            Content = output
        };
    }

    public static DeserializationResult<T> Failed()
    {
        return new DeserializationResult<T>
        {
            _result = Result.Failed
        };
    }

    public static DeserializationResult<T> Failed(T content)
    {
        return new DeserializationResult<T>
        {
            _result = Result.Failed,
            Content = content
        };
    }

    public static DeserializationResult<T> ConnectionError()
    {
        return new DeserializationResult<T>
        {
            _result = Result.ConnectionError,
        };
    }
}
