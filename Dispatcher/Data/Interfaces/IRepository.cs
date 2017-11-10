using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Dispatcher.Data.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        IQueryable<T> GetAll();

        T GetById(int id);

        void Add(T entity);

        void Update(T request);

        void Delete(T entity);

        void Delete(int id);

        DbEntityEntry<T> GetEntry(T entity);
    }
}
