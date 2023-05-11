using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Endpoints.DTO.Category;
using Swashbuckle.AspNetCore.Annotations;

namespace Minimal_EF_Dapper.Endpoints.Segmented.Categories
{
    public class CategoryGet
    {
        public static string Template => "Category/{id:guid}";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        //-----------------------------------------------------------------------
        //Observacao: IActionResult está trabalhando com uma operacao sincrona

        [SwaggerOperation(Tags = new[] { "Segmented Category" })]
        public static IActionResult Action([FromRoute] Guid id, ApplicationDbContext dbContext)
        {
            var Categorys = dbContext.Categories
                                     .AsNoTracking()
                                     .ToList();

            var categoryResponseDTO = Categorys.Select(p => new CategoryResponseDTO(
                                                        p.Id,
                                                        p.Name,
                                                        p.Active
                                                     ));

            return new ObjectResult(categoryResponseDTO)
            {
                StatusCode = StatusCodes.Status200OK
            };
        }
    }
}