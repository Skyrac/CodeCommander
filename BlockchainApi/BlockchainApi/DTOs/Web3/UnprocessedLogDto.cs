using Nethereum.RPC.Eth.DTOs;

namespace BlockchainApi.DTOs.Web3
{
    public record UnprocessedLogDto(int chainId, FilterLog log);
}
