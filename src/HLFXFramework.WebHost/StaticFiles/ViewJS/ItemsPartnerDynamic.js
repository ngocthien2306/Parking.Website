

function FN_Validate_18_11(insertData) {
    var obj = JSON.parse(insertData);
    var rs;

    if (obj[0].PostData.AddedRows.length != 0) {
        $.ajax({
            url: getLanguages() + '/ValidateCRUDData/ItemPartnerValidateCRUD',
            data: { validateData: JSON.stringify(obj[0].PostData.AddedRows) },
            type: 'POST',
            dataType: 'json',
            async: false
        }).done(function (result) {
            rs = result;
        });
        return rs;
    }
}