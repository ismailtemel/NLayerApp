using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLayer.API.Filter;
using NLayer.API.Middlewares;
using NLayer.Core.Repositories;
using NLayer.Core.Services;
using NLayer.Core.UnitOfWorks;
using NLayer.Repository;
using NLayer.Repository.Repositories;
using NLayer.Repository.UnitOfWorks;
using NLayer.Service.Mapping;
using NLayer.Service.Services;
using NLayer.Service.Validation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
// Configuration
// Add services to the container.
//AddFluentValidation(x=>x.RegisterValidatorsFromAssemblyContaining<>); = Bu sat�r�n anlam� ilk �nce validatorlar�n nerde oldu�unu a��k a��k belirtiriz.registerla ba�layan yerin anlam� bana bir class ver ben o class'�n i�ermi� oldu�u assembly'i alay�m diyor class�m�z da productdtovalidator'� verebiliriz.
// A�a��da addcontroller i�inde yapt���m�z i�lem her controller'�n i�ine ayr� ayr� validator attribute'� eklemek istemedi�imiz i�in yapt���m�z bir i�lemdir. Options ile gireriz ve global olarak yeni filterlar ekleriz. Art�k global olarak t�m filter'lar�m�za a�a��daki filter uygulan�r.
// Art�k bizim a�a��daki filter'�m�z araya girecek ve daha fluent validaditon kendi response'sini d�nemden �nce bizim filter'�m�z kendi istedi�imiz response modeli d�ner.
// Default olarak api taraf�nda validation default olarak aktif yani direk olarak bizim fluent validation�n d�nm�� oldu�u model aktif ediliyor. Bizim fluent validation d�nm�� oldu�u modeli pasif hale getirmemiz gerekiyor.Yani api'nin kendisinin default olarak d�nm�� oldu�u modeli kapatmam�z gerekiyor.Diyeceyiz ki apiye sen model d�nme ben model d�nece�im deriz.Biz api'ye sen filter'�n� aktif hale getirme bizim bu i� i�in kendi filter'�m�z var dememiz laz�m.

builder.Services.AddControllers(options => options.Filters.Add(new ValidateFilterAttribute())).AddFluentValidation(x=>x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>());

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // Burda biz api'nin kendi filtresini bask�l�yoruz.
    // A�a��dakinin anlam� invalid filter'� bask�lamak istiyormusun diyor ve bizde true'ya e�itliyerek bask�l�yoruz.
    // Yani filter'� napt�k kendi d�nm�� oldu�u model filtresini napt�k bask�lad�k yani apinin filtresine sen inaktif duruma ge� ��nk� bizim burda kendi filtremiz oldu�undan dolay�.�lk d�nen model yani apinin modeli filtreye g�re d�ner.O filter'� pasif hale getirdik.
    // MVC de apideki gibi bir bask�lama yapmam�za gerek yoktur.��nk� api taraf�nda default olarak bizim yazd���m�z filter aktiftir.MVC de b�yle bir durum yoktur.MVC de filterla beraber sadece yukar�daki kodu yazmam�z yeterli olur.
    options.SuppressModelStateInvalidFilter = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// A�a��daki 1.IGenericRepository birden fazla dinamik alsayd� e�er birden fazla tipi generic alsayd� 2 tane alsayd� 1 virg�l, 2 tane alsayd� 3 virg�l koyard�k.
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));

// Parantez i�ine bizim mapleme k�t�phanemiz neredeyse onu vermemiz gerekiyor.
// MapProfile dan birden fazla olabilir hepsini ayn� yere g�mmemize gerek yoktur profile s�n�fdan miras alm�� t�m mapprofile classlar�n� automapper kendi i� yap�s�na ekler bu y�zden istersek birden fazla olu�turabiliriz.
// E�er bir entityle ilgili �ok faznal d�n��t�rme varsa ona g�re mapprofile leri ay�r�r�z. 
builder.Services.AddAutoMapper(typeof(MapProfile));
builder.Services.AddScoped<IProductRespository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
// 30 tane entity'miz varsa 30 tane entity'i de buraya tan�mlarsak program.cs dosyas�n� kirletiriz.
// �lerde Autofac k�t�phanesiyle birlikte dinamik olarak bu servisleri tek bir sat�r kodla servise eklemi� olaca��z.

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), option =>
    {
        // A�a��daki kodun anlam� AppDbContext'in NLayer.Repository i�erisinde oldu�unu belirtmek
        // A�a��da s�n�f�n bulunmu� oldu�u Assemblyi al�yoruz ve ard�ndan ismini al�yoruz.
        option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);
    });
});

// Burdan itibaren app'le ba�layan ve use ifadesini kullananlar�n hepsi middleware'dir.
// Yanl�z buraya yazarsak buray� kirletiriz.Amac�m�z propgram.cs dosyas�n� olabildi�ince temiz b�rakmak.
// A�a��daki app'in tipi WebApplicaitondur. 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// A�a��daki UseHttpRedirection , e�er http ile ba�layan bir url varsa bunu https' e y�nelndirir.
app.UseHttpsRedirection();

// Olu�turdu�umuz middleware bir hata oldu�u i�in var olan middleware'lardan �stte olmas� �nemli.
// Clienttan kaynakl� bir hata oldu�unda uygulamamn�n herhangi bir yerinde biz bir hata f�rlataca��z bu middleware yakalayacak ve geriye bizim belirlemi� oldu�umuz modeli d�necek.
app.UserCustomException();

// A�a��daki authorization bir request geldi�inde token do�rulamas� burda ger�ekle�tirilir.
app.UseAuthorization();

app.MapControllers();

app.Run();
