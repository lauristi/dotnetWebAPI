using Microsoft.AspNetCore.Mvc;
using Minimal_EF_Dapper.AppDomain.Extensions.ErroDetailedExtension;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Domain.Database.Entities.Product;
using Minimal_EF_Dapper.Endpoints.DTO.Category;
using Swashbuckle.AspNetCore.Annotations;

namespace Minimal_EF_Dapper.Endpoints.Segmented.Categories
{
    public class CategoryPost
    {
        public static string Template => "Category";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        //----------------------------------------------------------------------
        //Observacao: Task<IActionResult> Está trabalhando com uma operacao assincrona

        [SwaggerOperation(Tags = new[] { "Segmented Category" })]
        public static async Task<IActionResult> Action(CategoryRequestDTO categoryRequestDTO,
                                                 HttpContext http,
                                                 ApplicationDbContext dbContext)
        {
            //Usuario fixo, mas  poderia vir de um identity
            string user = "doe joe";

            var category = new Category();

            category.AddCategory(categoryRequestDTO.Name,
                                  user);

            if (!category.IsValid)
            {
                return new ObjectResult(Results.ValidationProblem(category.Notifications.ConvertToErrorDetails()))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            await dbContext.Categories.AddAsync(category);
            dbContext.SaveChanges();

            return new ObjectResult(Results.Created($"/categories/{category.Id}", category.Id))
            {
                StatusCode = StatusCodes.Status201Created
            };
        }
    }
}