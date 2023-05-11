using AutoMapper;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.UnitOfWork.Interface;
using Controller_EF_Dapper_Repository_UnityOfWork.Controllers;
using Controller_EF_Dapper_Repository_UnityOfWork.Endpoints.Orders.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Controller_EF_Dapper_Repository_UnitOfWork_XunitTest
{
    public class OrderControllerTests
    {
        private readonly OrderController _orderController;
        private readonly Mock<ILogger<OrderController>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public OrderControllerTests()
        {
            _loggerMock = new Mock<ILogger<OrderController>>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _orderController = new OrderController(_loggerMock.Object, _mapperMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task OrderPost_CreatedWithSucess()
        {
            // Arrange

            var dummie_Total = 1500;
            var dummie_CategoryId = Guid.NewGuid();
            var dummie_ProductId1 = Guid.NewGuid();
            var dummie_ProductId2 = Guid.NewGuid();

            // Crio um Dummie de input dos dados
            var mockOrderRequestDTO = new OrderRequestDTO(new List<Guid>
                {
                    dummie_ProductId1,
                    dummie_ProductId2,
                });

            //------------------------------------------------------------------------
            // Crio um Dummie para a saida esperada de Order
            //------------------------------------------------------------------------

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
                    Price = 10.0m,
                    Active = true,
                    CategoryId = mockCategory.Id,
                    CreatedBy = "doe joe",
                    CreatedOn = DateTime.Now
                },
                new Product
                {
                    Id = dummie_ProductId2,
                    Name = "Product 2",
                    Description = "Description of product 2",
                    Price = 20.0m,
                    Active = true,
                    CategoryId = mockCategory.Id,
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

            //------------------------------------------------------------------------

            // Crio comportamentos virtuais para as dependencias
            // utilizadas no EndPoint OrderPost
            // Eles vão enganar o endpoint no teste integrado

            //UnitOfWork ------------------------------------------------------------------
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            //Simulo a recuperacao da lista de produtos
            unitOfWorkMock.Setup(x => x.Products.Find(p => mockOrderRequestDTO.ProductsId.Contains(p.Id)))
                                               .ReturnsAsync(mockProducts);

            //A criacao do order é simulada
            unitOfWorkMock.Setup(x => x.Orders.Add(It.IsAny<Order>()))
                                                .Callback<Order>(p => p.Id = mockOrder.Id);

            //Mapper ------------------------------------------------------------------
            var mapperMock = new Mock<IMapper>();

            //Logger ------------------------------------------------------------------
            var loggerMock = new Mock<ILogger<OrderController>>();

            //Controller------------------------------------------------------------------
            var orderController = new OrderController(loggerMock.Object, mapperMock.Object, unitOfWorkMock.Object);

            // Act
            var result = await orderController.OrderPost(mockOrderRequestDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            //Obtendo o valor de "Location" Por reflexao
            var locationProperty = objectResult.Value.GetType().GetProperty("Location");
            var locationValue = locationProperty.GetValue(objectResult.Value);

            Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);

            Assert.Equal($"/orders/{mockOrder.Id}", locationValue);
        }

        [Fact]
        public async Task OrderPost_Error_ProductsNotFound()
        {
            // Arrange

            var dummie_Total = 1500;
            var dummie_CategoryId = Guid.NewGuid();
            var dummie_ProductId1 = Guid.NewGuid();
            var dummie_ProductId2 = Guid.NewGuid();

            // Crio um Dummie de input dos dados
            var mockOrderRequestDTO = new OrderRequestDTO(new List<Guid>
                {
                    dummie_ProductId1,
                    dummie_ProductId2,
                });

            //------------------------------------------------------------------------
            // Crio um Dummie para a saida esperada de Order
            //------------------------------------------------------------------------

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
                ClientId = "Cod-2344",
                ClientName = "Doe Joe",
                Products = mockProducts,
                Total = dummie_Total,
                CreatedBy = "doe joe",
                CreatedOn = DateTime.Now
            };

            //------------------------------------------------------------------------

            // Crio comportamentos virtuais para as dependencias
            // utilizadas no EndPoint OrderPost
            // Eles vão enganar o endpoint no teste integrado

            //UnitOfWork ------------------------------------------------------------------
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            //Simulo a recuperacao da lista de produtos vazia
            unitOfWorkMock.Setup(x => x.Products.GetAll())
                                       .ReturnsAsync(new List<Product>());

            //A criacao do order é simulada
            unitOfWorkMock.Setup(x => x.Orders.Add(It.IsAny<Order>()))
                                                .Callback<Order>(p => p.Id = mockOrder.Id);

            //Mapper ------------------------------------------------------------------
            var mapperMock = new Mock<IMapper>();

            //Logger ------------------------------------------------------------------
            var loggerMock = new Mock<ILogger<OrderController>>();

            //Controller------------------------------------------------------------------
            var orderController = new OrderController(loggerMock.Object, mapperMock.Object, unitOfWorkMock.Object);

            // Act
            var result = await orderController.OrderPost(mockOrderRequestDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }
    }
}