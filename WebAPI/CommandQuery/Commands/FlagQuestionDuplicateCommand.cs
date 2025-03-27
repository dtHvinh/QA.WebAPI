using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

/// <summary>
/// Command to flag a question as a duplicate of another question.
/// </summary>
/// <param name="QuestionId">The question being flaged as duplicated.</param>
/// <param name="DuplicateQuestionUrl">The original question.</param>
public record FlagQuestionDuplicateCommand(int QuestionId, string DuplicateQuestionUrl) : ICommand<GenericResult<TextResponse>>;
