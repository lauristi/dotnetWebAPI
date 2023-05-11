using AutoMapper;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.UnitOfWork.Interface;
using Controller_EF_Dapper_Repository_UnityOfWork.Controllers;
using Controller_EF_Dapper_Repository_UnityOfWork.Endpoints.Categories.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Controller_EF_Dapper_Repository_UnitOfWork_XunitTest
{
    public class CategoryControllerTests
    {
        private readonly CategoryController _categoryController;
        private readonly Mock<ILogger<CategoryController>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public CategoryControllerTests()
        {
            _loggerMock = new Mock<ILogger<CategoryController>>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _categoryController = new CategoryController(_loggerMock.Object, _mapperMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CategoryPost_CreatedWithSucess()
        {
            // Arrange

            // Crio um Dummie de input dos dados
            var mockCategoryRequestDTO = new CategoryRequestDTO
            {
                Name = "Category Dumie",
                Active = true,
            };

            // Crio um Dummie para a saida esperada de Category
            var mockCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = mockCategoryRequestDTO.Name,
                Active = true,
                CreatedBy = "doe joe",
                CreatedOn = DateTime.Now
            };

            // Crio comportamentos virtuais para as dependencias
            // utilizadas no EndPoint CategoryPost
            // Eles vão enganar o endpoint no teste integrado

            //UnitOfWork ------------------------------------------------------------------
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            //A criacao do categoria é simulada
            unitOfWorkMock.Setup(x => x.Categories.Add(It.IsAny<Category>()))
                                                .Callback<Category>(p => p.Id = mockCategory.Id);

            //Mapper ------------------------------------------------------------------
            var mapperMock = new Mock<IMapper>();

            mapperMock.Setup(x => x.Map<Category>(It.IsAny<CategoryRequestDTO>()))
                                    .Returns(mockCategory);

            mapperMock.Setup(x => x.Map<CategoryResponseDTO>(It.IsAny<Category>()))
                                   .Returns(new CategoryResponseDTO { Id = mockCategory.Id });

            //Logger ------------------------------------------------------------------
            var loggerMock = new Mock<ILogger<CategoryController>>();

            //Controller------------------------------------------------------------------
            var categoryController = new CategoryController(loggerMock.Object, mapperMock.Object, unitOfWorkMock.Object);

            // Act
            var result = await categoryController.CategoryPost(mockCategoryRequestDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            //Obtendo o valor de "Location" Por reflexao
            var locationProperty = objectResult.Value.GetType().GetProperty("Location");
            var locationValue = locationProperty.GetValue(objectResult.Value);

            Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);

            Assert.Equal($"/categories/{mockCategory.Id}", locationValue);
        }

        [Fact]
        public async Task CategoryPost_Error_NameIsNull()
        {
            // Arrange

            // Crio um Dummie de input dos dados
            var mockCategoryRequestDTO = new CategoryRequestDTO
            {
                Name = null,
                Active = true,
            };

            // Crio um Dummie para a saida esperada de Category
            var mockCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = mockCategoryRequestDTO.Name,
                Active = true,
                CreatedBy = "doe joe",
                CreatedOn = DateTime.Now
            };

            // Crio comportamentos virtuais para as dependencias
            // utilizadas no EndPoint CategoryPost
            // Eles vão enganar o endpoint no teste integrado

            //UnitOfWork ------------------------------------------------------------------
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            //A criacao do categoria é simulada
            unitOfWorkMock.Setup(x => x.Categories.Add(It.IsAny<Category>()))
                                                .Callback<Category>(p => p.Id = mockCategory.Id);

            //Mapper ------------------------------------------------------------------
            var mapperMock = new Mock<IMapper>();

            mapperMock.Setup(x => x.Map<Category>(It.IsAny<CategoryRequestDTO>()))
                                    .Returns(mockCategory);

            mapperMock.Setup(x => x.Map<CategoryResponseDTO>(It.IsAny<Category>()))
                                   .Returns(new CategoryResponseDTO { Id = mockCategory.Id });

            //Logger ------------------------------------------------------------------
            var loggerMock = new Mock<ILogger<CategoryController>>();

            //Controller------------------------------------------------------------------
            var categoryController = new CategoryController(loggerMock.Object, mapperMock.Object, unitOfWorkMock.Object);

            // Act
            var result = await categoryController.CategoryPost(mockCategoryRequestDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }

        [Fact]
        public async Task CategoryPut_AlteredWithSucess()
        {
            // Arrange
            Guid categoryId_Dummie = Guid.NewGuid();

            // Crio um Dummie de input dos dados
            var mockCategoryRequestDTO = new CategoryRequestDTO
            {
                Name = "Category Dumie",
                Active = true,
            };

            // Crio um Dummie para a saida esperada de Category
            var mockCategory = new Category
            {
                Id = categoryId_Dummie,
                Name = mockCategoryRequestDTO.Name,
                Active = true,
                EditedBy = "doe joe",
                EditedOn = DateTime.Now
            };

            // Crio comportamentos virtuais para as dependencias
            // utilizadas no EndPoint CategoryPost
            // Eles vão enganar o endpoint no teste integrado

            //UnitOfWork ------------------------------------------------------------------
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            //A categoria simulada é recuperada
            unitOfWorkMock.Setup(x => x.Categories.Get(It.IsAny<Guid>()))
                                      .ReturnsAsync(mockCategory);

            // Simulo o update da entidade
            unitOfWorkMock.Setup(x => x.Categories.Update(It.IsAny<Category>()))
                                                  .Callback<Category>(p => p.Id = mockCategory.Id);

            //Mapper ------------------------------------------------------------------
            var mapperMock = new Mock<IMapper>();

            mapperMock.Setup(x => x.Map<Category>(It.IsAny<CategoryRequestDTO>()))
                                    .Returns(mockCategory);

            mapperMock.Setup(x => x.Map<CategoryResponseDTO>(It.IsAny<Category>()))
                                   .Returns(new CategoryResponseDTO { Id = mockCategory.Id });

            //Logger ------------------------------------------------------------------
            var loggerMock = new Mock<ILogger<CategoryController>>();

            //Controller------------------------------------------------------------------
            var categoryController = new CategoryController(loggerMock.Object, mapperMock.Object, unitOfWorkMock.Object);

            // Act
            var result = await categoryController.CategoryPutAsync(categoryId_Dummie, mockCategoryRequestDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
        }

        [Fact]
        public async Task CategoryPut_NotFound()
        {
            // Arrange
            Guid categoryId_Dummie = Guid.NewGuid();

            // Crio um Dummie de input dos dados
            var mockCategoryRequestDTO = new CategoryRequestDTO
            {
                Name = "Category Dumie",
                Active = true,
            };

            // Crio um Dummie vazio para a saida esperada de Category
            var mockCategory = new Category();

            // Crio comportamentos virtuais para as dependencias
            // utilizadas no EndPoint CategoryPost
            // Eles vão enganar o endpoint no teste integrado

            //UnitOfWork ------------------------------------------------------------------
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            //Forco a simulacao de um retorno nulo do objeto category
            unitOfWorkMock.Setup(x => x.Categories.Get(It.IsAny<Guid>()))
                                      .ReturnsAsync((Guid id) => null);

            // Simulo o update da entidade
            unitOfWorkMock.Setup(x => x.Categories.Update(It.IsAny<Category>()))
                                                  .Callback<Category>(p => p.Id = mockCategory.Id);

            //Mapper ------------------------------------------------------------------
            var mapperMock = new Mock<IMapper>();

            mapperMock.Setup(x => x.Map<Category>(It.IsAny<CategoryRequestDTO>()))
                                    .Returns(mockCategory);

            mapperMock.Setup(x => x.Map<CategoryResponseDTO>(It.IsAny<Category>()))
                                   .Returns(new CategoryResponseDTO { Id = mockCategory.Id });

            //Logger ------------------------------------------------------------------
            var loggerMock = new Mock<ILogger<CategoryController>>();

            //Controller------------------------------------------------------------------
            var categoryController = new CategoryController(loggerMock.Object, mapperMock.Object, unitOfWorkMock.Object);

            // Act
            var result = await categoryController.CategoryPutAsync(categoryId_Dummie, mockCategoryRequestDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }

        [Fact]
        public async Task CategoryPut_ReturnErrorNameIsNull()
        {
            // Arrange
            Guid categoryId_Dummie = Guid.NewGuid();

            // Crio um Dummie de input dos dados
            var mockCategoryRequestDTO = new CategoryRequestDTO
            {
                Name = null,
                Active = true,
            };

            // Crio um Dummie para a saida esperada de Category
            var mockCategory = new Category
            {
                Id = categoryId_Dummie,
                Name = mockCategoryRequestDTO.Name,
                Active = true,
                EditedBy = "doe joe",
                EditedOn = DateTime.Now
            };

            // Crio comportamentos virtuais para as dependencias
            // utilizadas no EndPoint CategoryPost
            // Eles vão enganar o endpoint no teste integrado

            //UnitOfWork ------------------------------------------------------------------
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            //A categoria simulada é recuperada
            unitOfWorkMock.Setup(x => x.Categories.Get(It.IsAny<Guid>()))
                                      .ReturnsAsync(mockCategory);

            // Simulo o update da entidade
            unitOfWorkMock.Setup(x => x.Categories.Update(It.IsAny<Category>()))
                                                  .Callback<Category>(p => p.Id = mockCategory.Id);

            //Mapper ------------------------------------------------------------------
            var mapperMock = new Mock<IMapper>();

            mapperMock.Setup(x => x.Map<Category>(It.IsAny<CategoryRequestDTO>()))
                                    .Returns(mockCategory);

            mapperMock.Setup(x => x.Map<CategoryResponseDTO>(It.IsAny<Category>()))
                                   .Returns(new CategoryResponseDTO { Id = mockCategory.Id });

            //Logger ------------------------------------------------------------------
            var loggerMock = new Mock<ILogger<CategoryController>>();

            //Controller------------------------------------------------------------------
            var categoryController = new CategoryController(loggerMock.Object, mapperMock.Object, unitOfWorkMock.Object);

            // Act
            var result = await categoryController.CategoryPutAsync(categoryId_Dummie, mockCategoryRequestDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }

        [Fact]
        public async Task CategoryDelete_DeleteWithSucess()
        {
            // Arrange

            Guid categoryId_Dummie = Guid.NewGuid();

            // Crio um Dummie para a recuperadao esperada
            var mockCategory = new Category
            {
                Id = categoryId_Dummie,
                Name = "Category Dumie",
                CreatedBy = "doe joe",
                CreatedOn = DateTime.Now
            };

            // Crio comportamentos virtuais para as dependencias
            // utilizadas no EndPoint CategoryPost
            // Eles vão enganar o endpoint no teste integrado

            //UnitOfWork ------------------------------------------------------------------
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            //O categoria simulado é recuperado
            unitOfWorkMock.Setup(x => x.Categories.Get(It.IsAny<Guid>()))
                                                  .ReturnsAsync(mockCategory);

            //A criacao do categoria é simulada
            unitOfWorkMock.Setup(x => x.Categories.Delete(It.IsAny<Category>()))
                                                .Callback<Category>(p => p.Id = mockCategory.Id);

            //Mapper ------------------------------------------------------------------
            var mapperMock = new Mock<IMapper>();

            //Logger ------------------------------------------------------------------
            var loggerMock = new Mock<ILogger<CategoryController>>();

            //Controller------------------------------------------------------------------
            var categoryController = new CategoryController(loggerMock.Object, mapperMock.Object, unitOfWorkMock.Object);

            // Act
            var result = await categoryController.CategoryDeleteAsync(categoryId_Dummie);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
        }
    }
}