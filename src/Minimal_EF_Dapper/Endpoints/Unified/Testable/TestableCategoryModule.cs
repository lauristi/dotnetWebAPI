using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_EF_Dapper.AppDomain.Extensions.ErroDetailedExtension;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Domain.Database.Entities.Product;
using Minimal_EF_Dapper.Endpoints.DTO.Category;

namespace Minimal_EF_Dapper.Endpoints.Unified.Direct
{
    public static class TestableCategoryModule
    {
        //==============================================================================================
        // Configuracao dos endpoints
        //==============================================================================================

        public static void AddTestableCategoriesEndPoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("unified/testable/Category/{id:guid}", FromModuleCategoryGet).WithTags("Unified Category for test");
            app.MapGet("unified/testableCategory", FromModuleCategoryGetAll).WithTags("Unified Category for test");

            app.MapPost("unified/testableCategory", FromModuleCategoryPost).WithTags("Unified Category for test");
            app.MapPut("unified/testableCategory/{id:guid}", FromModuleCategoryPut).WithTags("Unified Category for test");

            app.MapDelete("unified/testableCategory/{id:guid}", FromModuleCategoryDelete).WithTags("Unified Category for test");
        }

        //==============================================================================================
        // Endpoints
        //==============================================================================================

        public static IActionResult FromModuleCategoryGet(Guid id, ApplicationDbContext dbContext)
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

        public static IActionResult FromModuleCategoryGetAll(ApplicationDbContext dbContext)
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

            return new ObjectResult(categoriesResponseDTO)
            {
                StatusCode = StatusCodes.Status200OK
            };
        }

        public static async Task<IActionResult> FromModuleCategoryPost(CategoryRequestDTO categoryRequestDTO,
                                                 HttpContext http,
                                                 ApplicationDbContext dbContext)
        {
            //Usuario fixo, mas  poderia vir de um identity
            string user = "doe joe";

            var category = new Category();

            category.AddCategory(categoryRequestDTO.Name, user);

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

        public static IActionResult FromModuleCategoryPut(Guid id,
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

            return new ObjectResult(Results.Ok())
            {
                StatusCode = StatusCodes.Status200OK
            };
        }

        public static IActionResult FromModuleCategoryDelete(Guid id, ApplicationDbContext dbContext)
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

            return new ObjectResult(Results.Ok())
            {
                StatusCode = StatusCodes.Status200OK
            };
        }
    }
}