using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Extensions.ErroDetailedExtension;
using Controller_EF_Dapper_Repository_UnityOfWork.Business.Interface;
using Controller_EF_Dapper_Repository_UnityOfWork.Business.Models.Product;
using Controller_EF_Dapper_Repository_UnityOfWork.Domain.Database;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Controller_EF_Dapper_Repository_UnityOfWork.Business
{
    public class ServiceOrderDetailed : IServiceOrderDetailed
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public ServiceOrderDetailed()
        {
        }

        public ServiceOrderDetailed(IConfiguration configuration,
                                    ApplicationDbContext dbContext
                                    )
        {
            //Necessario para recuperar as configuracoes de conexao parao Dapper
            _configuration = configuration;

            _dbContext = dbContext;
        }

        public async Task<OrderDetailed> Get(Order order)
        {
            var products = await GetOrderProducts(order.Id);

            var orderDetailed = new OrderDetailed();

            orderDetailed.ClientId = order.ClientId;
            orderDetailed.ClientName = order.ClientName;
            orderDetailed.Products = products;

            return orderDetailed;
        }

        public async Task<IEnumerable<OrderProduct>> GetOrderProducts(Guid Id)
        {
            var db = new SqlConnection(_configuration["Database:SQlServer"]);

            var query = @$" SELECT A.ID,
                                  A.NAME
                             FROM PRODUCTS A
                       INNER JOIN ORDERPRODUCT B ON
                             B.ORDERSID = '{Id}'
                         AND A.ID = B.PRODUCTSID";
            //------------------------------------------------------------
            IEnumerable<OrderProduct> products = await db.QueryAsync<OrderProduct>(query);

            return products;
        }

        public async Task<ObjectResult> SaveOrder(List<Guid> orderProductsId, OrderBuyer orderBuyer)
        {
            List<Product> orderProducts = new List<Product>();

            //Recupero os produtos do banco para garantir consistencia dos dados
            if (orderProductsId.Any())
                orderProducts = _dbContext.Products.Where(p => orderProductsId.Contains(p.Id)).ToList();

            if (orderProducts == null)
                return new ObjectResult(Results.NotFound());

            //Total gasto
            decimal total = 0;
            foreach (var product in orderProducts)
            {
                total += product.Price;
            }

            var order = new Order();

            order.ClientId = orderBuyer.Id;
            order.ClientName = orderBuyer.Name;
            order.Products = orderProducts;
            order.Total = total;

            if (!order.IsValid)
            {
                return new ObjectResult(Results.ValidationProblem(order.Notifications.ConvertToErrorDetails()));
            }

            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            return new ObjectResult(Results.Created($"/orders/{order.Id}", order.Id));
        }
    }
}