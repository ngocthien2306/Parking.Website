using System;

namespace Modules.Common
{

    public class VerifyCodeToken
    {
        public string USER_CD { get; set; }
        public DateTime EXPIRYDATE { get; set; }
        public int ID { get; set; }
        public string VERIFYCODE { get; set; }
    }

}