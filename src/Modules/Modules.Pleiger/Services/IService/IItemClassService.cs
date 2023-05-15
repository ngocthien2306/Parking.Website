using InfrastructureCore;
using Modules.Pleiger.Models;
using System.Collections.Generic;

namespace Modules.Pleiger.Services.IService
{
    public interface IItemClassService
    {
        List<MES_ItemClass> GetListData();
        List<MES_ItemClass> GetListItemUpCode(string itemComCode);
        MES_ItemClass GetItemClassByCode(string itemClassCode, string itemComCode);
        Result SaveItemClass(string itemClassCode, string itemComCode, string itemUpCode, string itemCategory, string classNameKor, string classNameEng, string etc, string userModify);
        Result DeleteItemClass(string itemClassCode, string itemComCode);
        public List<MES_ItemClass> GetItemClassByCategory();
        List<MES_ItemClass> GetItemClassCodeByCategory();
        public List<MES_ItemClass> GetItemClassByCategory0304();


        #region Get data show in combo box-search form at screen Item
        List<MES_ItemClass> GetListItemClassByCategory(string categorySelected);
        List<MES_ItemClass> GetListItemClassSub1ByItemClass(string categorySelected, string itemClassSelected);
        List<MES_ItemClass> GetListItemClassSub2ByItemClassSub1(string categorySelected, string itemClassSub1Selected);
        #endregion
    }
}
