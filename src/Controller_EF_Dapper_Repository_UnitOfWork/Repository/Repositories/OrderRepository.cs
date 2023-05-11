using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities;
using Controller_EF_Dapper_Repository_UnityOfWork.Business.Interface;
using Controller_EF_Dapper_Repository_UnityOfWork.Business.Models.Product;
using Controller_EF_Dapper_Repository_UnityOfWork.Domain.Database;
using Controller_EF_Dapper_Repository_UnityOfWork.Repository.Base;
using Controller_EF_Dapper_Repository_UnityOfWork.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Controller_EF_Dapper_Repository_UnityOfWork.Repository.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly IServiceOrderDetailed _serviceOrderDetailed;

        public OrderRepository(ApplicationDbContext dbContext,
                                 IServiceOrderDetailed serviceOrderDetailed) : base(dbContext)
        {
            _serviceOrderDetailed = serviceOrderDetailed;
        }

        public Task<OrderDetailed> GetDetailedOrder(Order order)
        {
            return _serviceOrderDetailed.Get(order);
        }

        public Task<ObjectResult> SaveOrder(List<Guid> orderProductsId, OrderBuyer orderBuyer)
        {
            return _serviceOrderDetailed.SaveOrder(orderProductsId, orderBuyer);
        }
    }
}