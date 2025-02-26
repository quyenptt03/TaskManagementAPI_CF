using TaskManagement.DataAccess;
using TaskManagement.Models;

namespace TaskManagement.Repositories
{
    public class TaskRepository : GenericRepository<Models.Task>
    {
        public TaskRepository(TaskManagementContext context) : base(context)
        {
        }
    }
}
