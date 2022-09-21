using Autofac;
using NLayer.Caching;
using NLayer.Core.Repositories;
using NLayer.Core.Services;
using NLayer.Core.UnitOfWorks;
using NLayer.Repository;
using NLayer.Repository.Repositories;
using NLayer.Repository.UnitOfWorks;
using NLayer.Service.Mapping;
using NLayer.Service.Services;
using System.Reflection;
using Module = Autofac.Module;
namespace NLayer.API.Modules
{
    // Burda yazdığımız modulumuzu program.cs dosyasına yazmalıyız.
    // Aşağıda Module sınıfından miras aldık.Module sınıfıda AutoFac kütüphanesinin bir elemanıdır.
    public class RepoServiceModul : Module
    {
        // Aşağıda override işlemi gerçekleştiririz.
        protected override void Load(ContainerBuilder builder)
        {
            // Program.cs de önce interface sonra class'ı tanımlıyorduk.Autofac'de önce class sonra interface tanımlarız.As ile beraber interface'ini yazarız.Bu birincisiydi.Bunun aynısının birde genericservice için de vardır.
            // Aşağıdakiler generic olduğu için bu şekilde ekledik.
            builder.RegisterGeneric(typeof(GenericRepository<>)).As(typeof(IGenericRepository<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Service<>)).As(typeof(IService<>)).InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();

            // Autofac assemblyleri tarar ve istediğimiz tüm interface ve bu interface gelen classları dinamik olarak ekleyecek.
            // Aşağıda ilk önce apiAssembly'yi alırız. Yani çalışmış olduğumuz assembly zaten bu class'ın bulunmuş olduğu assembly api olduğu için üzerinde çalıştığın assembly'i al diyoruz.
            var apiAssembly = Assembly.GetExecutingAssembly();

            var repoAssembly = Assembly.GetAssembly(typeof(AppDbContext));

            var serviceAssembly = Assembly.GetAssembly(typeof(MapProfile));

            // Aşağıda builder üzerinden registerAssembly diyoruz ve yukarda tanımladığımız assemblyleri bu methodun içine tanımlıyoruz.Ardından where diyoruz x.name'i ara ve repositoryle biteni çek diyoruz.Arkasından bunların da interfacelerini implemente et diyoruz. İmplemenete ettikten sonra bunları da InstancePerLifetimeScope bu method bizim program.cs dosyamızdaki scope'ye karşılık geliyor bu methodunun çeşitli şekilleri de vardır.
            // AutoFac de InstancePerLifetimeScope bu method bizim asp.net core daki scope'a karşılık geliyor.
            // InstancePerDependency bu ise bizim transient'e karşılık geliyor. Aynı işi yapıyorlar ama methodları farklı.
            // Scope, bir request başlayıp bitene kadar aynı instanceyi kullansın.Transient ise herhangi bir classın constructor'unda o interface nerde geçildiyse her seferinde yeni bir instance oluşturuyordu.
            // Aşağıda demek istiyoruz ki classlardan repository ile bitenleri al classlara karşılık gelen yine sonu repository ile biten interfaceleri al diyoruz.Aynısını service için de yaparız.
            builder.RegisterAssemblyTypes(apiAssembly, repoAssembly, serviceAssembly).Where(x => x.Name.EndsWith("Respository")).AsImplementedInterfaces().InstancePerLifetimeScope();

            // Aşağıda service eklerekn productservice'i de ekliyordu fakat artık productservice'i eklememesi lazım onun yerine productservicewithccaching'i eklemesi gerekiyor.Artık burda manuel bir ekleme yapmamız gerekiyor.
            // ProductService eklememesi için service katmanındaki productservice classının adını değiştirmemiz gerekiyor. Çünkü ekleme yaparken son ekine bakıyor.
            builder.RegisterAssemblyTypes(apiAssembly, repoAssembly, serviceAssembly).Where(x => x.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerLifetimeScope();

            // Aşağıdaki işlemde manuel olarak cach class'ını çağırırız.Biz productserivcewithcaching'i alabilmek için api olarak referans vermemiz gerekiyor.Caching katmanı serivce katmanını içerdiği için biz apiye direk cachingden referans verebiliriz.ProductControllerda bir değişiklik yapmayız.Çünkü orda bir soyutlanma vardır.Orası zaten direk IProductService'i kullanıyor.Var olan kodu bozmadan yeni bir feature ekledik yeni bir class oluşturarak.
            builder.RegisterType<ProductServiceWithCaching>().As<IProductService>();



            base.Load(builder);
        }
    }
}
