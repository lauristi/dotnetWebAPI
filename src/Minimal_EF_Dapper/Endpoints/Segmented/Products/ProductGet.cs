using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Endpoints.DTO.Product;
using Swashbuckle.AspNetCore.Annotations;

namespace Minimal_EF_Dapper.Endpoints.Segmented.Products
{
    public class ProductGet
    {
        public static string Template => "Product/{id:guid}";

        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        //-----------------------------------------------------------------------
        //Observacao: IActionResult está trabalhando com uma operacao sincrona

        [SwaggerOperation(Tags = new[] { "Segmented Product" })]
        public static IActionResult Action([FromRoute] Guid id, ApplicationDbContext dbContext)
        {
            var products = dbContext.Products
                                   .Include(p => p.Category)
                                   .Where(p => p.Id == id)
                                   .OrderBy(p => p.Name)
                                   .ToList();

            var productResponseDTO = products.Select(p => new ProductResponseDTO(
                                                        p.Id,
                                                        p.Name,
                                                        p.Description,
                                                        p.Price,
                                                        p.Active
                                                     ));

            return new ObjectResult(productResponseDTO)
            {
                StatusCode = StatusCodes.Status200OK
            };
        }
    }
}