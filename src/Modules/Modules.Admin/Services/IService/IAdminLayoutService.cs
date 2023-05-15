using InfrastructureCore;
using InfrastructureCore.Models.Identity;
using Modules.Admin.Models;
using Modules.Common;
using System.Collections.Generic;

namespace Modules.Admin.Services.IService
{
    public interface IAdminLayoutService
    {
        #region Page Layout
        Result SaveDataPageLayout(SYPageLayout item, string action, SYLoggedUser info);
        Result SaveDataListControls(SYPageLayElements item, string action);
        Result DeletePageLayout(SYPageLayout item);
        List<SYPageLayout> SelectPageLayout();
        List<SYPageLayout> SelectPageLayoutTypePopup();
        #endregion


        #region GridDataPageLayElementDatasourceForCBRC
        List<SYPageLayElements> GridDataPageLayElementDatasourceForCBRC(string PAG_ID, string PEL_ID);
        Result SaveDataPageLayElementDatasourceForCBRC(SYPageLayElements item, string action);
        Result DeleteDataPageLayElementDatasourceForCBRC(SYPageLayElements item);
        #endregion

        #region Page Element
        Result SaveDataElementControls(SYPageLayElements item, string action);
        Result DeleteDataElementControls(SYPageLayElements item);
        List<SYPageLayElements> SelectPageLayoutElement(string PAG_ID, string PEL_ID);
        List<SYPageLayout> GetPageELWithType(string type);
        #endregion

        #region Page Relationship
        List<SYPageRelationship> GetDataGridPageRelationshipLayout(string PAG_ID);
        Result SavePageRelationshipLayout(SYPageRelationship item, string action);
        Result DeletePageRelationshipLayout(SYPageRelationship item);
        List<SYPageLayout> GetPageInSYRelationship(int PAG_ID);
        #endregion

        #region Get data for display in combo box
        List<SYComCode> SelectComCodeByGRP(string GrpCode);
        List<SYComCode> SelectComCodeAndGrpByGRP(string GrpCode);
        List<SYPageLayout> SelectComboBoxType(string codeType);
        List<DynamicCombobox> GetComboboxValueDynamicByGroupCodeAndConnection(string GRP_CD, string CONNECTION_NM);
        List<DynamicCombobox> GetComboboxValueDynamicByGroupCodeAndConnectionForGrid(string GRP_CD, string CONNECTION_NM);
        List<DynamicCombobox> GetComboboxValueDynamicByCustomSPAndConnection(string GRP_CD, string CONNECTION_NM);
        List<DynamicComboboxCustom> GetComboboxValueDynamicCustom(string GRP_CD, string CONNECTION_NM);
        List<DynamicComboboxCustom> GetReferDataCustomAutocomplete(string SP_CUSTOM, string value, string CONNECTION_NM);
        List<DynamicCombobox> GetComboboxValueDynamicByGroupCodeCustomSP(string GRP_CD_CUSTOM);
        List<DynamicRadioCheckbox> GetRadiocheckboxDynamicByGroupCodeAndConnection(string GRP_CD, string CONNECTION_NM);
        List<DynamicRadioCheckbox> GetRadiocheckboxDynamicByCustomSPAndConnection(string GRP_CD_CUSTOM, string CONNECTION_NM);
        List<DynamicAutocomplete> GetDataAutocompleteDynamicByCustomSPAndConnection(string SP_CUSTOM, string CONNECTION_NM);
        List<DynamicCombobox> GetComboboxValueMasterLzDxp200T();
        List<DynamicCombobox> GetComboboxValueMasterBySPDatasource();
        #endregion

        #region SY Data Map SP
        List<SYDataMap> GetDataGridDataMappingSPLayout(string PAG_ID, string PEL_ID);
        Result SaveDataMappingSPLayout(SYDataMap item, string action);
        Result DeleteDataMappingSPLayout(SYDataMap item);
        #endregion

        #region SY Data Mapping details
        List<SYDataMapDetails> GetDataGridDataMappingDetails(int MAP_ID, string PAG_ID, string PEL_ID);
        List<string> GetMapFieldOnGridType(int PAG_ID);
        Result SaveDataMappingDetailLayout(SYDataMapDetails item, string action);
        Result DeleteDataMappingDetailLayout(SYDataMapDetails item);
        #endregion

        #region SY Setting Page Actions
        List<SYPageActions> GridDataPageActionsLayout(string PAG_ID);
        Result SavedDataPageActionsLayout(SYPageActions item, string action);
        Result DeletedDataPageActionsLayout(SYPageActions item);
        List<SYPageActions> SelectActionMappingName(int PAG_ID);
        #endregion

        #region SY Setting Page Actions Details
        List<SYPageActionDetails> GetDataGridPageActionDetails(int PAG_ID, int ACT_ID);
        Result SavedDataPageActionDetailsLayout(SYPageActionDetails item, string action);
        Result DeletedDataPageActionDetailsLayout(SYPageActionDetails item);
        List<SYDataMap> GetMapPelIDInSYDataMap(int PAG_ID);

        #endregion

        #region SY Tool bar actions
        List<SYToolbarActions> GetDataGridPageToolbarActions(int PAG_ID);
        Result SavedDataPageToolbarActionsLayout(SYToolbarActions item, string action);
        Result DeletedDataPageToolbarActionsLayout(SYToolbarActions item);
        List<SYPageActions> GetMapPageAction(int PAG_ID);
        #endregion

        #region Clear action
        List<SYPageLayElements> GetDataGridClearElements(int PAG_ID, int ACT_ID);
        List<SYClearElements> GetDataSelectedGridClearElements(int PAG_ID, int ACT_ID);
        int SaveDataGridClearElements(List<SYPageLayElements> data, int PAG_ID, int ACT_ID);
        #endregion

        #region Setting reference
        List<SYPageLayElementReference> GridDataPageLayElementReference(int PAG_ID, string PEL_ID);
        Result SavedDataDataPageLayElementReference(SYPageLayElementReference item, string action);
        Result DeletedDataPageLayElementReference(SYPageLayElementReference item);
        List<SYPageLayElementReference> GetListColumnLayout(int PAG_ID, string PEL_ID, string type);
        List<string> GetPageElementRelationship(int PAG_ID, string PEL_ID);
        #endregion

        #region Test MySQL
        List<VerifyCodeToken> GetAllVerifyCodeToken();
        #endregion

        List<DynamicCombobox> GetComboboxValueDynamicByGroupCodeAndConnection_MultiLang(string GRP_CD, string CONNECTION_NM, string Lang);
        List<DynamicCombobox> GetComboboxValueDynamicByGroupCodeAndConnection_MultiLang_CustomeSoure(string GRP_CD_CUSTOM, string CONNECTION_NM, string Lang);

        List<DynamicRadioCheckbox> GetRadiocheckboxDynamicByGroupCodeAndConnection_MultiLang(string GRP_CD, string CONNECTION_NM, string Lang);
        List<DynamicRadioCheckbox> GetRadiocheckboxDynamicByCustomSPAndConnection_MultiLang(string GRP_CD_CUSTOM, string CONNECTION_NM, string Lang);

    }
}
