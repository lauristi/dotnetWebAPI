using Controller_EF_Dapper_Repository_UnityOfWork.Business.Models.Product;

namespace Controller_EF_Dapper_Repository_UnityOfWork.Business.Interface
{
    public interface IServiceProductSold
    {
        Task<IEnumerable<ProductSold>> GetAll();
    }
}