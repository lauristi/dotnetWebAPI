using Controller_EF_Dapper.Business.Models;

namespace Controller_EF_Dapper.Business.Interface
{
    public interface IServiceAllProductSold
    {
        Task<IEnumerable<ProductSold>> Execute();
    }
}