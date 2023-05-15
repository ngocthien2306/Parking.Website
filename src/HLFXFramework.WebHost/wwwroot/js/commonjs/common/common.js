//CommonClass
var hlboot = {};


function ParsingDateyyyyMMdd(value) {
    if (value !== null && value !== "" && value !== undefined) {
        var dateFormat = new Date(value);
        var dateString = new Date(dateFormat.getTime() - (dateFormat.getTimezoneOffset() * 60000)).toISOString().split("T")[0];
        return dateString;
    }
    else {
        return null;
    }
}
function GetStartDateDefaultOfDynamicPage(value) {
    switch (value) {
        case 'FirstDateOfYear':
            return new Date(new Date().getFullYear(), 0, 1);
        case 'FirstDateOfMonth':
            return new Date(new Date().getFullYear(), new Date().getMonth(), 1);
        default:
            return new Date(new Date().getFullYear(), new Date().getMonth(), 1);
    }
}
function ValidateRangeDate(startDate, endDate) {

    if ((startDate === null || startDate === undefined) && (endDate === null || endDate === undefined)) {
        return true;
    }
    if (startDate === null || startDate === undefined) {
        DevExpress.ui.dialog.alert("Please select StartDate!", "Error");
        return false;
    }
    if (endDate === null || endDate === undefined) {
        DevExpress.ui.dialog.alert("Please select EndDate!", "Error");
        return false;
    }
    if (startDate > endDate) {
        DevExpress.ui.dialog.alert("StartDate cannot higher than EndDate!", "Error");
        return false;
    }
    return true;
}

//function export excel
function ExportExcelCommon(dataGrid, worksheetname) {
   
    var dateFormat = new Date();
    var dateString = new Date(dateFormat.getTime() - (dateFormat.getTimezoneOffset() * 60000)).toISOString().split("T")[0];
    let fileName = menuObject.MenuName + "_" + dateString;
    //Note: count page which have grid and choose grid which need export
    var workbook = new ExcelJS.Workbook();
    var worksheet = workbook.addWorksheet(worksheetname);
    //worksheet.columns = [
    //    { width: 5 }, { width: 30 }, { width: 25 }, { width: 15 }, { width: 25 }, { width: 40 }
    //];

    $(dataGrid).dxDataGrid({
        export: {
            enabled: true,
        },
        onExporting: function (e) {
            var workbook = new ExcelJS.Workbook();
            var worksheet = workbook.addWorksheet(worksheetname, {
                headerFooter: { firstHeader: "", firstFooter: "Copyright by Pleiger" }
            });

            DevExpress.excelExporter.exportDataGrid({
                component: e.component,
                worksheet: worksheet,
                customizeCell: function (options) {
                    var { gridCell, excelCell } = options;

                    if (gridCell.rowType === "header") {
                        excelCell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: "FFFF00" } };  
                    }
                   
                }
                }).then(function () {
                    workbook.xlsx.writeBuffer().then(function (buffer) {
                        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), `${fileName}.xlsx`);
                    });
                });
            e.cancel = true;
        }
    });
    if (dataGrid[0].id != undefined) {
        $('#' + dataGrid[0].id + ' .dx-datagrid-export-button').trigger("click");
        $('#' + dataGrid[0].id + ' .dx-datagrid-header-panel').attr('style', 'display:none');
    }
    else {
        $(dataGrid + ' .dx-datagrid-export-button').trigger("click");
        $(dataGrid + ' .dx-datagrid-header-panel').attr('style', 'display:none');
    }
   
}

