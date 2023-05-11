using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Domain.Database.Entities.Product;
using Minimal_EF_Dapper.Endpoints.DTO.Order;
using Minimal_EF_Dapper.Endpoints.Segmented.Orders;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace Minimal_EF_Dapper_XunitTest
{
    public class OrderPostTests
    {
        private readonly ApplicationDbContext _dbContextMock;
        private readonly HttpContext _httpContextMock;

        public OrderPostTests()
        {
            // Configura o mock dos contextos
            _dbContextMock = Substitute.For<ApplicationDbContext>();
            _httpContextMock = Substitute.For<HttpContext>();
        }

        [Fact]
        public async Task OrderPost_CreatedWithSucess()
        {
            // Arrange --------------------------------------------------------------------------------------------------

            // Mock dos dados
            var dummie_CategoryId = Guid.NewGuid();
            var dummie_Total = 1500;

            var dummie_ProductId1 = Guid.NewGuid();
            var dummie_ProductId2 = Guid.NewGuid();

            // Crio um Dummie de input dos dados
            var mockOrderRequestDTO = new OrderRequestDTO(new List<Guid>
                {
                    dummie_ProductId1,
                    dummie_ProductId2,
                });

            var mockCategory = new Category
            {
                Id = dummie_CategoryId,
                Name = "Category 1",
                Active = true,
                CreatedBy = "doe joe",
                CreatedOn = DateTime.Now
            };

            var mockProducts = new List<Product>
            {
                new Product
                {
                    Id = dummie_ProductId1,
                    Name = "Product 1",
                    Description = "Description of product 1",
                    Price = 1500,
                    Active = true,
                    CategoryId = dummie_CategoryId,
                    CreatedBy = "doe joe",
                    CreatedOn = DateTime.Now
                },
                new Product
                {
                    Id = dummie_ProductId2,
                    Name = "Product 2",
                    Description = "Description of product 2",
                    Price = 2000,
                    Active = true,
                    CategoryId = dummie_CategoryId,
                    CreatedBy = "doe joe",
                    CreatedOn = DateTime.Now
                }
            };

            var mockOrder = new Order
            {
                Id = Guid.NewGuid(),
                ClientId = "Client1",
                ClientName = "Client Name 1",
                Products = mockProducts,
                Total = dummie_Total,
                CreatedBy = "doe joe",
                CreatedOn = DateTime.Now
            };

            //Preparando o DBSet para interagir com metodos IQueryable

            //1 - Crio uma lista com os dados mockados
            var mockOrders = new List<Order> { mockOrder };

            //2- Transformo a lista em um tipo queryable
            var mockOrdersQueryable = mockOrders.AsQueryable().BuildMockDbSet();
            var mockProductsQueryable = mockProducts.AsQueryable().BuildMockDbSet();

            //3- Digo qual sera o retorno do retorno do DbSet<Category>
            _dbContextMock.Orders.Returns(mockOrdersQueryable);
            _dbContextMock.Products.Returns(mockProductsQueryable);

            // Act ----------------------------------------------------------------------------------------------------
            var result = await OrderPost.Action(mockOrderRequestDTO, _httpContextMock, _dbContextMock);

            var objectResponse = (ObjectResult)result;

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);
        }

        [Fact]
        public async Task OrderPost_ProductNotFound()
        {
            // Arrange --------------------------------------------------------------------------------------------------

            // Mock dos dados
            var dummie_CategoryId = Guid.NewGuid();
            var dummie_Total = 1500;

            var dummie_ProductId1 = Guid.NewGuid();
            var dummie_ProductId2 = Guid.NewGuid();

            // Crio um Dummie de input dos dados
            var mockOrderRequestDTO = new OrderRequestDTO(new List<Guid>
                {
                    dummie_ProductId1,
                    dummie_ProductId2,
                });

            var mockCategory = new Category
            {
                Id = dummie_CategoryId,
                Name = "Category 1",
                Active = true,
                CreatedBy = "doe joe",
                CreatedOn = DateTime.Now
            };

            //Mock com erro, products = null
            var mockProducts = new List<Product>();

            var mockOrder = new Order
            {
                Id = Guid.NewGuid(),
                ClientId = "Client1",
                ClientName = "Neil Armstrong",
                Products = mockProducts,
                Total = dummie_Total,
                CreatedBy = "doe joe",
                CreatedOn = DateTime.Now
            };

            //------------------------------------------------------------------------

            //Preparando o DBSet para interagir com metodos IQueryable

            //1 - Crio uma lista com os dados mockados
            var mockOrders = new List<Order> { mockOrder };

            //2- Transformo a lista em um tipo queryable
            var mockOrdersQueryable = mockOrders.AsQueryable().BuildMockDbSet();
            var mockProductsQueryable = mockProducts.AsQueryable().BuildMockDbSet();

            //3- Digo qual sera o retorno do retorno do DbSet<Category>
            _dbContextMock.Orders.Returns(mockOrdersQueryable);
            _dbContextMock.Products.Returns(mockProductsQueryable);

            // Act ----------------------------------------------------------------------------------------------------
            var result = await OrderPost.Action(mockOrderRequestDTO, _httpContextMock, _dbContextMock);

            var objectResponse = (ObjectResult)result;

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }
    }
}