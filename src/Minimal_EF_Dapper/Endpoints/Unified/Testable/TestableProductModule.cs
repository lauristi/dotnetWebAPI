using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_EF_Dapper.AppDomain.Extensions.ErroDetailedExtension;
using Minimal_EF_Dapper.Business.Interface;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Domain.Database.Entities.Product;
using Minimal_EF_Dapper.Endpoints.DTO.Product;

namespace Minimal_EF_Dapper.Endpoints.Unified.Direct
{
    public static class TestableProductModule
    {
        //==============================================================================================
        // Configuracao dos endpoints
        //==============================================================================================

        public static void AddTestableProductsEndPoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("unified/testable/Product/{id:guid}", FromModuleProductGet).WithTags("Unified Product for test");
            app.MapGet("unified/testableProduct", FromModuleProductGetAll).WithTags("Unified Product for test");

            app.MapPost("unified/testableProduct", FromModuleProductPost).WithTags("Unified Product for test");
            app.MapPut("unified/testableProduct/{id:guid}", FromModuleProductPut).WithTags("Unified Product for test");

            app.MapDelete("unified/testableProduct/{id:guid}", FromModuleProductDelete).WithTags("Unified Product for test");

            ////GetAllSold (Caso especial classe estatica não aceita injecao de dependencia de interface)
            //app.MapGet("unified/Product/sold", async (ServiceAllProductsSold serviceAllProductsSold) =>
            //{
            //    var result = await serviceAllProductsSold.Execute();
            //    return Results.Ok(result);
            //}).WithTags("Unified Product");
        }

        //==============================================================================================
        // Endpoints
        //==============================================================================================

        public static IActionResult FromModuleProductGet(Guid id, ApplicationDbContext dbContext)
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

        public static async Task<IActionResult> FromModuleProductPost(ProductRequestDTO productRequestDTO,
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

        public static IActionResult FromModuleProductGetAll(ApplicationDbContext dbContext)
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

        public static async Task<IActionResult> FromModuleProductGetSold(IServiceAllProductsSold serviceAllProductsSold)
        {
            var result2 = serviceAllProductsSold.Execute();

            var result = await serviceAllProductsSold.Execute();

            return new ObjectResult(Results.Ok)
            {
                StatusCode = StatusCodes.Status200OK
            };
        }

        public static IActionResult FromModuleProductPut(Guid id,
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

        public static IActionResult FromModuleProductDelete(Guid id, ApplicationDbContext dbContext)
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

        //        private readonly IServiceAllProductsSold _serviceAllProductsSold;
        //
        //        public ProductSoldGet(IServiceAllProductsSold serviceAllProductsSold)
        //        {
        //            _serviceAllProductsSold = serviceAllProductsSold;
        //        }
    }
}