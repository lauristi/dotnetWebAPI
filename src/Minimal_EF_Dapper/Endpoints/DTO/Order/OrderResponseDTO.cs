namespace Minimal_EF_Dapper.Endpoints.DTO.Order
{
    public record OrderResponseDTO(Guid id,
                                   string ClientName,
                                   IEnumerable<OrderProductDTO> Products
                                );

    public record OrderProductDTO(Guid Id, string Name);
}