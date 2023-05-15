using InfrastructureCore;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Kiosk.Management.Repositories.IRepositories
{
    public interface IKIOCommonCodeRepository
    {
        List<KIO_CommonCode> GetCommonCode(string code, string subCode, bool status);
        List<KIO_MasterCode> GetMasterCode(string code, bool status);
        Result SaveMasterCode(KIO_MasterCode masterCode);
        Result SaveCommonCode(KIO_CommonCode commonCode);
        Result DeleteCommonCode(string code);
        Result DeleteMasterCode(string code);
    }
}
