﻿using Riok.Mapperly.Abstractions;
using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Response.QuestionResponses;

namespace WebAPI.Utilities.Extensions;

[Mapper]
public static partial class QuestionExtensions
{
    public static Question ToQuestion(this CreateQuestionDto dto, int authorId)
    {
        return new Question
        {
            Title = dto.Title,
            Content = dto.Content,
            Slug = dto.Title.GenerateSlug(),
            AuthorId = authorId,
        };
    }

    public static Question ToQuestion(this UpdateQuestionDto dto, int authorId)
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
        current.Slug = other.Title.GenerateSlug();

        return current;
    }

    public static partial GetQuestionResponse ToGetQuestionResponse(this Question source);

    public static GetQuestionResponse ToGetQuestionResponse(this Question obj, int requesterId)
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
            IsSolved = obj.IsSolved,
            IsDeleted = obj.IsDeleted,

            ViewCount = obj.ViewCount,
            AnswerCount = obj.AnswerCount,
            CommentCount = obj.CommentCount,

            Score = obj.Score,
            CreationDate = obj.CreationDate,
            ModificationDate = obj.ModificationDate,

            DuplicateQuestionUrl = obj.DuplicateQuestionUrl,
        };

        if (obj.Tags != null)
            response.Tags = obj.Tags.Select(x => x.ToTagResonse()).ToList();

        if (obj.Answers != null)
            response.Answers = obj.Answers.Select(x => x.ToAnswerResponse().SetResourceRight(requesterId)).ToList();

        if (obj.Comments != null)
            response.Comments = obj.Comments.Select(e => e.ToCommentResponse().SetResourceRight(requesterId)).ToList();

        return response;
    }
}
