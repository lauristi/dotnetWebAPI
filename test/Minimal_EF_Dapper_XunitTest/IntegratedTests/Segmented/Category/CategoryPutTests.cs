using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Domain.Database.Entities.Product;
using Minimal_EF_Dapper.Endpoints.DTO.Category;
using Minimal_EF_Dapper.Endpoints.Segmented.Categories;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace Minimal_EF_Dapper_XunitTest
{
    public class CategoryPutTests
    {
        private readonly ApplicationDbContext _dbContextMock;
        private readonly HttpContext _httpContextMock;

        public CategoryPutTests()
        {
            // Configura o mock dos contextos
            _dbContextMock = Substitute.For<ApplicationDbContext>();
            _httpContextMock = Substitute.For<HttpContext>();
        }

        [Fact]
        public void CategoryPut_AlteredWithSucess()
        {
            // Arrange

            //Dados
            var dummie_CategoryId = Guid.NewGuid();
            var dummie_user = "Doe Joe";

            var mockCategoryRequestDTO = new CategoryRequestDTO
            {
                Name = "Category Dumie",
                Active = true,
            };

            var mockCategory = new Category
            {
                Id = dummie_CategoryId,
                Name = mockCategoryRequestDTO.Name,
                Active = true,
                EditedBy = dummie_user,
                EditedOn = DateTime.Now
            };

            // Configurando as tabelas vituais

            //1 - Crio uma lista com os dados mockados
            var mockCategories = new List<Category> { mockCategory };

            //2- Transformo a lista em um tipo queryable
            var mockCategoriesQueryable = mockCategories.AsQueryable().BuildMockDbSet();

            //3- Digo qual sera o retorno do retorno do DbSet<Category>
            _dbContextMock.Categories.Returns(mockCategoriesQueryable);

            // Act
            var result = CategoryPut.Action(dummie_CategoryId,
                                           mockCategoryRequestDTO,
                                           _httpContextMock,
                                           _dbContextMock);
            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
        }

        [Fact]
        public void CategoryPut_NotFound()
        {
            // Arrange

            //Dados
            var dummie_CategoryId = Guid.NewGuid();

            var mockCategoryRequestDTO = new CategoryRequestDTO
            {
                Name = "Category Dumie",
                Active = true,
            };

            //Empty (not found)
            var mockCategory = new Category { };

            // Configurando as tabelas vituais

            //1 - Crio uma lista com os dados mockados
            var mockCategories = new List<Category> { mockCategory };

            //2- Transformo a lista em um tipo queryable
            var mockCategoriesQueryable = mockCategories.AsQueryable().BuildMockDbSet();

            //3- Digo qual sera o retorno do retorno do DbSet<Category>
            _dbContextMock.Categories.Returns(mockCategoriesQueryable);

            // Act
            var result = CategoryPut.Action(dummie_CategoryId,
                                           mockCategoryRequestDTO,
                                           _httpContextMock,
                                           _dbContextMock);
            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }

        [Fact]
        public void CategoryPut_ReturnErrorNameIsNull()
        {
            // Arrange

            //Dados
            var dummie_CategoryId = Guid.NewGuid();
            var dummie_user = "Doe Joe";

            var mockCategoryRequestDTO = new CategoryRequestDTO
            {
                Name = null,
                Active = true,
            };

            var mockCategory = new Category
            {
                Id = dummie_CategoryId,
                Name = mockCategoryRequestDTO.Name,
                Active = true,
                EditedBy = dummie_user,
                EditedOn = DateTime.Now
            };

            // Configurando as tabelas vituais

            //1 - Crio uma lista com os dados mockados
            var mockCategories = new List<Category> { mockCategory };

            //2- Transformo a lista em um tipo queryable
            var mockCategoriesQueryable = mockCategories.AsQueryable().BuildMockDbSet();

            //3- Digo qual sera o retorno do retorno do DbSet<Category>
            _dbContextMock.Categories.Returns(mockCategoriesQueryable);

            // Act
            var result = CategoryPut.Action(dummie_CategoryId,
                                           mockCategoryRequestDTO,
                                           _httpContextMock,
                                           _dbContextMock);
            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }
    }
}