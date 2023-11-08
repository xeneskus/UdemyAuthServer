using SharedLibrary.Dtos;
using UdemyAuthServer.Core.DTOs;

namespace UdemyAuthServer.Core.Services
{
    public interface IAuthenticationService //kullanıcının login işlemleri kullanıcı şif kullanıcı ad giricek dogruysa jwt dönücez
    {
        Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto); //eger bu logindto nesnesi doğruysa geriye bir token dönecek

        Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken);

        Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken); // refresh tokeni null a set et dedik - refreshtoken çalınırsa sürekli yeni token alabilir önüne geçmek için bunu kullanabiliriz
   
        Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto); // gelen clienttoekndto içindekei parametreler appseting dosyasında da varsa geriye token dönücez - client token dönücez refresh değil çünkü gerek yok
    
    }
}
