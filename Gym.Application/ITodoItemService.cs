using System.Collections.Generic;
using System.Threading.Tasks;
using Gym.Domain;

namespace Gym.Application
{
    public interface ITodoItemService
    {
        List<TodoItem> GetData();
    }
}
