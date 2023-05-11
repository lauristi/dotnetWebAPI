namespace Controller_EF_Dapper_Repository_UnityOfWork.Endpoints.Products.DTO
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