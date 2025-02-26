using TaskManagement.DataAccess;
using TaskManagement.Models;

namespace TaskManagement.Repositories
{
    public class TaskCommentRepository : GenericRepository<TaskComment>
    {
        public TaskCommentRepository(TaskManagementContext context) : base(context)
        {
        }
    }
}
