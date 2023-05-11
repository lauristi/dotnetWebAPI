using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Domain.Database.Entities.Product;
using Minimal_EF_Dapper.Endpoints.DTO.Product;
using Minimal_EF_Dapper.Endpoints.Segmented.Products;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace Minimal_EF_Dapper_XunitTest
{
    public class ProductPutTests
    {
        private readonly ApplicationDbContext _dbContextMock;
        private readonly HttpContext _httpContextMock;

        public ProductPutTests()
        {
            // Configura o mock dos contextos
            _dbContextMock = Substitute.For<ApplicationDbContext>();
            _httpContextMock = Substitute.For<HttpContext>();
        }

        [Fact]
        public void ProductPut_AlteredWithSucess()
        {
            // Arrange

            //Dados
            var dummie_ProductId = Guid.NewGuid();
            var dummie_CategoryId = Guid.NewGuid();
            var dummie_user = "doe joe";

            var mockProductRequestDTO = new ProductRequestDTO
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 10,
                CategoryId = dummie_CategoryId
            };

            var mockCategory = new Category
            {
                Id = dummie_CategoryId,
                Name = "Test Category",
                Active = true
            };

            var mockProduct = new Product
            {
                Id = dummie_ProductId,
                Category = mockCategory,
                Name = mockProductRequestDTO.Name,
                Description = mockProductRequestDTO.Description,
                Price = mockProductRequestDTO.Price,
                EditedBy = dummie_user,
                EditedOn = DateTime.Now
            };

            // Configurando as tabelas vituais

            //1 - Crio uma lista com os dados mockados
            var mockCategories = new List<Category> { mockCategory };
            var mockProducts = new List<Product> { mockProduct };

            //2- Transformo a lista em um tipo queryable
            var mockCategoriesQueryable = mockCategories.AsQueryable().BuildMockDbSet();
            var mockProductsQueryable = mockProducts.AsQueryable().BuildMockDbSet();

            //3- Digo qual sera o retorno do retorno do DbSet<Category>
            _dbContextMock.Products.Returns(mockProductsQueryable);
            _dbContextMock.Categories.Returns(mockCategoriesQueryable);

            // Act
            var result = ProductPut.Action(dummie_ProductId,
                                           mockProductRequestDTO,
                                           _httpContextMock,
                                           _dbContextMock);
            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            //Testes adiconais possiveis
            //------------------------------------------------------------------------------------------------------
            //Assert.Equal(productRequestDTO.Name, productQuery.First().Name);
            //Assert.Equal(productRequestDTO.Price, productQuery.First().Price);
            //Assert.True(productQuery.First().IsActive);
            //Assert.Equal(categoryId, productQuery.First().CategoryId);
            //Assert.Equal(user, productQuery.First().LastEditor);
            //dbContext.Received(1).SaveChanges();
        }

        [Fact]
        public void ProductPut_ReturnError_NameIsNull()
        {
            // Arrange

            //Dados
            var dummie_ProductId = Guid.NewGuid();
            var dummie_CategoryId = Guid.NewGuid();
            var dummie_user = "doe joe";

            var mockProductRequestDTO = new ProductRequestDTO
            {
                Name = null,
                Description = "Test Description",
                Price = 10,
                CategoryId = dummie_CategoryId
            };

            var mockCategory = new Category
            {
                Id = dummie_CategoryId,
                Name = "Test Category",
                Active = true
            };

            var mockProduct = new Product
            {
                Id = dummie_ProductId,
                Category = mockCategory,
                Name = mockProductRequestDTO.Name,
                Description = mockProductRequestDTO.Description,
                Price = mockProductRequestDTO.Price,
                EditedBy = dummie_user,
                EditedOn = DateTime.Now
            };

            // Configurando as tabelas vituais

            //1 - Crio uma lista com os dados mockados
            var mockCategories = new List<Category> { mockCategory };
            var mockProducts = new List<Product> { mockProduct };

            //2- Transformo a lista em um tipo queryable
            var mockCategoriesQueryable = mockCategories.AsQueryable().BuildMockDbSet();
            var mockProductsQueryable = mockProducts.AsQueryable().BuildMockDbSet();

            //3- Digo qual sera o retorno do retorno do DbSet<Category>
            _dbContextMock.Products.Returns(mockProductsQueryable);
            _dbContextMock.Categories.Returns(mockCategoriesQueryable);

            // Act
            var result = ProductPut.Action(dummie_ProductId,
                                           mockProductRequestDTO,
                                           _httpContextMock,
                                           _dbContextMock);
            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }
    }
}