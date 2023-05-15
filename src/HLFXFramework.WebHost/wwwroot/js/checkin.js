"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/checkinHub").build();
$(function () {
    connection.start().then(function () {

        InvokeCheckins();
    }).catch(function (err) {
        return console.error(err.toString());
    });
});
function InvokeCheckins() {
    connection.invoke("SendCheckIns").catch(function (err) {
        return console.error(err.toString());
    });
}
connection.on("ReceivedCheckIns", function (checkins) {
    var store = GetGridIntance("#gridStoreCheckInPage").getSelectedRowsData();
    var startTime = formatDate("#startDateCheckIn");
    var endTime = formatDate("#endDateCheckIn");
    var storeNo = store.length > 0 ? store[0].storeNo : null;
    if (storeNo !== null) {
        checkins = checkins.filter(d => d.storeNo == storeNo);
    }
    //if (startTime != null && endTime != null) {
    //    checkins = checkins.filter(d => d.useDate > startTime && d.useDate < endTime);
    //}
    
    GetGridIntance("#gridCheckInUserRealTime").option("dataSource", checkins)
})

connection.on("SendDeployedSuccess", function () {
    LoadingPage(0);
    DevExpress.ui.notify(
        {
            message: 'Deployed to all devices successfull',
            position: {
                my: 'bottom right',
                at: 'bottom right'
            },
            width: '30%'
        },
        'success',
        4000
    );
})