function ExportExcelCommonDataSelected(dataGrid, worksheetname) {
    var dateFormat = new Date();
    var dateString = new Date(dateFormat.getTime() - (dateFormat.getTimezoneOffset() * 60000)).toISOString().split("T")[0];
    let fileName = menuObject.MenuName + "_" + dateString;
    var workbook = new ExcelJS.Workbook();
    var worksheet = workbook.addWorksheet(worksheetname);

    $(dataGrid).dxDataGrid({
        export: {
            enabled: true,
            allowExportSelectedData: true
        }
        ,
        onExporting: function (e) {
            var workbook = new ExcelJS.Workbook();
            var worksheet = workbook.addWorksheet(worksheetname, {
                headerFooter: { firstHeader: "", firstFooter: "Copyright by Pleiger" }
            });

            DevExpress.excelExporter.exportDataGrid({
                component: e.component,
                worksheet: worksheet,
                customizeCell: function (options) {
                    var { gridCell, excelCell } = options;

                    if (gridCell.rowType === "header") {
                        excelCell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: "FFFF00" } };  
                    }
                   
                }
                }).then(function () {
                    workbook.xlsx.writeBuffer().then(function (buffer) {
                        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), `${fileName}.xlsx`);
                    });
                });
            e.cancel = true;
        }
    });
    $('.dx-datagrid-export-button').trigger("click");
    $('.dx-datagrid-header-panel').attr('style', 'display:none');
}


function LoadingPage(d) {
    if (d == 1) {
        loadAjaxMask();
    } else {
        closeAjaxMask();
    }
}

function loadAjaxMask() {
    $.blockUI({
        message: '<div class="div-blockui">' +
            '<img src="/images/loading_1.gif" class="img-blockui" /><p class="txt-blockui"><span style="float:left">Please wait...</span><br />Loading data.</p>' +
            '</div>',
        css: {
            backgroundColor: '#fff',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: 1,
            color: 'black',
            width: '20%',
            left: '40%',
            border: '1px solid #ced4da',
            'border-radius': '5px'
        }
    });
}

function closeAjaxMask() {
    $.unblockUI();
}

function getLanguages() {
    let cookie = {};
    document.cookie.split(';').forEach(function (el) {
        let [k, v] = el.split('=');
        cookie[k.trim()] = v;
    })
    var lang = '/en';
    if (cookie["langname"] !== undefined) {
        lang = '/' + cookie["langname"];
    }
    if (cookie["langname"] !== undefined && cookie["langname"] == "ko") {
        $('#btnlanguage').html("KR")
    }
    if (cookie["langname"] !== undefined && cookie["langname"] == "en") {
        $('#btnlanguage').html("EN")
    }
    return lang;
}



var MES_ComCodeMst = [], MES_ComCodeDtls = [], MES_Item = [], MES_Item_Raw = [], MES_AllMenu = []


$.ajax({
    url: getLanguages() + '/Common/GetAllComCodeDTL',
    data: { Lang: getLanguages().replace('/', '') },
    type: 'GET',
    dataType: 'json'
}).done(function (result) {
    MES_ComCodeDtls = result;
});


$.ajax({
    url: getLanguages() + '/Common/GetAllItem',
    data: { Lang: getLanguages().replace('/', '') },
    type: 'GET',
    dataType: 'json'
}).done(function (result) {
    MES_Item = result;
});


function RecalculateResize(per, style, iddiv, param) {
    //console.log("---------RecalculateResize------------", param, " per ", per , " style ", style);
    if (style == 'width') {
        let WidthTotal = 0;

        WidthTotal = $(`.${param}`).outerWidth();
        //console.log("HeightTotal f ", WidthTotal);
        if (WidthTotal != undefined && WidthTotal != null && WidthTotal != "") {
            //console.log("---------ok-----------");
        }
        else {
            WidthTotal = $(`#${param}`).outerWidth();
            //console.log("HeightTotal s ", WidthTotal);
        }
        //console.log("WidthTotal s ", WidthTotal);
        let tmp = per * WidthTotal;
        //console.log("---------Width tmp------------", tmp);
        $(`#${iddiv}`).width(tmp);
    }
    else {
        let HeightTotal = 0;
        // 
        HeightTotal = $(`.${param}`).outerHeight();
        //console.log("HeightTotal f ", HeightTotal);
        if (HeightTotal != undefined && HeightTotal != null && HeightTotal != "") {
            //console.log("---------ok-----------");
        }
        else {
            HeightTotal = $(`#${param}`).outerHeight();
            //console.log("HeightTotal s ", HeightTotal);
        }
        //console.log("HeightTotal s ", HeightTotal);
        let tmp = per * HeightTotal;
        //console.log("---------Height tmp------------", tmp);
        //alert(style);
        //document.getElementById(iddiv).style.height = tmp;
        $(`#${iddiv}`).height(tmp);
    }
    //console.log("---------End RecalculateResize------------");
}

