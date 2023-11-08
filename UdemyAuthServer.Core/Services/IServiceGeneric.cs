using SharedLibrary.Dtos;
using System.Linq.Expressions;

namespace UdemyAuthServer.Core.Services
{
    public interface IServiceGeneric<TEntity, TDto> where TEntity : class where TDto:class
    {
        Task<Response<TDto>> GetByIdAsync(int id); 

        Task<Response<IEnumerable<TDto>>> GetAllAsync();

        Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate); //func delegeyi temsil ediyor bir metotu işaret ediyor entityi ve geriye bool dönecek
        //where(X=>x.id>5) buradaki x=> tentitye karşılık geliyor. x.id>5 bool kısmı büyük mü deegil mi
        Task<Response<TDto>> AddAsync(TDto entity);

        Task<Response<NoDataDto>> Remove(int id);

        Task<Response<NoDataDto>> Update(TDto entity, int id);


    }
}
