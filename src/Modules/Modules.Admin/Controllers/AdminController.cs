using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Admin.Models;
using System.Collections.Generic;
using System.Linq;
using Modules.Common;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;

namespace Modules.Admin.Controllers
{
    /// <summary>
    /// Represents Admin user control make dynamic page
    /// </summary>
    ///
    
    public class AdminController : BaseController
    {
        #region Properties

        private IAdminLayoutService _adminLayoutService;
        private readonly IHttpContextAccessor _contextAccessor;

        #endregion

        #region Constructor
        public AdminController(IAdminLayoutService adminLayoutService, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this._adminLayoutService = adminLayoutService;
            this._contextAccessor = contextAccessor;
        }

        #endregion

        #region "Get Data"

        #region Page Element Layout

        /// <summary>
        /// Show page element in admin page
        /// </summary>
        /// <returns></returns>
        //[CustomAuthorization]
        public IActionResult PageElement()
        {
            ViewBag.PageID = 0;
            return View();
        }

        #endregion

        #region GridDataPageLayElementDatasourceForCBRC

        [HttpGet]
        public object GridDataPageLayElementDatasourceForCBRC(DataSourceLoadOptions loadOptions, string PAG_ID, string PEL_ID)
        {
            List<SYPageLayElements> lstData = new List<SYPageLayElements>();
            lstData = _adminLayoutService.GridDataPageLayElementDatasourceForCBRC(PAG_ID, PEL_ID);
            return Json(lstData);
        }

        #endregion

        #region Page Relationship

        [HttpGet]
        public object GetDataGridPageRelationshipLayout(DataSourceLoadOptions loadOptions, string PAG_ID)
        {
            List<SYPageRelationship> lstData = new List<SYPageRelationship>();
            lstData = _adminLayoutService.GetDataGridPageRelationshipLayout(PAG_ID);
            return DataSourceLoader.Load(lstData, loadOptions);
        }

        #endregion

        #region Get data for display in combo box

        [HttpGet]
        //[CustomAuthorization]
        public object GetPEL_TYPE(DataSourceLoadOptions loadOptions, string type)
        {
            List<SYComCode> lstType = new List<SYComCode>();
            lstType = _adminLayoutService.SelectComCodeByGRP("G002");
            if (type != null && type.Equals("ChildrenLayout"))
            {
                lstType = lstType.Where(x => x.DTL_CD != "C001" && x.DTL_CD != "C002").ToList();
            }
            else if (type != null && type.Equals("ParentLayout"))
            {
                lstType = lstType.Where(x => x.DTL_CD == "C001" || x.DTL_CD == "C002" || x.DTL_CD == "C017" || x.DTL_CD == "C018" || x.DTL_CD == "C022").ToList();
            }

