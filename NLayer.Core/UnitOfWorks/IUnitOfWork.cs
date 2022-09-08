namespace NLayer.Core.UnitOfWorks
{
    public interface IUnitOfWork
    {
        //Bunları implemente ettiğimiz zaman dbcontext'in savechange ve savechangeasync methodlarını çağırıyor olacağız.
        Task CommitAsync();
        void Commit();

    }
}
