using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace UdemyAuthServer.Service.Services
{
    public static class SignService
    {// a     b  iki arkadaş birbirine güvenli yoldan şifre yollamak istiyor public ve private key var birbirlerine puublic key yolluyorlar bunu şifreleyip b de bu imzalanmış dosyayı private key ile açıyor

        public static SecurityKey GetSymmetricSecurityKey(string securityKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)); //bir tane nesne örnegi oluşturduk içerisinde byte attık bu bize securtiy dönecek

        }
    }
}
