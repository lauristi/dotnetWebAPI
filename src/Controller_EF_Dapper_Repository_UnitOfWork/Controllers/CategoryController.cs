using AutoMapper;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Extensions.ErroDetailedExtension;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.UnitOfWork.Interface;
using Controller_EF_Dapper_Repository_UnityOfWork.Endpoints.Categories.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Controller_EF_Dapper_Repository_UnityOfWork.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryController(ILogger<CategoryController> logger,
                                  IMapper mapper,
                                  IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        //------------------------------------------------------------------------------------
        //EndPoints
        //------------------------------------------------------------------------------------

        [HttpGet, Route("{id:guid}")]
        public async Task<ActionResult<CategoryResponseDTO>> CategoryGet([FromRoute] Guid id)
        {
            var category = await _unitOfWork.Categories.Get(id);

            if (category == null)
                return new ObjectResult(Results.NotFound());

            var categoryResponseDTO = _mapper.Map<CategoryResponseDTO>(category);
            return new ObjectResult(categoryResponseDTO);
        }

        [HttpGet, Route("")]
        public async Task<ActionResult<IEnumerable<CategoryResponseDTO>>> CategorysGetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAll();

            if (categories == null)
                return new ObjectResult(Results.NotFound());

            var categoriesResponseDTO = _mapper.Map<IEnumerable<CategoryResponseDTO>>(categories);
            return new ObjectResult(categoriesResponseDTO);
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> CategoryPost(CategoryRequestDTO categoryRequestDTO)
        {
            //Usuario fixo, mas  poderia vir de um identity
            string user = "doe joe";

            Category category = _mapper.Map<Category>(categoryRequestDTO);

            category.CreatedBy = user;
            category.CreatedOn = DateTime.Now;

            category.Validate();
            if (!category.IsValid)
            {
                return new ObjectResult(Results.ValidationProblem(category.Notifications.ConvertToErrorDetails()))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            else
            {
                await _unitOfWork.Categories.Add(category);
                _unitOfWork.Commit();

                var categoryResponseDTO = _mapper.Map<CategoryResponseDTO>(category);

                return new ObjectResult(Results.Created($"/categories/{category.Id}", category.Id))
                {
                    StatusCode = StatusCodes.Status201Created
                };
            }
        }

        [HttpPut, Route("{id:guid}")]
        public async Task<IActionResult> CategoryPutAsync([FromRoute] Guid id,
                                         CategoryRequestDTO categoryRequestDTO)
        {
            //Usuario fixo, mas  poderia vir de um identity
            string user = "doe joe";

            //Recupero a categoria de forma sincrona
            Category category = await _unitOfWork.Categories.Get(id);

            //nao encontrado
            if (category == null)
                return new ObjectResult(Results.NotFound())
                {
                    StatusCode = StatusCodes.Status404NotFound
                };

            category.Name = categoryRequestDTO.Name;
            category.Active = true;
            //-----------------------------------------
            category.EditedBy = user;
            category.EditedOn = DateTime.Now;

            category.Validate();
            if (!category.IsValid)
            {
                return new ObjectResult(Results.ValidationProblem(category.Notifications.ConvertToErrorDetails()))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            else
            {
                _unitOfWork.Categories.Update(category);
                _unitOfWork.Commit();

                return new ObjectResult(Results.Ok())
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
        }

        [HttpDelete, Route("{id:guid}")]
        public async Task<IActionResult> CategoryDeleteAsync([FromRoute] Guid id)
        {
            //Recupero a categoria
            var category = await _unitOfWork.Categories.Get(id);

            if (category == null)
                return new ObjectResult(Results.NotFound());

            //nao encontrado
            if (category == null) return new ObjectResult(Results.NotFound());

            _unitOfWork.Categories.Delete(category);
            _unitOfWork.Commit();

            return new ObjectResult(Results.Ok())
            {
                StatusCode = StatusCodes.Status200OK
            };
        }
    }
}