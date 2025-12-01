namespace Shared.Abstractions.Exceptions;

public class DomainModelArgumentException(string message) : CustomException(message);