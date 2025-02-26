using TaskManagement.DataAccess;
using TaskManagement.Interfaces;
using TaskManagement.Models;

namespace TaskManagement.Repositories
{
    public class CategoryRepository:GenericRepository<Category>
    {
        public CategoryRepository(TaskManagementContext context) : base(context)
        {
        }
    }
}
