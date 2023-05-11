using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities;
using static Controller_EF_Dapper_Repository_UnityOfWork.Repository.Base.IGenericRepository;

namespace Controller_EF_Dapper_Repository_UnityOfWork.Repository.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
    }
}