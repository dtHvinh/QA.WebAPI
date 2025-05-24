using Riok.Mapperly.Abstractions;
using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Response.AsnwerResponses;

namespace WebAPI.Utilities.Extensions;

[Mapper]
public static partial class AnswerExtensions
{
    public static partial AnswerResponse ToAnswerResponse(this Answer answer);
    public static partial Answer ToAnswer(this CreateAnswerDto dto, int authorId, int questionId);
}
