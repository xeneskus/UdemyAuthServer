using System.Linq.Expressions;

namespace UdemyAuthServer.Core.Repositories 
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);

        Task<IEnumerable<TEntity>> GetAllAsync();

        IQueryable<TEntity> Where(Expression<Func<TEntity,bool>> predicate); //func delegeyi temsil ediyor bir metotu işaret ediyor entityi ve geriye bool dönecek
        //where(X=>x.id>5) buradaki x=> tentitye karşılık geliyor. x.id>5 bool kısmı büyük mü deegil mi
        Task AddAsync(TEntity entity);

        void Remove(TEntity entity);

        TEntity Update(TEntity entity);



    }
}
