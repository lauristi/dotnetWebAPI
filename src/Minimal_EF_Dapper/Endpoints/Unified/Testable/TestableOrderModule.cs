using Microsoft.AspNetCore.Mvc;
using Minimal_EF_Dapper.AppDomain.Extensions.ErroDetailedExtension;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Domain.Database.Entities.Product;
using Minimal_EF_Dapper.Endpoints.DTO.Order;

namespace Minimal_EF_Dapper.Endpoints.Unified.Direct
{
    public static class TestableOrderModule
    {
        //==============================================================================================
        // Configuracao dos endpoints
        //==============================================================================================

        public static void AddTestableOrdersEndPoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("unified/testable/Order/{id:guid}", FromModuleOrderGet).WithTags("Unified Order for test");
            app.MapPost("unified/testableOrder", FromModuleOrderPost).WithTags("Unified Order for test");
        }

        //==============================================================================================
        // Endpoints
        //==============================================================================================

        public static IActionResult FromModuleOrderGet(Guid id, HttpContext http, ApplicationDbContext context)
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
            return new ObjectResult(orderResponseDTO)
            {
                StatusCode = StatusCodes.Status200OK
            };
        }

        public static async Task<IActionResult> FromModuleOrderPost(OrderRequestDTO orderRequestDTO,
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
            {
                return new ObjectResult(Results.ValidationProblem(order.Notifications.ConvertToErrorDetails()))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();

            return new ObjectResult(Results.Created($"/orders/{order.Id}", order.Id))
            {
                StatusCode = StatusCodes.Status201Created
            };
        }
    }
}