using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Response.QuestionResponses;

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

    public static Question FromUpdateObject(this Question current, UpdateQuestionDto other)
    {
        current.Title = other.Title;
        current.Content = other.Content;

        return current;
    }

    public static GetQuestionResponse ToGetQuestionResponse(this Question obj)
    {
        var response = new GetQuestionResponse()
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
            CommentCount = obj.CommentCount,

            Upvote = obj.Upvote,
            Downvote = obj.Downvote,
            CreatedAt = obj.CreatedAt,
            UpdatedAt = obj.UpdatedAt,
        };

        if (obj.Tags != null)
            response.Tags = obj.Tags.Select(x => x.ToTagResonse()).ToList();

        if (obj.Answers != null)
            response.Answers = obj.Answers.Select(x => x.ToAnswerResponse()).ToList();

        if (obj.Comments != null)
            response.Comments = obj.Comments.Select(e => e.ToCommentResponse().SetResourceRight(obj.Author?.Id)).ToList();

        return response;
    }
}
