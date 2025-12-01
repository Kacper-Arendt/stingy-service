using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace Shared.Infrastructure.Services;

public class AzureOpenAiService: IAzureOpenAiService
{
    public AzureOpenAiService(IConfiguration configuration)
    {
        var azureClient = new AzureOpenAIClient(
            new Uri(configuration["AzureOpenAI:Endpoint"]),
            // new DefaultAzureCredential()
            new AzureKeyCredential(configuration["AzureOpenAI:ApiKey"])
        );

        Chat = azureClient.GetChatClient(configuration["AzureOpenAI:DeploymentName"]);
    }

    public ChatClient Chat { get; }
}