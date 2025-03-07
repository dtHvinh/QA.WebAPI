namespace WebAPI.Utilities;

public static class Constants
{
    public const int StringContentMaxChar = 20000;

    public static class CronExpressions
    {
        public const string Minutely = "* * * * *";
        public const string Hourly = "0 * * * *";
        public const string Daily = "0 0 * * *";
        public const string Weekly = "0 0 * * 0";
        public const string Monthly = "0 0 1 * *";
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Moderator = "Moderator";
    }

    public static class EG
    {
        private const string _prefix = "/api";
        public const string Auth = _prefix + "/auth";
        public const string Admin = _prefix + "/admin";
        public const string Collection = _prefix + "/collection";
        public const string AI = _prefix + "/ai";
        public const string Question = _prefix + "/question";
        public const string Tag = _prefix + "/tag";
        public const string Report = _prefix + "/report";
        public const string Answer = _prefix + "/answer";
        public const string User = _prefix + "/user";
        public const string Comment = _prefix + "/comment";
        public const string Bookmark = _prefix + "/bookmark";
    }

    public static class Endpoints
    {
        public const string Register = "/register";
        public const string Login = "/login";
    }

    public static class EM
    {
        public const string ES_INDEX_OR_UPDATE_DOCUMENT_FAILED = "Something wrong with ES";

        public const string REP_NOT_MEET_REQ = "You need at least {0} reputation to do this";

        public const string USER_EMAIL_NOTFOUND = "User with email {0} not found";
        public const string USERNAME_NOTFOUND = "User with username {0} not found";
        public const string BOOKMARK_NOT_FOUND = "Bookmark not found";
        public const string USER_ID_NOTFOUND = "User with id {0} not found";
        public const string ANSWER_ID_NOTFOUND = "Answer with id {0} not found";
        public const string ENDPOINT_NOARG = "No argument found at this endpoint";
        public const string VALIDATION_REQ_FIRSTARG = "First argument must be a request object";

        public const string QUESTION_ID_NOTFOUND = "Question with id {0} not found";
        public const string QUESTION_DELETE_UNAUTHORIZED = "Do not has ownership to delete this question";
        public const string QUESTION_CLOSED_COMMENT_RESTRICT = "You can not comment to closed question";

        public const string PasswordWrong = "Password is wrong";
        public const string TAG_NOTFOUND = "Tag not found";

        public const string ROLE_NOT_MEET_REQ = "You need at least {0} role to do this";

        public const string ACTION_REQUIRE_RESOURCE_OWNER = "You are not allow to do this";
    }

    public static class RedisKeyGen
    {
        public static string UserEmail(string email) => $"email:{email}";
        public static string AppUserKey(int id) => $"appuser:#{id}";
        public static string Question(int id) => $"question:#{id}";
        public static string GetTagDetail(int id, string orderBy, int questionPage, int questionPageSize)
            => $"tag-detail:#{id}:&orderBy={orderBy}&qSkip={questionPage}&qTake={questionPageSize}";
        public static string GetTags(string orderBy, int skip, int take)
            => $"tags:orderBy={orderBy}&skip={skip}&take={take}";
    }
}
