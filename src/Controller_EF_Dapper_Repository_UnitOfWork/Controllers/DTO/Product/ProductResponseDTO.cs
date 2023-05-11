namespace Controller_EF_Dapper_Repository_UnityOfWork.Endpoints.Products.DTO
{
    public class ProductResponseDTO
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public string Description { get; set; }
        public Decimal Price { get; set; }
        public bool Active { get; set; }
    }
}