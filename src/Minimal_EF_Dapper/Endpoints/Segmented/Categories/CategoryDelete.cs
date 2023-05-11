using Microsoft.AspNetCore.Mvc;
using Minimal_EF_Dapper.Domain.Database;
using Swashbuckle.AspNetCore.Annotations;

namespace Minimal_EF_Dapper.Endpoints.Segmented.Categories
{
    public class CategoryDelete
    {
        public static string Template => "Category/{id:guid}";
        public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };
        public static Delegate Handle => Action;

        //-----------------------------------------------------------------------

        [SwaggerOperation(Tags = new[] { "Segmented Category" })]
        public static IActionResult Action([FromRoute] Guid id, ApplicationDbContext dbContext)
        {
            //Recupero o produto do banco
            var category = dbContext.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return new ObjectResult(Results.NotFound())
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            dbContext.Categories.Remove(category);
            dbContext.SaveChanges();

            return new ObjectResult(Results.Ok)
            {
                StatusCode = StatusCodes.Status200OK
            };
        }
    }
}