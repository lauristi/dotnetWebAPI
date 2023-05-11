using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Endpoints.DTO.Product;
using Swashbuckle.AspNetCore.Annotations;

namespace Minimal_EF_Dapper.Endpoints.Segmented.Products
{
    public class ProductGetAll
    {
        public static string Template => "Product";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => PAction;

        //---------------------------------------------------------------------
        //Observacao: IActionResult está trabalhando com uma operacao sincrona

        [SwaggerOperation(Tags = new[] { "Segmented Product" })]
        public static IActionResult PAction(ApplicationDbContext dbContext)
        {
            var products = dbContext.Products
                                  .AsNoTracking()
                                  .Include(p => p.Category)
                                  .OrderBy(p => p.Name)
                                  .ToList();

            var productsResponseDTO = products.Select(p => new ProductResponseDTO(
                                                        p.Id,
                                                        p.Name,
                                                        p.Description,
                                                        p.Price,
                                                        p.Active
                                                     ));

            return new ObjectResult(productsResponseDTO)
            {
                StatusCode = StatusCodes.Status200OK
            };
        }
    }
}