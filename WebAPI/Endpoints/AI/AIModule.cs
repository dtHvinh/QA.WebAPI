using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Services;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.AI;

public class AIModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.AI);

        group.MapPost("/chat", SendMessage);
    }

    public static async Task SendMessage(
        [FromBody] ChatRequest request, AIService service, HttpContext httpContext, AuthenticationContext authenticationContext, CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "text/event-stream";

        var userMessage = new UserChatMessage(request.NewMessage);

        var chatMessages = request.Messages.Select<ChatMessageRequest, ChatMessage>(m =>
            m.Role == "user" ? new UserChatMessage(m.Content) : new AssistantChatMessage(m.Content)
        ).ToList();

        chatMessages.Add(userMessage);

        var streamingReply = service.ChatClient.CompleteChatStreamingAsync(
            chatMessages,
            new() { EndUserId = authenticationContext.UserId.ToString() },
            cancellationToken);

        await foreach (var streamingMessage in streamingReply)
        {
            if (streamingMessage.ContentUpdate.Count > 0)
            {
                var content = streamingMessage.ContentUpdate[0].Text;

                await httpContext.Response.WriteAsync($"{content}", cancellationToken);
                await httpContext.Response.Body.FlushAsync(cancellationToken);
            }
        }
    }

    public class ChatRequest
    {
        public List<ChatMessageRequest> Messages { get; set; } = default!;
        public string NewMessage { get; set; } = default!;
    }

    public class ChatMessageRequest
    {
        public string Content { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
