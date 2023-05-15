/* eslint-disable no-debugger */;

// Mapping Object Data
function mappingObjectData(dataObject, listMapDetails) {
    debugger;
    for (var i = 0; i < dataObject.length; i++) {
        $.each(listMapDetails, function (index, element) {
            let tempKey = dataObject[i].Key;
            if (element.MAP_FROM === tempKey) {
                eval('setValue_' + element.MAP_TO + '("' + dataObject[i].Value + '");');
            }
        })
    }
}

// Get Url Parameter
function getUrlParameter() {
    debugger;
    var lstTemp = [];
    var sPageURL = window.location.search.substring(1),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        var param = {
            key: '',
            value: ''
        }
        sParameterName = sURLVariables[i].split('=');
        param.key = sParameterName[0];
        param.value = sParameterName[1];
        lstTemp.push(param);
    }
    return lstTemp;
};