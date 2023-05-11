using Microsoft.AspNetCore.Mvc;
using Minimal_EF_Dapper.Domain.Database;
using Swashbuckle.AspNetCore.Annotations;

namespace Minimal_EF_Dapper.Endpoints.Segmented.Products
{
    public class ProductDelete
    {
        public static string Template => "Product/{id:guid}";
        public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };
        public static Delegate Handle => Action;

        //-----------------------------------------------------------------------
        //Observacao: IActionResult está trabalhando com uma operacao sincrona

        [SwaggerOperation(Tags = new[] { "Segmented Product" })]
        public static IActionResult Action([FromRoute] Guid id, ApplicationDbContext dbContext)
        {
            //Recupero o produto do banco
            var product = dbContext.Products.FirstOrDefault(c => c.Id == id);

            if (product == null)
            {
                return new ObjectResult(Results.NotFound())
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            dbContext.Products.Remove(product);
            dbContext.SaveChanges();

            return new ObjectResult(Results.Ok())
            {
                StatusCode = StatusCodes.Status200OK
            };
        }
    }
}