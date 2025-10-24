using Microsoft.EntityFrameworkCore.Storage;
using solidhardware.storeCore.Domain.IRepositoryContract;
using solidhardware.storeCore.IUnitOfWork;
using solidhardware.storeinfrastraction.Data;
using solidhardware.storeinfrastraction.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeinfrastraction.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
        
    {
        
        private readonly AppDbContext _db;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
        public UnitOfWork(AppDbContext db)
        {
            _db = db;
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return _repositories[typeof(T)] as IGenericRepository<T>;
            }
            var repository = new GenricRepository<T>(_db);
            _repositories.Add(typeof(T), repository);

            return repository;
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync() => await _db.Database.BeginTransactionAsync();
        public async Task CommitTransactionAsync() => await _db.Database.CommitTransactionAsync();
        public async Task RollbackTransactionAsync() => await _db.Database.RollbackTransactionAsync();
        public async Task<int> CompleteAsync() => await _db.SaveChangesAsync();

        public void Dispose()
        {
            _db.Dispose();
        }


    }
}
