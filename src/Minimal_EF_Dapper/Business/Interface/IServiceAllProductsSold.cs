using Minimal_EF_Dapper.Business.Models;

namespace Minimal_EF_Dapper.Business.Interface
{
    public interface IServiceAllProductsSold
    {
        Task<IEnumerable<ProductSold>> Execute();
    }
}