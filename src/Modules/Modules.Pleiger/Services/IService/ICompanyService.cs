using Modules.Pleiger.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Services.IService
{
    public interface ICompanyService
    {
        List<SYCompanyInformation> GetListCompanyInfor();
        SYCompanyInformation GetCompanyInforByID(int BusinessNumber);
    }
}
