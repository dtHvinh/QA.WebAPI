
using WebAPI.Filters.Requirement.Base;
using WebAPI.Utilities.Context;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Filters.Requirement.RoleReq;

public class RequireModerator(AuthenticationContext authenticationContext) : RoleReqFilter(Roles.Moderator, authenticationContext);
