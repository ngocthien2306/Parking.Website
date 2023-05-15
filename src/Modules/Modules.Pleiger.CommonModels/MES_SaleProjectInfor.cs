using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Modules.Pleiger.CommonModels
{
    public class MES_SaleProjectInfor
    {
        //public int No { get; set; }
        //public string ProjectCode { get; set; }
        //public string VatType { get; set; }
        public string DIVISION { get; set; }
        public string CO_CD { get; set; }
        public string IN_DT { get; set; }
        public int IN_SQ { get; set; }
        public int LN_SQ { get; set; }
        public string ISU_DT { get; set; }
        public int ISU_SQ { get; set; }
        public string DIV_CD { get; set; }
        public string DEPT_CD { get; set; }
        public string EMP_CD { get; set; }
        public string DRCR_FG { get; set; }
        public string ACCT_CD { get; set; }
        public string REG_NB { get; set; }
        public decimal? ACCT_AM { get; set; }
        public string RMK_DC { get; set; }
        public string RMK_DOCK { get; set; }
        public string CCODE_TY { get; set; }
        public string CT_DEPT { get; set; }
        public string DCODE_TY { get; set; }
        public string PJT_CD { get; set; }
        public string CT_AM { get; set; }
        public string CT_DEAL { get; set; }
        public string NONSUB_TY { get; set; }
        public string FR_DT { get; set; }
        public string TO_DT { get; set; }
        public string ISU_DOC { get; set; }
        public string ISU_DOCK { get; set; }
        public string JEONJA_YN { get; set; }
        public string CT_NB { get; set; }
        public decimal? CT_QT { get; set; }
        public string DUMMY1 { get; set; }
        public string DUMMY2 { get; set; }
        public string EMPTY1 { get; set; }
        public string INSERT_ID { get; set; }
        public decimal? INSERT_DT { get; set; }
        public string INSERT_IP { get; set; }
        public string MODIFY_ID { get; set; }
        public DateTime? MODIFY_DT { get; set; }
        public string MODIFY_IP { get; set; }
        public string CEO_NM { get; set; }
        public string TR_NM { get; set; }
        public string TR_NMK { get; set; }
        public string BUSINESS { get; set; }
        public string JONGMOK { get; set; }
        public string ADDR1 { get; set; }
        public string ADDR2 { get; set; }
        public string TR_CD { get; set; }
        public string CT_USER1 { get; set; }
        public string CT_USER2 { get; set; }
        public string USER1_TY { get; set; }
        public string USER2_TY { get; set; }
        public string TRCHARGE_EMP { get; set; }
        public string TRCHARGE_EMAIL { get; set; }
        public string LOGIC_CD { get; set; }
        public string ATTR_CD { get; set; }

    }
    public class MES_SaleProjectExcelInfor
    {

        [ColumName("구분")]
        [JsonProperty(Order = 1)]
        public string DIVISION { get; set; }

        [ColumName("회사코드")]
        [JsonProperty(Order = 2)]
        public string CO_CD { get; set; }

        [ColumName("処理日付/처리일자")]
        [JsonProperty(Order = 3)]
        public string IN_DT { get; set; }

        [ColumName("処理番号/처리번호")]
        [JsonProperty(Order = 4)]
        public int IN_SQ { get; set; }

        [ColumName("処理LINE番号/처리라인번호")]
        [JsonProperty(Order = 5)]
        public int LN_SQ { get; set; }

        [ColumName("決議日付/결의일자")]
        [JsonProperty(Order = 6)]
        public string ISU_DT { get; set; }

        [ColumName("決議番号/결의번호")]
        [JsonProperty(Order = 7)]
        public int ISU_SQ { get; set; }

        [ColumName("会計単位/회계단위")]
        [JsonProperty(Order = 8)]
        public string DIV_CD { get; set; }

        [ColumName("決議部署/결의부서")]
        [JsonProperty(Order = 9)]
        public string DEPT_CD { get; set; }

        [ColumName("作業者/작업자")]
        [JsonProperty(Order = 10)]
        public string EMP_CD { get; set; }

        [ColumName("借方貸方区分/차.대구분")]
        [JsonProperty(Order = 11)]
        public string DRCR_FG { get; set; }

        [ColumName("勘定科目/계정과목")]
        [JsonProperty(Order = 12)]
        public string ACCT_CD { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 13)]
        public string REG_NB { get; set; }

        [ColumName("金額/금액")]
        [JsonProperty(Order = 14)]
        public decimal? ACCT_AM { get; set; }

        [ColumName("적요")]
        [JsonProperty(Order = 15)]
        public string RMK_DC { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 16)]
        public string RMK_DOCK { get; set; }

        [ColumName("관리항목C 타입")]
        [JsonProperty(Order = 17)]
        public string CCODE_TY { get; set; }

        [ColumName("관리항목C 코드")]
        [JsonProperty(Order = 18)]
        public string CT_DEPT { get; set; }

        [ColumName("관리항목D 타입")]
        [JsonProperty(Order = 19)]
        public string DCODE_TY { get; set; }
    
        [ColumName("관리항목D 코드")]
        [JsonProperty(Order = 20)]
        public string PJT_CD { get; set; }
        
        [ColumName("계정금액")]
        [JsonProperty(Order = 21)]
        public string CT_AM { get; set; }
        [ColumName("세무구분")]
        [JsonProperty(Order = 22)]
        public string CT_DEAL { get; set; }
        [ColumName("    ")]
        [JsonProperty(Order = 23)]
        public string NONSUB_TY { get; set; }
        [ColumName("신고기준일")]
        [JsonProperty(Order = 24)]
        public string FR_DT { get; set; }
        [ColumName("    ")]
        [JsonProperty(Order = 25)]
        public string TO_DT { get; set; }
        [ColumName("품의내역")]
        [JsonProperty(Order = 26)]
        public string ISU_DOC { get; set; }
        [ColumName("    ")]
        [JsonProperty(Order = 27)]
        public string ISU_DOCK { get; set; }
        
        [ColumName("    ")]
        [JsonProperty(Order = 28)]
        public string JEONJA_YN { get; set; }
        [ColumName("환율")]
        [JsonProperty(Order = 29)]
        public string CT_NB { get; set; }
        [ColumName("환종")]
        [JsonProperty(Order = 30)]
        public decimal? CT_QT { get; set; }
        [ColumName("외화금액")]
        [JsonProperty(Order = 31)]
        public string DUMMY1 { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 32)]
        public string DUMMY2 { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 33)]
        public string EMPTY1 { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 34)]
        public string INSERT_ID { get; set; }


        [ColumName("    ")]
        [JsonProperty(Order = 35)]
        public decimal? INSERT_DT { get; set; }
        [ColumName("    ")]
        [JsonProperty(Order = 36)]
        public string INSERT_IP { get; set; }

        [ColumName("수정자 ID")]
        [JsonProperty(Order = 37)]
        public string MODIFY_ID { get; set; }

        [ColumName("수정일자")]
        [JsonProperty(Order = 38)]
        public DateTime? MODIFY_DT { get; set; }

        [ColumName("수정자 IP")]
        [JsonProperty(Order = 39)]
        public string MODIFY_IP { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 40)]
        public string CEO_NM { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 41)]
        public string TR_NM { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 42)]
        public string TR_NMK { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 43)]
        public string BUSINESS { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 44)]
        public string JONGMOK { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 45)]
        public string ADDR1 { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 46)]
        public string ADDR2 { get; set; }

        [ColumName("거래처코드")]
        [JsonProperty(Order = 47)]
        public string TR_CD { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 48)]
        public string CT_USER1 { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 49)]
        public string CT_USER2 { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 50)]
        public string USER1_TY { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 51)]
        public string USER2_TY { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 52)]
        public string TRCHARGE_EMP { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 53)]
        public string TRCHARGE_EMAIL { get; set; }

        [ColumName("전표구분")]
        [JsonProperty(Order = 54)]
        public string LOGIC_CD { get; set; }

        [ColumName("    ")]
        [JsonProperty(Order = 55)]
        public string ATTR_CD { get; set; }
    }

    //[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]  // Multiuse attribute.  
    //public class ColumNameAttribute : Attribute
    //{
    //    string name;

    //    public ColumNameAttribute(string name)
    //    {
    //        this.name = name;
    //    }

    //    public string GetName()
    //    {
    //        return name;
    //    }
    //}
}
