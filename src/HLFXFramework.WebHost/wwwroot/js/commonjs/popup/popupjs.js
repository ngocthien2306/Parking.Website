/* eslint-disable no-debugger */;

// Generate Popup Dynamic in Layout
function GeneratePopupDynamic(pageID, data) {
    $("#divModal").append('<div id="divPopUp' + pageID + '"></div>');
    $.each(data, function (index, item) {
        let popupID = item.PAG_ID;
        let html = '';
        $.ajax({
            url: getLanguages() + '/DynamicMagt/GeneratePopup',
            type: "GET",
            data: { pageID: popupID },
            dataType: "html",
            async: false,
            success: function (result) {
                html = result;
            }
        });
        $("#divPopUp" + pageID).prepend(html);
    });
}
function GeneratePopupCustom(pageID, data) {
    $("#divModal").append('<div id="divPopUp' + pageID + '"></div>');
    $.each(data, function (index, item) {
        let html = '';
        $.ajax({
            url: getLanguages() + '/DynamicMagt/GeneratePopupCustom',
            type: "GET",
            data: { viewPage: item },
            traditional: true,
            dataType: "html",
            async: false,
            success: function (result) {
                html = result;
            }
        });
        $("#divPopUp" + pageID).prepend(html);
    });
}

// Remove class size popup
function RemoveClassPopup() {
    $("#modalContent").removeClass("modal-lg");
    $("#modalContent").removeClass("modal-md");
    $("#modalContent").removeClass("modal-sm");
}

// Show popup User Information
function ShowPopupUserInformation() {
    $.ajax({
        url: getLanguages() + '/User/ShowPopupUserInformation',
        type: 'GET',
        dataType: 'html',
        success: function (result) {
            $("#modalUserContent").html(result);
            $("#modalUserContent").addClass('modal-md');
            $('#modalUserControl').modal('show');
        }
    });
}