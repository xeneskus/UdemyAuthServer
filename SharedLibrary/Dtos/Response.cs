using System.Text.Json.Serialization;

namespace SharedLibrary.Dtos
{
    public class Response<T> where T : class 
    {
        public T Data { get; private set; }
        public int StatusCode { get; private set; }

        [JsonIgnore] //json dataya dönüştügünde ignore edilecek yok sayılacak kendi içimde kullanıcam
        public bool IsSuccessful { get; private set; } //clinte açmadık bunu görmesini istemedik kendi iç yapımızda kullanııcaz apilerde kısa yoldan statustan kontrol etmek yerine buradan kontrol edicez 
        public ErrorDto Error { get; private set; } //birden fazla hata tutabilir list olarak çünkü dtoda öyle yaptık

        public static Response<T> Success(T data, int statusCode)
        {
            return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessful = true };// gelen statuscodu verdik ve datayı
        } 

        //birde başarılı oldugunda istersek data göndermeyiz meselea ürün silmek istedi idsi 5olan geriye herhangi bir şey dönmeye biliriz datayı herhangi bir şey dönmeden boş yapabiliriz
        public static Response<T> Success(int statusCode)
        {
            return new Response<T> { Data = default, StatusCode = statusCode, IsSuccessful = true };//data boş olacagını default keywordu ile verdik
        }
        //ürünü update ettigimizde veya sildigimizde herhangi bir güncelleme veya bir şey yyaptıgımızda bu dataları geriye dönmeye gerek yok 200 koduyla boş data dönün ama ürün ekleme veya herhangi bir add metodunda dönmek gerekiyor sadece

        public static Response<T> Fail(ErrorDto errorDto, int statusCode)
        {
            return new Response<T>
            {
                Error = errorDto,
                StatusCode = statusCode,
                IsSuccessful = false
            };//yeni bir response dön
        }
        public static Response<T> Fail(string errorMessage, int statusCode, bool isShow)
        {
            var errorDto = new ErrorDto(errorMessage, isShow);
            return new Response<T> { Error=errorDto, StatusCode = statusCode, IsSuccessful = false };
        }

    }
}
