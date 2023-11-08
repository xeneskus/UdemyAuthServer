using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Configurations;
using UdemyAuthServer.Core.Configuration;
using UdemyAuthServer.Core.Models;
using UdemyAuthServer.Core.Repositories;
using UdemyAuthServer.Core.Services;
using UdemyAuthServer.Core.UnitOfWork;
using UdemyAuthServer.Data;
using UdemyAuthServer.Data.Repositories;
using UdemyAuthServer.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IServiceGeneric<,>),typeof(ServiceGeneric<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"), sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("UdemyAuthServer.Data");//migration data katmanýnda istiyoruz
    });
});


builder.Services.AddIdentity<UserApp, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true; //veritabanýnýnda uniq olmasýný istedik emailin
    options.Password.RequireNonAlphanumeric = false; //non alphanemetik yýldýz soru iþareti gibi þeyler olmasýn desin alphanamitk olanlar a dan z olanlar non olan yýldýz vs

}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); //kullanýcým user app rolüm ise ýdentity kütüphanesinden gelen default 
//adddefault token þifre sýfýrlama gibi iþlemlerde token üretiyoruz bu tokený üretmek için defautprovide saðlýyor
builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOption"));
builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));
var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOptions>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // bu þema iki tane ayrý üyelik sistemi olabilir bayiler için ayrý normal kullanýcý için ayrý üyelik olabilir. kullanýcý olarak gir bayi olarak gir bunlara þema diyoruz bizde tek üyelik sistemi var birden fazla olsaydý "Bayi","Kullanýcý" diyerek ayýrabilirdik.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // þemalarý birbirine baðladýk.

}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts => //Jwt bazlý kimlik doðrulama yaptýk API projesi olduðu için diðer türlü cookie bazlý 
{
    opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidIssuer = tokenOptions.Issuer,  //gelen issuer ile tanýmlanan issuer ayný ise doðrulanacak
        ValidAudience = tokenOptions.Audience[0],
        IssuerSigningKey=SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),


        ValidateIssuerSigningKey=true,//imzasýný doðrula
        ValidateAudience=true, //audience ý doðrula
        ValidateIssuer=true, //gerçekten benim gönderdiðim issuer mu kontrol et
        ValidateLifetime=true,//ömrünü kontrol et geçmiþ mi geçerli mi

        ClockSkew=TimeSpan.Zero//bir token a 1 saat zaman verince default olarak fazladan 5dakika zaman veriyor. farklý serverlarýn kurmuþ olduðu token ömürlerini doðrulama esnasýnda 10 saniye 30 saniye fark olacaktýr mutlaka o farký tolera etmek için 5 dakika ekliyor. Biz tam ayný zamanda serverlar çalýþacak dedik. Tek Api varsa yine zeroya çevirmek uygundur.
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
