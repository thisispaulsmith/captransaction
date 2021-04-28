using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace CapTrans
{
    public class MyDbContext : DbContext
    {
        private IDbContextTransaction _currentTransaction;

        private ICapPublisher _publisher;

        public MyDbContext(DbContextOptions<MyDbContext> options, ICapPublisher publisher)
            : base(options)
        {
            _publisher = publisher;
        }

        public bool HasActiveTransaction => _currentTransaction != null;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                return null;
            }

            _currentTransaction = await Database.BeginTransactionAsync();

            // The below isn't ideal as it only persists the tranaction on the current thread
            // and copies it to nested call. Causing all sorted of issues with the transaction
            //_currentTransaction = Database.BeginTransaction(_publisher, false)

            return _currentTransaction;
        }

        public IDbContextTransaction BeginTransaction()
        {
            if (_currentTransaction != null)
            {
                return null;
            }

            _currentTransaction = Database.BeginTransaction(_publisher, autoCommit: false);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            if (transaction != _currentTransaction)
            {
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");
            }

            try
            {
                await SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public DbSet<Person> Persons { get; set; }
    }
}
