using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLayer.API.Filter;
using NLayer.API.Filters;
using NLayer.API.Middlewares;
using NLayer.API.Modules;
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
// Aþaðýda addcontroller içinde yaptýðýmýz iþlem her controller'ýn içine ayrý ayrý validator attribute'ü eklemek istemediðimiz için yaptýðýmýz bir iþlemdir. Options ile gireriz ve global olarak yeni filterlar ekleriz. Artýk global olarak tüm filter'larýmýza aþaðýdaki filter uygulanýr.
// Artýk bizim aþaðýdaki filter'ýmýz araya girecek ve daha fluent validaditon kendi response'sini dönemden önce bizim filter'ýmýz kendi istediðimiz response modeli döner.
// Default olarak api tarafýnda validation default olarak aktif yani direk olarak bizim fluent validationýn dönmüþ olduðu model aktif ediliyor. Bizim fluent validation dönmüþ olduðu modeli pasif hale getirmemiz gerekiyor.Yani api'nin kendisinin default olarak dönmüþ olduðu modeli kapatmamýz gerekiyor.Diyeceyiz ki apiye sen model dönme ben model döneceðim deriz.Biz api'ye sen filter'ýný aktif hale getirme bizim bu iþ için kendi filter'ýmýz var dememiz lazým.

builder.Services.AddControllers(options => options.Filters.Add(new ValidateFilterAttribute())).AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>());

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // Burda biz api'nin kendi filtresini baskýlýyoruz.
    // Aþaðýdakinin anlamý invalid filter'ý baskýlamak istiyormusun diyor ve bizde true'ya eþitliyerek baskýlýyoruz.
    // Yani filter'ý naptýk kendi dönmüþ olduðu model filtresini naptýk baskýladýk yani apinin filtresine sen inaktif duruma geç çünkü bizim burda kendi filtremiz olduðundan dolayý.Ýlk dönen model yani apinin modeli filtreye göre döner.O filter'ý pasif hale getirdik.
    // MVC de apideki gibi bir baskýlama yapmamýza gerek yoktur.Çünkü api tarafýnda default olarak bizim yazdýðýmýz filter aktiftir.MVC de böyle bir durum yoktur.MVC de filterla beraber sadece yukarýdaki kodu yazmamýz yeterli olur.
    options.SuppressModelStateInvalidFilter = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();



// Generic olduðu için typeof ile içine gireriz.Generic olduðu için oklarý açtýk kapattýk.
// Filterlarýn burda durmsýný istemesek eðer bu filterlar için bir filter modul yaparýz ve bu sefer filter modulleri ekleriz.Nasýl ekleyeceðimizi autofac'in internet sayfasýndan öðrenebiliriz.
builder.Services.AddScoped(typeof(NotFoundFilter<>));


//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Aþaðýdaki 1.IGenericRepository birden fazla dinamik alsaydý eðer birden fazla tipi generic alsaydý 2 tane alsaydý 1 virgül, 2 tane alsaydý 3 virgül koyardýk.
//builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));

// Parantez içine bizim mapleme kütüphanemiz neredeyse onu vermemiz gerekiyor.
// MapProfile dan birden fazla olabilir hepsini ayný yere gömmemize gerek yoktur profile sýnýfdan miras almýþ tüm mapprofile classlarýný automapper kendi iç yapýsýna ekler bu yüzden istersek birden fazla oluþturabiliriz.
// Eðer bir entityle ilgili çok faznal dönüþtürme varsa ona göre mapprofile leri ayýrýrýz. 
builder.Services.AddAutoMapper(typeof(MapProfile));
//builder.Services.AddScoped<IProductRespository, ProductRepository>();
//builder.Services.AddScoped<IProductService, ProductService>();
// 30 tane entity'miz varsa 30 tane entity'i de buraya tanýmlarsak program.cs dosyasýný kirletiriz.
// Ýlerde Autofac kütüphanesiyle birlikte dinamik olarak bu servisleri tek bir satýr kodla servise eklemiþ olacaðýz.

//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
//builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), option =>
    {
        // Aþaðýdaki kodun anlamý AppDbContext'in NLayer.Repository içerisinde olduðunu belirtmek
        // Aþaðýda sýnýfýn bulunmuþ olduðu Assemblyi alýyoruz ve ardýndan ismini alýyoruz.
        option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);
    });
});


// Burada UseServiceProviderFactory methodumuza AutofacServiceProviderFactory'ýmýzý yani kütüphaneden gelen sýnýfýmýzý ekliyoruz.
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
//Yukarýdakinin ardýndan bir modül ekleyeceðiz.Bu modülümüz içerisinde dinamik olarak napacaðýmýzý ekleyeceðiz.
// Aþaðýdaki gibi modul class'ýmýzý aktif edebiliriz. 
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new RepoServiceModul()));

// Burdan itibaren app'le baþlayan ve use ifadesini kullananlarýn hepsi middleware'dir.
// Yanlýz buraya yazarsak burayý kirletiriz.Amacýmýz program.cs dosyasýný olabildiðince temiz býrakmak.
// Aþaðýdaki app'in tipi WebApplicaitondur. 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aþaðýdaki UseHttpRedirection , eðer http ile baþlayan bir url varsa bunu https' e yönelndirir.
app.UseHttpsRedirection();

// Oluþturduðumuz middleware bir hata olduðu için var olan middleware'lardan üstte olmasý önemli.
// Clienttan kaynaklý bir hata olduðunda uygulamamnýn herhangi bir yerinde biz bir hata fýrlatacaðýz bu middleware yakalayacak ve geriye bizim belirlemiþ olduðumuz modeli dönecek.
app.UseCustomException();

// Aþaðýdaki authorization bir request geldiðinde token doðrulamasý burda gerçekleþtirilir.
app.UseAuthorization();

app.MapControllers();

app.Run();
