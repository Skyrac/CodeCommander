using AutotradingSignaler.Contracts.Web3.Events;
using AutotradingSignaler.Core.Web;
using BlockchainApi.DTOs.Web3;
using BlockchainApi.Features.Web3.EventProcessors;
using Nethereum.Contracts;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Reactive.Eth.Subscriptions;
using System.Collections.Concurrent;
using System.Threading;

namespace BlockchainApi.HostedServices
{
    public class BlockchainLogProcessor(
        ILogger<BlockchainLogProcessor> _logger,
        IServiceProvider _serviceProvider,
        Web3Service _web3Service)
        : BackgroundService
    {
        private readonly List<UnprocessedLogDto> _unprocessed
            = new List<UnprocessedLogDto>();

        private readonly ConcurrentQueue<UnprocessedLogDto> _unprocessedSwapEvents
            = new ConcurrentQueue<UnprocessedLogDto>();

        private readonly List<(StreamingWebSocketClient, EthLogsObservableSubscription)> _subs
            = new List<(StreamingWebSocketClient, EthLogsObservableSubscription)>();

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await RestartSubs();
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await CheckSubscriptionStatesAndRestartIfRequired();
                    await ProcessLogs();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }

        private async Task ProcessLogs()
        {
            while (_unprocessedSwapEvents.TryDequeue(out var unprocessedEvent))
            {
                _unprocessed.Add(unprocessedEvent);
                if (_unprocessedSwapEvents.Count <= 0 || _unprocessed.Count >= 100)
                {
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();
                        await mediatr.Send(new BlockchainLogHandler.ProcessBlockchainLogsCommand(_unprocessed));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Exception in BackgroundService while ProcessingSwapEvents: {nameof(BlockchainLogProcessor)} - {ex?.InnerException?.Message ?? ex?.Message}");
                    }
                }

            }
        }

        private async Task CheckSubscriptionStatesAndRestartIfRequired()
        {
            if (_subs.Any(sub =>
            {
                return sub.Item1.WebSocketState != System.Net.WebSockets.WebSocketState.Open;
            }))
            {
                await RestartSubs();
            }
        }

        private async Task RestartSubs()
        {
            if (_subs.Any())
            {
                _subs.ForEach(async sub =>
                {
                    try
                    {
                        await sub.Item1.StopAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, ex.Message);
                    }
                });
                await Task.Delay(TimeSpan.FromSeconds(3));
                _subs.Clear();
            }
            foreach (var chain in _web3Service.GetWeb3Instances())
            {
                _subs.Add(await SubscribeToChainLogs(chain.Key));
            }
        }

        private async Task<(StreamingWebSocketClient, EthLogsObservableSubscription)> SubscribeToChainLogs(int chainId)
        {
            // ** SEE THE TransferEventDTO class below **
            var blockchainInfo = _web3Service.GetBlockchainInfoOf(chainId);
            var client = new StreamingWebSocketClient(blockchainInfo.WssUrl);

            //var filterTransfers = Event<SwapEventV2>.GetEventABI().CreateFilterInput();

            var subscription = new EthLogsObservableSubscription(client);
            client.Error += (sender, ex) =>
            {
                _logger.LogError($"Error in Websocket Connection: {ex.Message}");
                client.StopAsync().Wait();
                subscription.UnsubscribeAsync().Wait();
                client.StartAsync().Wait();
                subscription.SubscribeAsync().Wait();

                //subscription.SubscribeAsync(filterTransfers).Wait();
            };
            subscription.GetSubscriptionDataResponsesAsObservable().Subscribe(log =>
            {
                try
                {
                    //decode the log into a typed event log
                    //if (!_watchlist.Any(w => w.Address.Equals(log.Address, StringComparison.OrdinalIgnoreCase)))
                    //{
                    //    return;
                    //}
                    _unprocessedSwapEvents.Enqueue(new UnprocessedLogDto(chainId, log));
                    var decoded = Event<SwapEventV2>.DecodeEvent(log);
                    if (decoded != null)
                    {
                        _logger.LogInformation($"Chain {chainId}: Contract address: " + log.Address + " Log Transfer from:" + decoded.Event.From);
                    }
                    else
                    {
                        // the log may be an event which does not match the event
                        // the name of the function may be the same
                        // but the indexed event parameters may differ which prevents decoding
                        _logger.LogWarning($"Chain {chainId}:Found not standard swap log {log.Address}: {log.TransactionHash}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Chain {chainId}: Log Address: {log.Address}: {log.TransactionHash} is not a standard transfer log: {ex.Message}");
                }
            });

            // open the web socket connection
            await client.StartAsync();
            // begin receiving subscription data
            // data will be received on a background thread
            //await subscription.SubscribeAsync(filterTransfers);
            await subscription.SubscribeAsync();

            //// run for a while
            //await Task.Delay(TimeSpan.FromMinutes(60));

            return (client, subscription);
        }
    }
}
