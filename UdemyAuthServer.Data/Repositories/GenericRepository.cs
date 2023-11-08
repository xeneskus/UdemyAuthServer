using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UdemyAuthServer.Core.Repositories;

namespace UdemyAuthServer.Data.Repositories
{
    public class GenericRepository<Tentity> : IGenericRepository<Tentity> where Tentity : class //eneric oldugundan doalyı birde tentity veriyoruz classtan aldıgımız tentityi interface imize veiryoruz ardından where ile tentityin bir class olacagını söyledik
    {
        private readonly DbContext _context;
        private readonly DbSet<Tentity> _dbset;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbset = context.Set<Tentity>();//üzerinde çalışacagımız entity verdik
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
                _context.Entry(entity).State = EntityState.Detached;// neden bunu yapıyorum üstteki id in takip edilmesini istemiyoruz memoryde tutulmasını istemiyoruz
            }
            return entity;
        }

        public void Remove(Tentity entity)
        {
            _dbset.Remove(entity);
        }

        public Tentity Update(Tentity entity)
        {
            //
            _context.Entry(entity).State = EntityState.Modified; //aynı olsa dahi tüm alanları günceller
            return entity;
        }

        public IQueryable<Tentity> Where(Expression<Func<Tentity, bool>> predicate)
        {
            return _dbset.Where(predicate);
        }
    }
}
