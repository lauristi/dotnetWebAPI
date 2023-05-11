namespace Controller_EF_Dapper_Repository_UnityOfWork.Endpoints.Products.DTO
{
    public class ProductSoldResponseDTO
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public int Amount { get; set; }
    }
}