namespace Controller_EF_Dapper_Repository_UnityOfWork.Endpoints.Orders.DTO
{
    public class OrderDetailedResponseDTO
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public IEnumerable<OrderProductDTO> Products { get; set; }

        public OrderDetailedResponseDTO(string clientId, string clientName, IEnumerable<OrderProductDTO> products)
        {
            ClientId = clientId;
            ClientName = clientName;
            Products = products;
        }
    }
}