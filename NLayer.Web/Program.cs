using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using NLayer.Repository;
using NLayer.Service.Mapping;
using NLayer.Service.Validation;
using NLayer.Web;
using NLayer.Web.Modules;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
//Yukar�dakinin ard�ndan bir mod�l ekleyece�iz.Bu mod�l�m�z i�erisinde dinamik olarak napaca��m�z� ekleyece�iz.
// A�a��daki gibi modul class'�m�z� aktif edebiliriz. 
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new RepoServiceModul()));

builder.Services.AddControllersWithViews().AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>());

builder.Services.AddAutoMapper(typeof(MapProfile));



builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), option =>
    {
        // A�a��daki kodun anlam� AppDbContext'in NLayer.Repository i�erisinde oldu�unu belirtmek
        // A�a��da s�n�f�n bulunmu� oldu�u Assemblyi al�yoruz ve ard�ndan ismini al�yoruz.
        option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);
    });
});


builder.Services.AddScoped(typeof(NotFoundFilter<>));



var app = builder.Build();

// Burda bir UseExceptionHandler middlerware'i var fakat bu sadece development olmad��� esnada �al���yor.Yani biz �imdi uygulamay� aya�a kald�rd���m�zdaik olay development olay�d�r.O zaman UseExceptionHandler'� d��ar�ya almal�y�z ki bizi error sayfas�na y�nlendirdi�ini ba�ar�l� bir �ekilde g�relim.
// Development esnas�nda error sayfas�na y�nlenmesin bize hatay� deminki gibi direk g�stersin ��nk� biz daha geli�tirme a�amas�nday�z.Hatay� direk g�rmeliyiz.Canl�ya ��kt���m�zda environmentlerde a�a��daki hata sayfas�na gitsin.Biz g�rebilmek i�in if bloklar�n�n d���na ��kard�k.Normalde if bloklar� i�inde olmas� daha uygun.
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
