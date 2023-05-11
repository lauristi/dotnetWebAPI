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
    public class CategoryPostTests
    {
        private readonly ApplicationDbContext _dbContextMock;
        private readonly HttpContext _httpContextMock;

        public CategoryPostTests()
        {
            // Configura o mock dos contextos
            _dbContextMock = Substitute.For<ApplicationDbContext>();
            _httpContextMock = Substitute.For<HttpContext>();
        }

        [Fact]
        public async Task CategoryPost_CreatedWithSucess()
        {
            // Arrange --------------------------------------------------------------------------------------------------

            // Mock dos dados
            var dummie_CategoryId = Guid.NewGuid();

            var mockCategoryRequestDTO = new CategoryRequestDTO
            {
                Name = "Category Dumie",
                Active = true,
            };

            var mockCategory = new Category
            {
                Id = dummie_CategoryId,
                Name = "Category Dumie",
                Active = true
            };

            //Preparando o DBSet para interagir com metodos IQueryable

            //1 - Crio uma lista com os dados mockados
            var mockCategories = new List<Category> { mockCategory };

            //2- Transformo a lista em um tipo queryable
            var mockCategoriesQueryable = mockCategories.AsQueryable().BuildMockDbSet();

            //3- Digo qual sera o retorno do retorno do DbSet<Category>
            _dbContextMock.Categories.Returns(mockCategoriesQueryable);

            // Act ----------------------------------------------------------------------------------------------------
            var result = await CategoryPost.Action(mockCategoryRequestDTO, _httpContextMock, _dbContextMock);

            var objectResponse = (ObjectResult)result;

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);
        }

        [Fact]
        public async Task CategoryPost_ReturnError_NameIsNull()
        {
            var dummie_CategoryId = Guid.NewGuid();

            var mockCategoryRequestDTO = new CategoryRequestDTO
            {
                Name = null,
                Active = true,
            };

            var mockCategory = new Category
            {
                Id = dummie_CategoryId,
                Name = mockCategoryRequestDTO.Name,
                Active = true
            };

            // Mockando o DbSet<Category> com MockQueryable.NSubstitute
            var categoryList = new List<Category> { mockCategory };
            var categoryQueryable = categoryList.AsQueryable().BuildMockDbSet();
            _dbContextMock.Categories.Returns(categoryQueryable);

            // Act
            var result = await CategoryPost.Action(mockCategoryRequestDTO, _httpContextMock, _dbContextMock);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }
    }
}