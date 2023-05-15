/* eslint-disable no-debugger */;
var menuIDoTabActive = -1;
// Tab Devextreme
// Generate TabPanel
function GenerateTabPanel() {
    let html = '<div id="tabPanel"></div>';
    $(".content-wrapper").find(".content").append(html);

    tabPanel = $("#tabPanel").dxTabPanel({
        itemTitleTemplate: TitleTemplate,
        itemTemplate: ItemTemplate,
        onSelectionChanged: ClickTabItem,
    }).dxTabPanel("instance");
}

// Generate Tab in TabPanel

var idactivetab;
function GenerateTab22(name, url, level, id, parentID) {
    $.blockUI();
    let count = tabDataSource.filter(x => x.title === name).length;
    if (count === 0) {
        var view = '';
        $.ajax({
            url: getLanguages() + url,
            type: "GET",
            async: false,
            dataType: "html",
            success: function (result) {
                view = result;
            }
        })

        tabItem = {
            canClose: true,
            title: name,
            url: getLanguages() + url,
            text: view,
            level: level,
            id: id,
            parentID: parentID
        };
        tabDataSource.push(tabItem);
        UpdateTab();
    }
    else {
        tabItem = tabDataSource.filter(x => x.title === name)[0];
    }

    tabPanel.option("selectedItem", tabItem);
    ActiveMenu(id);

    $.unblockUI();
}

// Update Tab of TabPanel
function UpdateTab() {
     
    if (tabPanel === undefined) {
        GenerateTabPanel();
    }
    tabPanel.option("items", tabDataSource);
}

// Click Tab item 
function ClickTabItem(e) {
    var data = e.addedItems[0];
    ActiveMenu(data.id);
}

// Title Template of tab
function TitleTemplate(itemData, itemIndex, itemElement) {
    if (itemData.canClose == false) {
        itemElement.append(
            $("<i>")
                .addClass("dx-icon")
                .addClass("dx-icon-home"));
    }
    // icon before menu title
    if (itemData.canClose == true) {
        itemElement.append(
            $("<i>").addClass("dx-icon")
                .addClass("dx-icon-file"));
    }

    itemElement.append(
        $("<span>")
            .text(`${itemData.title}`)
    );
    if (itemData.canClose == true) {
        itemElement.append(' ');
        itemElement.append(
            $("<i>")
                .addClass("dx-icon")
                .addClass("dx-icon-close")
                .click(function () { CloseTab(itemData); })
        );
    }
}

// Content of Tab
function ItemTemplate(itemData, itemIndex, itemElement) {
    itemElement.append(itemData.text);
}

// Close Tab
function CloseTab2(itemData) {
    $.blockUI();
    if (!itemData) {
        $.unblockUI();
        return;
    }
    else {
        if (itemData.url.includes("/DynamicMagt/?PageID=")) {
            let start ="/DynamicMagt/?PageID=".length;
            let id = itemData.url.substr(start, itemData.url.length);
            $("#divPopUp" + id).remove();
        }

        var index = tabDataSource.indexOf(itemData);

        tabDataSource.splice(index, 1);
        if (tabDataSource.length > 0) {
            if (index >= tabDataSource.length && index > 0) {
                tabPanel.option("selectedIndex", index - 1);
            }
            tabPanel.option("items", tabDataSource);
        }
        else {
            tabPanel = undefined;
            $("#tabPanel").remove();
        }
    }

    $.unblockUI();
}

//============================================================

