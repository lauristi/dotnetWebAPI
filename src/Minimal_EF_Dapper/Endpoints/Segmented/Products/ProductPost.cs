using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_EF_Dapper.AppDomain.Extensions.ErroDetailedExtension;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Domain.Database.Entities.Product;
using Minimal_EF_Dapper.Endpoints.DTO.Product;
using Swashbuckle.AspNetCore.Annotations;

namespace Minimal_EF_Dapper.Endpoints.Segmented.Products
{
    public class ProductPost
    {
        public static string Template => "Product";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        //----------------------------------------------------------------------
        //Observacao: Task<IActionResult> Está trabalhando com uma operacao assincrona

        [SwaggerOperation(Tags = new[] { "Segmented Product" })]
        public static async Task<IActionResult> Action(ProductRequestDTO productRequestDTO,
                                                 HttpContext http,
                                                 ApplicationDbContext dbContext)
        {
            //Usuario fixo, mas  poderia vir de um identity
            string user = "doe joe";

            //Recupero a categoria de forma sincrona
            var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == productRequestDTO.CategoryId);

            var product = new Product();

            product.AddProduct(productRequestDTO.Name,
                                productRequestDTO.Description,
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

            await dbContext.Products.AddAsync(product);
            dbContext.SaveChanges();

            return new ObjectResult(Results.Created($"/products/{product.Id}", product.Id))
            {
                StatusCode = StatusCodes.Status201Created
            };
        }
    }
}