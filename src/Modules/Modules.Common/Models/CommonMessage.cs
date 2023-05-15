using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Renci.SshNet;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;

namespace Modules.Common.Models
{
    public static class MessageCode
    {
        public static string ME0001 = "Please select store!";
        public static string ME0002 = "Update password successful";
        public static string ME0003 = "Update password failed";

        public static string MEPASS01 = "Not found password default! please update or add DPSS01 to Common Code screen.";

        public static string ME0004 = "Please select an account";
        public static string MEA001 = "Status updated successful!";
        public static string MEA002 = "Status updated fail!";

        public static string MD0001 = "Are you sure to edit data?";
        public static string MD0002 = "Are you sure to delete data?";
        public static string MD0003 = "Are you sure to save data?";
        public static string MD0004 = "Save data successfully.";
        public static string MD0005 = "Save data fail.";
        public static string MD0006 = "Yes";
        public static string MD0007 = "No";
        public static string MD0008 = "Delete data successfully.";
        public static string MD0009 = "Edit data is not successfully.";
        public static string MD00010 = "Sent mail successfully.";
        public static string MD00011 = "Are you sure Request Production?";
        public static string MD00012 = "Request Production successfully.";
        public static string MD00013 = "Save data successfully.You must add an item Warehouse to use it.";
        public static string MD00014 = "The status has changed. Can not delete rows";
        public static string MD0015 = "Delete data fail";
        public static string MD00016 = "Are you sure you want to create a copy of the selected PO?";
        public static string MD00017 = "Edit data is successfully.";
        public static string MD00018 = "User Sales Order Project Code already exist";
        public static string MD00019 = "The data has been changed before, please reload the data first.";

        public static string MD0020 = "Do you want to update status device?";
        public static string MD0021 = "Do you want to update Approve/Reject user?";

        public static string DEPLOYAUDIO = "Do you want to deploy audio?";
        // CRUD Message Store
        public static string MESTS1 = "Save data store device failed!";
        public static string MESTS2 = "Save data store device successfully!";
        public static string MESTD3 = "Delete store device failed!";
        public static string MESTD4 = "Delete store device successfull!";
        public static string MESTD5 = "Update the status of device successfully!";
        public static string MESTD6 = "Update the status of device fairly!";


        // CRUD User Store Mgt

        // CRUD Message AdMgt 
        public static string MEAD01 = "Save data advertisement successfully!";
        public static string MEAD02 = "Save data advertisement failed!";
        public static string MEAD03 = "Delete advertisement successfully!";
        public static string MEAD04 = "Delete advertisement failed!";

        // Validate message Equiqment

        public static string MEQ001 = "Please input RegistDate";
        public static string MEQ002 = "Please select device type";
        public static string MEQ003 = "Please input device name";
        public static string MEQ004 = "Please input device key";
        public static string MEQ005 = "Please input IP addess";
        public static string MEQ006 = "Please input device port";

        // Validate password
        public static string MEP001 = "Do you want to changes password?";
        public static string MEP002 = "Are you sure to changes default password";

        /// <summary>
        /// PVN Add
        /// Message error For Inventory Managerment Upload File
        /// </summary>
        public static string MD00013_KR = "중복된 품번이 있습니다. 엑셀 파일을 확인하여 다시 업로드 해 주십시오.";
        public static string MD00013_EN = "Please Remove Duplicate Item(s) In Excel File!";
        public static string MD00014_KR = "다운받은 엑셀의 양식과 날짜 등의 형식을 그대로 유지하여 주십시오.";
        public static string MD00014_EN = "Please Re-Format DateTime Field In Excel File!";

        //request PO message
        public static string MReqPO = "Are you sure to request this purchase order ?";

        // confirm delete code
        public static string DELETE_CODE = "This action may be delete all record in Common Sub Code! Are you sure to delete Code Master?";

        // error
        public static string conflictKeyMessage = "The DELETE statement conflicted with the REFERENCE constraint \"FK__tblAdStore__AdNo__367C1819\". The conflict occurred in database \"KIOSK\", table \"dbo.tblAdStoreMgt\", column 'AdNo'.";
        public static string conflictChangeMessage = "Please cancel the registration advertisement for each store before deleting."; //The DELETE statement conflicted with the REFERENCE constraint \"Advertisement Store\". \n
    }

    public static class CommonAction
    {
        public static string INSERT = "Insert";
        public static string UPDATE = "Update";
        public static string DELETE = "Delete";
        public static string SELECT = "Select";
        public static string ADD = "Add";
        public static string EDIT = "Edit";
        public static string LOGIN = "LOGIN";
        public static string LOGOUT = "LOGOUT";

    }
    public static class RemoteProgramData
    {
        static public List<string> RemoteData()
        {
            List<string> remote = new List<string>();
            remote.Add("screen mode id:i:2");
            remote.Add("use multimon:i:0");
            remote.Add("desktopwidth:i:1920");
            remote.Add("desktopheight:i:1080");
            remote.Add("session bpp:i:32");
            remote.Add("winposstr:s:0,1,27,0,1938,1127");
            remote.Add("compression:i:1");
            remote.Add("keyboardhook:i:2");
            remote.Add("audiocapturemode:i:0");
            remote.Add("videoplaybackmode:i:1");
            remote.Add("connection type:i:7");
            remote.Add("networkautodetect:i:1");
            remote.Add("bandwidthautodetect:i:1");
            remote.Add("displayconnectionbar:i:1");
            remote.Add("enableworkspacereconnect:i:0");
            remote.Add("disable wallpaper:i:0");
            remote.Add("allow font smoothing:i:0");
            remote.Add("allow desktop composition:i:0");
            remote.Add("disable full window drag:i:1");
            remote.Add("disable menu anims:i:1");
            remote.Add("disable themes:i:0");
            remote.Add("disable cursor setting:i:0");
            remote.Add("bitmapcachepersistenable:i:1");
            remote.Add("full address:s:45.119.147.214:1072");
            remote.Add("audiomode:i:0");
            remote.Add("redirectprinters:i:1");
            remote.Add("redirectcomports:i:0");
            remote.Add("redirectsmartcards:i:1");
            remote.Add("redirectclipboard:i:1");
            remote.Add("redirectposdevices:i:0");
            remote.Add("autoreconnection enabled:i:1");
            remote.Add("authentication level:i:2"); 
            remote.Add("prompt for credentials:i:0");
            remote.Add("negotiate security layer:i:1");
            remote.Add("remoteapplicationmode:i:0");
            remote.Add("alternate shell:s:");
            remote.Add("shell working directory:s:");
            remote.Add("gatewayhostname:s:");
            remote.Add("gatewayusagemethod:i:4");
            remote.Add("gatewaycredentialssource:i:4");
            remote.Add("gatewayprofileusagemethod:i:0");
            remote.Add("promptcredentialonce:i:0"); 
            remote.Add("gatewaybrokeringtype:i:0");
            remote.Add("use redirection server name:i:0");
            remote.Add("rdgiskdcproxy:i:0");
            remote.Add("kdcproxyname:s:");
            return remote;
        }
    }
}
