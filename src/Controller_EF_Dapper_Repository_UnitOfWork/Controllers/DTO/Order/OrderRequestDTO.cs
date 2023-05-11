namespace Controller_EF_Dapper_Repository_UnityOfWork.Endpoints.Orders.DTO
{
    public class OrderRequestDTO
    {
        public List<Guid> ProductsId { get; set; }

        public OrderRequestDTO(List<Guid> productsId)
        {
            ProductsId = productsId;
        }
    }
}