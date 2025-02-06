using WebAPI.Model;
using WebAPI.Response.BookmarkResponses;

namespace WebAPI.Utilities.Mappers;

public static class BookmarkMap
{
    public static BookmarkResponse ToBookmarkResponse(this BookMark bookMark)
    {
        var qResponse = bookMark.Question!.ToGetQuestionResponse();

        var res = new BookmarkResponse(bookMark.Id, bookMark.CreatedAt, qResponse);

        return res;
    }
}
