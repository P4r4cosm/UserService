using System;

namespace Application.Exceptions;

/// <summary>
/// Базовый класс для всех кастомных исключений приложения.
/// </summary>
public abstract class ApplicationExceptionBase : Exception
{
    protected ApplicationExceptionBase(string message) : base(message)
    {
    }
}