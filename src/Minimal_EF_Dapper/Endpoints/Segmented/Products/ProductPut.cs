using Microsoft.AspNetCore.Mvc;
using Minimal_EF_Dapper.AppDomain.Extensions.ErroDetailedExtension;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Endpoints.DTO.Product;
using Swashbuckle.AspNetCore.Annotations;

namespace Minimal_EF_Dapper.Endpoints.Segmented.Products
{
    public class ProductPut
    {
        public static string Template => "Product/{id:guid}";
        public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
        public static Delegate Handle => Action;

        //---------------------------------------------------------------------------------
        //Observacao: IActionResult está trabalhando com uma operacao sincrona

        [SwaggerOperation(Tags = new[] { "Segmented Product" })]
        public static IActionResult Action([FromRoute] Guid id,
                                                 ProductRequestDTO productRequestDTO,
                                                 HttpContext http,
                                                 ApplicationDbContext dbContext)
        {
            //Usuario fixo, mas  poderia vir de um identity
            string user = "doe joe";

            //Recupero o produto do banco
            var product = dbContext.Products.FirstOrDefault(c => c.Id == id);

            if (product == null)
            {
                return new ObjectResult(Results.NotFound())
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            //Recupero a categoria de forma sincrona
            var category = dbContext.Categories.FirstOrDefault(c => c.Id == productRequestDTO.CategoryId);

            if (category == null)
            {
                return new ObjectResult(Results.NotFound())
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            product.EditProduct(productRequestDTO.Name,
                                productRequestDTO.Price,
                                true,
                                category,
                                user
                                );

            if (!product.IsValid)
            {
                return new ObjectResult(Results.ValidationProblem(product.Notifications.ConvertToErrorDetails()))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            dbContext.SaveChanges();

            return new ObjectResult(Results.Ok())
            {
                StatusCode = StatusCodes.Status200OK
            };
        }
    }
}