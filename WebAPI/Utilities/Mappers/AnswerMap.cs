using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Response.AsnwerResponses;

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
            Score = answer.Score,
            CreatedAt = answer.CreatedAt,
            UpdatedAt = answer.UpdatedAt,
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
