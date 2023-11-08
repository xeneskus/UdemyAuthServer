using Microsoft.AspNetCore.Identity;

namespace UdemyAuthServer.Core.Models
{
    public class UserApp : IdentityUser  //içinde default username email fail olma durumu erişme gibi birçok hazır prop geliyor migration yapınca sütun olarak geliyorlar
    { //
        public string? City { get; set; }  
        
    }
}
