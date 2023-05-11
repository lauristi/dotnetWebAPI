using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities;
using Controller_EF_Dapper_Repository_UnityOfWork.Domain.Database;
using Controller_EF_Dapper_Repository_UnityOfWork.Repository.Base;
using Controller_EF_Dapper_Repository_UnityOfWork.Repository.Repositories.Interfaces;

namespace Controller_EF_Dapper_Repository_UnityOfWork.Repository.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}