namespace Controller_EF_Dapper.Endpoints.DTO.Product
{
    public class ProductSoldResponseDTO
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public int Amount { get; set; }
    }
}