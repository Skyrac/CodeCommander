﻿using Backend.Shared.Interfaces;
using MediatR.Pipeline;

namespace Backend.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly IUser _user;

    public LoggingBehaviour(ILogger<TRequest> logger, IUser user)
    {
        _logger = logger;
        _user = user;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _user.Id;


        _logger.LogInformation("{Time} - Request: {Name} {@UserId} {@Request}",
            DateTime.UtcNow, requestName, userId, request);
    }
}
