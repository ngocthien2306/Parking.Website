using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels.Models
{
    public class KIO_AlarmToStoreMessage
    {
        public int storeNo { get; set; }
        public int alarmAdminNo { get; set; }
        public bool apprOpenDoorAlarm { get; set; }
        public bool apprUserRegistAlarm { get; set; }
        public string workerPhoneNumber { get; set; }
        public string telegramId { get; set; }
        public string userRegAlarmMsg { get; set; }
        public string openDoorAlarmMgt { get; set; }
        public string telegramToken { get; set; }
        public bool apprUserFaceOkAlarm { get; set; }
        public string userFaceMessage { get; set; }
        public string sendToMessage { get; set; }
    }
}
