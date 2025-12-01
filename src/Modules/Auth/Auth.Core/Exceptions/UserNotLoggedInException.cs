using Shared.Abstractions.Exceptions;

namespace Auth.Core.Exceptions;

internal class UserNotLoggedInException() : CustomException("User is not logged in");
