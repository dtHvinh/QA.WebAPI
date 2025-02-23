﻿using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface ICommentRepository : IRepository<Comment>
{
    int CountQuestionComment(int questionId);
    Task<int> CountUserComment(int userId, CancellationToken cancellationToken);
    Task<Comment?> GetCommentByIdAsync(int commentId);
    void UpdateComment(Comment comment);
}
