
namespace Application.Exceptions;

/// <summary>
/// Исключение, выбрасываемое при ошибках валидации.
/// </summary>
public class ValidationException : ApplicationExceptionBase
{
    /// <summary>
    /// Словарь, содержащий ошибки валидации для каждого поля.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IReadOnlyDictionary<string, string[]> errors) 
        : base("One or more validation failures have occurred.")
    {
        Errors = errors;
    }
}