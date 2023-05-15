using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Models.Menu;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HLFXFramework.WebHost.ViewComponents
{
    public class AccessToolbarViewComponent : ViewComponent
    {
        private IToolbarService toolbarService;
        public AccessToolbarViewComponent(IToolbarService toolbarService)
        {
            this.toolbarService = toolbarService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int PageID ,string PageType, SYMenuAccess pageSetting, List<ToolbarInfo> lstNewToolbar,string threadID,string GridMenuID)
        {
            var userInfo = HttpContext.Session.Get<SYLoggedUser>("UserInfo");
            //string urlTab = HttpContext.Session.Get<string>("UrlGenerateTab");
            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            

            SYMenuAccess toolbar = new SYMenuAccess();
            
            List<SYToolbarActions> lstToolActionDynamic = new List<SYToolbarActions>();
            if (PageID != 0)
            {
                curUrl = "/DynamicMagt/?PageID=" + PageID + "&MenuID="+ GridMenuID;
                
                lstToolActionDynamic = await toolbarService.GetToolbarActionsWithID(PageID).ConfigureAwait(true);
            }
            var curMenu = userInfo !=null ? userInfo.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;
            
            bool dynamicPage = false;
            if(PageType != "DynamicPop")
            {
                if(curMenu != null)
                {
                    toolbar = SetAccessToolbar(userInfo, curMenu.MenuID);
                    if (curMenu.MenuType == "Y" || Request.Path.Value == "/DynamicMagt/" || Request.Path.Value == "/DynamicMagt")
                    {
                        dynamicPage = true;
                        ViewBag.lstToolActionDynamic = lstToolActionDynamic;
                    }
                }
                else
                {
                    toolbar.SEARCH_YN = true;
                    toolbar.CREATE_YN = true;
                    toolbar.SAVE_YN = true;
                    toolbar.EDIT_YN = true;
                    toolbar.DELETE_YN = true;
                    toolbar.EXCEL_YN = true;
                    toolbar.PRINT_YN = true;
                    //add test
                    toolbar.IMPORT_EXCEL_YN = true;
                    ViewBag.lstToolActionDynamic = lstToolActionDynamic;
                }               
            }
            else
            {
                toolbar.SEARCH_YN = true;
                toolbar.CREATE_YN = true;
                toolbar.SAVE_YN = true;
                toolbar.EDIT_YN = true;
                toolbar.DELETE_YN = true;
                toolbar.EXCEL_YN = true;
                toolbar.PRINT_YN = true;
                //add 
                toolbar.IMPORT_EXCEL_YN = true;
                dynamicPage = true;
                ViewBag.lstToolActionDynamic = lstToolActionDynamic;
            }
            
            ViewBag.DynamicPage = dynamicPage;
            int MenuID = 0;
            if(curMenu != null)
            {
                MenuID = curMenu.MenuID;
            }    
            List<ToolbarInfo> lstToolbarInfo = SetSettingPageToolbar(toolbar, pageSetting, MenuID, lstNewToolbar, lstToolActionDynamic);
            ViewBag.Thread = threadID;
            return View(lstToolbarInfo);
        }
        protected static List<ToolbarInfo> SetSettingPageToolbar(SYMenuAccess toolbar, SYMenuAccess pageSetting, int MenuID, List<ToolbarInfo> lstNewToolbar, List<SYToolbarActions> lstToolActionDynamic)
        {
            //Comcode G004
            List<ToolbarInfo> listInfo = new List<ToolbarInfo>();
            if(lstToolActionDynamic.Count > 0)
            {
                var btnSearch = lstToolActionDynamic.Where(m => m.ACT_TYP == "C002").FirstOrDefault();              
                if (pageSetting.SEARCH_YN == true && btnSearch != null)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "Search";
                    info.ID = "btnSearch";
                    info.MenuID = MenuID;
                    info.Icon = "<i class='fa fa-search'></i>";
                    info.Sort = 5;
                    info.Action = "RUN_" + btnSearch.ACT_ID + "_" + btnSearch.PAG_ID + "();";
                    listInfo.Add(info);
                }
                var btnCreate = lstToolActionDynamic.Where(m => m.ACT_TYP == "C001").FirstOrDefault();
                if (pageSetting.CREATE_YN == true && btnCreate != null)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "Create";
                    info.ID = "btnCreate";
                    info.MenuID = MenuID;
                    info.Sort = 6;
                    info.Action = "RUN_" + btnCreate.ACT_ID + "_" + btnCreate.PAG_ID + "();";
                    listInfo.Add(info);
                }
                var btnSave = lstToolActionDynamic.Where(m => m.ACT_TYP == "C003").FirstOrDefault();
                if (pageSetting.SAVE_YN == true && btnSave != null)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "Save";
                    info.ID = "btnSave";
                    info.MenuID = MenuID;
                    info.Sort = 7;
                    info.Icon = "<i class='fa fa-save'></i>";
                    info.Action = "RUN_" + btnSave.ACT_ID + "_" + btnSave.PAG_ID + "();";
                    listInfo.Add(info);
                }
                //var btnEdit = lstToolActionDynamic.Where(m => m.ACT_TYP == "C003").FirstOrDefault();
                //if (pageSetting.EDIT_YN == true && toolbar.EDIT_YN == true)
                //{
                //    ToolbarInfo info = new ToolbarInfo();
                //    info.Name = "Edit";
                //    info.ID = "btnEdit";
                //    info.MenuID = MenuID;
                //    info.Icon = "<i class='fa fa-edit'></i>";
                //    info.Action = "RUN_" + btnEdit.ACT_ID + "_" + btnEdit.PAG_ID + "();";
                //    listInfo.Add(info);
                //}
                var btnDelete = lstToolActionDynamic.Where(m => m.ACT_TYP == "C004").FirstOrDefault();
                if (pageSetting.DELETE_YN == true && btnDelete != null)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "Delete";
                    info.ID = "btnDelete";
                    info.MenuID = MenuID;
                    info.Sort = 8;
                    info.Icon = "<i class='fa fa-trash'></i>";
                    info.Action = "RUN_" + btnDelete.ACT_ID + "_" + btnDelete.PAG_ID + "();";
                    listInfo.Add(info);
                }
                var btnAddrow = lstToolActionDynamic.Where(m => m.ACT_TYP == "C005").FirstOrDefault();
                if (btnAddrow != null)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "AddRow";
                    info.ID = "btnAddRow";
                    info.MenuID = MenuID;
                    info.Sort = 9;
                    info.Icon = "<i class='fas fa-plus'></i>";
                    info.Action = "RUN_" + btnAddrow.ACT_ID + "_" + btnAddrow.PAG_ID + "();";
                    listInfo.Add(info);
                }
                var btnDeleterow = lstToolActionDynamic.Where(m => m.ACT_TYP == "C006").FirstOrDefault();
                if (btnDeleterow != null)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "DeleteRow";
                    info.ID = "btnDeleteRow";
                    info.MenuID = MenuID;
                    info.Sort = 10;
                    info.Icon = "<i class='fas fa-minus'></i>";
                    info.Action = "RUN_" + btnDeleterow.ACT_ID + "_" + btnDeleterow.PAG_ID + "();";
                    listInfo.Add(info);
                }
                var btnExport = lstToolActionDynamic.Where(m => m.ACT_TYP == "C007").FirstOrDefault();
                if (pageSetting.EXCEL_YN == true && btnExport != null)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "Export Excel";
                    info.ID = "btnExcel";
                    info.MenuID = MenuID;
                    info.Sort = 11;
                    info.Icon = "<i class='fas fa-download'></i>";
                    info.Action = "RUN_" + btnExport.ACT_ID + "_" + btnExport.PAG_ID + "();";
                    listInfo.Add(info);
                }
                var btnPrint = lstToolActionDynamic.Where(m => m.ACT_TYP == "C008").FirstOrDefault();
                if (pageSetting.PRINT_YN == true && btnPrint != null)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "Print";
                    info.ID = "btnPrint";
                    info.MenuID = MenuID;
                    info.Sort = 14;
                    info.Icon = "<i class='fa fa-print'></i>";
                    info.Action = "RUN_" + btnPrint.ACT_ID + "_" + btnPrint.PAG_ID + "();";
                    listInfo.Add(info);
                }
                var btnRedirect = lstToolActionDynamic.Where(m => m.ACT_TYP == "C010").FirstOrDefault();
                if (btnRedirect != null)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "Redirect";
                    info.ID = "btnRedirect";
                    info.MenuID = MenuID;
                    info.Sort = 13;
                    info.Icon = "<i class='fas fa-directions'></i>";
                    info.Action = "RUN_" + btnRedirect.ACT_ID + "_" + btnRedirect.PAG_ID + "();";
                    listInfo.Add(info);
                }
                var btnUploadExcel = lstToolActionDynamic.Where(m => m.ACT_TYP == "C011").FirstOrDefault();
                if (pageSetting.UPLOAD_FILE_YN == true && btnUploadExcel != null)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "Upload Excel";
                    info.ID = "btnUploadExcel";
                    info.MenuID = MenuID;
                    info.Sort = 12;
                    info.Icon = "<i class='fas fa-upload'></i>";
                    info.Action = "RUN_UPLOAD_EXCEL_" + btnUploadExcel.ACT_ID + "_" + btnUploadExcel.PAG_ID + "();";
                    listInfo.Add(info);
                }
            }
            else
            {
                if (pageSetting.SEARCH_YN == true && toolbar.SEARCH_YN == true)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "Search";
                    info.ID = "btnSearch";
                    info.MenuID = MenuID;
                    info.Sort = 5;
                    info.Icon = "<i class='fa fa-search'></i>";
                    listInfo.Add(info);
                }
                if (pageSetting.CREATE_YN == true && toolbar.CREATE_YN == true)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "Create";
                    info.ID = "btnCreate";
                    info.MenuID = MenuID;
                    info.Sort = 6;
                    info.Icon = "<i class='fa fa-pen'></i>";
                    listInfo.Add(info);
                }
                if (pageSetting.SAVE_YN == true && toolbar.SAVE_YN == true)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "Save";
                    info.ID = "btnSave";
                    info.MenuID = MenuID;
                    info.Sort = 7;
                    info.Icon = "<i class='fa fa-save'></i>";
                    listInfo.Add(info);
                }
                if (pageSetting.EDIT_YN == true && toolbar.EDIT_YN == true)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "Edit";
                    info.ID = "btnEdit";
                    info.MenuID = MenuID;
                    info.Sort = 8;
                    info.Icon = "<i class='fa fa-edit'></i>";
                    listInfo.Add(info);
                }

                if (pageSetting.DELETE_YN == true && toolbar.DELETE_YN == true)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "Delete";
                    info.ID = "btnDelete";
                    info.MenuID = MenuID;
                    info.Sort = 9;
                    info.Icon = "<i class='fa fa-trash'></i>";
                    listInfo.Add(info);
                }
                if (pageSetting.EXCEL_YN == true && toolbar.EXCEL_YN == true)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "Export Excel";
                    info.ID = "btnExcel";
                    info.MenuID = MenuID;
                    info.Sort = 10;
                    info.Icon = "<i class='fas fa-download'></i>";
                    listInfo.Add(info);
                }            
                if (pageSetting.PRINT_YN == true && toolbar.PRINT_YN == true)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "Print";
                    info.ID = "btnPrint";
                    info.MenuID = MenuID;
                    info.Sort = 12;
                    info.Icon = "<i class='fa fa-print'></i>";
                    listInfo.Add(info);
                }
                //Add test
                if (pageSetting.IMPORT_EXCEL_YN == true && toolbar.IMPORT_EXCEL_YN == true)
                {
                    ToolbarInfo info = new ToolbarInfo();
                    info.Name = "Import Excel";
                    info.ID = "btnImportExcel";
                    info.MenuID = MenuID;
                    info.Sort = 11;
                    info.Icon = "<i class='fas fa-upload'></i>";
                    listInfo.Add(info);
                }
            }            
            if(lstNewToolbar != null && lstNewToolbar.Count > 0)
            {
                foreach(var item in lstNewToolbar)
                {
                    item.MenuID = MenuID;
                }
                listInfo.AddRange(lstNewToolbar);
            }
            listInfo = listInfo.OrderBy(m => m.Sort).ToList();
            return listInfo;
        }
        protected static SYMenuAccess SetAccessToolbar(SYLoggedUser userInfo,int MenuID)
        {                      
            var toolbar = new SYMenuAccess();
            toolbar.MENU_ID = MenuID;
            if(userInfo.UserType == "G000C001" || userInfo.UserType == "G000C002")
            {
                toolbar.SEARCH_YN = true;
                toolbar.CREATE_YN = true;
                toolbar.SAVE_YN = true;
                toolbar.EDIT_YN = true;
                toolbar.DELETE_YN = true;
                toolbar.EXCEL_YN = true;
                toolbar.PRINT_YN = true;
                toolbar.IMPORT_EXCEL_YN = true;
            }
            else
            {
                var listMenuToolbar = userInfo.MenuAccessList.Where(m => m.MENU_ID == MenuID).ToList();
                toolbar.SEARCH_YN = listMenuToolbar.Where(m => m.SEARCH_YN == true).ToList().Count > 0 ? true : false;
                toolbar.CREATE_YN = listMenuToolbar.Where(m => m.CREATE_YN == true).ToList().Count > 0 ? true : false;
                toolbar.SAVE_YN = listMenuToolbar.Where(m => m.SAVE_YN == true).ToList().Count > 0 ? true : false;
                toolbar.EDIT_YN = listMenuToolbar.Where(m => m.EDIT_YN == true).ToList().Count > 0 ? true : false;
                toolbar.DELETE_YN = listMenuToolbar.Where(m => m.DELETE_YN == true).ToList().Count > 0 ? true : false;
                toolbar.EXCEL_YN = listMenuToolbar.Where(m => m.EXCEL_YN == true).ToList().Count > 0 ? true : false;
                toolbar.PRINT_YN = listMenuToolbar.Where(m => m.PRINT_YN == true).ToList().Count > 0 ? true : false;
                //toolbar.IMPORT_EXCEL_YN = listMenuToolbar.Where(m => m.IMPORT_EXCEL_YN == true).ToList().Count > 0 ? true : false;
                toolbar.IMPORT_EXCEL_YN = true;

            }

            return toolbar;
        }

    }
}
