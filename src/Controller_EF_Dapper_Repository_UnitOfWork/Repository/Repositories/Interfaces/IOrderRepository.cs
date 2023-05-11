using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities;
using Controller_EF_Dapper_Repository_UnityOfWork.Business.Models.Product;
using Microsoft.AspNetCore.Mvc;
using static Controller_EF_Dapper_Repository_UnityOfWork.Repository.Base.IGenericRepository;

namespace Controller_EF_Dapper_Repository_UnityOfWork.Repository.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        //Metodos personalizados
        Task<OrderDetailed> GetDetailedOrder(Order order);

        Task<ObjectResult> SaveOrder(List<Guid> orderProductsId, OrderBuyer orderBuyer);
    }
}