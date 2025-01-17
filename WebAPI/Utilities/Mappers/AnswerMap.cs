using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Utilities.Response.AsnwerResponses;

namespace WebAPI.Utilities.Mappers;

public static class AnswerMap
{
    public static AnswerResponse ToAnswerResponse(this Answer answer)
    {
        return new AnswerResponse
        {
            Id = answer.Id,
            Content = answer.Content,
            IsAccepted = answer.IsAccepted,
            Upvote = answer.Upvote,
            Downvote = answer.Downvote,
            CreatedAt = answer.CreatedAt,
            UpdatedAt = answer.UpdatedAt,
            Author = answer.Author.ToAuthorResponse(),
        };
    }


    public static Answer ToAnswer(this CreateAnswerDto dto, Guid authorId, Guid questionId)
    {
        return new Answer
        {
            Content = dto.Content,
            AuthorId = authorId,
            QuestionId = questionId,
        };
    }
}
