using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_EF_Dapper.AppDomain.Extensions.ErroDetailedExtension;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Domain.Database.Entities.Product;
using Minimal_EF_Dapper.Endpoints.DTO.Category;

namespace Minimal_EF_Dapper.Endpoints.Unified.Direct
{
    public static class CategoryModule
    {
        public static void AddCategoriesEndPoints(this IEndpointRouteBuilder app)
        {
            //Get
            app.MapGet("unified/Category/{id:guid}", ([FromRoute] Guid id, ApplicationDbContext dbContext) =>
            {
                var Categorys = dbContext.Categories
                                         .AsNoTracking()
                                         .ToList();

                var categoryResponseDTO = Categorys.Select(p => new CategoryResponseDTO(
                                                            p.Id,
                                                            p.Name,
                                                            p.Active
                                                         ));

                return Results.Ok(categoryResponseDTO);
            }).WithTags("Unified Category");

            //GetAll
            app.MapGet("unified/Category", (ApplicationDbContext dbContext) =>
            {
                var categories = dbContext.Categories
                                          .AsNoTracking()
                                          .ToList();

                var categoriesResponseDTO = categories.Select(c => new CategoryResponseDTO
                (
                    c.Id,
                    c.Name,
                    c.Active
                ));

                return Results.Ok(categoriesResponseDTO);
            }).WithTags("Unified Category");

            //Post
            app.MapPost("unified/Category", async (CategoryRequestDTO categoryRequestDTO,
                                             HttpContext http,
                                             ApplicationDbContext dbContext) =>
            {
                //Usuario fixo, mas  poderia vir de um identity
                string user = "doe joe";

                var category = new Category();

                category.AddCategory(categoryRequestDTO.Name,
                                      user);

                if (!category.IsValid)
                {
                    return Results.ValidationProblem(category.Notifications.ConvertToErrorDetails());
                }

                await dbContext.Categories.AddAsync(category);
                dbContext.SaveChanges();

                return Results.Created($"/categories/{category.Id}", category.Id);
            }).WithTags("Unified Category");

            //Put
            app.MapPut("unified/Category/{id:guid}", ([FromRoute] Guid id,
                                                        CategoryRequestDTO categoryRequestDTO,
                                                        HttpContext http,
                                                        ApplicationDbContext dbContext) =>
            {
                //Usuario fixo, mas  poderia vir de um identity
                string user = "doe joe";

                var category = dbContext.Categories.FirstOrDefault(c => c.Id == id);

                if (category == null)
                {
                    return Results.NotFound();
                }

                category.EditInfo(categoryRequestDTO.Name,
                                  categoryRequestDTO.Active,
                                  user);

                if (!category.IsValid)
                {
                    return Results.ValidationProblem(category.Notifications
                                                             .ConvertToErrorDetails());
                }

                dbContext.SaveChanges();

                return Results.Ok();
            }).WithTags("Unified Category");

            //Delete
            app.MapDelete("unified/Category/{id:guid}", ([FromRoute] Guid id, ApplicationDbContext dbContext) =>
            {
                //Recupero o produto do banco
                var category = dbContext.Categories.FirstOrDefault(c => c.Id == id);

                if (category == null)
                {
                    return Results.NotFound();
                }

                dbContext.Categories.Remove(category);
                dbContext.SaveChanges();

                return Results.Ok();
            }).WithTags("Unified Category");
        }
    }
}