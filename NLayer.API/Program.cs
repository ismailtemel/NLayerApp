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
//AddFluentValidation(x=>x.RegisterValidatorsFromAssemblyContaining<>); = Bu satýrýn anlamý ilk önce validatorlarýn nerde olduðunu açýk açýk belirtiriz.registerla baþlayan yerin anlamý bana bir class ver ben o class'ýn içermiþ olduðu assembly'i alayým diyor classýmýz da productdtovalidator'ý verebiliriz.
builder.Services.AddControllers().AddFluentValidation(x=>x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Aþaðýdaki 1.IGenericRepository birden fazla dinamik alsaydý eðer birden fazla tipi generic alsaydý 2 tane alsaydý 1 virgül, 2 tane alsaydý 3 virgül koyardýk.
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));

// Parantez içine bizim mapleme kütüphanemiz neredeyse onu vermemiz gerekiyor.
// MapProfile dan birden fazla olabilir hepsini ayný yere gömmemize gerek yoktur profile sýnýfdan miras almýþ tüm mapprofile classlarýný automapper kendi iç yapýsýna ekler bu yüzden istersek birden fazla oluþturabiliriz.
// Eðer bir entityle ilgili çok faznal dönüþtürme varsa ona göre mapprofile leri ayýrýrýz. 
builder.Services.AddAutoMapper(typeof(MapProfile));
builder.Services.AddScoped<IProductRespository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
// 30 tane entity'miz varsa 30 tane entity'i de buraya tanýmlarsak program.cs dosyasýný kirletiriz.
// Ýlerde Autofac kütüphanesiyle birlikte dinamik olarak bu servisleri tek bir satýr kodla servise eklemiþ olacaðýz.

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), option =>
    {
        // Aþaðýdaki kodun anlamý AppDbContext'in NLayer.Repository içerisinde olduðunu belirtmek
        // Aþaðýda sýnýfýn bulunmuþ olduðu Assemblyi alýyoruz ve ardýndan ismini alýyoruz.
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
