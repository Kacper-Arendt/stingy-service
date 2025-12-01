using OpenAI.Chat;

namespace Shared.Infrastructure.Services;

public interface IAzureOpenAiService
{
    ChatClient Chat { get; }
}