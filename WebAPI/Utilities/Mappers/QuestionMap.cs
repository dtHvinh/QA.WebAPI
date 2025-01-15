using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Utilities.Response.QuestionResponses;

namespace WebAPI.Utilities.Mappers;

public static class QuestionMap
{
    public static Question ToQuestion(this CreateQuestionDto dto, Guid authorId, bool isDraft)
    {
        return new Question
        {
            Title = dto.Title,
            Content = dto.Content,
            Slug = dto.Title.GenerateSlug(),
            AuthorId = authorId,
            IsDraft = isDraft
        };
    }

    public static Question ToQuestion(this UpdateQuestionDto dto, Guid authorId)
    {
        return new Question
        {
            Id = dto.Id,
            Title = dto.Title,
            Content = dto.Content,
            Slug = dto.Title.GenerateSlug(),
            AuthorId = authorId,
        };
    }

    public static GetQuestionResponse ToGetQuestionResponse(this Question obj)
    {
        return new GetQuestionResponse()
        {
            Id = obj.Id,
            Title = obj.Title,
            Slug = obj.Slug,
            Content = obj.Content,
            Author = obj.Author.ToAuthorResponse(),
            IsDuplicate = obj.IsDuplicate,
            IsClosed = obj.IsClosed,
            IsDraft = obj.IsDraft,
            ViewCount = obj.ViewCount,
            AnswerCount = obj.AnswerCount,
            Upvote = obj.Upvote,
            Downvote = obj.Downvote,
            Tags = obj.QuestionTags.Select(x => x.Tag.ToTagResonse()).ToList(),
            Answers = obj.Answers.Select(x => x.ToAnswerResponse()).ToList(),
            Comments = obj.Comments.Select(x => x.ToCommentResponse()).ToList()
        };
    }
}
