using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UdemyAuthServer.Core.Models;

namespace UdemyAuthServer.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product> 
    {
        //product dosyasında üzerine [key] vs yazmak yerine böyle yapıyoruz daha temiz kod
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);//primary key olacak alan x yuakrıdaki product a karşılık geliyor
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200); //en fazla 200 karakter olsuın name dedik - isreqquired not null yapar yani boş olmayacak
            builder.Property(x => x.Stock).IsRequired();
            builder.Property(x => x.Price).HasColumnType("decimal(18,2)"); // 100000000000000.00 virgül sonra 2 tane toplam 18
            builder.Property(x => x.UserId).IsRequired();
        
        
        }
    }
}
