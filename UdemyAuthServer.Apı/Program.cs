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
        sqlOptions.MigrationsAssembly("UdemyAuthServer.Data");//migration data katman�nda istiyoruz
    });
});


builder.Services.AddIdentity<UserApp, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true; //veritaban�n�nda uniq olmas�n� istedik emailin
    options.Password.RequireNonAlphanumeric = false; //non alphanemetik y�ld�z soru i�areti gibi �eyler olmas�n desin alphanamitk olanlar a dan z olanlar non olan y�ld�z vs

}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); //kullan�c�m user app rol�m ise �dentity k�t�phanesinden gelen default 
//adddefault token �ifre s�f�rlama gibi i�lemlerde token �retiyoruz bu token� �retmek i�in defautprovide sa�l�yor
builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOption"));
builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));
var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOptions>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // bu �ema iki tane ayr� �yelik sistemi olabilir bayiler i�in ayr� normal kullan�c� i�in ayr� �yelik olabilir. kullan�c� olarak gir bayi olarak gir bunlara �ema diyoruz bizde tek �yelik sistemi var birden fazla olsayd� "Bayi","Kullan�c�" diyerek ay�rabilirdik.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // �emalar� birbirine ba�lad�k.

}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts => //Jwt bazl� kimlik do�rulama yapt�k API projesi oldu�u i�in di�er t�rl� cookie bazl� 
{
    opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidIssuer = tokenOptions.Issuer,  //gelen issuer ile tan�mlanan issuer ayn� ise do�rulanacak
        ValidAudience = tokenOptions.Audience[0],
        IssuerSigningKey=SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),


        ValidateIssuerSigningKey=true,//imzas�n� do�rula
        ValidateAudience=true, //audience � do�rula
        ValidateIssuer=true, //ger�ekten benim g�nderdi�im issuer mu kontrol et
        ValidateLifetime=true,//�mr�n� kontrol et ge�mi� mi ge�erli mi

        ClockSkew=TimeSpan.Zero//bir token a 1 saat zaman verince default olarak fazladan 5dakika zaman veriyor. farkl� serverlar�n kurmu� oldu�u token �m�rlerini do�rulama esnas�nda 10 saniye 30 saniye fark olacakt�r mutlaka o fark� tolera etmek i�in 5 dakika ekliyor. Biz tam ayn� zamanda serverlar �al��acak dedik. Tek Api varsa yine zeroya �evirmek uygundur.
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
