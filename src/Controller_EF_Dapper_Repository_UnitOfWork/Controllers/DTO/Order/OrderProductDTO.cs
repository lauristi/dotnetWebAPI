namespace Controller_EF_Dapper_Repository_UnityOfWork.Endpoints.Orders.DTO
{
    public class OrderProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public OrderProductDTO(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}