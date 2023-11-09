using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UdemyAuthServer.Core.Repositories;

namespace UdemyAuthServer.Data.Repositories
{
    public class GenericRepository<Tentity> : IGenericRepository<Tentity> where Tentity : class 
    {
        private readonly DbContext _context;
        private readonly DbSet<Tentity> _dbset;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbset = context.Set<Tentity>();
        }

        public async Task AddAsync(Tentity entity)
        {
            await _dbset.AddAsync(entity);
        }

        public async Task<IEnumerable<Tentity>> GetAllAsync()
        {
           return await _dbset.ToListAsync();
        }

        public async Task<Tentity> GetByIdAsync(int id)
        {
            var entity = await _dbset.FindAsync(id);
            if (entity!=null)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }
            return entity;
        }

        public void Remove(Tentity entity)
        {
            _dbset.Remove(entity);
        }

        public Tentity Update(Tentity entity)
        {
            _context.Entry(entity).State = EntityState.Modified; 
            return entity;
        }

        public IQueryable<Tentity> Where(Expression<Func<Tentity, bool>> predicate)
        {
            return _dbset.Where(predicate);
        }
    }
}
