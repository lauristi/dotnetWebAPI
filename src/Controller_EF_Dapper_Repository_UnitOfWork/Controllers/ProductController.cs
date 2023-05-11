using AutoMapper;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Extensions.ErroDetailedExtension;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.UnitOfWork.Interface;
using Controller_EF_Dapper_Repository_UnityOfWork.Endpoints.Products.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Controller_EF_Dapper_Repository_UnityOfWork.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(ILogger<ProductController> logger,
                                 IMapper mapper,
                                 IUnitOfWork unitOfWork)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        //------------------------------------------------------------------------------------
        //EndPoints
        //------------------------------------------------------------------------------------

        [HttpGet, Route("{id:guid}")]
        public async Task<ActionResult<ProductResponseDTO>> ProductGet([FromRoute] Guid id)
        {
            var product = await _unitOfWork.Products.Get(id);

            if (product == null)
                return new ObjectResult(Results.NotFound())
                {
                    StatusCode = StatusCodes.Status404NotFound
                };

            var productResponseDTO = _mapper.Map<ProductResponseDTO>(product);
            return new ObjectResult(productResponseDTO);
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> ProductPost(ProductRequestDTO productRequestDTO)
        {
            //Usuario fixo, mas  poderia vir de um identity
            string user = "doe joe";

            Product product = _mapper.Map<Product>(productRequestDTO);

            //Recupero a categoria de forma sincrona
            Category category = await _unitOfWork.Categories.Get(productRequestDTO.CategoryId);
            product.Category = category;
            //-----------------------------------------
            product.CreatedBy = user;
            product.CreatedOn = DateTime.Now;

            product.Validate();
            if (!product.IsValid)
            {
                return new ObjectResult(Results.ValidationProblem(product.Notifications.ConvertToErrorDetails()))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            else
            {
                await _unitOfWork.Products.Add(product);
                _unitOfWork.Commit();
                var productResponseDTO = _mapper.Map<ProductResponseDTO>(product);

                return new ObjectResult(Results.Created($"/products/{product.Id}", product.Id))
                {
                    StatusCode = StatusCodes.Status201Created
                };
            }
        }

        [HttpPut, Route("{id:guid}")]
        public async Task<IActionResult> ProductPutAsync([FromRoute] Guid id,
                                         ProductRequestDTO productRequestDTO)
        {
            //Usuario fixo, mas  poderia vir de um identity
            string user = "doe joe";

            //Recupero o produto do banco
            var product = await _unitOfWork.Products.Get(id);

            //nao encontrado
            if (product == null)
                return new ObjectResult(Results.NotFound())
                {
                    StatusCode = StatusCodes.Status404NotFound
                }
            ;

            //Recupero a categoria de forma sincrona
            Category category = await _unitOfWork.Categories.Get(productRequestDTO.CategoryId);
            product.Category = category;

            //nao encontrado
            if (category == null) return new ObjectResult(Results.NotFound());

            product.Name = productRequestDTO.Name;
            product.Price = productRequestDTO.Price;
            product.Description = productRequestDTO.Description;
            product.Active = true;
            product.Category = category;
            //-----------------------------------------
            product.EditedBy = user;
            product.EditedOn = DateTime.Now;

            product.Validate();
            if (!product.IsValid)
            {
                return new ObjectResult(Results.ValidationProblem(product.Notifications.ConvertToErrorDetails()))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            else
            {
                _unitOfWork.Products.Update(product);
                _unitOfWork.Commit();

                return new ObjectResult(Results.Ok())
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
        }

        [HttpDelete, Route("{id:guid}")]
        public async Task<IActionResult> ProductDeleteAsync([FromRoute] Guid id)
        {
            //Recupero o produto do banco
            var product = await _unitOfWork.Products.Get(id);

            //nao encontrado
            if (product == null)
                return new ObjectResult(Results.NotFound());

            _unitOfWork.Products.Delete(product);
            _unitOfWork.Commit();

            return new ObjectResult(Results.Ok())
            {
                StatusCode = StatusCodes.Status200OK
            };
        }

        [HttpGet, Route("")]
        public async Task<ActionResult<IEnumerable<ProductSoldResponseDTO>>> ProductGetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAll();

            if (products == null)
                return new ObjectResult(Results.NotFound());

            var productsResponseDTO = _mapper.Map<IEnumerable<ProductResponseDTO>>(products);
            return new ObjectResult(productsResponseDTO);
        }

        [HttpGet, Route("/solds")]
        public async Task<ActionResult<IEnumerable<ProductSoldResponseDTO>>> ProductSoldGetAsync()
        {
            var products = await _unitOfWork.Products.GetSoldProducts();

            if (products == null)
                return new ObjectResult(Results.NotFound());

            var productsSoldResponseDTO = _mapper.Map<IEnumerable<ProductSoldResponseDTO>>(products);

            return new ObjectResult(productsSoldResponseDTO);
        }

        [HttpGet, Route("/genericFind")]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> ProductFind()
        {
            var products = await _unitOfWork.Products.Find(p => p.Name.Contains("IBM"));

            if (products == null)
                return new ObjectResult(Results.NotFound());

            var productsResponseDTO = _mapper.Map<IEnumerable<ProductResponseDTO>>(products);
            return new ObjectResult(productsResponseDTO);
        }
    }
}