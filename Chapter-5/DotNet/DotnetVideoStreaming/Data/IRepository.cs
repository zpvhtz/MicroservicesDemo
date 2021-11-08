using Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Table { get; }
        IQueryable<TEntity> TableNoTracking { get; }

        Task<TEntity> GetByIdAsync(int id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
    }
}