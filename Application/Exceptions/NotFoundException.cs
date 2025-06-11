
using System;

namespace Application.Exceptions;

/// <summary>
/// Исключение, выбрасываемое, когда запрашиваемая сущность не найдена.
/// </summary>
public class NotFoundException : ApplicationExceptionBase
{
    public NotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// Создает исключение с сообщением по умолчанию на основе имени сущности и ключа.
    /// </summary>
    /// <param name="entityName">Имя сущности (например, "User").</param>
    /// <param name="key">Ключ, по которому искали (например, логин или Guid).</param>
    public NotFoundException(string entityName, object key)
        : base($"Entity \"{entityName}\" with key ({key}) was not found.")
    {
    }
}