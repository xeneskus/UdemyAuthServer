using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;
using System.Linq.Expressions;
using UdemyAuthServer.Core.Repositories;
using UdemyAuthServer.Core.Services;
using UdemyAuthServer.Core.UnitOfWork;

namespace UdemyAuthServer.Service.Services
{
    public class ServiceGeneric<TEntity, TDto> : IServiceGeneric<TEntity, TDto> where TEntity : class where TDto : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<TEntity> _genericRepository;

        public ServiceGeneric(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
        }

        public async Task<Response<TDto>> AddAsync(TDto entity)
        {
            var newEntity = ObjectMapper.Mapper.Map<TEntity>(entity); 
            await _genericRepository.AddAsync(newEntity);
            await _unitOfWork.CommitAsync();

            var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);
            return Response<TDto>.Success(newDto, 200);
        }

        public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
            var products = ObjectMapper.Mapper.Map<List<TDto>>(await _genericRepository.GetAllAsync()); 
            return Response<IEnumerable<TDto>>.Success(products, 200);  //gelen data üzerinde başka işlem yoksa ıenumarable dönebilirsin

        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var product = await _genericRepository.GetByIdAsync(id); 

            if (product==null) 
            {
                return Response<TDto>.Fail("Id not found", 404, true); // yeni response oluşturuyoruz fail ile hata verdik  durum kodu verdik birde 404 hata kodu en son trueda cliente gösterilsin mi demek evet dedik 
            }
            return Response<TDto>.Success(ObjectMapper.Mapper.Map<TDto>(product), 200); //product yukarda entity ama bizden tdto istiyor ondan dönüştürdük
        }

        public async Task<Response<NoDataDto>> Remove(int id)
        {
            var isExistEntity = await _genericRepository.GetByIdAsync(id);
            if (isExistEntity == null)
            {
                return Response<NoDataDto>.Fail("Id not found",404,true);
            }
            _genericRepository.Remove(isExistEntity); //memoryden sildi stateini delete olarak işaretledik
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(204);//data dönmedik geriye - 204 no content bodyde hiç bir data olmayacak nodatadto oldugu için
        }

        public async Task<Response<NoDataDto>> Update(TDto entity,int id) //tdto da alabilirsin id alırsak ama daha iyii
        {
            var isExistEntity = await _genericRepository.GetByIdAsync(id);
            if (isExistEntity ==null)
            {
                return Response<NoDataDto>.Fail("Id not fount", 404, true);
            }
            var updateEntity = ObjectMapper.Mapper.Map<TEntity>(entity);//geybyıdasync detache olarak işaretlenmişti
            _genericRepository.Update(updateEntity);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(204); //client zaten görüyor update edilmiş datayı tekrar dönmeyee gerek yok

        }

        public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            //where(x=>x.id>5) x tentity karşılı kgeliyor id 5den büyük olması bool a karşılık geliyor
            var list = _genericRepository.Where(predicate);
            //list.Take(5); //5tanesini getir dedik
            //list.ToListAsync();//veritabanına yansır
       
            return Response<IEnumerable<TDto>>.Success(ObjectMapper.Mapper.Map<IEnumerable<TDto>>(await list.ToListAsync()),200);

        }
    }
}
