using TaskManagement.DataAccess;
using TaskManagement.Interfaces;

namespace TaskManagement.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly TaskManagementContext _context;

        public GenericRepository(TaskManagementContext context)
        {
            _context = context;
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public void Add(T entity)
        {
            if (entity != null)
            {
                _context.Set<T>().Add(entity);
                _context.SaveChanges();
            }
        }

        public void Update(T entity)
        {
            if (entity != null)
            {
                _context.Set<T>().Update(entity);
                _context.SaveChanges();
            }
        }
        public void Delete(int id)
        {
            T entity = GetById(id);

            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                _context.SaveChanges();
            }
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        public bool Any(Func<T, bool> value)
        {
            return _context.Set<T>().Any(value);
        }
    }
}
