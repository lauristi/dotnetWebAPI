namespace Controller_EF_Dapper.Endpoints.DTO.Product
{
    public class ProductResponseDTO
    {
        public ProductResponseDTO(Guid id, string name, string description, decimal price, bool active)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Active = active;
        }

        public Guid Id { get; set; }
        public String Name { get; set; }
        public string Description { get; set; }
        public Decimal Price { get; set; }
        public bool Active { get; set; }
    }
}