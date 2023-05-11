namespace Controller_EF_Dapper.Endpoints.DTO.Product
{
    public class ProductRequestDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool Active { get; set; }
        public Guid CategoryId { get; set; }
    }
}