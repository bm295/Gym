using BlazorApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Service
{
    public class TodoItemService
    {
        private static List<TodoItem> _data = new List<TodoItem>() {
            new TodoItem{ Title = "Meet Donald Trump"},
            new TodoItem{ Title = "Lunch with Barack Obama"},
            new TodoItem{ Title = "Go fishing with Bill Gates"},
            new TodoItem{ Title = "Walking with Putin"},
        };

        public List<TodoItem> GetData()
        {
            return _data;
        }
    }
}
