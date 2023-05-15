var MenuName;

//$(() => {
//    alert('heeloo menmnnnn!');
//    MenuName = document.getElementsByTagName
//});


//9 => Action ID ; 2 => Page ID
function FN_Validate_9_2(insertData) {
     
    console.log(insertData);
    var obj = JSON.parse(insertData);
    var rs;

    if (obj[0].PostData.AddedRows.length != 0) {
        $.ajax({
            url: getLanguages() + '/ValidateCRUDData/WarehouseValidateCRUD',
            data: { validateData: JSON.stringify(obj[0].PostData.AddedRows) },
            type: 'POST',
            dataType: 'json',
            async: false
        }).done(function (result) {
             
            rs = result;
        });
        return rs;
        //console.log(rs);
    }
}