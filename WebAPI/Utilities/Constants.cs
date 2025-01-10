namespace WebAPI.Utilities;

public static class Constants
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }

    public static class EndpointGroup
    {
        private const string _prefix = "/api";
        public const string Auth = _prefix + "/auth";
    }

    public static class Endpoints
    {
        public const string Register = "/register";
        public const string Login = "/login";
    }

    public static class EM
    {
        public const string EMAIL_NOTFOUND = "User with email {0} not found";
        public const string ENDPOINT_NOARG = "No argument found at this endpoint";
        public const string VALIDATION_REQ_FIRSTARG = "First argument must be a request object";

        public const string PasswordWrong = "Password is wrong";
    }

    public static class RedisKeyGen
    {
        public static string ForEmailDuplicate(string email) => $"email:{email}";
    }
}
