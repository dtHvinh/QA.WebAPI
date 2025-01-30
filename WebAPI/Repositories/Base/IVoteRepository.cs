
using static WebAPI.Utilities.Enums;

namespace WebAPI.Repositories.Base;

public interface IVoteRepository
{
    /// <summary>
    /// Upvote a question
    /// </summary>
    /// 
    /// <remarks>
    /// <list type="table">
    /// <item>
    /// If vote <strong>exists</strong> and <see cref="Model.Vote.IsUpvote"/> is <see langword="true"/>, nothing will happen.
    /// Return <see cref="VoteUpdateTypes.NoChange"/>.
    /// </item>
    /// 
    /// <item>
    /// If vote <strong>exists</strong> but and <see cref="Model.Vote.IsUpvote"/> is <see langword="false"/>, it will be changed to
    /// <see langword="true"/>. Return <see cref="VoteUpdateTypes.ChangeVote"/>.
    /// </item>
    /// 
    /// <item>
    /// if vote <strong>not exists</strong>, a new vote will be created with <see cref="Model.Vote.IsUpvote"/> will set to 
    /// <see langword="true"/>. Return <see cref="VoteUpdateTypes.CreateNew"/>.
    /// </item>
    /// 
    /// </list>
    /// 
    /// Either way, the <see cref="Model.Vote"/> will be updated or added in the database.
    /// </remarks>
    /// <param name="questionId"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<VoteUpdateTypes> UpvoteQuestion(Guid questionId, Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Downvote a question
    /// </summary>
    /// 
    /// <remarks>
    /// <list type="table">
    /// <item>
    /// If vote <strong>exists</strong> and <see cref="Model.Vote.IsUpvote"/> is <see langword="false"/>, 
    /// nothing will happen.
    /// Return <see cref="VoteUpdateTypes.NoChange"/>.
    /// </item>
    /// 
    /// <item>
    /// If vote <strong>exists</strong> but and <see cref="Model.Vote.IsUpvote"/> is <see langword="true"/>, 
    /// it will be changed to
    /// <see langword="false"/>. Return <see cref="VoteUpdateTypes.ChangeVote"/>.
    /// </item>
    /// 
    /// <item>
    /// if vote <strong>not exists</strong>, a new vote will be created with <see cref="Model.Vote.IsUpvote"/> 
    /// will set to 
    /// <see langword="false"/>. Return <see cref="VoteUpdateTypes.CreateNew"/>.
    /// </item>
    /// 
    /// </list>
    /// 
    /// Either way, the <see cref="Model.Vote"/> will be updated or added in the database.
    /// </remarks>
    /// <param name="questionId"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<VoteUpdateTypes> DownvoteQuestion(Guid questionId, Guid userId, CancellationToken cancellationToken);
    Task<VoteUpdateTypes> DownvoteAnswer(Guid answerId, Guid userId, CancellationToken cancellationToken);
    Task<VoteUpdateTypes> UpvoteAnswer(Guid answerId, Guid userId, CancellationToken cancellationToken);
}
