using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Response.AsnwerResponses;

namespace WebAPI.Utilities.Extensions;

public static class AnswerExtensions
{
    public static AnswerResponse ToAnswerResponse(this Answer answer)
    {
        return new AnswerResponse
        {
            Id = answer.Id,
            Content = answer.Content,
            IsAccepted = answer.IsAccepted,
            Score = answer.Score,
            CreatedAt = answer.CreationDate,
            UpdatedAt = answer.ModificationDate,
            Author = answer.Author.ToAuthorResponse(),
        };
    }


    public static Answer ToAnswer(this CreateAnswerDto dto, int authorId, int questionId)
    {
        return new Answer
        {
            Content = dto.Content,
            AuthorId = authorId,
            QuestionId = questionId,
        };
    }
}
