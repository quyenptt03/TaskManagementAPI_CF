using TaskManagement.DataAccess;
using TaskManagement.Models;

namespace TaskManagement.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(TaskManagementContext context) : base(context)
        {
        }
    }
}
