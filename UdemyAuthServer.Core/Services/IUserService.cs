using SharedLibrary.Dtos;
using UdemyAuthServer.Core.DTOs;

namespace UdemyAuthServer.Core.Services
{
    public interface IUserService //bunun repositorysini oluşturmadık çünkü ıdentity kütüphanesi ile beraber ordan zaten hazır method geliyor ayrıca bir repository katmanına gerek yok 
    {
        //identityden 3 tane şey geliyor user manager role manager rol ekleme rol silme sign manager kullanıcı login olması geliyor identityden hazır bir şekilde geldigi için direkt service yazıyoruz repository yapmıyoruz

        //bizim api ile haberleşecegi için geriye tekrar response dönüyoruz - herhangi bi controllerin ctorunda geçiricez
        Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto);

        Task<Response<UserAppDto>> GetUserByNameAsync(string userName);


    }
}
