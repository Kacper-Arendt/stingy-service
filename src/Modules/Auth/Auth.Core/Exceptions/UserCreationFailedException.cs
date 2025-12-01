using Microsoft.AspNetCore.Identity;
using Shared.Abstractions.Exceptions;

namespace Auth.Core.Exceptions;

internal class UserCreationFailedException(List<IdentityError> errors) : CustomException(string.Join(", ", errors.Select(x => x.Description)))
{
};