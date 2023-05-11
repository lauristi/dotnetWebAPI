namespace Controller_EF_Dapper.Endpoints.DTO.Order
{
    public record OrderRequestDTO(
        List<Guid> ProductListIds
    );
}