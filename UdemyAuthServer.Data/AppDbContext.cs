using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UdemyAuthServer.Core.Models;

namespace UdemyAuthServer.Data
{
    //identity üyelik tablolar aynı appdbcontextte tutmak istiyoruz ayrı ayrı 2 hem user hem produc için ayrı ayrı tutmaya gerek yok
    //core katmanında 2 tane product user app var birde serrefresh 2 tane entity var bunları identity üyelik sisteminin tablolarının oluşturdugu yerde kullanmak istiyoruz
    public class AppDbContext : IdentityDbContext<UserApp,IdentityRole,string> //rol istiyor identity kütüphanesine ait classı koyduk - primary key için tip ver nasıl tutayım bunları diyor
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) //bunu startupta dolduracaz< dbcontexti - generic olarak bu işlem appdbcontexte yapacagımızı yani bir options ekleycem ama hangi context üzerine appdb context üzerine
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; } //identity ile beraber çok tablo oluşacak kullanıcı ile ilgili otomatik olarak

      
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly); //sen bana bir configuration ver ben tüm bunları classları bulup ben ekleyeceğim diyor
            base.OnModelCreating(builder);
        }



    }
}
