using Shared.Abstractions.ValueObjects;

namespace Shared.Abstractions.Communication;

public interface IEmailSender
{
    Task SendEmailAsync(List<Email> mails, string subject, string body);
}
