using TaskManagement.DataAccess;
using TaskManagement.Models;

namespace TaskManagement.Repositories
{
    public class LabelRepository : GenericRepository<Label>
    {
        public LabelRepository(TaskManagementContext context) : base(context)
        {
        }
    }
}
