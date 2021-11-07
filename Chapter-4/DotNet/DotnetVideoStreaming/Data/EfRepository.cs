using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly VideoStreamingContext _context;
        

        public EfRepository(VideoStreamingContext context)
        {
            _context = context;
        }

        private DbSet<TEntity> _entities;
        protected virtual DbSet<TEntity> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<TEntity>();

                return _entities;
            }
        }

        public virtual IQueryable<TEntity> Table => Entities;
        public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();
    }
}