            return DataSourceLoader.Load(lstType, loadOptions);
        }
        [HttpGet]
        public object GetPEL_DATA_TYPE(DataSourceLoadOptions loadOptions)
        {
            List<SYComCode> lstType = new List<SYComCode>();
            lstType = _adminLayoutService.SelectComCodeByGRP("G003");
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetParamMappingInput(DataSourceLoadOptions loadOptions)
        {
            List<SYComCode> lstType = new List<SYComCode>();
            lstType = _adminLayoutService.SelectComCodeByGRP("G007");
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetTypeTrueFalse(DataSourceLoadOptions loadOptions)
        {
            List<Combobox> lstType = new List<Combobox>() {
                new Combobox { ID = "False", Name = "False"},
                new Combobox { ID = "True", Name = "True"}
            };
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetTypeTrueFalseBool(DataSourceLoadOptions loadOptions)
        {
            List<Combobox> lstType = new List<Combobox>() {
                new Combobox { ID = "False", Name = "False"},
                new Combobox { ID = "True", Name = "True"}
            };
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetTypePEL_ALIGN(DataSourceLoadOptions loadOptions)
        {
            List<SYComCode> lstType = new List<SYComCode>();
            lstType = _adminLayoutService.SelectComCodeByGRP("G008");
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetPAG_TYPE(DataSourceLoadOptions loadOptions)
        {
            List<SYComCode> lstType = new List<SYComCode>();
            // lstType = _adminLayoutService.SelectComCodeByGRP("G001");
            lstType = _adminLayoutService.SelectComCodeAndGrpByGRP("G001");
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetTypeColumnFormat(DataSourceLoadOptions loadOptions)
        {
            List<SYComCode> lstType = new List<SYComCode>();
            lstType = _adminLayoutService.SelectComCodeByGRP("G009");
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetActionType(DataSourceLoadOptions loadOptions)
        {
            List<SYComCode> lstType = new List<SYComCode>();
            lstType = _adminLayoutService.SelectComCodeByGRP("G010");
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetPageELWithType(DataSourceLoadOptions loadOptions)
        {
            List<SYPageLayout> lstType = new List<SYPageLayout>();
            lstType = _adminLayoutService.GetPageELWithType("G001C001");
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetEditType(DataSourceLoadOptions loadOptions)
        {
            List<SYComCode> lstType = new List<SYComCode>();
            lstType = _adminLayoutService.SelectComCodeAndGrpByGRP("G011");
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        /// <summary>
        /// Get Data Type Page from table SYPageLayout, Type = Page C001
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetComboboxTypePage(DataSourceLoadOptions loadOptions)
        {
            List<SYPageLayout> lstType = new List<SYPageLayout>();
            lstType = _adminLayoutService.SelectComboBoxType("G001C001");
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        /// <summary>
        /// Get Data Type Page from table SYPageLayout, Type = pop C002
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        [HttpGet]
        public object SelectPageLayoutTypePopup(DataSourceLoadOptions loadOptions)
        {
            List<SYPageLayout> lstType = new List<SYPageLayout>();
            lstType = _adminLayoutService.SelectPageLayoutTypePopup();
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        /// <summary>
        /// Get Data Type Page from table SYPageLayout, Type = pop C002
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetComboboxTypePop(DataSourceLoadOptions loadOptions)
        {
            List<SYPageLayout> lstType = new List<SYPageLayout>();
            lstType = _adminLayoutService.SelectComboBoxType("G001C002");
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetConnectionType(DataSourceLoadOptions loadOptions)
        {
            List<SYComCode> lstType = new List<SYComCode>();
            lstType = _adminLayoutService.SelectComCodeByGRP("G013");
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetSP_TYPE(DataSourceLoadOptions loadOptions)
        {
            List<SYComCode> lstType = new List<SYComCode>();
            lstType = _adminLayoutService.SelectComCodeByGRP("G006");
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetToolbarType(DataSourceLoadOptions loadOptions)
        {
            List<SYComCode> lstType = new List<SYComCode>();
            lstType = _adminLayoutService.SelectComCodeByGRP("G004");
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        public object GetReferenceType(DataSourceLoadOptions loadOptions)
        {
            List<SYComCode> lstType = new List<SYComCode>();
            lstType = _adminLayoutService.SelectComCodeByGRP("G012");
            return DataSourceLoader.Load(lstType, loadOptions);
        }
        
        public object GetGridModeType(DataSourceLoadOptions loadOptions)
        {
            List<SYComCode> lstType = new List<SYComCode>();
            lstType = _adminLayoutService.SelectComCodeByGRP("G014");
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetFieldIOParamInput(DataSourceLoadOptions loadOptions)
        {
            List<Combobox> lstType = new List<Combobox>() {
                new Combobox { ID = "0", Name = "Output"},
                new Combobox { ID = "1", Name = "Input"}
            };
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetComboboxValueDynamicBySP(DataSourceLoadOptions loadOptions, string GRP_CD)
        {
            List<DynamicCombobox> lstType = new List<DynamicCombobox>();
            if (GRP_CD != null)
            {
                // lstType = _adminLayoutService.GetComboboxValueDynamicBySP(GRP_CD);
            }

            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetDataAutocompleteDynamicByCustomSPAndConnection(DataSourceLoadOptions loadOptions, string SP_CUSTOM, string CONNECTION_NM)
        {
            List<DynamicAutocomplete> lstType = new List<DynamicAutocomplete>();
            if (SP_CUSTOM != null)
            {
                lstType = _adminLayoutService.GetDataAutocompleteDynamicByCustomSPAndConnection(SP_CUSTOM, CONNECTION_NM);
            }

            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetRadiocheckboxDynamicByGroupCodeAndConnection(DataSourceLoadOptions loadOptions, string GRP_CD, string CONNECTION_NM)
        {
            List<DynamicRadioCheckbox> lstType = new List<DynamicRadioCheckbox>();
            if (GRP_CD != null)
            {
                lstType = _adminLayoutService.GetRadiocheckboxDynamicByGroupCodeAndConnection(GRP_CD, CONNECTION_NM);
            }

            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetRadiocheckboxDynamicByGroupCodeAndConnection_MultiLang(DataSourceLoadOptions loadOptions, string GRP_CD, string CONNECTION_NM, string Lang)
        {
            List<DynamicRadioCheckbox> lstType = new List<DynamicRadioCheckbox>();
            if (GRP_CD != null)
            {
                lstType = _adminLayoutService.GetRadiocheckboxDynamicByGroupCodeAndConnection_MultiLang(GRP_CD, CONNECTION_NM, Lang.Replace('/', ' '));
            }

            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetRadiocheckboxDynamicByCustomSPAndConnection_MultiLang(DataSourceLoadOptions loadOptions, string GRP_CD_CUSTOM, string CONNECTION_NM, string Lang)
        {
            List<DynamicRadioCheckbox> lstType = new List<DynamicRadioCheckbox>();
            if (GRP_CD_CUSTOM != null)
            {
                lstType = _adminLayoutService.GetRadiocheckboxDynamicByCustomSPAndConnection_MultiLang(GRP_CD_CUSTOM, CONNECTION_NM, Lang.Replace('/', ' '));
            }

            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetRadiocheckboxDynamicByCustomSPAndConnection(DataSourceLoadOptions loadOptions, string GRP_CD_CUSTOM, string CONNECTION_NM)
        {
            List<DynamicRadioCheckbox> lstType = new List<DynamicRadioCheckbox>();
            if (GRP_CD_CUSTOM != null)
            {
                lstType = _adminLayoutService.GetRadiocheckboxDynamicByCustomSPAndConnection(GRP_CD_CUSTOM, CONNECTION_NM);
            }

            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetComboboxValueDynamicByGroupCodeCustomSP(DataSourceLoadOptions loadOptions, string GRP_CD_CUSTOM)
        {
            List<DynamicCombobox> lstType = new List<DynamicCombobox>();
            if (GRP_CD_CUSTOM != null)
            {
                lstType = _adminLayoutService.GetComboboxValueDynamicByGroupCodeCustomSP(GRP_CD_CUSTOM);
            }

            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetComboboxValueDynamicByGroupCodeAndConnection(DataSourceLoadOptions loadOptions, string GRP_CD, string CONNECTION_NM)
        {
            List<DynamicCombobox> lstType = new List<DynamicCombobox>();
            if (GRP_CD != null)
            {
                lstType = _adminLayoutService.GetComboboxValueDynamicByGroupCodeAndConnection(GRP_CD, CONNECTION_NM);
            }

            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetComboboxValueDynamicByGroupCodeAndConnection_MultiLang(DataSourceLoadOptions loadOptions, string GRP_CD, string CONNECTION_NM, string Lang)
        {
            List<DynamicCombobox> lstType = new List<DynamicCombobox>();
            if (GRP_CD != null)
            {
                lstType = _adminLayoutService.GetComboboxValueDynamicByGroupCodeAndConnection_MultiLang(GRP_CD, CONNECTION_NM, Lang.Replace('/', ' '));
            }

            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetComboboxValueDynamicByGroupCodeAndConnection_MultiLang_CustomeSoure(DataSourceLoadOptions loadOptions, string GRP_CD_CUSTOM, string CONNECTION_NM, string Lang)
        {
            List<DynamicCombobox> lstType = new List<DynamicCombobox>();
            if (GRP_CD_CUSTOM != null)
            {
                lstType = _adminLayoutService.GetComboboxValueDynamicByGroupCodeAndConnection_MultiLang_CustomeSoure(GRP_CD_CUSTOM, CONNECTION_NM, Lang.Replace('/', ' '));
            }

            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetComboboxValueDynamicByGroupCodeAndConnectionForGrid(DataSourceLoadOptions loadOptions, string GRP_CD, string CONNECTION_NM)
        {
            List<DynamicCombobox> lstType = new List<DynamicCombobox>();
            if (GRP_CD != null)
            {
                lstType = _adminLayoutService.GetComboboxValueDynamicByGroupCodeAndConnectionForGrid(GRP_CD, CONNECTION_NM);
            }

            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetComboboxValueDynamicByCustomSPAndConnection(DataSourceLoadOptions loadOptions, string GRP_CD_CUSTOM, string CONNECTION_NM)
        {
            List<DynamicCombobox> lstType = new List<DynamicCombobox>();
            if (GRP_CD_CUSTOM != null)
            {
                lstType = _adminLayoutService.GetComboboxValueDynamicByCustomSPAndConnection(GRP_CD_CUSTOM, CONNECTION_NM);
            }

            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetComboboxValueDynamicCustom(DataSourceLoadOptions loadOptions, string GRP_CD_CUSTOM, string CONNECTION_NM)
        {
            List<DynamicComboboxCustom> lstType = new List<DynamicComboboxCustom>();
            if (GRP_CD_CUSTOM != null)
            {
                lstType = _adminLayoutService.GetComboboxValueDynamicCustom(GRP_CD_CUSTOM, CONNECTION_NM);
            }

            return DataSourceLoader.Load(lstType, loadOptions);
        }
        
        [HttpGet]
        public object GetReferDataCustomAutocomplete(DataSourceLoadOptions loadOptions, string SP_CUSTOM, string value, string CONNECTION_NM)
        {
            List<DynamicComboboxCustom> lstType = new List<DynamicComboboxCustom>();
            if (CONNECTION_NM != null)
            {
                lstType = _adminLayoutService.GetReferDataCustomAutocomplete(SP_CUSTOM, value, CONNECTION_NM);
            }

            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetComboboxValueMasterBySPDatasource(DataSourceLoadOptions loadOptions)
        {
            List<DynamicCombobox> lstType = new List<DynamicCombobox>();
            lstType = _adminLayoutService.GetComboboxValueMasterBySPDatasource();
            foreach (var item in lstType)
            {

            }

            return DataSourceLoader.Load(lstType, loadOptions);
        }

        /// <summary>
        /// Get Data source of grid component
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetDataSourceNameOfGridType(DataSourceLoadOptions loadOptions)
        {
            List<string> lstType = new List<string>();
            lstType.Add("A1");
            lstType.Add("A2");
            lstType.Add("A3");
            lstType.Add("A4");
            lstType.Add("A5");
            //lstType = _adminLayoutService.SelectComCodeByGRP("G006");
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        #endregion

        #region "Page Layout"

        [HttpGet]
        public object GetDataGridPageLayout(DataSourceLoadOptions loadOptions)
        {
            // List<GridColumnView> lstData = new List<GridColumnView>();
            List<SYPageLayout> lstData = new List<SYPageLayout>();
            lstData = _adminLayoutService.SelectPageLayout();
            return DataSourceLoader.Load(lstData, loadOptions);
        }

        #endregion

        #region "Page Element"

        [HttpGet]
        public object GetDataGridPageElement(DataSourceLoadOptions loadOptions, string PAG_ID, string PEL_ID)
        {
            // List<GridColumnView> lstData = new List<GridColumnView>();
            List<SYPageLayElements> lstData = new List<SYPageLayElements>();
            lstData = _adminLayoutService.SelectPageLayoutElement(PAG_ID, PEL_ID);
            return DataSourceLoader.Load(lstData, loadOptions);
        }

        [HttpGet]
        public IActionResult GetDataGridPageControls(DataSourceLoadOptions loadOptions, string PAG_ID, string PEL_ID)
        {
            // List<GridColumnView> lstData = new List<GridColumnView>();
            List<SYPageLayElements> lstData = new List<SYPageLayElements>();
            lstData = _adminLayoutService.SelectPageLayoutElement(PAG_ID, PEL_ID);
            return Json(lstData);
        }

        #endregion

        #region "Page Setting Properties"

        // show page PageSettingProperties
        //[CustomAuthorization]
        public ActionResult PageSettingProperties(string pagID = "", string pelID = "", string pagTp = "")
        {
            ViewBag.PageID = 0;
            ViewBag.pagID = pagID.Trim();
            ViewBag.pelID = pelID.Trim();
            ViewBag.pagTp = pagTp.Trim();
            ViewBag.Id = pagID.Trim() + "_" + pelID.Trim() + "_" + pagTp.Trim();

            return View();
        }

        [HttpGet]
        public IActionResult GetDataGridDataMappingSPLayout(DataSourceLoadOptions loadOptions, string PAG_ID, string PEL_ID)
        {
            List<SYDataMap> lstData = new List<SYDataMap>();
            lstData = _adminLayoutService.GetDataGridDataMappingSPLayout(PAG_ID, PEL_ID);
            return Json(lstData);
        }

        #endregion

        #region "Setting Data Mapping Details"

        // Show Popup Data Mapping Details
        [HttpGet]
        public IActionResult ShowPopupDataMappingDetails(int MAP_ID, string PAG_ID, string PEL_ID, string datasourceType, string datasourceName)
        {
            ViewBag.MAP_ID = MAP_ID;
            ViewBag.PAG_ID = PAG_ID;
            ViewBag.PEL_ID = PEL_ID;
            ViewBag.datasourceType = datasourceType;
            ViewBag.datasourceName = datasourceName;

            //Show page popup PopupDataMappingDetails
            return PartialView("PopupDataMappingDetails");
        }

        [HttpGet]
        public IActionResult GetDataGridDataMappingDetails(DataSourceLoadOptions loadOptions, int MAP_ID, string PAG_ID, string PEL_ID)
        {
            List<SYDataMapDetails> lstData = new List<SYDataMapDetails>();
            lstData = _adminLayoutService.GetDataGridDataMappingDetails(MAP_ID, PAG_ID, PEL_ID);
            foreach (var item in lstData)
            {
                item.FLD_IO_CONVERT = item.FLD_IO.ToString();
            }
            return Json(lstData);
        }

        [HttpGet]
        public object GetMapFieldOnGridType(DataSourceLoadOptions loadOptions, int PAG_ID)
        {
            List<string> lstData = new List<string>();
            List<Combobox> lstDataCombobox = new List<Combobox>();
            lstData = _adminLayoutService.GetMapFieldOnGridType(PAG_ID);
            foreach (var item in lstData)
            {
                Combobox combobox = new Combobox();
                combobox.ID = item;
                combobox.Name = item;
                lstDataCombobox.Add(combobox);
            }
            return DataSourceLoader.Load(lstDataCombobox, loadOptions);
        }

        #endregion

        #region "Setting Page Actions"

        [HttpGet]
        public IActionResult GridDataPageActionsLayout(DataSourceLoadOptions loadOptions, string PAG_ID)
        {
            List<SYPageActions> lstData = new List<SYPageActions>();
            lstData = _adminLayoutService.GridDataPageActionsLayout(PAG_ID);
            return Json(lstData);
        }

        [HttpGet]
        public object SelectActionMappingName(DataSourceLoadOptions loadOptions, int PAG_ID)
        {
            List<SYPageActions> lstData = new List<SYPageActions>();
            List<ComboboxTypeInt> lstDataCombobox = new List<ComboboxTypeInt>();
            lstData = _adminLayoutService.SelectActionMappingName(PAG_ID);
            foreach (var item in lstData)
            {
                ComboboxTypeInt combobox = new ComboboxTypeInt();
                combobox.ID = item.ACT_ID;
                combobox.Name = item.ACT_NM + " (Action ID: " + item.ACT_ID + ")";
                lstDataCombobox.Add(combobox);
            }
            return DataSourceLoader.Load(lstDataCombobox, loadOptions);
        }

        #endregion

        #region "Setting Page Actions Details"

        [HttpGet]
        public IActionResult GetDataGridPageActionDetails(DataSourceLoadOptions loadOptions, int PAG_ID, int ACT_ID)
        {
            List<SYPageActionDetails> lstData = new List<SYPageActionDetails>();
            lstData = _adminLayoutService.GetDataGridPageActionDetails(PAG_ID, ACT_ID);
            return Json(lstData);
        }

        [HttpGet]
        public IActionResult GetDataGridClearElements(DataSourceLoadOptions loadOptions, int PAG_ID, int ACT_ID)
        {
            List<SYPageLayElements> lstData = new List<SYPageLayElements>();
            lstData = _adminLayoutService.GetDataGridClearElements(PAG_ID, ACT_ID);
            return Json(lstData);
        }

        [HttpGet]
        public IActionResult GetDataSelectedGridClearElements(int pageID, int actionID)
        {
            List<SYClearElements> lstData = new List<SYClearElements>();
            lstData = _adminLayoutService.GetDataSelectedGridClearElements(pageID, actionID);
            return Json(lstData);
        }

        [HttpGet]
        public object GetMapPelIDInSYDataMap(DataSourceLoadOptions loadOptions, int PAG_ID)
        {
            List<SYDataMap> lstData = new List<SYDataMap>();
            List<ComboboxTypeInt> lstDataCombobox = new List<ComboboxTypeInt>();
            lstData = _adminLayoutService.GetMapPelIDInSYDataMap(PAG_ID);
            foreach (var item in lstData)
            {
                ComboboxTypeInt combobox = new ComboboxTypeInt();
                combobox.ID = item.MAP_ID;
                combobox.Name = item.MAP_PEL_ID + " (Map ID: " + item.MAP_ID + ")" + " (Mapping Name: " + item.MAP_SPNM + ")";
                lstDataCombobox.Add(combobox);
            }
            var lstPage = _adminLayoutService.GetPageInSYRelationship(PAG_ID);
            foreach (var item in lstPage)
            {
                ComboboxTypeInt combobox = new ComboboxTypeInt();
                combobox.ID = item.PAG_ID;
                combobox.Name = item.PAG_KEY + " (Popup page: " + item.PAG_ID + ")";
                lstDataCombobox.Add(combobox);
            }
            return DataSourceLoader.Load(lstDataCombobox, loadOptions);
        }

        #endregion

        #region "Setting Page Tool bar Action"

        [HttpGet]
        public IActionResult GetDataGridPageToolbarActions(SYToolbarActions loadOptions, int PAG_ID)
        {
            List<SYToolbarActions> lstData = new List<SYToolbarActions>();
            lstData = _adminLayoutService.GetDataGridPageToolbarActions(PAG_ID);
            return Json(lstData);
        }

        [HttpGet]
        public object GetMapPageAction(DataSourceLoadOptions loadOptions, int PAG_ID)
        {
            List<SYPageActions> lstData = new List<SYPageActions>();
            List<ComboboxTypeInt> lstDataCombobox = new List<ComboboxTypeInt>();
            lstData = _adminLayoutService.GetMapPageAction(PAG_ID);
            foreach (var item in lstData)
            {
                ComboboxTypeInt combobox = new ComboboxTypeInt();
                combobox.ID = item.ACT_ID;
                combobox.Name = item.ACT_NM;
                lstDataCombobox.Add(combobox);
            }
            return DataSourceLoader.Load(lstDataCombobox, loadOptions);
        }

        #endregion

        #region "Setting Calculate Expression"

        [HttpGet]
        public IActionResult GridDataPageLayElementReference(SYPageLayElementReference loadOptions, int PAG_ID, string PEL_ID)
        {
            List<SYPageLayElementReference> lstData = new List<SYPageLayElementReference>();
            lstData = _adminLayoutService.GridDataPageLayElementReference(PAG_ID, PEL_ID);
            return Json(lstData);
        }

        [HttpGet]
        public object GetListColumnInGrid(DataSourceLoadOptions loadOptions, int PAG_ID, string PEL_ID)
        {
            List<SYPageLayElementReference> lstData = new List<SYPageLayElementReference>();
            List<Combobox> lstDataCombobox = new List<Combobox>();
            lstData = _adminLayoutService.GetListColumnLayout(PAG_ID, PEL_ID, "G002C002");
            foreach (var item in lstData)
            {
                Combobox combobox = new Combobox();
                combobox.ID = item.PEL_ID;
                combobox.Name = item.PEL_ID;
                lstDataCombobox.Add(combobox);
            }
            return DataSourceLoader.Load(lstDataCombobox, loadOptions);
        }

        [HttpGet]
        public object GetListColumnInForm(DataSourceLoadOptions loadOptions, int PAG_ID, string PEL_ID)
        {
            List<SYPageLayElementReference> lstData = new List<SYPageLayElementReference>();
            List<Combobox> lstDataCombobox = new List<Combobox>();
            lstData = _adminLayoutService.GetListColumnLayout(PAG_ID, PEL_ID, "G002C001");
            foreach (var item in lstData)
            {
                Combobox combobox = new Combobox();
                combobox.ID = item.PEL_ID;
                combobox.Name = item.PEL_ID;
                lstDataCombobox.Add(combobox);
            }
            return DataSourceLoader.Load(lstDataCombobox, loadOptions);
        }

        [HttpGet]
        public object GetPageElementRelationship(DataSourceLoadOptions loadOptions, int PAG_ID, string PEL_ID)
        {
            List<string> lstData = new List<string>();
            List<Combobox> lstDataCombobox = new List<Combobox>();
            lstData = _adminLayoutService.GetPageElementRelationship(PAG_ID, PEL_ID);
            foreach (var item in lstData)
            {
                Combobox combobox = new Combobox();
                combobox.ID = item;
                combobox.Name = item;
                lstDataCombobox.Add(combobox);
            }
            return DataSourceLoader.Load(lstDataCombobox, loadOptions);
        }

        #endregion

        #endregion

        #region "Insert - Update - Delete"

        #region Page Relationship

        [HttpPost]
        public IActionResult SavePageRelationshipLayout(SYPageRelationship data, string state)
        {
            Result result = _adminLayoutService.SavePageRelationshipLayout(data, state);
            return Json(new { result.Success, result.Message });
        }

        [HttpPost]
        public IActionResult DeletePageRelationshipLayout(SYPageRelationship data)
        {
            Result result = _adminLayoutService.DeletePageRelationshipLayout(data);
            return Json(new { result.Success, result.Message });
        }

        #endregion

        #region Page Layout

        [HttpPost]
        public IActionResult SaveDataPageLayout(SYPageLayout data, string state)
        {
            Result result = _adminLayoutService.SaveDataPageLayout(data, state, CurrentUser);
            return Json(new { result.Success, result.Message });
        }

        [HttpPost]
        public IActionResult DeletePageLayout(SYPageLayout data, string state)
        {
            Result result = _adminLayoutService.DeletePageLayout(data);
            return Json(new { result.Success, result.Message });
        }

        #endregion

        #region Page Element 

        [HttpPost]
        public IActionResult SaveDataElementControls(SYPageLayElements data, string state)
        {
            Result result = _adminLayoutService.SaveDataElementControls(data, state);
            return Json(new { result.Success, result.Message });
        }

        [HttpPost]
        public IActionResult DeleteDataGridPageElement(SYPageLayElements data)
        {
            Result result = _adminLayoutService.DeleteDataElementControls(data);
            return Json(new { result.Success, result.Message });
        }

        [HttpPost]
        public IActionResult SaveDataListControls(SYPageLayElements data, string state)
        {
            Result result = _adminLayoutService.SaveDataElementControls(data, state);
            return Json(new { result.Success, result.Message });
        }

        #endregion

        #region "Page Setting Properties"

        [HttpPost]
        public IActionResult SaveDataMappingSPLayout(SYDataMap data, string state)
        {

            Result result = _adminLayoutService.SaveDataMappingSPLayout(data, state);
            return Json(new { result.Success, result.Message });
        }

        [HttpPost]
        public IActionResult DeleteDataMappingSPLayout(SYDataMap data)
        {
            Result result = _adminLayoutService.DeleteDataMappingSPLayout(data);
            return Json(new { result.Success, result.Message });
        }

        #endregion

        #region "Setting Data Mapping Details"

        [HttpPost]
        public IActionResult SaveDataMappingDetailLayout(SYDataMapDetails data, string state)
        {
            Result result = _adminLayoutService.SaveDataMappingDetailLayout(data, state);
            return Json(new { result.Success, result.Message });
        }

        [HttpPost]
        public IActionResult DeleteDataMappingDetailLayout(SYDataMapDetails data)
        {
            Result result = _adminLayoutService.DeleteDataMappingDetailLayout(data);
            return Json(new { result.Success, result.Message });
        }

        #endregion

        #region Setting Page Actions

        [HttpPost]
        public IActionResult SavedDataPageActionsLayout(SYPageActions data, string state)
        {
            Result result = _adminLayoutService.SavedDataPageActionsLayout(data, state);
            return Json(new { result.Success, result.Message });
        }

        [HttpPost]
        public IActionResult DeletedDataPageActionsLayout(SYPageActions data)
        {
            Result result = _adminLayoutService.DeletedDataPageActionsLayout(data);
            return Json(new { result.Success, result.Message });
        }

        public IActionResult ShowPopupDataPageActionsDetails(int PAG_ID, int ACT_ID)
        {
            ViewBag.ACT_ID = ACT_ID;
            ViewBag.PAG_ID = PAG_ID;

            //Show page popup PopupDataPageActionDetails
            return PartialView("PopupDataPageActionDetails");
        }

        #endregion

        #region Setting Page Actions Details

        [HttpPost]
        public IActionResult SavedDataPageActionDetailsLayout(SYPageActionDetails data, string state)
        {
            Result result = _adminLayoutService.SavedDataPageActionDetailsLayout(data, state);
            return Json(new { result.Success, result.Message });
        }

        [HttpPost]
        public IActionResult DeletedDataPageActionDetailsLayout(SYPageActionDetails data)
        {
            Result result = _adminLayoutService.DeletedDataPageActionDetailsLayout(data);
            return Json(new { result.Success, result.Message });
        }

        [HttpPost]
        public IActionResult SaveDataGridClearElements(List<SYPageLayElements> data, int pageID, int actionID)
        {
            var result = _adminLayoutService.SaveDataGridClearElements(data, pageID, actionID);
            // return Json(new { result.Success, result.Message });
            return Json(new { result = result });
        }

        #endregion

        #region Setting Page Tool bar Action

        [HttpPost]
        public IActionResult SavedDataPageToolbarActionsLayout(SYToolbarActions data, string state)
        {
            Result result = _adminLayoutService.SavedDataPageToolbarActionsLayout(data, state);
            return Json(new { result.Success, result.Message });
        }

        [HttpPost]
        public IActionResult DeletedDataPageToolbarActionsLayout(SYToolbarActions data)
        {
            Result result = _adminLayoutService.DeletedDataPageToolbarActionsLayout(data);
            return Json(new { result.Success, result.Message });
        }

        #endregion

        #region Setting Calculate Expression

        [HttpPost]
        public IActionResult SavedDataDataPageLayElementReference(SYPageLayElementReference data, string state)
        {
            Result result = _adminLayoutService.SavedDataDataPageLayElementReference(data, state);
            return Json(new { result.Success, result.Message });
        }

        [HttpPost]
        public IActionResult DeletedDataPageLayElementReference(SYPageLayElementReference data)
        {
            Result result = _adminLayoutService.DeletedDataPageLayElementReference(data);
            return Json(new { result.Success, result.Message });
        }

        #endregion

        #endregion

        #region Test MySQL
        [HttpGet]
        public IActionResult GetAllVerifyCodeToken()
        {
            List<VerifyCodeToken> lstData = _adminLayoutService.GetAllVerifyCodeToken();
            return Json(lstData);
        }

        #endregion
    }
}
