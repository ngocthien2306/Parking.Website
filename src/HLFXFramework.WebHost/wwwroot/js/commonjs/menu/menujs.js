/* eslint-disable no-debugger */;

// De-active all top menu
function DeactiveTopMenu() {
    $(".navbar-nav .nav-link").removeClass("active");
}

// Active Top Menu
function ActiveTopMenu(id) {
    $(".navbar-nav .nav-link[menu-id='" + id + "']").addClass("active");
}

// Active Left Menu
function ActiveLeftMenu(id) {
    $(".nav .nav-item .nav-link[menu-id='" + id + "']").addClass("active");
}

// De-active all Left Menu
function DeactiveLeftMenu() {
    $(".nav .nav-item .nav-link").removeClass("active");
}

// Open Parent Menu
function OpenParentMenu() {
    var listParentMenu = $(".has-treeview");
    if (listParentMenu.length > 0) {
        $.each(listParentMenu, function () {
            let menu = $(this).find(".parent-link")[0];
            let menuID = $(menu).attr("menu-id");
            let listMenuChild = $(this).find(".nav-treeview")[0];

            let isExist = parentMenuID.includes(parseInt(menuID));
            if (isExist) {
                $(menu).addClass("active");
                $(this).addClass("menu-open");
                $(listMenuChild).css("display", "block");
            }
            else {
                $(menu).removeClass("active");
                $(this).removeClass("menu-open");
                $(listMenuChild).css("display", "none");
            }
        });
    }
}

// Active Menu
function ActiveMenu(id) {
     
    parentMenuID = [];
    var menu = listMenuObject.find(x => x.MenuID == id);
    menuObject = menu;

    if (siteSettingObject.MenuType === "TopLeft") {
        if (menu.MenuLevel == 1) {
            DeactiveTopMenu();
            ActiveTopMenu(menu.MenuID);
            ClearLeftMenu();
        }
        else {
            var parentMenu = SecursiveParentMenu(menu.MenuParentID);
            GetListLeftMenu(parentMenu.MenuID, 0);
            ActiveLeftMenu(id);
            if (menu.MenuLevel == 3) {
                OpenParentMenu();
                ActiveLeftMenu(menu.MenuParentID);
            }

            DeactiveTopMenu();
            ActiveTopMenu(parentMenu.MenuID);
            return;
        }
    }
    else {
        DeactiveLeftMenu();

        if (menu.MenuLevel == 1) {
            ActiveLeftMenu(menu.MenuID);
        }
        else {
            ActiveLeftMenu(menu.MenuID);
            SecursiveParentMenu(menu.MenuParentID);
        }
        OpenParentMenu();
    }
}

// Clear Left Menu
function ClearLeftMenu() {
    $("#divLeftMenu").html(null);
}

// Recursive Parent Menu
function SecursiveParentMenu(parentID) {
    var menu = listMenuObject.find(x => x.MenuID == parentID);
    if (menu !== 'undefined') {
        if (siteSettingObject.MenuType === "TopLeft") {
            if (menu.MenuLevel == 1) {
                parentMenuID.push(menu.MenuID);
                return menu;
            }
            else {
                parentMenuID.push(menu.MenuID);
                return SecursiveParentMenu(menu.MenuParentID);
            }
        }
        else {
            parentMenuID.push(menu.MenuID);
            if (menu.MenuLevel == 1) {
                return;
            }
            else {
                SecursiveMenu(menu.MenuParentID);
            }
        }
    }
}

// Show Left Menu
function ShowLeftMenu(obj) {
     
    CheckSession();
    let menuID = $(obj).attr("menu-id");
    idMenuActive = menuID;
    GetListLeftMenu(menuID, 1);
}

// Get list Left Menu
function GetListLeftMenu(id, isDirect) {
     
    $.ajax({
        url: getLanguages() + '/MenuLogin/LoadLeftMenuComponent',
        type: 'GET',
        data: {
            menuID: id
        },
        async: false,
        dataType: "html",
        success: function (result) {
             
            $("#divLeftMenu").html(result);
            if (isDirect == 1) {
                DeactiveTopMenu();
                ActiveTopMenu(id);        

            }
        }
    });
      
   
    // Quan add 2020-11-27
    // Quan add Set tabActive default   
    //var flagActiveMenu = true;
    //if (id != null && localStorage.getItem("menuIDActiveFirst") == "false")
    //{
    //    flagActiveMenu = false;
    //} 
 
    //if (flagActiveMenu) {
    //    $.ajax({
    //        url: getLanguages() + '/MenuLogin/GetTabActiveDefault',
    //        type: 'GET',
    //        data: {
    //            menuID: id
    //        },
    //        async: false,
    //        dataType: "json",
    //        success: function (result) {
    //            if (isDirect == 1 && result.length > 0) {
    //                GenerateTab(result[0].MenuNameActivefirst, result[0].MenuPathActivefirst, result[0].MenuIDActivefirst, false);
    //                 
    //                //localStorage.clear();
    //                if (id != null)
    //                {
    //                    localStorage.setItem('menuIDActiveFirst', 'false');
    //                }                   
    //            }
    //        }
    //    });
    //}    
    // End Quan add
}

