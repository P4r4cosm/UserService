namespace Application.Exceptions;


/// <summary>
/// Исключение, выбрасываемое, когда у пользователя нет прав на выполнение операции.
/// </summary>
public class UnauthorizedAccessException: ApplicationExceptionBase
{
    public UnauthorizedAccessException(string message) : base(message)
    {
    }
}
