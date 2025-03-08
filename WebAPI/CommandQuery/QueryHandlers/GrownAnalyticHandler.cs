using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Repositories.AdminRepository;
using static WebAPI.Utilities.Enums;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GrownAnalyticHandler(IAdminRepository adminRepository) : IQueryHandler<AdminAnalyticQuery, GenericResult<GrownAnalyticResponse>>
{
    private readonly IAdminRepository _adminRepository = adminRepository;

    public async Task<GenericResult<GrownAnalyticResponse>> Handle(AdminAnalyticQuery request, CancellationToken cancellationToken)
    {
        GrownAnalytic grownAnalytic;
        var analyticPeriod = Enum.Parse<AnalyticPeriod>(request.Period, true);

        if (string.Equals(request.What, "question", StringComparison.InvariantCultureIgnoreCase))
        {
            grownAnalytic = await _adminRepository.GetGrownAnalytic<Question>(analyticPeriod);
        }
        else if (string.Equals(request.What, "answer", StringComparison.InvariantCultureIgnoreCase))
        {
            grownAnalytic = await _adminRepository.GetGrownAnalytic<Answer>(analyticPeriod);
        }
        else if (string.Equals(request.What, "user", StringComparison.InvariantCultureIgnoreCase))
        {
            grownAnalytic = await _adminRepository.GetGrownAnalytic<AppUser>(analyticPeriod);
        }
        else
        {
            return GenericResult<GrownAnalyticResponse>.Failure("Invalid analytic type");
        }

        return GenericResult<GrownAnalyticResponse>.Success(new GrownAnalyticResponse(grownAnalytic.PerviousCount, grownAnalytic.CurrentCount, grownAnalytic.PercentageDifferent));
    }
}
