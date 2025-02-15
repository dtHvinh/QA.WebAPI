using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace WebAPI.Utilities.Services;

public class AIService
{
    private readonly OpenAIClient _openAI;
    private readonly ChatClient _chatClient;

    public ChatClient ChatClient => _chatClient;

    public AIService(Uri endpoint, string model)
    {
        _openAI = new OpenAIClient(new ApiKeyCredential("api-key"), new OpenAIClientOptions
        {
            Endpoint = endpoint
        });

        _chatClient = _openAI.GetChatClient(model);
    }
}
