using Microsoft.Extensions.Options;
using WebAPI.Filters.Requirement.Base;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Options;

namespace WebAPI.Filters.Requirement.RepReq;

public class ForEditTag(
    IUserRepository userRepository,
    AuthenticationContext authContext,
    ICacheService cache,
    IOptions<ApplicationProperties> options)
    : ReputationRequirementFilter(userRepository, authContext, cache, options.Value.ActionRepRequirement.EditTagWiki)
{
}