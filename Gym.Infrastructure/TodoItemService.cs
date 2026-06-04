using System.Collections.Generic;
using Gym.Application;
using Gym.Domain;

namespace Gym.Infrastructure
{
    public class TodoItemService : ITodoItemService
    {
        private static readonly List<TodoItem> _data = new()
        {
            new TodoItem { Title = "Meet Donald Trump" },
            new TodoItem { Title = "Lunch with Barack Obama" },
            new TodoItem { Title = "Go fishing with Bill Gates" },
            new TodoItem { Title = "Walking with Putin" },
        };

        public List<TodoItem> GetData()
        {
            return _data;
        }
    }
}
