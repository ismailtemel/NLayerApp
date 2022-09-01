using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddControllers().AddFluentValidation(x=>x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>());
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
