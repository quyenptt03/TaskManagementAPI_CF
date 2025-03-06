using Microsoft.EntityFrameworkCore;
using TaskManagement.DataAccess;
using TaskManagement.Interfaces;
using Task = System.Threading.Tasks.Task;


namespace TaskManagement.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly TaskManagementContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(TaskManagementContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task Add(T entity)
        {
            if (entity != null)
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Update(T entity)
        {
            if (entity != null)
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task Delete(int id)
        {
            T entity = await GetById(id);

            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(T entity)
        {
             _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public bool Any(Func<T, bool> predicate)
        {
            return _dbSet.Any(predicate);
        }
    }
}
