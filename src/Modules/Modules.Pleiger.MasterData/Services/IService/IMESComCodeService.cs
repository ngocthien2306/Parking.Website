using InfrastructureCore;
using InfrastructureCore.Models.Identity;
using Modules.Pleiger.CommonModels;
using System.Collections.Generic;

namespace Modules.Pleiger.MasterData.Services.IService
{
    public interface IMESComCodeService
    {
        #region "MES_ComCode Master"

        List<MES_ComCodeMst> GetListComCodeMST();
        Result SaveDataComCodeMST(List<MES_ComCodeMst> dataMST, List<MES_ComCodeDtls> dataDTL, List<MES_ComCodeDtls> dataDTLDelete,
            string groupCdSelected, SYLoggedUser info);
        Result DeleteGroupComCodeMST(MES_ComCodeMst item, SYLoggedUser info);

        #endregion

        #region "MES_ComCode Detail"

        List<MES_ComCodeDtls> GetListComCodeDTL(string groupCD);
        List<MES_ComCodeDtls> GetListComCodeCategoryMaterial(string groupCD, string baseCD1, string baseCD2, string baseCD3, string baseCD4);

        Result SaveDataComCodeDTL(MES_ComCodeDtls item, SYLoggedUser info);
        Result DeleteGroupComCodeDTL(MES_ComCodeDtls item, SYLoggedUser info);
  
        List<MES_ComCodeDtls>GetListComCodeDTLAll(string groupCD);
        List<MES_ComCodeDtls> GetListComCodeDTLProduct();
        
        #endregion

    }
}
