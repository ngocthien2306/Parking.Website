using Modules.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Admin.Services.IService
{
    public interface IToolbarService
    {
        Task<List<SYToolbarActions>> GetToolbarActionsWithID(int PageID);
    }
}
