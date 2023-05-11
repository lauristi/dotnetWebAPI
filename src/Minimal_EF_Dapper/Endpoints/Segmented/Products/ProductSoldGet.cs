using Microsoft.AspNetCore.Mvc;
using Minimal_EF_Dapper.Business.Interface;
using Swashbuckle.AspNetCore.Annotations;

namespace Minimal_EF_Dapper.Endpoints.Segmented.Products
{
    public class ProductSoldGet
    {
        public static string Template => "Product/sold";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        private readonly IServiceAllProductsSold _serviceAllProductsSold;

        //GetAllSold (Caso especial aceita injecao de dependencia de interface pois a classe é publica)
        public ProductSoldGet(IServiceAllProductsSold serviceAllProductsSold)
        {
            _serviceAllProductsSold = serviceAllProductsSold;
        }

        //----------------------------------------------------------------------
        //Observacao: Task<IActionResult> Está trabalhando com uma operacao assincrona

        [SwaggerOperation(Tags = new[] { "Segmented Product" })]
        public static async Task<IActionResult> Action([FromServices] IServiceAllProductsSold serviceAllProductsSold)
        {
            var result2 = serviceAllProductsSold.Execute();

            var result = await serviceAllProductsSold.Execute();

            return new ObjectResult(Results.Ok)
            {
                StatusCode = StatusCodes.Status200OK
            };
        }
    }
}