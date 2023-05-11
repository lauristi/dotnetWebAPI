using Microsoft.AspNetCore.Mvc;
using Minimal_EF_Dapper.AppDomain.Extensions.ErroDetailedExtension;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Domain.Database.Entities.Product;
using Minimal_EF_Dapper.Endpoints.DTO.Order;

namespace Minimal_EF_Dapper.Endpoints.Unified.Direct
{
    public static class OrderModule
    {
        public static void AddOrdersEndPoints(this IEndpointRouteBuilder app)
        {
            //Get
            app.MapGet("unified/Order/{id:guid}", ([FromRoute] Guid id,
                                                    HttpContext http,
                                                    ApplicationDbContext context
                                                  ) =>
            {
                //Usuario fixo, mas  poderia vir de um identity
                string userName = "doe joe";

                var order = context.Orders.FirstOrDefault(order => order.Id == id);

                var productsResponseDTO = order.Products.Select(p => new OrderProductDTO(p.Id,
                                                                                         p.Name));

                var orderResponseDTO = new OrderResponseDTO(order.Id,
                                                            userName,
                                                            productsResponseDTO
                                                            );
                return Results.Ok(orderResponseDTO);
            }).WithTags("Unified Order");

            //Post
            app.MapPost("unified/Order", async (OrderRequestDTO orderRequestDTO,
                                                   HttpContext http,
                                                   ApplicationDbContext dbContext) =>
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
                    return Results.NotFound();
                }

                var order = new Order();

                order.AddOrder(userId, userName, orderProducts);

                if (!order.IsValid)
                {
                    return Results.ValidationProblem(order.Notifications.ConvertToErrorDetails());
                }

                await dbContext.Orders.AddAsync(order);
                await dbContext.SaveChangesAsync();

                return Results.Created($"/orders/{order.Id}", order.Id);
            }).WithTags("Unified Order");
        }
    }
}