//function recalculate resize  
function ReCalResize(divrecal, typediv, divtotal, divfirst, type) {
    // 
    //divrecal la ten "Class" hoac "ID" cua the DIV can tinh resize
    //typediv định nghĩa "divrecal" truyen vào là "class" hay là "ID" cua thẻ DIV
    //divtotal la ten "class" hoa "ID" cua the DIV lớn
    //divfirst la ten "class" hoa "ID" cua the DIV nhỏ
    //type định nghĩa đang tính "Height" hay la "Width"
    
   
    if (type == "width") {
       // console.log("---------ReCalResize width------------");
       
        let WidthTotal = 0;
        // 
        WidthTotal = $(`.${divtotal}`).outerWidth();
        if (WidthTotal != undefined && WidthTotal != null && WidthTotal != "") { 
        }
        else {
            WidthTotal = $(`#${divtotal}`).outerWidth();
        }

        let WidthFirst = 0;
        WidthFirst = $(`.${divfirst}`).outerWidth();
        if (WidthFirst != undefined && WidthFirst != null && WidthFirst != "") {
        }
        else {
            WidthFirst = $(`#${divfirst}`).outerWidth();
        }
        let SizeDivRecal = WidthTotal - WidthFirst;
        
        if (typediv == "class") {
            $(`.${divrecal}`).width(SizeDivRecal);
        }
        else {
            $(`#${divrecal}`).width(SizeDivRecal);
        }
        //console.log("divtotal ", divtotal, " ", WidthTotal);
        //console.log("divfirst ", divfirst, " ", WidthFirst);
        //console.log("divrecal ", divrecal, " ", SizeDivRecal);

    }
    else {
        //console.log("---------ReCalResize height------------");

        let HeightTotal = 0;
        HeightTotal = $(`.${divtotal}`).outerHeight();
        
        if (HeightTotal != undefined && HeightTotal != null && HeightTotal != "" && HeightTotal != 0) {
            
        }
        else {
            HeightTotal = $(`#${divtotal}`).outerHeight();
        }

        let HeightFirst = 0;
        HeightFirst = $(`.${divfirst}`).outerHeight();
        if (HeightFirst != undefined && HeightFirst != null && HeightFirst != "") {
        }
        else {
            HeightFirst = $(`#${divfirst}`).outerHeight();
        }
        let SizeDivRecal = HeightTotal - HeightFirst;

        if (typediv == "class") {
            $(`.${divrecal}`).height(SizeDivRecal);
        }
        else {
            $(`#${divrecal}`).height(SizeDivRecal);
        }
        //console.log("divtotal ", divtotal, " ", HeightTotal);
        //console.log("divfirst ", divfirst, " ", HeightFirst);
        //console.log("divrecal ", divrecal, " ", SizeDivRecal);
        //console.log("---------End ReCalResize height------------");
    }
}

$.ajax({
    url: getLanguages() + '/Common/GetItemRaw',
    data: { Lang: getLanguages().replace('/', '') },
    type: 'GET',
    dataType: 'json'
}).done(function (result) {
    MES_Item_Raw = result;
});

//function search system
function fnSearchSys(method, keyword) {
    var data = {};
    switch (method) {
        case 'SaleProject':
            data[ProjectCode] = keyword;
            data[UserProjectCode] = keyword;
            data[ProjectName] = keyword;
            data[ItemCode] = keyword;
            data[ItemName] = keyword;
            data[PartnerName] = keyword;
            data[OrderTeamCode] = keyword;
            $.ajax({
                url: getLanguages() + '/Common/SearchSystem',
                type: 'GET',
                data: {
                    data: data,
                    Method: method
                },
                datatype: 'json',
                success: function (result) {
                    if (result.Success) {
                        
                    }
                }
            });
            break;
        case '':
            break;
        case '':
            break;
        default:
            break;

            

    }
}


hlboot.GRID_PAGING_TYPE = [
    { ID: "1", Name: "Paging" },
    { ID: "0", Name: "No Paging" },
];