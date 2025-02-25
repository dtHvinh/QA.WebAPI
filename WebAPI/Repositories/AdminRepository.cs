using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IAdminRepository))]
public class AdminRepository(ApplicationDbContext dbContext) : RepositoryBase<AppUser>(dbContext), IAdminRepository
{
}