// Tab Bootstrap
// Generate Tab
function GenerateTab(name, url, id, isRedirect) {
     
    CheckSession();
    // 
    //CheckMobi(); // call func from layout page
    // 
    //điều kiện nếu mở quá 10 tab => show dialog thong báo và ko cho generate thêm nữa
    var numberOfElement = $("#divTabLink li").length; 
    // 
    let countExistTab = 0;
    var numbertabclose = 10;
    countExistTab = $("#divTabLink").find('li[menu-id="' + id + '"][is-redirect="' + isRedirect + '"]').length;
    if (CheckMobiNew()) {
        numbertabclose = 2;
    }
    // 
    if (countExistTab === 0) {
        if (numberOfElement > numbertabclose) {
            var ob = $("#divTabLink li")[1];
            console.log(ob);
            let menuIDC = $(ob).attr("menu-id");
            $('li[menu-id="' + menuIDC + '"][is-redirect="' + isRedirect + '"]').remove();
            $("#divTab").css("display", "block");

            $("#divTabLink").append(GenerateTabLink(id, name, isRedirect));
            $("#divTabContent").append(GenerateTabContent(id, url, isRedirect));
            ActiveTab(id, isRedirect);
        }
        else {
            $("#divTab").css("display", "block");

            $("#divTabLink").append(GenerateTabLink(id, name, isRedirect));
            $("#divTabContent").append(GenerateTabContent(id, url, isRedirect));
            ActiveTab(id, isRedirect);
        }
        
    }
    else {
        ActiveTab(id, isRedirect);
    }
}



// Get Tab's Content
function GetTabContent(url) {
    let content = '';
    let urlLang = getLanguages() + url;
    console.log(url);
    console.log(urlLang);
     

    //if (url.includes('DynamicMagt')) {
    //    console.log('contain DynamicMagt');
    //    let splitUrl = url.split('?');
    //    console.log(splitUrl);
    //    let urlGetJSFile = getLanguages() + splitUrl[0] + 'JavascriptFromViewString/?' + splitUrl[1];
    //    console.log('urlGetJSFile');
    //    console.log(urlGetJSFile);
    //    $.ajax({
    //        url: urlGetJSFile,
    //        type: "GET",
    //        async: false,
    //        dataType: "html",
    //        success: function (result) {
    //             
    //            content = result;
    //        }
    //    });
    //}

    console.log(content);
    $.ajax({
        url: urlLang,
        type: "GET",
        async: false,
        dataType: "html",
        success: function (result) {
             
            content = result;
        }
    });
    return content;
}

// Generate Tab content
function GenerateTabContent(id, url, isRedirect) {
 

    let html = '';
    html += '<div class="tab-menu-pane" menu-id="' + id + '" is-redirect="' + isRedirect + '" style="display:none">';
    html += GetTabContent(url);
    html += '</div>';
    return html;
}

// De-active Tab
function DeactiveTab() {
     
    $("li").removeClass("active");
    $(".tab-menu-pane").css("display", "none");
}
// Generate Tab header
function GenerateTabLink(id, name, isRedirect) {
     
    // Quan change   
    // Load tab header  Name with db
    // Check multilang
    let menu = listMenuObject.find(x => x.MenuID === parseInt(id));
    if (menu != null && menu != undefined)
    {
        var Languages = getLanguages();
        if (Languages == "/en") {
            name = menu.MenuNameEng;
        }
        else {
            name = menu.MenuName;
        }
    }
  
     
    let canClose = isRedirect === true ? "Y" : menu.IsCanClose;
    let html = '';
    html += '<li class="active" menu-id="' + id + '" is-redirect="' + isRedirect + '" onclick="ActiveTab(' + "'" + id + "'" + ',' + isRedirect + ')" ><div class="tab-template">';
    html += '<a class="tab-link">' + name + '</a>';
    if (canClose === "Y") {
         
        html += '<i class="tab-icon fa fa-times" onclick="CloseTab(this)" menu-id="' + id + '" is-redirect="' + isRedirect + '"></i>';
    }
    html += '</div></li>';
    return html;
}
// Active Tab
function ActiveTab(id, isRedirect) {
    
    //console.log("idactivetab = id; ", idactivetab);
    //case : nếu id có ...mà tablink đó ko còn tồn tại do đã click đóng tab
    //var Tabzero = $("#divTabLink li")[0];
    //var idZero = $(idDashboard).attr("menu-id");
     
    
    if (id !== null && isRedirect !== null) {
        var liClosed = $('li[menu-id="' + id + '"][is-redirect="' + isRedirect + '"]');
        // 
        if ($(liClosed).html() === undefined) {
            return;
        }
    }
    // 
    DeactiveTab();

    menuIDoTabActive = id;
    //idactivetab = id;
    //truong hop ko có id sẽ cho active thằng dashboard lên
    //var dashBoardTab = $("#divTabLink li").first();
   
    //var idDashboard = $(dashBoardTab).attr("menu-id");
    var dashBoardTab = $("#divTabLink li")[0];
    let idDashboard = $(dashBoardTab).attr("menu-id");
    // 
    var li = $('li[menu-id="' + id + '"][is-redirect="' + isRedirect + '"]');
    var tabPane = $('.tab-menu-pane[menu-id="' + id + '"][is-redirect="' + isRedirect + '"]');
    // 
    if (id !== undefined && isRedirect !== undefined) {
        // 
        //nếu có tồn tại 2 thẻ này ..thì mới cho hiện ..còn ko thì cho hien dasboard
        li.addClass("active");
        tabPane.css("display", "block");
    } else {
        // 
        var isRedirect = $(dashBoardTab).attr("is-redirect");
        dashBoardTab.addClass("active");
        $('.tab-menu-pane[menu-id="' + idDashboard + '"][is-redirect="' + isRedirect + '"]').css("display", "block");
        ActiveMenu(idDashboard);
        // 
    }
    //add


    if (isRedirect === false || isRedirect === "false") {
        if (id === undefined) {
            ActiveMenu(idDashboard);
            return;
        }
        ActiveMenu(id);
    } else {
        if (siteSettingObject.MenuType === "TopLeft") {
            DeactiveTopMenu();
            ClearLeftMenu();

        }
        else {
            DeactiveLeftMenu();
        }
    }
}

