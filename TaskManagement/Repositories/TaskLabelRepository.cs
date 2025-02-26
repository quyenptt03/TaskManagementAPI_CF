using TaskManagement.DataAccess;
using TaskManagement.Models;

namespace TaskManagement.Repositories
{
    public class TaskLabelRepository : GenericRepository<TaskLabel>
    {
        public TaskLabelRepository(TaskManagementContext context) : base(context)
        {
        }
    }
}
