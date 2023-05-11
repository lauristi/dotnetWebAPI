using Microsoft.AspNetCore.Mvc;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Endpoints.DTO.Order;
using Swashbuckle.AspNetCore.Annotations;

namespace Minimal_EF_Dapper.Endpoints.Segmented.Orders
{
    public class OrderGet
    {
        public static string Template => "Order/{id}";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        //----------------------------------------------------------------------
        //Observacao: Task<IActionResult> Está trabalhando com uma operacao assincrona

        [SwaggerOperation(Tags = new[] { "Segmented Order" })]
        public static async Task<IActionResult> Action(Guid id,
                                                 HttpContext http,
                                                 ApplicationDbContext context
                                                )
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
    }
}