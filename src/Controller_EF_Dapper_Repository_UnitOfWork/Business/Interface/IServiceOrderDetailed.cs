using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities;
using Controller_EF_Dapper_Repository_UnityOfWork.Business.Models.Product;
using Microsoft.AspNetCore.Mvc;

namespace Controller_EF_Dapper_Repository_UnityOfWork.Business.Interface
{
    public interface IServiceOrderDetailed
    {
        Task<OrderDetailed> Get(Order order);

        Task<IEnumerable<OrderProduct>> GetOrderProducts(Guid id);

        Task<ObjectResult> SaveOrder(List<Guid> orderProductsId, OrderBuyer orderBuyer);
    }
}