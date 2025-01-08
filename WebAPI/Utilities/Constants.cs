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

    public class Endpoints
    {
        public const string Register = "/register";
        public const string Login = "/login";
    }
}
