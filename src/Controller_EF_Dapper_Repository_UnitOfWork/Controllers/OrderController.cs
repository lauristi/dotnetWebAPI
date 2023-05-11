using AutoMapper;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Extensions.ErroDetailedExtension;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.UnitOfWork.Interface;
using Controller_EF_Dapper_Repository_UnityOfWork.Endpoints.Orders.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Controller_EF_Dapper_Repository_UnityOfWork.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderController(ILogger<OrderController> logger,
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
        public async Task<ActionResult<OrderDetailedResponseDTO>> OrderGetAsync([FromRoute] Guid id)
        {
            Order order = await _unitOfWork.Orders.Get(id);

            if (order == null)
                return new ObjectResult(Results.NotFound());

            var orderDetailed = await _unitOfWork.Orders.GetDetailedOrder(order);

            var orderDetailedResponseDTO = _mapper.Map<OrderDetailedResponseDTO>(orderDetailed);

            return new ObjectResult(orderDetailedResponseDTO);
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> OrderPost(OrderRequestDTO orderRequestDTO)
        {
            List<Product> orderProducts = new List<Product>();

            //Recupero os produtos do banco para garantir consistencia dos dados
            if (orderRequestDTO.ProductsId.Any())
                orderProducts = await _unitOfWork.Products.Find(p => orderRequestDTO.ProductsId
                                                                                    .Contains(p.Id));

            if (orderProducts == null || orderProducts.Count == 0)
                return new ObjectResult(Results.NotFound())
                {
                    StatusCode = StatusCodes.Status404NotFound
                };

            //Total gasto
            decimal total = 0;
            foreach (var product in orderProducts)
            {
                total += product.Price;
            }

            var order = new Order();

            order.ClientId = "cod-1345";
            order.ClientName = "Doe joe";
            order.Products = orderProducts;
            order.Total = total;
            //--------------------------------------------
            order.CreatedBy = "Neil Armstrong";
            order.CreatedOn = DateTime.Now;

            order.Validate();
            if (!order.IsValid)
            {
                return new ObjectResult(Results.ValidationProblem(order.Notifications.ConvertToErrorDetails()))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            else
            {
                await _unitOfWork.Orders.Add(order);
                _unitOfWork.Commit();

                return new ObjectResult(Results.Created($"/orders/{order.Id}", order.Id))
                {
                    StatusCode = StatusCodes.Status201Created
                };
            }
        }
    }
}