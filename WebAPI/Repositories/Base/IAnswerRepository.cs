﻿using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface IAnswerRepository : IRepository<Answer>
{
    void AddAnswer(Answer answer);
    int CountQuestionAnswer(int questionId);
    Task<Answer?> FindAnswerById(int id, CancellationToken cancellationToken = default);
    void TrySoftDeleteAnswer(Answer answer, out string? errMsg);
    void TryEditAnswer(Answer answer, out string? errMsg);
    Task<List<Answer>> GetAnswersAsync(int questionId, CancellationToken cancellation = default);
    Task<Answer?> FindAnswerWithAuthorById(int id, CancellationToken cancellationToken = default);
    Task<int> CountUserAnswer(int userId, CancellationToken cancellationToken = default);
    Task<int> CountUserAcceptedAnswer(int userId, CancellationToken cancellationToken = default);
    Task AddAnswerAndLoadAuthor(Answer answer, CancellationToken cancellationToken = default);
    void UpdateAnswer(Answer answer);
}
