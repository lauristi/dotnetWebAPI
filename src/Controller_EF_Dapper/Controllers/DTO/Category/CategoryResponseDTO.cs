namespace Controller_EF_Dapper.Endpoints.DTO.Category
{
    public class CategoryResponseDTO
    {
        public CategoryResponseDTO(Guid id, string name, bool active)
        {
            Id = id;
            Name = name;
            Active = active;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}