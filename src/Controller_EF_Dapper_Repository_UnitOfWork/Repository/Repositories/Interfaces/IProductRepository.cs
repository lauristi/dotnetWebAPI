using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities;
using Controller_EF_Dapper_Repository_UnityOfWork.Business.Models.Product;
using static Controller_EF_Dapper_Repository_UnityOfWork.Repository.Base.IGenericRepository;

namespace Controller_EF_Dapper_Repository_UnityOfWork.Repository.Repositories.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        //Metodos personalizados
        Task<IEnumerable<ProductSold>> GetSoldProducts();
    }
}