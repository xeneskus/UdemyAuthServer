using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using UdemyAuthServer.Core.Configuration;
using UdemyAuthServer.Core.DTOs;
using UdemyAuthServer.Core.Models;
using UdemyAuthServer.Core.Services;

namespace UdemyAuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager; 
        private readonly CustomTokenOptions _tokenOption;

        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOptions> options) 
        {
            _userManager = userManager;
            _tokenOption = options.Value;
        }

        private string CreateRefreshToken() //random string değer yapıyoruz.
        {
            var numberByte = new Byte[32]; //kaç değerlik bir string byte onu belirttik
            using var rnd = RandomNumberGenerator.Create(); //random bir değer üretecek
            rnd.GetBytes(numberByte);// Byte'leri al ve numberByte'a aktar.
            return Convert.ToBase64String(numberByte);// Dönüşüm yaptık, 32 byte'lık rastgele bir byte dizisi üretecek.
        }

        private IEnumerable<Claim> GetClaims(UserApp userApp, List<String> audiences) // UserApp'ı kullanıcı bilgileri için aldık - audience da bu token'ın hangi API'lere istek yapacağını belirtiyor.
        {
            var userList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userApp.Id), //name identifier kimlik idye karşılık geliyor
                new Claim(JwtRegisteredClaimNames.Email, userApp.Email),
                new Claim(ClaimTypes.Name,userApp.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())//jsonu kimliklendirecek identity veriyor random bir değer üretip verdik bunlar kullanıcı ile ilgili claimler hepsi payloada eklenecek
            }; //kullanıcının idsini jwt.io da payload kısmında görmek istiyoruz üyelik sistemiyle ilgili bir tokensa kesin idsi name kısmıda olmalı

            userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x))); //her birine git yeni bir claim oluştur - bir api istek yaptıgımızda bu token audisine bakacak gerçekten uygun mu kontrol edecek degilse geri çekecek aud dan bulacak bunu uygun olup olmadıgını
            return userList;
        
        }//üyelik sistemi gerektiren claimler burada istemediğimde alttaki


        private IEnumerable<Claim> GetClaimByClient(Client client)
        {
            var claims = new List<Claim>();
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x))); //her birine git yeni bir claim oluştur - bir api istek yaptıgımızda bu token audisine bakacak gerçekten uygun mu kontrol edecek degilse geri çekecek aud dan bulacak bunu uygun olup olmadıgını
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());//jsonu kimliklendirecek identity vberiyor random bir değer üretip verdik bunlar kullanıcı ile ilgili claimler hepsi payloada eklenecek random oluşturuyor
            new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString());   
            return claims;
        }

        public TokenDto CreateToken(UserApp userApp)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration); //şu anki tarihi al buna addminutle ilgili kaç dakika ekle bizim tokenoption ile expirationa kaç dk gelirse onu ekleyecek bize var olan saate
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.RefreshTokenExpiration); //şu anki tarihi al buna addminutle ilgili kaç dakika ekle bizim tokenoption ile expirationa kaç dk gelirse onu ekleyecek bize var olan saate
            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey); //tokenı imzalayacak key

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature); //tokeni imzayalcak security key ve imzalayacak algoritma verify signature kısmını belirttik jwt sitesindeki
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,//benim vermiş dolgumu dakikadan itibaren sadece bu aralıkta geçerli olacak
                claims: GetClaims(userApp, _tokenOption.Audience),
                signingCredentials: signingCredentials);

            var handler = new JwtSecurityTokenHandler(); //handler oluşturduk bu arkadaş token oluşturacak

            var token = handler.WriteToken(jwtSecurityToken);//bir token ver diyor oluşturacak bunu
            var tokenDto = new TokenDto
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration
            };
            return tokenDto;
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration); //şu anki tarihi al buna adminutle ilgili kaç dakika ekle bizim tokenoption ile expirationa kaç dk gelirse onu ekleyecek bize var olan saate
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.RefreshTokenExpiration); //şu anki tarihi al buna adminutle ilgili kaç dakika ekle bizim tokenoption ile expirationa kaç dk gelirse onu ekleyecek bize var olan saate
            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);// signservice den gelecek - tokenı imzalayacak key

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature); //tokeni imzayalcak security key ve imzalayacak algoritma verify signature kısmını belirttik jwt sitesindeki
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,//benim vermiş dolgumu dakikadan itibaren sadece bu aralıkta geçerli olacak
                claims: GetClaimByClient(client),
                signingCredentials: signingCredentials); 

            var handler = new JwtSecurityTokenHandler(); //handler oluşturduk bu arkadaş token oluşturacak

            var token = handler.WriteToken(jwtSecurityToken);
            var tokenDto = new ClientTokenDto
            {
                AccessToken = token,
              
                AccessTokenExpiration = accessTokenExpiration,
              
            };
            return tokenDto;
        }
    }
}
