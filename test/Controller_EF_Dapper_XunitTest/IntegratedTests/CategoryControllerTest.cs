using Controler_EF_Dapper.Domain.Database;
using Controler_EF_Dapper.Domain.Database.Entities.Product;
using Controller_EF_Dapper.Controllers;
using Controller_EF_Dapper.Endpoints.DTO.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace Controller_EF_Dapper_XunitTest
{
    public class CategoryControllerTests
    {
        private readonly ILogger<CategoryController> _loggerMock;
        private readonly ApplicationDbContext _dbContextMock;

        private readonly CategoryController _categoryControllerMock;

        public CategoryControllerTests()
        {
            // Configura o mock dos contextos
            _loggerMock = Substitute.For<ILogger<CategoryController>>();
            _dbContextMock = Substitute.For<ApplicationDbContext>();

            _categoryControllerMock = new CategoryController(_loggerMock, _dbContextMock);
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
            var result = await _categoryControllerMock.CategoryPost(mockCategoryRequestDTO);

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
            var result = await _categoryControllerMock.CategoryPost(mockCategoryRequestDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
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
            var result = _categoryControllerMock.CategoryPut(dummie_CategoryId, mockCategoryRequestDTO);

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
            var result = _categoryControllerMock.CategoryPut(dummie_CategoryId, mockCategoryRequestDTO);

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
            var result = _categoryControllerMock.CategoryPut(dummie_CategoryId, mockCategoryRequestDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }

        [Fact]
        public void CategoryDelete_DeletedWithSucess()
        {
            // Arrange

            //Dados
            var dummie_CategoryId = Guid.NewGuid();

            var mockCategory = new Category
            {
                Id = dummie_CategoryId,
                Name = "Test Category",
                Active = true
            };

            // Configurando as tabelas vituais

            //1 - Crio uma lista com os dados mockados
            var mockCategories = new List<Category> { mockCategory };

            //2- Transformo a lista em um tipo queryable
            var mockCategoriesQueryable = mockCategories.AsQueryable().BuildMockDbSet();

            //3- Digo qual sera o retorno do retorno do DbSet<Category>
            _dbContextMock.Categories.Returns(mockCategoriesQueryable);

            // Act
            var result = _categoryControllerMock.CategoryDelete(dummie_CategoryId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
        }

        [Fact]
        public void CategoryDelete_NotFound()
        {
            // Arrange

            //Dados
            var dummie_CategoryId = Guid.NewGuid();

            //Not Found
            var mockCategory = new Category { };

            // Configurando as tabelas vituais

            //1 - Crio uma lista com os dados mockados
            var mockCategories = new List<Category> { mockCategory };

            //2- Transformo a lista em um tipo queryable
            var mockCategoriesQueryable = mockCategories.AsQueryable().BuildMockDbSet();

            //3- Digo qual sera o retorno do retorno do DbSet<Category>
            _dbContextMock.Categories.Returns(mockCategoriesQueryable);

            // Act
            var result = _categoryControllerMock.CategoryDelete(dummie_CategoryId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }
    }
}