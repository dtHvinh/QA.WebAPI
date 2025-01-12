using Microsoft.Extensions.Options;
using WebAPI.Filters.Requirement.Base;
using WebAPI.Repositories;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Options;

namespace WebAPI.Filters.Requirement;

public class CreateTagReputationRequirement(UserRepository userRepository,
                                    AuthenticationContext authContext,
                                    IOptions<ApplicationProperties> options)
    : ReputationRequirement(userRepository,
                            authContext,
                            options.Value.ActionRequirements.CreateTagReputationRequirement)
{
}
