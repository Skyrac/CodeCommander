using Backend.Database;
using Backend.Shared.Models;
using BlockchainApi.DTOs.Web3;

namespace BlockchainApi.Features.Web3.EventProcessors
{
    public static class BlockchainLogHandler
    {
        public record ProcessBlockchainLogsCommand(IEnumerable<UnprocessedLogDto> logs) : ICommand;

        public class ProcessBlockchainLogsCommandHandler(
            ILogger<ProcessBlockchainLogsCommandHandler> _logger,
            ApplicationDbContext _dbContext)
            : IRequestHandler<ProcessBlockchainLogsCommand>
        {
            public Task Handle(ProcessBlockchainLogsCommand request, CancellationToken cancellationToken)
            {
                //1. Store Unprocessed Logs to database
                //2. Extract all important information
                //3. Store all important information
                return Task.CompletedTask;
            }
        }
    }
}
