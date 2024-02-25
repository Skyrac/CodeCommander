using Backend.Database;
using Backend.Shared.Models;
using System.Transactions;

namespace Backend.Behaviours;

public class TransactionScopeBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ApplicationDbContext _dbContext;

    public TransactionScopeBehaviour(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!IsCommand(request))
        {
            return await next();
        }
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var response = await next();

        await _dbContext.SaveChangesAsync(cancellationToken);
        transactionScope.Complete();
        return response;
    }

    private bool IsCommand(TRequest request)
    {
        return request.GetType().IsAssignableTo(typeof(IBaseCommand));
    }
}
