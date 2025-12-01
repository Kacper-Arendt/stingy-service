namespace Shared.Abstractions.Exceptions;

public class ValueObjectInvalidTypeException(string message) : CustomException(message);