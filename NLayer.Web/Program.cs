using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using NLayer.Repository;
using NLayer.Service.Mapping;
using NLayer.Service.Validation;
using NLayer.Web;
using NLayer.Web.Modules;
using NLayer.Web.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
//Yukarýdakinin ardýndan bir modül ekleyeceðiz.Bu modülümüz içerisinde dinamik olarak napacaðýmýzý ekleyeceðiz.
// Aþaðýdaki gibi modul class'ýmýzý aktif edebiliriz. 
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new RepoServiceModul()));

builder.Services.AddControllersWithViews().AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>());

builder.Services.AddAutoMapper(typeof(MapProfile));



builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), option =>
    {
        // Aþaðýdaki kodun anlamý AppDbContext'in NLayer.Repository içerisinde olduðunu belirtmek
        // Aþaðýda sýnýfýn bulunmuþ olduðu Assemblyi alýyoruz ve ardýndan ismini alýyoruz.
        option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);
    });
});

// Biz aþaðýdaki httpclient'ý productapiservice ve categoryapiservice için kullanýyorduk.Aþaðýda productapiservice'i kullanacaðýz. Arkasýndan options ile beraber base'ini vereceðiz.
// Httpclient ile generic bir sýnýf verdiðimizde bu þu anlama gelir artýk gidip mvc sýnýfý içerisindeki service içindeki classlarýn constructoruna yazýp direk olarak kullanabiliriz.
builder.Services.AddHttpClient<ProductApiService>(opt =>
{
    // Burda options üzerinden baseurl bu deðeri builder üzerinden configuration diyoruz ve baseurl'i okuyoruz.
    opt.BaseAddress = new Uri(builder.Configuration["BaseUrl"]);
});

// Yukarýdaki iþlemin aynsýný categoryapiservice için de yaptýk.
builder.Services.AddHttpClient<CategoryApiService>(opt =>
{
    // Burda options üzerinden baseurl bu deðeri builder üzerinden configuration diyoruz ve baseurl'i okuyoruz.
    opt.BaseAddress = new Uri(builder.Configuration["BaseUrl"]);
});


builder.Services.AddScoped(typeof(NotFoundFilter<>));



var app = builder.Build();

// Burda bir UseExceptionHandler middlerware'i var fakat bu sadece development olmadýðý esnada çalýþýyor.Yani biz þimdi uygulamayý ayaða kaldýrdýðýmýzdaik olay development olayýdýr.O zaman UseExceptionHandler'ý dýþarýya almalýyýz ki bizi error sayfasýna yönlendirdiðini baþarýlý bir þekilde görelim.
// Development esnasýnda error sayfasýna yönlenmesin bize hatayý deminki gibi direk göstersin çünkü biz daha geliþtirme aþamasýndayýz.Hatayý direk görmeliyiz.Canlýya çýktýðýmýzda environmentlerde aþaðýdaki hata sayfasýna gitsin.Biz görebilmek için if bloklarýnýn dýþýna çýkardýk.Normalde if bloklarý içinde olmasý daha uygun.
app.UseExceptionHandler("/Home/Error");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
