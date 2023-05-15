using HLFXFramework.WebHost.ViewComponents;
using InfrastructureCore.Models.Menu;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Pleiger.SystemMgt.ViewComponents
{
    public class PriorityList : ViewComponent
    {

        public  async Task<IViewComponentResult> InvokeAsync(int maxPriority, bool isDone)
        {
            TodoItem item = new TodoItem();
            item.Priority = maxPriority;
            item.IsDone = isDone;
            return View(item);
        }

        //private Task<TodoItem> GetItemsAsync(int maxPriority, bool isDone)
        //{
        //    return TodoItem(maxPriority, isDone);
        //}

        
    }

    public class TodoItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public bool IsDone { get; set; }
    }
}
