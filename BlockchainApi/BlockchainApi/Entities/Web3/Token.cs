using Backend.Shared.Entities;

namespace BlockchainApi.Entities.Web3;

public class Token : AuditableEntity
{
    public string Address { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public int Decimals { get; set; }
    public int ChainId { get; set; }
    public double Price { get; set; } = 0;
    public string? LogoURI { get; set; }
}
