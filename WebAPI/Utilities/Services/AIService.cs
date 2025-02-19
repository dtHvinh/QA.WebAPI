using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace WebAPI.Utilities.Services;

public class AIService
{
    private readonly OpenAIClient _openAI;
    private readonly ChatClient _chatClient;
    private readonly ChatClient _chatReasoningClient;

    public ChatClient ChatClient => _chatClient;
    public ChatClient ReasoningChatClient => _chatReasoningClient;

    public AIService(Uri endpoint, string model, string reasoningModel)
    {
        _openAI = new OpenAIClient(new ApiKeyCredential("api-key"), new OpenAIClientOptions
        {
            Endpoint = endpoint
        });

        _chatClient = _openAI.GetChatClient(model);
        _chatReasoningClient = _openAI.GetChatClient(reasoningModel);
    }
}
