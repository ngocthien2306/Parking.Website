using Modules.Admin.Models.WidgetModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Admin.Services.IService
{
    public interface IDynamicWidgetService
    {
        //
        dynamic ExecuteProcedure(string procName, ICollection<SYWidgetStoreProcedureParams> paras, string connectionType);
    }
}
