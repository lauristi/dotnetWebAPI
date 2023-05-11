using Microsoft.AspNetCore.Mvc;
using Minimal_EF_Dapper.AppDomain.Extensions.ErroDetailedExtension;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Domain.Database.Entities.Product;
using Minimal_EF_Dapper.Endpoints.DTO.Order;
using Swashbuckle.AspNetCore.Annotations;

namespace Minimal_EF_Dapper.Endpoints.Segmented.Orders
{
    public class OrderPost
    {
        public static string Template => "Order";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        //----------------------------------------------------------------------
        //Observacao: Task<IActionResult> Está trabalhando com uma operacao assincrona

        [SwaggerOperation(Tags = new[] { "Segmented Order" })]
        public static async Task<IActionResult> Action(OrderRequestDTO orderRequestDTO,
                                                 HttpContext http,
                                                 ApplicationDbContext dbContext)
        {
            //Usuario fixo, mas  poderia vir de um identity
            var userId = "123456";
            var userName = "Doe Joe Client";

            var products = new List<Product>();

            List<Product> orderProducts = new List<Product>();

            if (orderRequestDTO.ProductListIds.Any())

                orderProducts = dbContext.Products.Where(p => orderRequestDTO.ProductListIds
                                                                           .Contains(p.Id))
                                                                           .ToList();
            if (orderProducts == null || orderProducts.Count == 0)
            {
                return new ObjectResult(Results.NotFound())
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            var order = new Order();

            order.AddOrder(userId, userName, orderProducts);

            if (!order.IsValid)
                return new ObjectResult(Results.ValidationProblem(order.Notifications.ConvertToErrorDetails()))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };

            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();

            return new ObjectResult(Results.Created($"/orders/{order.Id}", order.Id))
            {
                StatusCode = StatusCodes.Status201Created
            };
        }
    }
}