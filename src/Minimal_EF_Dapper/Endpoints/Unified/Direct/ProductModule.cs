using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_EF_Dapper.AppDomain.Extensions.ErroDetailedExtension;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Domain.Database.Entities.Product;
using Minimal_EF_Dapper.Endpoints.DTO.Product;

namespace Minimal_EF_Dapper.Endpoints.Unified.Direct
{
    public static class ProductModule
    {
        public static void AddProductsEndPoints(this IEndpointRouteBuilder app)
        {
            //Get
            app.MapGet("unified/Product/{id:guid}", ([FromRoute] Guid id, ApplicationDbContext dbContext) =>
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

                return Results.Ok(productResponseDTO);
            }).WithTags("Unified Product");

            //GetAll
            app.MapGet("unified/Product", (ApplicationDbContext dbContext) =>
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

                return Results.Ok(productsResponseDTO);
            }).WithTags("Unified Product");

            //Post
            app.MapPost("unified/Product", async (ProductRequestDTO productRequestDTO,
                                            HttpContext http,
                                            ApplicationDbContext dbContext) =>
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
                    return Results.ValidationProblem(product.Notifications.ConvertToErrorDetails());
                }

                await dbContext.Products.AddAsync(product);
                dbContext.SaveChanges();

                return Results.Created($"/products/{product.Id}", product.Id);
            }).WithTags("Unified Product");

            //Put
            app.MapPut("unified/Product/{id:guid}", ([FromRoute] Guid id,
                                                     ProductRequestDTO productRequestDTO,
                                                     HttpContext http,
                                                     ApplicationDbContext dbContext) =>
            {
                //Usuario fixo, mas  poderia vir de um identity
                string user = "doe joe";

                //Recupero o produto do banco
                var product = dbContext.Products.FirstOrDefault(c => c.Id == id);

                if (product == null)
                {
                    return Results.NotFound();
                }

                //Recupero a categoria de forma sincrona
                var category = dbContext.Categories.FirstOrDefault(c => c.Id == productRequestDTO.CategoryId);

                if (category == null)
                {
                    return Results.NotFound();
                }

                product.EditProduct(productRequestDTO.Name,
                                    productRequestDTO.Price,
                                    true,
                                    category,
                                    user
                                    );

                if (!product.IsValid)
                {
                    return Results.ValidationProblem(product.Notifications.ConvertToErrorDetails());
                }

                dbContext.SaveChanges();

                return Results.Ok();
            }).WithTags("Unified Product");

            //Delete
            app.MapDelete("unified/Product/{id:guid}", ([FromRoute] Guid id, ApplicationDbContext dbContext) =>
            {
                //Recupero o produto do banco
                var product = dbContext.Products.FirstOrDefault(c => c.Id == id);

                if (product == null)
                {
                    return Results.NotFound();
                }

                dbContext.Products.Remove(product);
                dbContext.SaveChanges();

                return Results.Ok();
            }).WithTags("Unified Product");

            ////GetAllSold (Caso especial classe estatica não aceita injecao de dependencia de interface)
            //app.MapGet("unified/Product/sold", async (ServiceAllProductsSold serviceAllProductsSold) =>
            //{
            //    var result = await serviceAllProductsSold.Execute();
            //    return Results.Ok(result);
            //}).WithTags("Unified Product");
        }
    }
}