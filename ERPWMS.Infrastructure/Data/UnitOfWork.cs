using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Data
{
    // ✅ FIX: C# 12 Primary Constructor use kiya hai (context ko seedha class ke bahar le liya)
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private IDbContextTransaction? _currentTransaction;
        private bool _disposed;

        public bool HasActiveTransaction => _currentTransaction != null;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
                throw new InvalidOperationException("A transaction is already in progress.");

            _currentTransaction = await context.Database.BeginTransactionAsync(cancellationToken);
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("No transaction in progress.");

            try
            {
                await context.SaveChangesAsync(cancellationToken);
                await _currentTransaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("No transaction in progress.");

            try
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            // ✅ FIX: GC.SuppressFinalize ko hata diya kyunke yahan koi finalizer use nahi ho raha
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _currentTransaction?.Dispose();
                context.Dispose();
            }
            _disposed = true;
        }
    }
}