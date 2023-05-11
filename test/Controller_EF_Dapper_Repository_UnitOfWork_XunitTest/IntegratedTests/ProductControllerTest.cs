using AutoMapper;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.UnitOfWork.Interface;
using Controller_EF_Dapper_Repository_UnityOfWork.Controllers;
using Controller_EF_Dapper_Repository_UnityOfWork.Endpoints.Products.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Controller_EF_Dapper_Repository_UnitOfWork_XunitTest
{
    public class ProductControllerTests
    {
        private readonly ProductController _productController;
        private readonly Mock<ILogger<ProductController>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public ProductControllerTests()
        {
            _loggerMock = new Mock<ILogger<ProductController>>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _productController = new ProductController(_loggerMock.Object, _mapperMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task ProductPost_CreatedWithSucess()
        {
            // Arrange

            // Crio um Dummie para categoria
            var mockCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Category Dumie"
            };

            // Crio um Dummie de input dos dados
            var mockProductRequestDTO = new ProductRequestDTO
            {
                Name = "Product Dumie",
                Description = "Description for product Dumie",
                Price = 1500,
                Active = true,
                CategoryId = mockCategory.Id,
            };

            // Crio um Dummie para a saida esperada de Product
            var mockProduct = new Product
            {
                Id = Guid.NewGuid(),
                Category = mockCategory,
                Name = mockProductRequestDTO.Name,
                Description = mockProductRequestDTO.Description,
                Price = mockProductRequestDTO.Price,
                CreatedBy = "doe joe",
                CreatedOn = DateTime.Now
            };

            // Crio comportamentos virtuais para as dependencias
            // utilizadas no EndPoint ProductPost
            // Eles vão enganar o endpoint no teste integrado

            //UnitOfWork ------------------------------------------------------------------
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            //A categoria simulada é recuperada
            unitOfWorkMock.Setup(x => x.Categories.Get(It.IsAny<Guid>()))
                                                  .ReturnsAsync(mockCategory);

            //A criacao do produto é simulada
            unitOfWorkMock.Setup(x => x.Products.Add(It.IsAny<Product>()))
                                                .Callback<Product>(p => p.Id = mockProduct.Id);

            //Mapper ------------------------------------------------------------------
            var mapperMock = new Mock<IMapper>();

            mapperMock.Setup(x => x.Map<Product>(It.IsAny<ProductRequestDTO>()))
                                    .Returns(mockProduct);

            mapperMock.Setup(x => x.Map<ProductResponseDTO>(It.IsAny<Product>()))
                                   .Returns(new ProductResponseDTO { Id = mockProduct.Id });

            //Logger ------------------------------------------------------------------
            var loggerMock = new Mock<ILogger<ProductController>>();

            //Controller------------------------------------------------------------------
            var productController = new ProductController(loggerMock.Object, mapperMock.Object, unitOfWorkMock.Object);

            // Act
            var result = await productController.ProductPost(mockProductRequestDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            //Obtendo o valor de "Location" Por reflexao
            var locationProperty = objectResult.Value.GetType().GetProperty("Location");
            var locationValue = locationProperty.GetValue(objectResult.Value);

            Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);

            Assert.Equal($"/products/{mockProduct.Id}", locationValue);
        }

        [Fact]
        public async Task ProductPost_ReturnError_PriceZero()
        {
            // Arrange

            // Crio um Dummie para categoria
            var mockCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Category Dumie"
            };

            // Crio um Dummie de input dos dados com um erro simulado
            var mockProductRequestDTO = new ProductRequestDTO
            {
                Name = "Product Dumie",
                Description = "Description for product Dumie",
                Price = 0,
                Active = true,
                CategoryId = mockCategory.Id,
            };

            // Crio um Dummie para a saida Esperada de Product
            var mockProduct = new Product
            {
                Id = Guid.NewGuid(),
                Category = mockCategory,
                Name = mockProductRequestDTO.Name,
                Description = mockProductRequestDTO.Description,
                Price = mockProductRequestDTO.Price,
                CreatedBy = "doe joe",
                CreatedOn = DateTime.Now
            };

            // Crio comportamentos virtuais para as dependencias
            // utilizadas no EndPoint ProductPost
            // Eles vão enganar o endpoint no teste integrado

            //UnitOfWork ------------------------------------------------------------------
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.Setup(x => x.Categories.Get(It.IsAny<Guid>()))
                                                  .ReturnsAsync(mockCategory);

            unitOfWorkMock.Setup(x => x.Products.Add(It.IsAny<Product>()))
                                                .Callback<Product>(p => p.Id = mockProduct.Id);

            //Mapper ------------------------------------------------------------------
            var mapperMock = new Mock<IMapper>();

            mapperMock.Setup(x => x.Map<Product>(It.IsAny<ProductRequestDTO>()))
                                    .Returns(mockProduct);

            mapperMock.Setup(x => x.Map<ProductResponseDTO>(It.IsAny<Product>()))
                                   .Returns(new ProductResponseDTO { Id = mockProduct.Id });

            //Logger ------------------------------------------------------------------
            var loggerMock = new Mock<ILogger<ProductController>>();

            //Controller------------------------------------------------------------------
            var productController = new ProductController(loggerMock.Object, mapperMock.Object, unitOfWorkMock.Object);

            // Act
            var result = await productController.ProductPost(mockProductRequestDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }

        [Fact]
        public async Task ProductPut_AlteredWithSucess()
        {
            // Arrange
            Guid productId_Dummie = Guid.NewGuid();

            // Crio um Dummie para categoria
            var mockCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Category Dumie"
            };

            // Crio um Dummie de input dos dados
            var mockProductRequestDTO = new ProductRequestDTO
            {
                Name = "Product Dumie",
                Description = "This Description has altered",
                Price = 1500,
                Active = true,
                CategoryId = mockCategory.Id,
            };

            // Crio um Dummie para a saida esperada de Product
            var mockProduct = new Product
            {
                Id = productId_Dummie,
                Category = mockCategory,
                Name = mockProductRequestDTO.Name,
                Description = mockProductRequestDTO.Description,
                Price = mockProductRequestDTO.Price,
                EditedBy = "doe joe",
                EditedOn = DateTime.Now
            };

            // Crio comportamentos virtuais para as dependencias
            // utilizadas no EndPoint ProductPost
            // Eles vão enganar o endpoint no teste integrado

            //UnitOfWork ------------------------------------------------------------------
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            //O produto simulado é recuperado
            unitOfWorkMock.Setup(x => x.Products.Get(It.IsAny<Guid>()))
                                                  .ReturnsAsync(mockProduct);

            //A categoria simulada é recuperada
            unitOfWorkMock.Setup(x => x.Categories.Get(It.IsAny<Guid>()))
                                      .ReturnsAsync(mockCategory);

            // Simulo o update da entidade
            unitOfWorkMock.Setup(x => x.Products.Update(It.IsAny<Product>()))
                                                .Callback<Product>(p => p.Id = mockProduct.Id);

            //Mapper ------------------------------------------------------------------
            var mapperMock = new Mock<IMapper>();

            mapperMock.Setup(x => x.Map<Product>(It.IsAny<ProductRequestDTO>()))
                                    .Returns(mockProduct);

            mapperMock.Setup(x => x.Map<ProductResponseDTO>(It.IsAny<Product>()))
                                   .Returns(new ProductResponseDTO { Id = mockProduct.Id });

            //Logger ------------------------------------------------------------------
            var loggerMock = new Mock<ILogger<ProductController>>();

            //Controller------------------------------------------------------------------
            var productController = new ProductController(loggerMock.Object, mapperMock.Object, unitOfWorkMock.Object);

            // Act
            var result = await productController.ProductPutAsync(productId_Dummie, mockProductRequestDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
        }

        [Fact]
        public async Task ProductPut_NotFound()
        {
            // Arrange
            Guid productId_Dummie = Guid.NewGuid();

            // Crio um Dummie para categoria
            var mockCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Category Dumie"
            };

            // Crio um Dummie de input dos dados
            var mockProductRequestDTO = new ProductRequestDTO
            {
                Name = "Product Dumie",
                Description = "This Description has altered",
                Price = 1500,
                Active = true,
                CategoryId = mockCategory.Id,
            };

            // Crio um Dummie vazio para a saida esperada de Product
            var mockProduct = new Product();

            // Crio comportamentos virtuais para as dependencias
            // utilizadas no EndPoint ProductPost
            // Eles vão enganar o endpoint no teste integrado

            //UnitOfWork ------------------------------------------------------------------
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            //Forco a simulacao de um retorno nulo do objeto product
            unitOfWorkMock.Setup(x => x.Products.Get(It.IsAny<Guid>()))
                                                 .ReturnsAsync((Guid id) => null);

            //A categoria simulada é recuperada
            unitOfWorkMock.Setup(x => x.Categories.Get(It.IsAny<Guid>()))
                                      .ReturnsAsync(mockCategory);

            // Simulo o update da entidade
            unitOfWorkMock.Setup(x => x.Products.Update(It.IsAny<Product>()))
                                                .Callback<Product>(p => p.Id = mockProduct.Id);

            //Mapper ------------------------------------------------------------------
            var mapperMock = new Mock<IMapper>();

            mapperMock.Setup(x => x.Map<Product>(It.IsAny<ProductRequestDTO>()))
                                    .Returns(mockProduct);

            mapperMock.Setup(x => x.Map<ProductResponseDTO>(It.IsAny<Product>()))
                                   .Returns(new ProductResponseDTO { Id = mockProduct.Id });

            //Logger ------------------------------------------------------------------
            var loggerMock = new Mock<ILogger<ProductController>>();

            //Controller------------------------------------------------------------------
            var productController = new ProductController(loggerMock.Object, mapperMock.Object, unitOfWorkMock.Object);

            // Act
            var result = await productController.ProductPutAsync(productId_Dummie, mockProductRequestDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }

        [Fact]
        public async Task ProductPut_ReturnError_NameIsNull()
        {
            // Arrange
            Guid productId_Dummie = Guid.NewGuid();

            // Crio um Dummie para categoria
            var mockCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Category Dumie"
            };

            // Crio um Dummie de input dos dados com um erro simulado
            var mockProductRequestDTO = new ProductRequestDTO
            {
                Name = null,
                Description = "Description for product Dumie",
                Price = 0,
                Active = true,
                CategoryId = mockCategory.Id,
            };

            // Crio um Dummie para a saida Esperada de Product
            var mockProduct = new Product
            {
                Id = Guid.NewGuid(),
                Category = mockCategory,
                Name = mockProductRequestDTO.Name,
                Description = mockProductRequestDTO.Description,
                Price = mockProductRequestDTO.Price,
                CreatedBy = "doe joe",
                CreatedOn = DateTime.Now
            };

            // Crio comportamentos virtuais para as dependencias
            // utilizadas no EndPoint ProductPost
            // Eles vão enganar o endpoint no teste integrado

            //UnitOfWork ------------------------------------------------------------------
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            //O produto simulado é recuperado
            unitOfWorkMock.Setup(x => x.Products.Get(It.IsAny<Guid>()))
                                                  .ReturnsAsync(mockProduct);

            //A categoria simulada é recuperada
            unitOfWorkMock.Setup(x => x.Categories.Get(It.IsAny<Guid>()))
                                      .ReturnsAsync(mockCategory);

            // Simulo o update da entidade
            unitOfWorkMock.Setup(x => x.Products.Update(It.IsAny<Product>()))
                                                .Callback<Product>(p => p.Id = mockProduct.Id);

            //Mapper ------------------------------------------------------------------
            var mapperMock = new Mock<IMapper>();

            mapperMock.Setup(x => x.Map<Product>(It.IsAny<ProductRequestDTO>()))
                                    .Returns(mockProduct);

            mapperMock.Setup(x => x.Map<ProductResponseDTO>(It.IsAny<Product>()))
                                   .Returns(new ProductResponseDTO { Id = mockProduct.Id });

            //Logger ------------------------------------------------------------------
            var loggerMock = new Mock<ILogger<ProductController>>();

            //Controller------------------------------------------------------------------
            var productController = new ProductController(loggerMock.Object, mapperMock.Object, unitOfWorkMock.Object);

            // Act
            var result = await productController.ProductPutAsync(productId_Dummie, mockProductRequestDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }

        [Fact]
        public async Task ProductDelete_DeleteWithSucess()
        {
            // Arrange

            Guid productId_Dummie = Guid.NewGuid();

            // Crio um Dummie para a recuperadao esperada
            var mockProduct = new Product
            {
                Id = productId_Dummie,
                Name = "Product Dumie",
                Description = "Description for product Dumie",
                Price = 1500,
                Active = true,
                CategoryId = Guid.NewGuid(),
                CreatedBy = "doe joe",
                CreatedOn = DateTime.Now
            };

            // Crio comportamentos virtuais para as dependencias
            // utilizadas no EndPoint ProductPost
            // Eles vão enganar o endpoint no teste integrado

            //UnitOfWork ------------------------------------------------------------------
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            //O produto simulado é recuperado
            unitOfWorkMock.Setup(x => x.Products.Get(It.IsAny<Guid>()))
                                                  .ReturnsAsync(mockProduct);

            //A criacao do produto é simulada
            unitOfWorkMock.Setup(x => x.Products.Delete(It.IsAny<Product>()))
                                                .Callback<Product>(p => p.Id = mockProduct.Id);

            //Mapper ------------------------------------------------------------------
            var mapperMock = new Mock<IMapper>();

            //Logger ------------------------------------------------------------------
            var loggerMock = new Mock<ILogger<ProductController>>();

            //Controller------------------------------------------------------------------
            var productController = new ProductController(loggerMock.Object, mapperMock.Object, unitOfWorkMock.Object);

            // Act
            var result = await productController.ProductDeleteAsync(productId_Dummie);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
        }
    }
}