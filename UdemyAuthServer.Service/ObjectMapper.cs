using AutoMapper;

namespace UdemyAuthServer.Service
{
    public static class ObjectMapper
    {
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DtoMapper>();
            });
            return config.CreateMapper();

        });// Lazyloading sadece ihtiyaç oldugu anda yüklemeye yarıyor. Biz ne zaman çağırırsak

        public static IMapper Mapper => lazy.Value; // Çağırmak için kullandığımız yerde prop olarak tanımladık. Burada sadece veri alacak şekilde yapılandırdık, set işlemi yok.
      // Bu mapper çağrıldığında yukarıdaki kod belleğe bir kere yüklenecek ve kullanılmaya devam edilecek.

    }
}
