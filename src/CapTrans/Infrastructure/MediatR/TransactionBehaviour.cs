using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CapTrans
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly MyDbContext _dbContext;

        public TransactionBehaviour(MyDbContext context, ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
        {
            _dbContext = context ?? throw new ArgumentException(nameof(MyDbContext));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);

            try
            {
                if (_dbContext.HasActiveTransaction)
                {
                    return await next();
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = await _dbContext.BeginTransactionAsync())
                    {
                        response = await next();

                        await _dbContext.CommitTransactionAsync(transaction);
                    }
                });

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