// Active first Tab
function ActiveFirstTab() {
    let firstTab = $("#divTabLink li").first();
    let id = $(firstTab).attr("menu-id");
    let isRedirect = $(firstTab).attr("is-redirect");

    ActiveTab(id, isRedirect);
}

// Close tab - Bootstrap
function CloseTab(obj) {
     
    let menuID = $(obj).attr("menu-id");
    let isRedirect = $(obj).attr("is-redirect");
    //previous element li
    var prevEle = $(obj).closest("li").prev();
    var prevEleId = $(prevEle).attr("menu-id");
    var prevEleIsRedirect = $(prevEle).attr("is-redirect");
    $('li[menu-id="' + menuID + '"][is-redirect="' + isRedirect + '"]').remove();
    $('.tab-menu-pane[menu-id="' + menuID + '"][is-redirect="' + isRedirect + '"]').remove();
    let countTab = $("#divTabLink").find(".tab-link").length;
    if (countTab === 0) {
        $("#divTab").css("display", "none");
    }
    else {
        if (menuID === menuIDoTabActive.toString()) {
            var dashBoardTab = $("#divTabLink li").first();
            //let id = $(dashBoardTab).attr("menu-id");
            //let tabRedirect = $(dashBoardTab).attr("is-redirect");
            //
            DeactiveTab();
            ActiveTab(prevEleId, prevEleIsRedirect);
        }
            

    }
    
    
}

// Reload tab
function RefreshTab(obj) {
    $.blockUI();
    let menuID = $(obj).attr("menu-id");
    let menu = listMenuObject.find(x => x.MenuID === parseInt(menuID));

    let tabContent = '';
    tabContent = GetTabContent(menu.MenuPath);
    $('.tab-menu-pane[menu-id="' + menuID + '"]').html(null);
    $('.tab-menu-pane[menu-id="' + menuID + '"]').append(tabContent);

    $.unblockUI();

}

// Reload tab by Id Menu
function RefreshTabByIDMenu(id) {
 
    $.blockUI();

    let menu = listMenuObject.find(x => x.MenuID === parseInt(id));

    let tabContent = '';
    tabContent = GetTabContent(menu.MenuPath);
    $('.tab-menu-pane[menu-id="' + menu.MenuID + '"]').html(null);
    $('.tab-menu-pane[menu-id="' + menu.MenuID + '"]').append(tabContent);

    $.unblockUI();
}

