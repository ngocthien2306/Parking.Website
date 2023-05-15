const METHOD = {
    POST: "POST",
    GET: "GET",
    PUT: "PUT",
    DELETE: "DELETE"
}

function LoadGridData(url, obj, gridId, method) {
    $.ajax({
        url: url,
        type: method,
        data: obj,
        dataType: 'json',
        success: function (result) {
            debugger;
            GetGridIntance(gridId).option("dataSource", result)
            LoadingPage(0);
        },
        error: function () {
            LoadingPage(0);
        }
    });
}

function CRUDData(url, obj, method, closePopup=true) {
    $.ajax({
        url: url,
        type: method,
        data: obj,
        dataType: 'json',
        async: false,
        success: function (result) {
            debugger;
            if (result.Success) {
                DevExpress.ui.dialog.alert(result.Message, "Notice");
                if (closePopup == true) {
                    $('#modalControl').modal('hide');
                } 
            }
            else {
                DevExpress.ui.dialog.alert(result.Message, "Error");
            }
            LoadingPage(0);
        },
        error: function () {
            LoadingPage(0);
        }
    });
}

function OpenPopup(url, obj, method) {
    debugger;
    $.ajax({
        url: url,
        type: method,
        data: obj,
        dataType: "html",
        success: function (result) {
            // parent popup
            $("#modalContent").removeClass("modal-md");
            $("#modalContent").html(result);
            $("#modalContent").addClass("modal-xl");
            $('#modalControl').modal('show');
            LoadingPage(0);
        }
    });
}
function LoadDataCommboBox(groupCode, url, elementId, initData) {
    $.ajax({
        url: url,
        type: 'GET',
        data:
        {
            code: groupCode,
            subCode: null,
            status: true,
        },
        dataType: 'json',
        success: function (result) {
            debugger;
            //const data = result.filter(d => d.commonCode == groupCode);
            if (initData === true) {
                $(elementId).dxSelectBox({
                    dataSource: result,
                    displayExpr: "commonSubName1",
                    valueExpr: "commonSubCode",
                    value: result[0].commonSubCode
                })
            }
            else {
                $(elementId).dxSelectBox({
                    dataSource: result,
                    displayExpr: "commonSubName1",
                    valueExpr: "commonSubCode"
                })
            }

            LoadingPage(0);
        },
        error: function () {
            LoadingPage(0);
        }
    });
}

function GetGridIntance(elemendId) {
    return $(elemendId).dxDataGrid("instance");
}

// Search extension
const searchStoreData = (no, location, storeName) => {
    return {
        storeNo: no,
        location: location,
        storeName: storeName
    }
}

// Validation extension
function SetDataElement(elementId, data) {
    var type = $(elementId).data().dxComponents[0];
    switch (type) {
        case "dxNumberBox":
            $(elementId).dxNumberBox('instance').option("value", data);
            break;
        case "dxSelectBox":
            $(elementId).dxSelectBox('instance').option("value", data);
            break;
        case "dxTextBox":
            $(elementId).dxTextBox('instance').option("value", data);
            break;
        case "dxDateBox":
            $(elementId).dxDateBox('instance').option("value", data);
            break;
        case "dxRadioGroup":
            $(elementId).dxRadioGroup('instance').option("value", data)
            break;
        case "dxTextArea":
            $(elementId).dxTextArea('instance').option("value", data)
            break;
        case "dxSwitch":
            $(elementId).dxSwitch('instance').option("value", data);
            break;
        default:
            console.log("Not thing");
    }
}
function GetDataElement(elementId) {
    var type = $(elementId).data().dxComponents[0];
    var data;
    switch (type) {
        case "dxSelectBox":
            data = $(elementId).dxSelectBox('instance').option("value");
            break;
        case "dxNumberBox":
            data = $(elementId).dxNumberBox('instance').option("value");
            break;
        case "dxTextBox":
            data = $(elementId).dxTextBox('instance').option("value");
            break;
        case "dxDateBox":
            data = ParsingDateyyyyMMdd($(elementId).dxDateBox('instance').option("value"));
            break;
        case "dxRadioGroup":
            data = $(elementId).dxRadioGroup('instance').option("value");
            break;
        case "dxTextArea":
            data = $(elementId).dxTextArea('instance').option("value");
            break;
        case "dxSwitch":
            data = $(elementId).dxSwitch('instance').option("value");
            break;
        default:
            data = null;
    }
    return data
}

function ValidateElement(elementId) {
    var type = $(elementId).data().dxComponents[0];
    switch (type) {
        case "dxNumberBox":
            $(`${elementId}`).dxNumberBox('instance')._$element[0].style.borderColor = "red";
            setTimeout(function () {
                $(`${elementId}`).dxNumberBox("focus");
            }, 2000)
            break;
        case "dxSelectBox":
            $(`${elementId}`).dxSelectBox('instance')._$element[0].style.borderColor = "red";
            setTimeout(function () {
                $(`${elementId}`).dxSelectBox("focus");
            }, 2000)
            break;
        case "dxTextBox":
            $(`${elementId}`).dxTextBox('instance')._$element[0].style.borderColor = "red";
            setTimeout(function () {
                $(`${elementId}`).dxTextBox("focus");
            }, 2000)
            break;
        case "dxDateBox":
            $(`${elementId}`).dxDateBox('instance')._$element[0].style.borderColor = "red";
            setTimeout(function () {
                $(`${elementId}`).dxDateBox("focus");
            }, 2000)
            break;
        case "dxTextArea":
            $(`${elementId}`).dxTextArea('instance')._$element[0].style.borderColor = "red";
            setTimeout(function () {
                $(`${elementId}`).dxTextArea("focus");
            }, 2000)
            break;
        case "dxRadioGroup":
            $(`${elementId}`).dxRadioGroup('instance')._$element[0].style.borderColor = "red";
            setTimeout(function () {
                $(`${elementId}`).dxTextArea("focus");
            }, 2000)
            break;
        default:
            console.log("");
    }
}



function RemoveColorElement(elementId) {
    var type = $(elementId).data().dxComponents[0];
    switch (type) {
        case "dxNumberBox":
            $(`${elementId}`).dxNumberBox('instance')._$element[0].style.borderColor = "";
            break;
        case "dxSelectBox":
            $(`${elementId}`).dxSelectBox('instance')._$element[0].style.borderColor = "";
            break;
        case "dxTextBox":
            $(`${elementId}`).dxTextBox('instance')._$element[0].style.borderColor = "";
            break;
        case "dxDateBox":
            $(`${elementId}`).dxDateBox('instance')._$element[0].style.borderColor = "";
            break;
        case "dxTextArea":
            $(`${elementId}`).dxTextArea('instance')._$element[0].style.borderColor = "";
            break;
        case "dxRadioGroup":
            $(`${elementId}`).dxRadioGroup('instance')._$element[0].style.borderColor = "";
            break;
        default:
            console.log("");
    }
}

// convert datetime
function padTo2Digits(num) {
    return num.toString().padStart(2, '0');
}

function formatDate(elementId) {
    var date = $(elementId).dxDateBox('instance').option("value");
    if (date == null || date == '' || typeof date == undefined) {
        return null;
    }
    return (
        [
            date.getFullYear(),
            padTo2Digits(date.getMonth() + 1),
            padTo2Digits(date.getDate()),
        ].join('-') +
        ' ' +
        [
            padTo2Digits(date.getHours()),
            padTo2Digits(date.getMinutes()),
            padTo2Digits(date.getSeconds()),
        ].join(':')
    );
}
