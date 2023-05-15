using InfrastructureCore;
using InfrastructureCore.Models.Identity;
using Modules.Admin.Models;
using Modules.Pleiger.Models;
using System.Collections.Generic;

namespace Modules.Pleiger.Services.IService
{
    public interface IEmployeeService
    {
        SYUser GetUserFromEmployessCode(string EmpCode);
        MESEmployees GetEmployess(string EmpCode);
        List<MESEmployees> GetListEmployess();
        Result SaveMESEmployee(MESEmployees model,string CurrentUser);
        Result DeleteMESEmployee(string EmpCode);
        List<MESEmployees> ListSearchEmployee(string PartnerCode, string EmployeeNumber, string EmployeeNameKr, string EmployeeNameEng, string UseYN);
        Result DeleteEmployeeInfo(List<MESEmployees> listEmployeeInfo);
        List<DynamicCombobox> GetListEmployees();

    }
}
