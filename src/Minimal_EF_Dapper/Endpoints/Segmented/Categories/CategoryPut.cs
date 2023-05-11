using Microsoft.AspNetCore.Mvc;
using Minimal_EF_Dapper.AppDomain.Extensions.ErroDetailedExtension;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Endpoints.DTO.Category;
using Swashbuckle.AspNetCore.Annotations;

namespace Minimal_EF_Dapper.Endpoints.Segmented.Categories
{
    public class CategoryPut
    {
        public static string Template => "Category/{id:guid}";
        public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
        public static Delegate Handle => Action;

        //-----------------------------------------------------------------------
        //Observacao: IActionResult está trabalhando com uma operacao sincrona

        [SwaggerOperation(Tags = new[] { "Segmented Category" })]
        public static IActionResult Action([FromRoute] Guid id,
                                                 CategoryRequestDTO categoryRequestDTO,
                                                 HttpContext http,
                                                 ApplicationDbContext dbContext)
        {
            //Usuario fixo, mas  poderia vir de um identity
            string user = "doe joe";

            var category = dbContext.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return new ObjectResult(Results.NotFound())
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            category.EditInfo(categoryRequestDTO.Name,
                              categoryRequestDTO.Active,
                              user);

            if (!category.IsValid)
            {
                return new ObjectResult(Results.ValidationProblem(category.Notifications.ConvertToErrorDetails()))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            dbContext.SaveChanges();

            return new ObjectResult(Results.Ok)
            {
                StatusCode = StatusCodes.Status200OK
            };
        }
    }
}