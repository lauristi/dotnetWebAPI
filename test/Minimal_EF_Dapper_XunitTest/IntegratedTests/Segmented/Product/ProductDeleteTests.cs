using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Domain.Database.Entities.Product;
using Minimal_EF_Dapper.Endpoints.Segmented.Products;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace Minimal_EF_Dapper_XunitTest
{
    public class ProductDeleteTests
    {
        private readonly ApplicationDbContext _dbContextMock;
        private readonly HttpContext _httpContextMock;

        public ProductDeleteTests()
        {
            // Configura o mock dos contextos
            _dbContextMock = Substitute.For<ApplicationDbContext>();
            _httpContextMock = Substitute.For<HttpContext>();
        }

        [Fact]
        public void ProductDelete_DeletedWithSucess()
        {
            // Arrange

            //Dados
            var dummie_ProductId = Guid.NewGuid();
            var dummie_CategoryId = Guid.NewGuid();
            var dummie_user = "doe joe";

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
                Name = "Test Product",
                Description = "Test Description",
                Price = 10,
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
            var result = ProductDelete.Action(dummie_ProductId, _dbContextMock);
            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
        }

        [Fact]
        public void ProductDelete_NotFound()
        {
            // Arrange

            //Dados
            var dummie_ProductId = Guid.NewGuid();
            var dummie_CategoryId = Guid.NewGuid();
            var dummie_user = "doe joe";

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
                Name = "Test Product",
                Description = "Test Description",
                Price = 10,
                EditedBy = dummie_user,
                EditedOn = DateTime.Now
            };

            // Configurando as tabelas vituais

            //1 - Crio uma lista com os dados mockados
            var mockCategories = new List<Category> { mockCategory };
            var mockProducts = new List<Product> { }; //==> NotFound

            //2- Transformo a lista em um tipo queryable
            var mockCategoriesQueryable = mockCategories.AsQueryable().BuildMockDbSet();
            var mockProductsQueryable = mockProducts.AsQueryable().BuildMockDbSet();

            //3- Digo qual sera o retorno do retorno do DbSet<Category>
            _dbContextMock.Products.Returns(mockProductsQueryable);
            _dbContextMock.Categories.Returns(mockCategoriesQueryable);

            // Act
            var result = ProductDelete.Action(dummie_ProductId, _dbContextMock);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }
    }
}