// Reload tab When CRUD success
function RefreshTabCRUD(menuID) {
    $.blockUI();

    //let menuID = $(obj).attr("menu-id");
    let menu = listMenuObject.find(x => x.MenuID == parseInt(menuID));

    let tabContent = '';
    tabContent = GetTabContent(menu.MenuPath);
    $('.tab-menu-pane[menu-id="' + menuID + '"]').html(null);
    $('.tab-menu-pane[menu-id="' + menuID + '"]').append(tabContent);

    $.unblockUI();
}

// Reload tab type Redirect
function RefreshTabRedirect(id, url) {
    $.blockUI();

    let tabContent = '';
    tabContent = GetTabContent(url);
    $('.tab-menu-pane[menu-id="' + id + '"]').html(null);
    $('.tab-menu-pane[menu-id="' + id + '"]').append(tabContent);

    $.unblockUI();
}

//====================================Tuan Add===============================
function GenerateTabConmonBoard(name, url, id, newTabYN) {
  
    if (newTabYN == false) {
        ReplaceTabContentCB(id, url)
        //ActiveTabCB(id, isRedirect);
    }
    else {
        let countExistTab = 0;
        countExistTab = $("#divTabLink").find('.tab-link[menu-id="' + id + '"]').length;
        if (countExistTab === 0) {
            $("#divTab").css("display", "block");
            $("#divTabLink").append(GenerateTabLinkCB(id, name, newTabYN));
            $("#divTabContent").append(GenerateTabContentCB(id, url, newTabYN));

            ActiveTabCB(id, newTabYN);
        }
        else {
            ReplaceTabContentCB(id, url, newTabYN)
            ActiveTabCB(id, newTabYN);
        }
    }
}
// Generate Tab header common board
function GenerateTabLinkCB(id, name, isRedirect) {
    // let menu = listMenuObject.find(x => x.MenuID === parseInt(id));
     
    let canClose = "true";
    let html = '';
    html += '<li class="active"><div class="tab-template">';
    html += '<a class="tab-link" menu-id="' + id + '" onclick="ActiveTabCB(' + "'" + id + "'" + ',' + isRedirect + ')">' + name + '</a>';
    html += '<i class="tab-icon fa fa-times" onclick="CloseTab(this)" menu-id="' + id + '"></i>';
    html += '</div></li>';
    return html;
}
// Active Tab
function ActiveTabCB(id, isRedirect) {
    menuIDoTabActive = id;
    DeactiveTab();
    $('.tab-link[menu-id="' + id + '"]').parent().parent().addClass("active");
    $('.tab-menu-pane[menu-id="' + id + '"]').css("display", "block");
    //if (isRedirect === false || isRedirect === "false") {
    //    ActiveMenu(id);
    //} else {
    //    if (siteSettingObject.MenuType === "TopLeft") {
    //        DeactiveTopMenu();
    //        ClearLeftMenu();
    //    }
    //    else {
    //        DeactiveLeftMenu();
    //    }
    //}
}
// Get Tab's Content
function GetTabContentCB(url) {
    let content = '';
    let urlLang = getLanguages() + url;
    $.ajax({
        url: urlLang,
        type: "GET",
        async: false,
        dataType: "html",
        success: function (result) {
            content = result;
        }
    });
    return content;
}

// Generate Tab content
function GenerateTabContentCB(id, url, isRedirect) {
   // $('div[menu-id="' + id + '"]').empty();
    let html = '';
    html += '<div class="tab-menu-pane" menu-id="' + id + '" style="display:none">';
    html += GetTabContentCB(url);
    html += '</div>';
    return html;
}
//Replace Tab content
function ReplaceTabContentCB(id, url) {
    $('div[menu-id="' + id + '"]').empty();
    let html = GetTabContentCB(url);
    $('div[menu-id="' + id + '"]').append(html);
    //html += '<div class="tab-menu-pane" menu-id="' + id + '" style="display:none">';
    //html += 
    //html += '</div>';
    //return html;
}
var arrBack = [];
function PushArrayBack(url) {
    arrBack.push(url);
}
function GetArrayBack() {
    return arrBack.pop();
}